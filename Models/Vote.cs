namespace votesystem_csharp.Models;

public class Vote
{
    public required User User { get; set; }
    public required Guid UserId { get; set; }

    public required Candidate Candidate { get; set; }
    public required Guid CandidateId { get; set; }

    public required Election Election { get; set; }
    public required Guid ElectionId { get; set; }
}