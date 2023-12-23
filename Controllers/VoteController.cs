using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using votesystem_csharp.Models;

namespace votesystem_csharp.Controllers;

[Authorize(Policy = "Users")]
public class VoteController : Controller
{
    private readonly ILogger<VoteController> _logger;
    private readonly ApplicationContext _db;
    private readonly IConfiguration _configuration;
    public VoteController(ILogger<VoteController> logger, ApplicationContext context, IConfiguration configuration)
    {
        _logger = logger;
        _db = context;
        _configuration = configuration;
    }

    [HttpGet("vote/{electionId:guid}")]
    public async Task<IActionResult> Vote(Guid electionId)
    {
        var user = await Models.User.GetUser(HttpContext);
        var userId = user!.Id;
        ViewBag.hasVoted = await _db.Votes.FirstOrDefaultAsync(v => v.UserId == userId && v.ElectionId == electionId) != null;
        var elections = await _db.Elections.FirstAsync(e => e.Id == electionId);
        ViewBag.Candidates = await _db.Candidates.Where(c => c.ElectionId == electionId).ToListAsync();
        return View(elections);
    }
    [HttpPost("vote/{electionId:guid}")]
    public async Task<IActionResult> Vote(Guid electionId, Guid CandidateId)
    {
        var user = await Models.User.GetUser(HttpContext);
        var existingVote = await _db.Votes.FirstOrDefaultAsync(v => v.UserId == user!.Id && v.ElectionId == electionId);
        if (existingVote != null) _db.Votes.Remove(existingVote);
        var election = await _db.Elections.FirstAsync(e => e.Id == electionId);
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
    public async Task<IActionResult> VoteSuccessAsync()
    {
        ViewBag.CurrentElection = await _db.Elections.FirstOrDefaultAsync(e => e.StartTime < DateTime.Now && DateTime.Now < e.EndTime);
        return View();
    }
}
