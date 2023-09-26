using System.Text.Json;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

namespace LeaderBoard.Controllers;

[DynamoDBTable("leaderboard")]
public record Scores : ItemBase<Scores>
{
    [DynamoDBHashKey("Pk")] 
    public string Pk { get; } = $"CHESS#{DateOnly.FromDateTime(DateTime.Now):yyyy-MM}";
    
    [DynamoDBRangeKey("Score")]
    public int Score { get; init; }
    
    [DynamoDBProperty("UserId")]
    public string UserId { get; init; }
    
    [DynamoDBProperty("UserName")]
    public string UserName { get; set; }    
    
}

public record ItemBase<T>
{
    public static T ToDto(Dictionary<string, AttributeValue> item)
    {
        var itemAsDocument = Document.FromAttributeMap(item);
        var dto = JsonSerializer.Deserialize<T>(itemAsDocument.ToJson());
        return dto;
    }
}