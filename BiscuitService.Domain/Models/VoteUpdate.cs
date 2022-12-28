using BiscuitService.Domain.Models.Dtos;

namespace BiscuitService.Domain.Models
{
    public class VoteUpdate
    {
        public string BiscuitId { get; set; } = string.Empty;
        public string OptionName { get; set; } = string.Empty;
        public string? PreviousVoteOptionName { get; set; }
        public UserDto CurrentUser { get; set; } = new UserDto();
    }
}
