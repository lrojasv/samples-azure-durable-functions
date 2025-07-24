# Samples Azure Durable Function

Este repositorio tiene el objetivo de compartir ejemplos de uso de Azure Durable Functions implementando diferentes patrones en los lenguajes C# (.Net 8) y Java (JDK 21).

## Lista de ejemplos

| Patron      | Lenguajes     | 
| ------------- | ------------- |
| <a href="samples/http-async/README.md">Http Async Api</a> | java, Net |

## Consideraciones con Azure Durable Function

### Debugging en Azure Durable Function Java
Para correr un azure durable function java con debug en vs code debe hacer los siguiente:

primero debe ejecutar
mvn clean package

segundo ubicarse en la siguiente ruta
cd target\azure-functions\{nombre-de-su-funcion}

ejecutar lo siguiente
para cmd 
set JAVA_OPTS=-agentlib:jdwp=transport=dt_socket,server=y,suspend=n,address=5005

para power shell
$env:JAVA_OPTS="-agentlib:jdwp=transport=dt_socket,server=y,suspend=n,address=5005"

luego ejecutar lo siguiente
func host start

Luego atachar el proceso en vs code