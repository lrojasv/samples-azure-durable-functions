package com.function;

import com.microsoft.azure.functions.annotation.*;
import com.microsoft.azure.functions.*;
import java.net.URI;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.util.*;
import java.util.logging.LogRecord;
import java.util.logging.Logger;

import javax.lang.model.type.ErrorType;

import com.microsoft.durabletask.*;
import com.microsoft.durabletask.azurefunctions.DurableActivityTrigger;
import com.microsoft.durabletask.azurefunctions.DurableClientContext;
import com.microsoft.durabletask.azurefunctions.DurableClientInput;
import com.microsoft.durabletask.azurefunctions.DurableOrchestrationTrigger;
import com.google.gson.Gson;
import com.google.gson.JsonObject;
import com.httpclient.HttpClientCustom;

/**
 * Please follow the below steps to run this durable function sample
 * 1. Send an HTTP GET/POST request to endpoint `StartHelloCities` to run a durable function
 * 2. Send request to statusQueryGetUri in `StartHelloCities` response to get the status of durable function
 * For more instructions, please refer https://aka.ms/durable-function-java
 * 
 * Please add com.microsoft:durabletask-azure-functions to your project dependencies
 * Please add `"extensions": { "durableTask": { "hubName": "JavaTestHub" }}` to your host.json
 */
public class DurableFunctionsOrchestratorSample {
    /**
     * This HTTP-triggered function starts the orchestration.
     */
    @FunctionName("StartOrchestration")
    public HttpResponseMessage startOrchestration(
            @HttpTrigger(name = "req", methods = {HttpMethod.POST}, authLevel = AuthorizationLevel.ANONYMOUS) HttpRequestMessage<Optional<String>> request,
            @DurableClientInput(name = "durableContext") DurableClientContext durableContext,
            final ExecutionContext context) {
        context.getLogger().info("Java HTTP trigger processed a request.");

        DurableTaskClient client = durableContext.getClient();
        String instanceId = client.scheduleNewOrchestrationInstance("OrquestadorHttpAsyncApi");
        context.getLogger().info("Created new Java orchestration with instance ID = " + instanceId);

        // Construimos el objeto de respuesta
        JsonObject responseBody = new JsonObject();
        responseBody.addProperty("instanceId", instanceId);
        responseBody.addProperty("statusQueryGetUri", getStatusQueryGetUri(instanceId));
        responseBody.addProperty("message", "Ejecución iniciada.");

        Gson gson = new Gson();

        // Utilizamos el builder proporcionado por el request
        return request.createResponseBuilder(HttpStatus.ACCEPTED)
                .header("Content-Type", "application/json")
                .body(gson.toJson(responseBody))
                .build();
    }

    /**
     * This is the orchestrator function, which can schedule activity functions, create durable timers,
     * or wait for external events in a way that's completely fault-tolerant.
     */
    @FunctionName("OrquestadorHttpAsyncApi")
    public String Orquestador(
            @DurableOrchestrationTrigger(name = "ctx") TaskOrchestrationContext ctx) {

        ctx.setCustomStatus("PENDING"); //con esto se maneja el custom error del workflow

        try{           
            ctx.callActivity("EjecutarModeloAnaliticoActivity", "Parameter1", String.class).await();           
        }
        catch(TaskFailedException  ex){
            //aqui puedes ejecutar alguna compensación
            ctx.setCustomStatus("ERROR");
        }
        
        ctx.setCustomStatus("OK");
        return "OK";
    }

    /**
     * This is the activity function that gets invoked by the orchestration.
     * @throws Exception 
     */
    @FunctionName("EjecutarModeloAnaliticoActivity")
    public String EjecutarModeloAnaliticoActivity(
            @DurableActivityTrigger(name = "parameter1") String parameter1,
            final ExecutionContext context) throws Exception {

        context.getLogger().info("Se inicia ejecución de modelo analítico");
        
        try{
            var httpClient = HttpClientCustom.getInstance().getHttpClient();
            var urlFunction = System.getenv("URL_FUNCTION_MODELO_ANALITICO") + "/api/fnc_modelo";
            
            JsonObject responseBody = new JsonObject();
            responseBody.addProperty("parameter1", "valor parameter 1");

            Gson gson = new Gson();
            final String payload = gson.toJson(responseBody);
            
            context.getLogger().info("Antes de generar el httprequest - payload " + payload);
            final HttpRequest httpRequest = HttpRequest.newBuilder()
                    .POST(HttpRequest.BodyPublishers.ofString(payload))
                    .uri(URI.create(urlFunction))
                    .setHeader("Content-Type","application/json")
                    .build();
            
            final HttpResponse<String> response = httpClient.send(httpRequest, 
                HttpResponse.BodyHandlers.ofString());

            if(response.statusCode() != HttpStatus.OK.value()){
                context.getLogger().info("error de invocacion: status"+ response.statusCode());
                return "ERROR";
            }  

            return "OK";
        }
        catch(Exception ex){
            context.getLogger().info(ex.getMessage() + ex.getStackTrace());
            throw new Exception("ERROR");
        }
        
    }

    private String getStatusQueryGetUri(String instanceId){
        return System.getenv("DOMINIO_HOST_PROXY") + "/runtime/webhooks/durabletask/instances/" + instanceId;
    }
}