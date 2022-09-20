

using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Logger;

public class AboutModel
{
    private readonly ILogger _logger;

    public AboutModel(ILogger<AboutModel> logger)
    {
        _logger = logger;
    }
    
    public void OnGet()
    {
        _logger.LogInformation("About page visited at {DT}", 
            DateTime.UtcNow.ToLongTimeString());
    }
}