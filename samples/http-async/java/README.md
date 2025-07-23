# Azure Durable Function - Http Async Api Sample Java

Ejemplo en java implementando el patron Http Async Api

## Pre-requisitos

- JDK 11
- Azure Function Core Tools V4.x
- Azurite (extensión de VS Code)

## Correr el ejemplo

1. Agregar en la raiz el archivo local.settings.json

   ```json
   {
        "IsEncrypted": false,
        "Values": {
            "AzureWebJobsStorage": "UseDevelopmentStorage=true",
            "FUNCTIONS_WORKER_RUNTIME": "java",
            "DOMINIO_HOST_PROXY": "https://mis-funciones.example.com", // URL que se usa para devolver el endpoint de GetStatus
            "URL_FUNCTION_MODELO_ANALITICO": "http://localhost:7072" // URL del function en python que se usa en este ejemplo
        }
    }

   ```
2. Iniciar Azurite en el command de VS Code

```bash

    Azurite: Start

```

3. Luego ejecutar los siguientes comandos

```bash

    mvn clean package

    mvn azure-functions:run 
```

