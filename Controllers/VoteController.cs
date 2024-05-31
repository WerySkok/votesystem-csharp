using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using votesystem_csharp.Models;

namespace votesystem_csharp.Controllers;

[Route("api/vote")]
[Authorize(Roles = "voter,admin")]
[ApiController]
public class VoteController(ILogger<VoteController> logger, ApplicationContext context, IConfiguration configuration) : BaseController(logger, context, configuration)
{
    class VotingBallot
    {
        public required Election Election { get; set; }
        public required List<Candidate> Candidates { get; set; }
        public required bool HasVoted { get; set; }
    }
    [HttpGet("{electionId:guid}")]
    public async Task<IActionResult> Vote(Guid electionId)
    {
        var election = await _db.Elections.FirstAsync(e => e.Id == electionId);
        if (!election.IsHappening()) return BadRequest("Сейчас нет выборов!");
        var user = await Models.User.GetUserByContext(HttpContext);
        var userId = user!.Id;
        return Json(new VotingBallot()
        {
            Election = election,
            Candidates = await _db.Candidates.Where(c => c.ElectionId == electionId).ToListAsync(),
            HasVoted = await _db.Votes.FirstOrDefaultAsync(v => v.UserId == userId && v.ElectionId == electionId) != null
        });
    }
    [HttpPost("{electionId:guid}")]
    public async Task<IActionResult> Vote(Guid electionId, [FromForm] Guid CandidateId)
    {
        var election = await _db.Elections.FirstAsync(e => e.Id == electionId);
        if (!election.IsHappening()) return BadRequest("Сейчас нет выборов!");
        var user = await Models.User.GetUserByContext(HttpContext);
        var existingVote = await _db.Votes.FirstOrDefaultAsync(v => v.UserId == user!.Id && v.ElectionId == electionId);
        if (existingVote != null) _db.Votes.Remove(existingVote);
        var candidate = await _db.Candidates.FirstAsync(c => c.Id == CandidateId);
        _db.Votes.Add(new Vote
        {
            User = user!,
            UserId = user!.Id,
            Candidate = candidate,
            CandidateId = CandidateId,
            Election = election,
            ElectionId = electionId
        });
        await _db.SaveChangesAsync();
        return Ok();
    }
}
