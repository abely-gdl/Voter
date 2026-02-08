using AutoMapper;
using VoterAPI.DTOs;
using VoterAPI.Models;

namespace VoterAPI.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

        // Board mappings
        CreateMap<Board, BoardDto>()
            .ForMember(dest => dest.CreatedByUsername, opt => opt.MapFrom(src => src.CreatedBy != null ? src.CreatedBy.Username : null))
            .ForMember(dest => dest.VotingType, opt => opt.MapFrom(src => src.VotingType.ToString()))
            .ForMember(dest => dest.SuggestionCount, opt => opt.MapFrom(src => src.Suggestions.Count(s => s.Status == SuggestionStatus.Approved && s.IsVisible)))
            .ForMember(dest => dest.TotalVotes, opt => opt.MapFrom(src => src.Suggestions.Where(s => s.Status == SuggestionStatus.Approved && s.IsVisible).SelectMany(s => s.Votes).Count()));

        CreateMap<Board, BoardDetailDto>()
            .ForMember(dest => dest.CreatedByUsername, opt => opt.MapFrom(src => src.CreatedBy != null ? src.CreatedBy.Username : null))
            .ForMember(dest => dest.VotingType, opt => opt.MapFrom(src => src.VotingType.ToString()))
            .ForMember(dest => dest.Suggestions, opt => opt.MapFrom(src => src.Suggestions));

        CreateMap<BoardCreateDto, Board>()
            .ForMember(dest => dest.VotingType, opt => opt.MapFrom(src => Enum.Parse<VotingType>(src.VotingType)));

        // Suggestion mappings
        CreateMap<Suggestion, SuggestionDto>()
            .ForMember(dest => dest.SubmittedByUsername, opt => opt.MapFrom(src => src.SubmittedBy != null ? src.SubmittedBy.Username : null))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Suggestion, SuggestionWithVotesDto>()
            .ForMember(dest => dest.SubmittedByUsername, opt => opt.MapFrom(src => src.SubmittedBy != null ? src.SubmittedBy.Username : null))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.VoteCount, opt => opt.MapFrom(src => src.Votes.Count))
            .ForMember(dest => dest.UserHasVoted, opt => opt.Ignore()); // Set manually in service

        CreateMap<SuggestionCreateDto, Suggestion>();

        // Vote mappings
        CreateMap<Vote, VoteDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.Username : null));
    }
}
