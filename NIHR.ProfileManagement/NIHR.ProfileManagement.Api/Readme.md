# Profile Management
## Overview
Profile management is a service to store and maintain profile information for a given authenticated user.

## Components
### Api
The API is simply the entry point for receiving HTTP requests to create a user profile.

### Authorizer
The authorizer is a lambda authorizer to attach to API Gateway and is responsible for handling authorization of HTTP requests.

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


