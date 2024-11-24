using PulseService.Domain.Models;

namespace PulseService.Domain.Validation
{
    public interface IArgumentValidationService
    {
        public string[] GetValidationErrorsForNewArgument(DiscussionArgument argument);
    }
}