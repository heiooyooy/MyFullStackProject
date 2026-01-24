1. dotnet ef migrations add InitialCreate --project ../Infrastructure --context MyDbContext

2. vite.config.ts will be used by all vite commands but the server(proxy) section will only be used when running in dev mode.

3. Kinaba is the official UI for ElasticSearch. - <http://localhost/kibana/app/home#/>

4. run [guid]::NewGuid().ToString() in terminal in Windows to generate a new guid for Kafka Cluster_ID

5. Manually create topic in Kafka after on-line
docker exec kafka kafka-topics --bootstrap-server kafka:29092 --create --if-not-exists --topic seckill_events --replication-factor 1 --partitions 1

6. Seq - <http://localhost/seq> 

7. By adding OpenTelmetry support, now the Seq will display more detailed/extra information to the original log, for example, how long the request was taken to finish.