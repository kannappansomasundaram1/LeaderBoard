using Amazon.DynamoDBv2.DataModel;

namespace LeaderBoard.Repository;

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