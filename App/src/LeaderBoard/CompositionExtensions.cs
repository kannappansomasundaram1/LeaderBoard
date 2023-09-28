using Amazon;
using Amazon.DynamoDBv2;

namespace LeaderBoard;

public static class CompositionExtensions
{
    public static void AddDynamoDbRepositories(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var dynamoDbConfig = configuration.GetSection("DynamoDb");
        var runLocalDynamoDb = dynamoDbConfig.GetValue<bool>("LocalMode");

        var amazonDynamoDbConfig = runLocalDynamoDb
            ? GetLocalDynamoDbConfig(dynamoDbConfig)
            : new AmazonDynamoDBConfig
            {
                RegionEndpoint = RegionEndpoint.EUWest1
            };

        services.AddSingleton<IAmazonDynamoDB>(sp => new AmazonDynamoDBClient(amazonDynamoDbConfig));
    }

    private static AmazonDynamoDBConfig GetLocalDynamoDbConfig(IConfigurationSection dynamoDbConfig)
    {
        //GET environment variable from OS
        var LOCALSTACK_HOSTNAME = Environment.GetEnvironmentVariable("LOCALSTACK_HOSTNAME");

        var serviceUrl = LOCALSTACK_HOSTNAME is not null
            ? $"http://{LOCALSTACK_HOSTNAME}:4566"
            : dynamoDbConfig.GetValue<string>("LocalServiceUrl");
        return new AmazonDynamoDBConfig
        {
            ServiceURL = serviceUrl,
            AuthenticationRegion = "eu-west-1" //Required to connect to specific region in localstack
        };
    }
}