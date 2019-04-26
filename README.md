# blip.webhookreceiver 
Project to receive webhook from BLIP messages

## blip.webhookreceiver.pubsub
Sub-project to process the messages using GPC PubSub

### Required Environment Variables:
- GOOGLE_APPLICATION_CREDENTIALS: Json with google credentials on GCP PubSub.
- EVENT_SUBSCRIPTION_NAME: Subscription name to the blip events on GCP PubSub.
- MESSAGE_SUBSCRIPTION_NAME: Subscription name to the blip messages on GCP PubSub.
- EVENT_TOPIC_NAME: Topc name to the blip events on GCP PubSub.
- MESSAGE_SUBSCRIPTION_NAME: Topc name to the blip messages on GCP PubSub.

## blip.webhookreceiver.bigquery
Sub-project to process the messages using GPC BigQuery

### Required Environment Variables:
- GOOGLE_APPLICATION_CREDENTIALS: Json with google credentials on GCP PubSub.
- DATASET_NAME: DataSet name on GCP BigQuery.
- EVENT_TABLE_NAME: Event table name on GCP BigQuery.
- MESSAGE_TABLE_NAME: Message table name on GCP BigQuery.

# Missing Features
- Logging (DONE!)
- Docker Containers (DONE!)
- Kubernetes (DONE!)
- Deploy Prod (DONE!)
- Health Monitoring Endpoing
- Unit Testing
