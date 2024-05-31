using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace votesystem_csharp.Models;

public class Candidate
{
    [Key]
    public required Guid Id { get; set; }
    public required string DisplayName { get; set; }
    
    public required Election Election { get; set; }
    public required Guid ElectionId { get; set; }
    [JsonIgnore]
    public List<Vote> Votes { get; set; } = [];
}