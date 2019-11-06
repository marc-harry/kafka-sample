# Kafka App

### Use Avro to generate classes

`dotnet tool install -g Confluent.Apache.Avro.AvroGen`

`avrogen -s your_schema.asvc .`

### To Run

1. docker-compose up -d
2. Run apps

## Infrastructure

#### Single typed message topics
If you need a single typed Producer/Consumer such as for Avro or just for a single type
you can use: `BaseConsumer<TKey, TValue>/BaseProducer<TKey, TValue>` or `SingleTypeJsonConsumer<TKey, TValue>/SingleTypeJsonProducer<TKey, TValue>`

#### Multi typed message topics (eg. Event Sourcing)
You can use `MultiTypeJsonConsumer<TKey, TValue>/MultiTypeJsonProducer<TKey, TValue>`

With this type of Consumer/Producer the result you will get back will be of type `ConsumeResult<long, object>` which you can then either use something like Mediatr or a simple type switch statement to perform actions the typed messages you have received for processing.