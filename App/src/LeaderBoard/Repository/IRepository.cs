namespace LeaderBoard.Repository;

public interface IRepository
{
    Task<bool> CreateScoreAsync(Scores scores);
    Task<List<Scores>> GetTopScores(string period, string game);
}