namespace VoterAPI.Models;

public enum UserRole
{
    User,
    Admin
}

public enum VotingType
{
    Single,
    Multiple
}

public enum SuggestionStatus
{
    Pending,
    Approved,
    Rejected
}
