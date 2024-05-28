using System.ComponentModel.DataAnnotations;

namespace votesystem_csharp.Models;

public class Candidate
{
    [Key]
    public required Guid Id { get; set; }
    public required string DisplayName { get; set; }

    public required Election Election { get; set; }
    public required Guid ElectionId { get; set; }
    
    public List<Vote> Votes { get; set; } = [];
}