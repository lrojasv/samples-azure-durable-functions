import azure.functions as func
import logging
import time

app = func.FunctionApp(http_auth_level=func.AuthLevel.ANONYMOUS)

@app.route(route="fnc_modelo")
def fnc_modelo(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')

    parameter = req.params.get('parameter1')
    if not parameter:
        try:
            req_body = req.get_json()
        except ValueError:
            pass
        else:
            parameter = req_body.get('parameter1')

    time.sleep(1) #timer de espera para simular largas esperas por procesamiento

    return func.HttpResponse(
             "This HTTP triggered function executed successfully." + parameter,
             status_code=200
        )
        