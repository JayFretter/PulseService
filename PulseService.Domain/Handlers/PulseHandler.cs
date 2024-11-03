using PulseService.Domain.Adapters;
using PulseService.Domain.Exceptions;
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

        public Task CreatePulseAsync(Pulse pulse)
        {
            return _pulseRepository.AddPulseAsync(pulse);
        }

        public Task<bool> DeletePulseAsync(string id, string currentUserId)
        {
            return _pulseRepository.DeletePulseAsync(id, currentUserId);
        }

        public Task<IEnumerable<Pulse>> GetAllPulsesAsync()
        {
            return _pulseRepository.GetAllPulsesAsync();
        }

        public async Task<Pulse> GetPulseAsync(string id)
        {
            var pulse = await _pulseRepository.GetPulseAsync(id);
            return pulse ?? throw new MissingDataException($"Failed to find Pulse with ID {id}.");
        }

        public async Task UpdatePulseVoteAsync(VoteUpdate voteUpdate)
        {
            var currentVote = await _userRepository.GetCurrentPulseVote(voteUpdate.CurrentUserId, voteUpdate.PulseId);
            voteUpdate.UnvotedOpinion = currentVote?.OpinionName;

            if (voteUpdate.VotedOpinion == voteUpdate.UnvotedOpinion)
                return;

            var updateTasks = new Task[]
            {
                _userRepository.UpdatePulseVoteAsync(voteUpdate),
                _pulseRepository.UpdatePulseVoteAsync(voteUpdate)
            };

            await Task.WhenAll(updateTasks);
        }
    }
}
