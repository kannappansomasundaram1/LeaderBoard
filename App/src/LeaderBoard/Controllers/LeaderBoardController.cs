using LeaderBoard.Repository;
using Microsoft.AspNetCore.Mvc;
using Scores = LeaderBoard.ApiModels.Scores;

namespace LeaderBoard.Controllers;

[ApiController]
[Route("[controller]")]
public class LeaderBoardController : ControllerBase
{
    private readonly IRepository _repository;

    public LeaderBoardController(IRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("topscores/{game}/{yearMonth}")]
    public async Task<IActionResult> TopScores(string game, string yearMonth)
    {
        var topScores = await _repository.GetTopScores(game, yearMonth);
        return Ok(topScores.Select(x=> x.FromDto()));
    }

    [HttpPut("scores")]
    public async Task<IActionResult> RecordScore([FromBody]Scores scores)
    {
        var isSuccessful = await _repository.CreateScoreAsync(scores.ToDto());
        return !isSuccessful ? StatusCode(500) : Ok();
    }
}