using LeaderBoard.Repository;
using Microsoft.AspNetCore.Mvc;
using LeaderBoard.ApiModels;
using Scores = LeaderBoard.ApiModels.Scores;

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

    [HttpGet("topscores/{period}/{game}")]
    public async Task<IActionResult> TopScores(string period, string game)
    {
        var topScores = await _repository.GetTopScores(period, game);
        return Ok(topScores.Select(x=> x.FromDto()));
    }

    [HttpPut("scores")]
    public async Task<IActionResult> RecordScore([FromBody]Scores scores)
    {
        var isSuccessful = await _repository.CreateScoreAsync(scores.ToDto());
        return !isSuccessful ? StatusCode(500) : Ok();
    }
}