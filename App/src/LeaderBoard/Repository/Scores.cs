using Amazon.DynamoDBv2.DataModel;

namespace LeaderBoard.Repository;

[DynamoDBTable("leaderboard")]
public record Scores
{
    [DynamoDBHashKey("Pk")] 
    public string Pk => GetPk(Period, Game);

    public static string GetPk(string period, string game)
    {
        return $"{period}#{game}";
    }

    [DynamoDBRangeKey("Score")]
    public int Score { get; init; }
    
    [DynamoDBProperty("UserId")]
    public string UserId { get; init; }
    
    [DynamoDBProperty("UserName")]
    public string UserName { get; init; }
    
    public string Game { get; init; }

    public string Period { get; init; }
    
    public LeaderBoard.ApiModels.Scores FromDto()
    {
        return new LeaderBoard.ApiModels.Scores
        {
            Score = Score,
            UserId = UserId,
            UserName = UserName,
            Game = Game,
            Period = Period
        };
    }
}