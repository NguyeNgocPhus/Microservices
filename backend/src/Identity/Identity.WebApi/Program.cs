using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Identity.Application.Commom.Configuration;
using Identity.Application.Common.Configuration;
using Identity.Application.gRPC;
using Identity.Infrastructure;
using Identity.Infrastructure.Databases;
using Identity.Infrastructure.Filters;
using Identity.Infrastructure.Handlers.Events.Dynamic;
using Identity.Infrastructure.ProtobuffServices.Services;
using Identity.Infrastructure.Rabbitmq.Interfaces;
using Identity.WebApi;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nobisoft.Core.DI;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

if (OperatingSystem.IsMacOS() || OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
{
    Console.WriteLine($"✅ ✅ ✅ ✅ ✅ Running on customized ConfigureKestrel");
    Console.WriteLine($"⚙️⚙️⚙️⚙️⚙️ ENV: {builder.Environment.EnvironmentName}");
    builder.WebHost.ConfigureKestrel(options =>
    {
        // Force grpc work without tls
        options.ListenAnyIP(5175, config => { config.Protocols = HttpProtocols.Http2; });
        // Force http work with bot http1 and http2
        options.ListenAnyIP(5165, config => { config.Protocols = HttpProtocols.Http1AndHttp2; });
    });
}
var msConfig = new MicroserviceConfiguration();
builder.Configuration.GetSection("Microservices").Bind(msConfig);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var appDb = builder.Configuration.GetSection("AppDb").Get<AppDbConfiguration>();

builder.Services.AddDbContextFactory<ApplicationDbContext>(option =>
{
    option.UseNpgsql($"Server={appDb.Server};Port={appDb.Port};User Id={appDb.UserName};Password={appDb.Password};Database={appDb.Database}");
    option.UseLoggerFactory(LoggerFactory.Create(loggingBuilder =>
        {
            loggingBuilder.AddConsole();
            loggingBuilder.AddSerilog();
        }
    ));
});
// Configure host
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.AddApplication(builder.Configuration);
// Call ConfigureContainer on the Host sub property 
builder.Host.ConfigureContainer<ContainerBuilder>(
    containerBuilder =>
    {
        // Use auto register modules from the code to auto register all our modules
        containerBuilder.AutoRegisterModules<IdentityModule>();
        //containerBuilder.RegisterMediatR(typeof(Program).Assembly);
    });

#region logger
builder.Logging.ClearProviders();
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    //.MinimumLevel.Verbose()
    .CreateLogger();
Log.Logger = logger;
builder.Logging.AddSerilog(logger);

#endregion

builder.Services.AddErrorFilter<GraphQLErrorFilter>();
builder
    .Host
    .UseSerilog(
        (context, _, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
            // To allow to add custom properties into the context
            configuration.Enrich.FromGlobalLogContext();
        }
    );

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}





// var factory = new ConnectionFactory {
//     Uri = new Uri("amqp://admin:admin@localhost:5672/")
// };
// //Create the RabbitMQ connection using connection factory details as i mentioned above
// var connection = factory.CreateConnection();
// //Here we create channel with session and model
// using
//     var channel = connection.CreateModel();
// channel.ExchangeDeclare("testExchange", "direct");
// //declare the queue after mentioning name and a few property related to that
// channel.QueueDeclare("testQueue", true, false, false, null);
// channel.QueueBind("testQueue", "testExchange","testEvent");
// //Set Event object which listen message from chanel which is sent by producer
// var consumer = new EventingBasicConsumer(channel);
// consumer.Received += (model, eventArgs) => {
//     var body = eventArgs.Body.ToArray();
//     var message = Encoding.UTF8.GetString(body);
//     object eventData = (object) JObject.Parse(message);
//     var raw = (JObject) (new JsonSerializer().Deserialize(new JTokenReader((JObject) eventData)));
//     foreach (var (key, value) in raw)
//     {
//         var a = key;
//         var b = value;
//     }
//
//     Console.WriteLine($"Product message received: {message}");
// };
// //read the message
// channel.BasicConsume(queue: "testQueue", autoAck: true, consumer: consumer);


#region EventBus

var eventBus = app.Services.GetRequiredService<IEventbus>();
eventBus.SubscribeDynamic<TestDynamicEventHandler>("testEvent");

#endregion



app.UseHttpsRedirection();

app.MapGrpcService<IdentityGrpcService>().RequireHost(msConfig.Services.First(x => x.Name.Equals("Identity")).GrpcUrl.Replace("http://", "").Replace("https://", ""));
app.UseAuthorization();
app.MapGraphQL();
app.MapControllers();
try
{
    Log.Information("Starting web host");
    app.UseSerilogRequestLogging();
    await app.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}