using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using votesystem_csharp.Models;

namespace votesystem_csharp.Controllers;

[Authorize(Policy = "Users")]
public class VoteController(ILogger<VoteController> logger, ApplicationContext context, IConfiguration configuration) : BaseController(logger, context, configuration)
{
    [HttpGet("vote/{electionId:guid}")]
    public async Task<IActionResult> Vote(Guid electionId)
    {
        var election = await _db.Elections.FirstAsync(e => e.Id == electionId);
        if (!election.IsHappening()) return BadRequest("Сейчас нет выборов!");
        var user = await Models.User.GetUser(HttpContext);
        var userId = user!.Id;
        ViewBag.hasVoted = await _db.Votes.FirstOrDefaultAsync(v => v.UserId == userId && v.ElectionId == electionId) != null;
        ViewBag.Candidates = await _db.Candidates.Where(c => c.ElectionId == electionId).ToListAsync();
        return View(election);
    }
    [HttpPost("vote/{electionId:guid}")]
    public async Task<IActionResult> Vote(Guid electionId, Guid CandidateId)
    {
        var election = await _db.Elections.FirstAsync(e => e.Id == electionId);
        if (!election.IsHappening()) return BadRequest("Сейчас нет выборов!");
        var user = await Models.User.GetUser(HttpContext);
        var existingVote = await _db.Votes.FirstOrDefaultAsync(v => v.UserId == user!.Id && v.ElectionId == electionId);
        if (existingVote != null) _db.Votes.Remove(existingVote);
        var candidate = await _db.Candidates.FirstAsync(c => c.Id == CandidateId);
        _db.Votes.Add(new Vote { 
            User = user!, 
            UserId = user!.Id, 
            Candidate = candidate, 
            CandidateId = CandidateId, 
            Election = election, 
            ElectionId = electionId
        });
        await _db.SaveChangesAsync();
        return Redirect("/vote_success");
    }
    [HttpGet("vote_success")]
    public async Task<IActionResult> VoteSuccess()
    {
        ViewBag.CurrentElection = await _db.Elections.FirstOrDefaultAsync(e => e.StartTime < DateTime.Now && DateTime.Now < e.EndTime);
        return View();
    }
}
