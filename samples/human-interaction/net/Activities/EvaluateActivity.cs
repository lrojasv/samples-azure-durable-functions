using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

public static class EvaluateActivity
{
    [Function(nameof(Evaluate))]
    public static string Evaluate([ActivityTrigger] Operation operation,
                                    FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("Evaluate");
        logger.LogInformation("Ejecutando activity Evaluate.");

        if (operation.Tipo == "Favoritos")
            return "OK";

        return "Requiere2Factor";
    }
}

