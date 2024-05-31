using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using votesystem_csharp.Models;

namespace votesystem_csharp.Controllers;
[Route("api/admin")]
[Authorize(Roles = "admin")]
[ApiController]
public class AdminController(ILogger<AdminController> logger, ApplicationContext context, IConfiguration configuration) : BaseController(logger, context, configuration)
{
    class ElectionSummary
    {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }
        public required List<CandidateSummary> Candidates { get; set; }
        public int? VotesCount { get; set; }
    }
    class CandidateSummary
    {
        public required Guid Id { get; set; }
        public required string DisplayName { get; set; }

        public required Guid ElectionId { get; set; }
        public int? VotesCount { get; set; }
    }
    [HttpGet("elections")]
    public async Task<IActionResult> GetElections()
    {
        var elections = await _db.Elections.Include(e => e.Candidates).Include(e => e.Votes).ToListAsync();
        var electionsSummary = elections.ConvertAll(e => new ElectionSummary()
        {
            Id = e.Id,
            Title = e.Title,
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            Candidates = e.Candidates.ConvertAll(c => new CandidateSummary()
            {
                Id = c.Id,
                DisplayName = c.DisplayName,
                ElectionId = c.ElectionId,
                VotesCount = DateTime.Now > e.StartTime ? c.Votes.Count : null
            }),
            VotesCount = DateTime.Now > e.StartTime ? e.Votes.Count : null
        });
        return Json(electionsSummary);
    }
    [HttpPost("elections")]
    public async Task<IActionResult> NewElection([FromForm] string Title, [FromForm] DateTime StartTime, [FromForm] DateTime EndTime)
    {
        if (Title == null || Title == "" || EndTime < StartTime) return BadRequest();
        bool hasConflictingElections = _db.Elections.Any(e =>
            (StartTime >= e.StartTime && StartTime < e.EndTime) ||
            (EndTime > e.StartTime && EndTime <= e.EndTime) ||
            (StartTime <= e.StartTime && EndTime >= e.EndTime));

        if (hasConflictingElections) return BadRequest("Конфликтующее время выборов.");

        Election election = new() { Id = Guid.NewGuid(), Title = Title, StartTime = StartTime, EndTime = EndTime };

        _db.Elections.Add(election);
        await _db.SaveChangesAsync();
        return Json(election);
    }

    [HttpGet("elections/{electionId:guid}")]
    public async Task<IActionResult> GetElection(Guid electionId)
    {
        var election = await _db.Elections.Include(e => e.Candidates).Include(e => e.Votes).FirstAsync(e => e.Id == electionId);
        var electionSummary = new ElectionSummary()
        {
            Id = election.Id,
            Title = election.Title,
            StartTime = election.StartTime,
            EndTime = election.EndTime,
            Candidates = election.Candidates.ConvertAll(c => new CandidateSummary()
            {
                Id = c.Id,
                DisplayName = c.DisplayName,
                ElectionId = c.ElectionId,
                VotesCount = DateTime.Now > election.EndTime ? c.Votes.Count : null
            }),
            VotesCount = DateTime.Now > election.StartTime ? election.Votes.Count : null
        };
        return Json(electionSummary);
    }
    [HttpPut("elections/{ElectionId:guid}")]
    public async Task<IActionResult> EditElection(Guid ElectionId, [FromForm]string Title, [FromForm]DateTime StartTime, [FromForm]DateTime EndTime)
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
        return Ok();
    }

    [HttpDelete("elections/{ElectionId:guid}")]
    public async Task<IActionResult> DeleteElection(Guid ElectionId)
    {
        _db.Elections.Remove(await _db.Elections.FirstAsync(e => e.Id == ElectionId));
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("candidates")]
    public async Task<IActionResult> NewCandidate([FromForm]Guid ElectionId, [FromForm]string DisplayName)
    {
        if (DisplayName == null || DisplayName == "" /* || ElectionId == null */) return BadRequest(); //I'm not sure if it works
        var election = await _db.Elections.FirstAsync(e => e.Id == ElectionId);
        _db.Candidates.Add(new Candidate() { Id = Guid.NewGuid(), DisplayName = DisplayName, ElectionId = ElectionId, Election = election });
        await _db.SaveChangesAsync();
        return Json(election);
    }

    [HttpDelete("candidates")]
    public async Task<IActionResult> DeleteCandidate([FromForm]Guid CandidateId)
    {
        var candidate = await _db.Candidates.FirstAsync(c => c.Id == CandidateId);
        var electionId = candidate.ElectionId;
        _db.Candidates.Remove(candidate);
        await _db.SaveChangesAsync();
        return Json(candidate);
    }
}