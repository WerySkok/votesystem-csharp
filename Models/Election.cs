using System.ComponentModel.DataAnnotations;

namespace votesystem_csharp.Models;

public class Election
{
    [Key]
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public List<Candidate> Candidates { get; set; } = new();
    public List<Vote> Votes { get; set; } = new();
}