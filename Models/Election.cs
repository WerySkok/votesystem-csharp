using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace votesystem_csharp.Models;

public class Election
{
    [Key]
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    [JsonIgnore]
    public List<Candidate> Candidates { get; set; } = [];
    [JsonIgnore]
    public List<Vote> Votes { get; set; } = [];
    public bool IsHappening()
    {
        return StartTime < DateTime.Now && DateTime.Now < EndTime;
    }
}