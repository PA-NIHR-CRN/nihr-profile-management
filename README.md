# Profile Management
## Overview
Profile management is a service to store and maintain profile information for a given authenticated user.

## Components
### Api
The API is simply the entry point for receiving HTTP requests to create a user profile.

#### Configuration
| Parameter    | Example value | Notes |
| -------- | ------- | ------- |
| Data__ConnectionString  | server=localhost;port=3306;database=profile_management;user=username;password=password| Only set 'password' for local development. Omit 'password=' for PROD and use PasswordSecretName |
| Data__PasswordSecretName |      | Leave empty for local development, otherwise set to AWS credential secret name to retrieve password value |
| AWS__Region | eu-west-2 | Leave empty for local development, otherwise set to AWS credential secret name to retrieve password value |
| ProfileManagementApi__JwtBearer__Authority | https://cognito-idp.eu-west-2.amazonaws.com/eu-west-2_cAq5FnIi4 |  |

### Authorizer
The authorizer is a lambda authorizer to attach to API Gateway and is responsible for handling authorization of HTTP requests.

#### Configuration
| Parameter    | Example value | Notes |
| -------- | ------- | ------- |
| WSO2_SERVICE_AUDIENCES  | The Cognito App client ID (we should discuss in more detail what this value should be i.e. should it be an IDG app client ID or a Cognito App Client ID - I think the latter) |  |
| WSO2_SERVICE_ISSUER  | https://dev.id.nihr.ac.uk:443/oauth2/token|  |
| WSO2_SERVICE_TOKEN_ENDPOINT  | https://dev.id.nihr.ac.uk/oauth2/oidcdiscovery |  |

### Cognito Pre sign-up trigger lambda
This lambda function is used as a Cognito trigger, specifically the 'Pre sign-up' trigger. It is invoked each time a new user is added to the user pool.
The purpose of the lambda function is to invoke operations within IProfileManagementService to register the new user in the Profile Management database.

#### Configuration
The lambda requires the following runtime configuration parameters

| Parameter    | Example value | Notes |
| -------- | ------- | ------- |
| Data__ConnectionString  | server=localhost;port=3306;database=profile_management;user=username;password=password| Only set 'password' for local development. Omit 'password=' for PROD and use PasswordSecretName |
| Data__PasswordSecretName |      | Leave empty for local development, otherwise set to AWS credential secret name to retrieve password value |

### Outbox processor
The outbox processor is a background processing task that reads unprocessed items from the Profile Management 'outboxEntry' table and delivers them to the configured Kafka topic.

#### Configuration
The outbox processor requires the following runtime configuration parameters

| Parameter    | Example value | Notes |
| -------- | ------- | ------- |
| Data__ConnectionString  | server=localhost;port=3306;database=profile_management;user=username;password=password| Only set 'password' for local development. Omit 'password=' for PROD and use PasswordSecretName |
| Data__PasswordSecretName |      | Leave empty for local development, otherwise set to AWS credential secret name to retrieve password value |
| MessageBus__Topic | topic| |
| MessageBus__BootstrapServers| localhost:9091     | |
| OutboxProcessor__SleepInterval| 60     | A delay (in seconds) between each iteration of processing. On each iteration, the processor reads and processes all available records. |


