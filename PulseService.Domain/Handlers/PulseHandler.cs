using PulseService.Domain.Adapters;
using PulseService.Domain.Models;

namespace PulseService.Domain.Handlers
{
    public class PulseHandler : IPulseHandler
    {
        private readonly IPulseRepository _pulseRepository;
        private readonly IUserRepository _userRepository;

        public PulseHandler(IPulseRepository pulseRepository, IUserRepository userRepository)
        {
            _pulseRepository = pulseRepository;
            _userRepository = userRepository;
        }

        public async Task CreatePulseAsync(Pulse pulse)
        {
            await _pulseRepository.AddPulseAsync(pulse);
        }

        public async Task<bool> DeletePulseAsync(string id, string currentUserId)
        {
            return await _pulseRepository.DeletePulseAsync(id, currentUserId);
        }

        public async Task<IEnumerable<Pulse>> GetAllPulsesAsync()
        {
            return await _pulseRepository.GetAllPulsesAsync();
        }

        public async Task<Pulse> GetPulseAsync(string id)
        {
            return await _pulseRepository.GetPulseAsync(id);
        }

        public async Task UpdatePulseVoteAsync(VoteUpdate voteUpdate)
        {
            var currentVote = await _userRepository.GetCurrentPulseVote(voteUpdate.CurrentUserId, voteUpdate.PulseId);
            voteUpdate.UnvotedOpinion = currentVote?.OpinionName;

            if (voteUpdate.VotedOpinion == voteUpdate.UnvotedOpinion)
                return;

            await _userRepository.UpdatePulseVoteAsync(voteUpdate);
            await _pulseRepository.UpdatePulseVoteAsync(voteUpdate);
        }
    }
}
