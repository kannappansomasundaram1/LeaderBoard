namespace LeaderBoard.ApiModels;

public record Scores
{
    public string Game { get; init; }

    public string YearMonth { get; init; }

    public int Score { get; init; }

    public string UserId { get; init; }

    public string UserName { get; set; }

    public LeaderBoard.Repository.Scores ToDto()
    {
        return new Repository.Scores
        {
            Score = Score,
            UserId = UserId,
            UserName = UserName,
            Game = Game,
            YearMonth = YearMonth
        };
    }
}