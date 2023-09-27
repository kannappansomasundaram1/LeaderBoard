using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using static LeaderBoard.Controllers.ItemUtils;

namespace LeaderBoard.Controllers;

public class DynamoDbRepository : IRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly ILogger<DynamoDbRepository> _logger;
    private const string TableName = "leaderboard";

    public DynamoDbRepository(IAmazonDynamoDB dynamoDb, ILogger<DynamoDbRepository> logger)
    {
        _dynamoDb = dynamoDb;
        _logger = logger;
    }
    public async Task<List<Scores>> GetTopScores()
    {
        var request = new QueryRequest
        {
            TableName = TableName,
            KeyConditionExpression = "Pk = :pk ",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                {":pk", new AttributeValue { S =  $"CHESS#{DateOnly.FromDateTime(DateTime.Now):yyyy-MM}" }}
            },
            ScanIndexForward = false
        };

        var response = await _dynamoDb.QueryAsync(request);

        return response.Items
            .Where(x => x != null)
            .Select(item => item.ToDto<Scores>())
            .ToList();
    }

    public async Task<bool> CreateScoreAsync(Scores scores)
    {
        var createItemRequest = new PutItemRequest
        {
            TableName = TableName,
            Item = scores.ToItem()
        };
        try
        {
            var response = await _dynamoDb.PutItemAsync(createItemRequest);
            return response.HttpStatusCode == HttpStatusCode.OK;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to save the score");
            return false;
        }
    }
}