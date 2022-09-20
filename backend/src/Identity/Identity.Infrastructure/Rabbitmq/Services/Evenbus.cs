using System.Reflection;
using System.Text;
using Identity.Application.Common.Configuration;
using Identity.Infrastructure.Rabbitmq.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Identity.Infrastructure.Rabbitmq.Services;

public class Eventbus : IEventbus
{
    private readonly EventBusConfiguration _eventBusConfig;
    private readonly IModel _consumerChannel;
    private readonly IEventBusSubscriptionsManager _eventBusSubscriptions;
    private readonly IServiceProvider _serviceProvider;


    public Eventbus(IOptions<EventBusConfiguration> options, IEventBusSubscriptionsManager eventBusSubscriptions, IServiceProvider serviceProvider)
    {
        _eventBusSubscriptions = eventBusSubscriptions;
        _serviceProvider = serviceProvider;
        _eventBusConfig = options.Value;
        _consumerChannel = CreateConsumerChannel();
    }


    public void Publish(IntegrationEvent @event)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqp://admin:admin@localhost:5672/")
        };

        var eventName = @event.GetType().Name;
        //Create the RabbitMQ connection using connection factory details as i mentioned above
        var connection = factory.CreateConnection();
        //Here we create channel with session and model
        using
            var channel = connection.CreateModel();
        //declare the queue after mentioning name and a few property related to that
        // channel.QueueDeclare("product", true, false, false, null);
        //Serialize the message
        var json = JsonConvert.SerializeObject(@event);
        var body = Encoding.UTF8.GetBytes(json);
        //put the data on to the product queue
        channel.BasicPublish(exchange: _eventBusConfig.ExchangeName, routingKey: "testEvent", body: body);
    }


    public void SubscribeDynamic<TIntegrationEventHandler>(string eventName)
    {
        DoInternalSubscription(eventName);
        _eventBusSubscriptions.AddDynamicSubscription<TIntegrationEventHandler>(eventName);
        StartBasicConsumer();
    }

    private IModel CreateConsumerChannel()
    {
        //amqp://guest:guest@localhost:5672/
        var uri = $"amqp://{_eventBusConfig.UserName}:{_eventBusConfig.Password}@{_eventBusConfig.HostName}:{_eventBusConfig.Port}/";
        var factory = new ConnectionFactory
        {
            Uri = new Uri(uri)
        };
        Console.WriteLine("Creating connection RabbitMQ consumer");
        
        var connection = factory.CreateConnection();

        var channel = connection.CreateModel();

        channel.ExchangeDeclare(_eventBusConfig.ExchangeName, _eventBusConfig.ExchangeType);
        //declare the queue after mentioning name and a few property related to that
        channel.QueueDeclare(_eventBusConfig.Queue, true, false, false, null);

        return channel;
    }

    public void DoInternalSubscription(string eventName)
    {
        _consumerChannel.QueueBind(_eventBusConfig.Queue, _eventBusConfig.ExchangeName, eventName);
    }

    private void StartBasicConsumer()
    {
        Console.WriteLine("Starting RabbitMQ basic consume");
        if (_consumerChannel != null)
        {
            var consumer = new EventingBasicConsumer(_consumerChannel);
            consumer.Received += Consumer_Received;
            _consumerChannel.BasicConsume(_eventBusConfig.Queue, true, consumer);
        }
    }

    private void Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
    {
        var eventName = eventArgs.RoutingKey;
        var body = eventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        object eventData = (object) JObject.Parse(message);
        if (_eventBusSubscriptions.HasSubscriptionsForEvent(eventName))
        {
            var eventType = _eventBusSubscriptions.GetHandlersForEvent(eventName);
            var eventHandler = eventType.Name;
            var handler = _serviceProvider.GetRequiredService<IDynamicEventHandler>();
            handler.Handle(eventData);
        }


        var raw = (JObject) (new JsonSerializer().Deserialize(new JTokenReader((JObject) eventData)));
        foreach (var (key, value) in raw)
        {
            var a = key;
            var b = value;
        }

        Console.WriteLine($"Product message received: {message}");
    }
}