using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace Company.Function;

public static class DurableFunctionsOrchestrationCSharp1
{
    [Function(nameof(DurableFunctionsOrchestrationCSharp1))]
    public static async Task<bool> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        ILogger logger = context.CreateReplaySafeLogger(nameof(DurableFunctionsOrchestrationCSharp1));
        logger.LogInformation("Iniciando orquestación.");

        var operation = context.GetInput<Operation>();

        var estadoEvaluate = await context.CallActivityAsync<string>("Evaluate", operation);

        await context.CallActivityAsync<string>("Notificate", estadoEvaluate);

        if (estadoEvaluate == "OK")
        {
            await context.CallActivityAsync<bool>("Execute", operation);
            await context.CallActivityAsync<string>("Notificate", "OperacionEjecutada");
            return true;
        }

        //Requiere intervencion usuario 2do factor
        var authorized = false;
        using (var timeoutCts = new CancellationTokenSource())
        {
            // The user has 90 seconds to respond with the code they received in the SMS message.
            DateTime expiration = context.CurrentUtcDateTime.AddSeconds(90);
            Task timeoutTask = context.CreateTimer(expiration, timeoutCts.Token);

            Task<string> FactorResultadoTask =
                    context.WaitForExternalEvent<string>("FactorResultado");

            Task winner = await Task.WhenAny(FactorResultadoTask, timeoutTask);
            if (winner == FactorResultadoTask)
            {
                // We got back a response! Compare it to the challenge code.
                if (FactorResultadoTask.Result == "Aprobado")
                {
                    authorized = true;
                }
            }

            if (!timeoutTask.IsCompleted)
            {
                // All pending timers must be complete or canceled before the function exits.
                timeoutCts.Cancel();
            }
        }

        if (!authorized)
        {
            await context.CallActivityAsync<string>("Notificate", "OperacionRechazada");
            return false;
        }

        await context.CallActivityAsync<string>("Notificate", "OperacionAprobada");
        await context.CallActivityAsync<bool>("Execute", operation);
        await context.CallActivityAsync<string>("Notificate", "OperacionEjecutada");

        return true;
    }

    [Function(nameof(SayHello))]
    public static string SayHello([ActivityTrigger] string name, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("SayHello");
        logger.LogInformation("Saying hello to {name}.", name);
        return $"Hello {name}!";
    }

    [Function("DurableFunctionsOrchestrationCSharp1_HttpStart")]
    public static async Task<HttpResponseData> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("DurableFunctionsOrchestrationCSharp1_HttpStart");

        var request = await System.Text.Json.JsonSerializer.DeserializeAsync<Operation>(req.Body);
        // Function input comes from the request content.
        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(DurableFunctionsOrchestrationCSharp1), request);

        logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

        // Returns an HTTP 202 response with an instance management payload.
        // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
        return await client.CreateCheckStatusResponseAsync(req, instanceId);
    }
}