namespace LeaderBoard.Repository;

public interface IRepository
{
    Task<List<Scores>> GetTopScores();
    
    Task<bool> CreateScoreAsync(Scores scores);
}