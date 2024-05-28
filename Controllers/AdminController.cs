using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using votesystem_csharp.Models;

namespace votesystem_csharp.Controllers;
[Route("admin")]
[Authorize(Policy = "Admins")]
public class AdminController(ILogger<AdminController> logger, ApplicationContext context, IConfiguration configuration) : BaseController(logger, context, configuration)
{
    [Route("")]
    public async Task<IActionResult> Index()
    {
        var elections = await _db.Elections.Include(e => e.Candidates).Include(e => e.Votes).ToListAsync();
        return View(elections);
    }

    [HttpGet("new_election")]
    public IActionResult NewElection()
    {
        return View();
    }
    [HttpPost("new_election")]
    public async Task<IActionResult> NewElection(string Title, DateTime StartTime, DateTime EndTime)
    {
        if (Title == null || Title == "" || EndTime < StartTime) return BadRequest();
        bool hasConflictingElections = _db.Elections.Any(e =>
            (StartTime >= e.StartTime && StartTime < e.EndTime) ||
            (EndTime > e.StartTime && EndTime <= e.EndTime) ||
            (StartTime <= e.StartTime && EndTime >= e.EndTime));

        if (hasConflictingElections) return BadRequest("Конфликтующее время выборов.");

        _db.Elections.Add(new Election() { Id = Guid.NewGuid(), Title = Title, StartTime = StartTime, EndTime = EndTime });
        await _db.SaveChangesAsync();
        return Redirect("/admin");
    }

    [HttpGet("edit/{electionId:guid}")]
    public async Task<IActionResult> EditElection(Guid electionId)
    {
        ViewBag.Candidates = await _db.Candidates.Where(c => c.ElectionId == electionId).Include(c => c.Votes).ToListAsync();
        return View(await _db.Elections.FirstAsync(e => e.Id == electionId));
    }
    [HttpPost("edit/{ElectionId:guid}")]
    public async Task<IActionResult> EditElection(Guid ElectionId, string Title, DateTime StartTime, DateTime EndTime)
    {
        if (Title == null || Title == "" || EndTime < StartTime) return BadRequest();
        bool hasConflictingElections = _db.Elections.Any(e =>
            e.Id != ElectionId && ((StartTime >= e.StartTime && StartTime < e.EndTime) ||
            (EndTime > e.StartTime && EndTime <= e.EndTime) ||
            (StartTime <= e.StartTime && EndTime >= e.EndTime)));

        if (hasConflictingElections) return BadRequest("Конфликтующее время выборов.");
        var election = await _db.Elections.FirstAsync(e => e.Id == ElectionId);
        election.Title = Title;
        election.StartTime = StartTime;
        election.EndTime = EndTime;
        await _db.SaveChangesAsync();
        return Redirect("/admin");
    }
    [HttpPost("delete/{ElectionId:guid}")]
    public async Task<IActionResult> DeleteElection(Guid ElectionId)
    {
        _db.Elections.Remove(await _db.Elections.FirstAsync(e => e.Id == ElectionId));
        await _db.SaveChangesAsync();
        return Redirect("/admin");
    }

    [HttpPost("new_candidate")]
    public async Task<IActionResult> NewCandidate(Guid ElectionId, string DisplayName)
    {
        if (DisplayName == null || DisplayName == "" || ElectionId == null) return BadRequest();
        var election = await _db.Elections.FirstAsync(e => e.Id == ElectionId);
        _db.Candidates.Add(new Candidate() { Id = Guid.NewGuid(), DisplayName = DisplayName, ElectionId = ElectionId, Election = election });
        await _db.SaveChangesAsync();
        return Redirect($"/admin/edit/{ElectionId}");
    }

    [HttpPost("delete_candidate")]
    public async Task<IActionResult> DeleteCandidate(Guid CandidateId)
    {
        var candidate = await _db.Candidates.FirstAsync(c => c.Id == CandidateId);
        var electionId = candidate.ElectionId;
        _db.Candidates.Remove(candidate);
        await _db.SaveChangesAsync();
        return Redirect($"/admin/edit/{electionId}");
    }
}