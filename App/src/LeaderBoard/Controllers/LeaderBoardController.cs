using LeaderBoard.Repository;
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

    [HttpPut("scores")]
    public async Task<IActionResult> RecordScore([FromBody]Scores scores)
    {
        var isSuccessful = await _repository.CreateScoreAsync(scores);
        return !isSuccessful ? StatusCode(500) : Ok();
    }
}