using Amazon.DynamoDBv2;

namespace LeaderBoard;

public static class CompositionExtensions
{
    public static void AddDynamoDbRepositories(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var dynamoDbConfig = configuration.GetSection("DynamoDb");
        var runLocalDynamoDb = dynamoDbConfig.GetValue<bool>("LocalMode");

        Console.WriteLine("runLocalDynamoDb : " + runLocalDynamoDb);
        if (runLocalDynamoDb)
        {
            //GET environment variable from OS
            var LOCALSTACK_HOSTNAME = Environment.GetEnvironmentVariable("LOCALSTACK_HOSTNAME");

            var serviceUrl = LOCALSTACK_HOSTNAME is not null
                ? $"http://{LOCALSTACK_HOSTNAME}:4566"
                : dynamoDbConfig.GetValue<string>("LocalServiceUrl");
            
            services.AddSingleton<IAmazonDynamoDB>(sp =>
            {
                var clientConfig = new AmazonDynamoDBConfig
                {
                    ServiceURL = serviceUrl,
                    AuthenticationRegion = "eu-west-1",
                };
                return new AmazonDynamoDBClient(clientConfig);
            });
        }
    }
}