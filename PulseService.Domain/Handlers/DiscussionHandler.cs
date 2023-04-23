using PulseService.Domain.Adapters;
using PulseService.Domain.Models;

namespace PulseService.Domain.Handlers
{
    public class DiscussionHandler : IDiscussionHandler
    {
        private readonly IDiscussionRepository _discussionRepository;
        private readonly IPulseRepository _pulseRepository;
        public DiscussionHandler(IDiscussionRepository discussionRepository, IPulseRepository pulseRepository)
        { 
            _discussionRepository = discussionRepository;
            _pulseRepository = pulseRepository;
        }

        public async Task CreateDiscussionCommentAsync(DiscussionComment discussionComment)
        {
            if (await _pulseRepository.GetPulseAsync(discussionComment.PulseId) is not null)
            {
                await _discussionRepository.AddDiscussionCommentAsync(discussionComment, CancellationToken.None);
            }
        }
    }
}
