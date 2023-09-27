using System.Text.Json;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

namespace LeaderBoard.Controllers;

[DynamoDBTable("leaderboard")]
public record Scores
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

public static class ItemUtils
{
    public static T ToDto<T>(this Dictionary<string, AttributeValue> item)
    {
        var itemAsDocument = Document.FromAttributeMap(item);
        var dto = JsonSerializer.Deserialize<T>(itemAsDocument.ToJson());
        return dto;
    }
    
    public static Dictionary<string, AttributeValue> ToItem<T>(this T dto)
    {
        var itemAsJson = JsonSerializer.Serialize(dto);
        var itemAsDocument = Document.FromJson(itemAsJson);
        return itemAsDocument.ToAttributeMap();
    }
}