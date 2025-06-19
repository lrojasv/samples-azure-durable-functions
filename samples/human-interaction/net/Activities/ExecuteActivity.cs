using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

public static class ExecuteActivity
{
    [Function(nameof(Execute))]
    public static bool Execute([ActivityTrigger] Operation operation,
                                    FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("Execute");
        logger.LogInformation("Ejecutando activity Execute.");

        return true;
    }
}

