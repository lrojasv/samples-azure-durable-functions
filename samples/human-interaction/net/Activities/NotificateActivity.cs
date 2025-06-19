using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

public static class NotificateActivity
{
    [Function(nameof(Notificate))]
    public static void Notificate([ActivityTrigger] string estado,
                                    FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("Notificate");
        logger.LogInformation("Ejecutando activity Notificate.");
        logger.LogInformation($"Notificate estado: {estado}");
    }
}

