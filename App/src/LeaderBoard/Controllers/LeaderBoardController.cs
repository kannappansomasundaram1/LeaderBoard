using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Mvc;

namespace LeaderBoard.Controllers;

[ApiController]
[Route("[controller]")]
public class LeaderBoardController : ControllerBase
{
    private readonly ILogger<LeaderBoardController> _logger;
    private readonly IRepository _repository;

    public LeaderBoardController(ILogger<LeaderBoardController> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpGet("topscores")]
    public async Task<IActionResult> TopScores()
    {
        var topScores = await _repository.GetTopScores();
        return Ok(topScores);
    }

    [HttpPost("scores")]
    public async Task<IActionResult> RecordScore([FromBody]Scores scores)
    {
        await _repository.CreateScoreAsync(scores);
        return Ok();
    }
}

public interface IRepository
{
    Task<List<Scores>> GetTopScores();
    
    Task<bool> CreateScoreAsync(Scores scores);
}

public class DynamoDbRepository : IRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName = "leaderboard";

    public DynamoDbRepository(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
    }
    public async Task<List<Scores>> GetTopScores()
    {
        var results = new List<Scores>();
        var request = new QueryRequest
        {
            TableName = _tableName,
            KeyConditionExpression = "Pk = :pk ",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                {":pk", new AttributeValue { S =  $"CHESS#{DateOnly.FromDateTime(DateTime.Now):yyyy-MM}" }}
            },
            ScanIndexForward = false
        };

        var response = await _dynamoDb.QueryAsync(request);

        foreach (var item in response.Items)
        {
            var itemAsDocument = Document.FromAttributeMap(item);
            var score = JsonSerializer.Deserialize<Scores>(itemAsDocument.ToJson());
            if (score!=null)
                results.Add(score);
        }
        return results;
    }

    public async Task<bool> CreateScoreAsync(Scores scores)
    {
        var customerAsJson = JsonSerializer.Serialize(scores);
        var itemAsDocument = Document.FromJson(customerAsJson);
        var itemAsAttributes = itemAsDocument.ToAttributeMap();

        var createItemRequest = new PutItemRequest
        {
            TableName = _tableName,
            Item = itemAsAttributes
        };
        var response = await _dynamoDb.PutItemAsync(createItemRequest);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }
}