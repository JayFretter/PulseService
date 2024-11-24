using System.Threading;
using PulseService.Domain.Adapters;
using PulseService.Domain.Exceptions;
using PulseService.Domain.Models;

namespace PulseService.Domain.Handlers;

public class PulseHandler : IPulseHandler
{
    private readonly IPulseRepository _pulseRepository;
    private readonly IUserRepository _userRepository;

    public PulseHandler(IPulseRepository pulseRepository, IUserRepository userRepository)
    {
        _pulseRepository = pulseRepository;
        _userRepository = userRepository;
    }

    public Task CreatePulseAsync(Pulse pulse, CancellationToken cancellationToken)
    {
        return _pulseRepository.AddPulseAsync(pulse, cancellationToken);
    }

    public Task<bool> DeletePulseAsync(string id, string currentUserId, CancellationToken cancellationToken)
    {
        return _pulseRepository.DeletePulseAsync(id, currentUserId, cancellationToken);
    }

    public Task<IEnumerable<Pulse>> GetAllPulsesAsync(CancellationToken cancellationToken)
    {
        return _pulseRepository.GetAllPulsesAsync(cancellationToken);
    }

    public async Task<Pulse> GetPulseAsync(string id, CancellationToken cancellationToken)
    {
        var pulse = await _pulseRepository.GetPulseAsync(id, cancellationToken);
        return pulse ?? throw new MissingDataException($"Failed to find Pulse with ID {id}.");
    }

    public async Task UpdatePulseVoteAsync(VoteUpdate voteUpdate, CancellationToken cancellationToken)
    {
        var currentVote = await _userRepository.GetCurrentPulseVoteAsync(voteUpdate.CurrentUserId, voteUpdate.PulseId, cancellationToken);
        voteUpdate.UnvotedOpinion = currentVote?.OpinionName;

        if (voteUpdate.VotedOpinion == voteUpdate.UnvotedOpinion)
            return;

        var updateTasks = new Task[]
        {
            _userRepository.UpdatePulseVoteAsync(voteUpdate, cancellationToken),
            _pulseRepository.UpdatePulseVoteAsync(voteUpdate, cancellationToken)
        };

        await Task.WhenAll(updateTasks);
    }

    public async Task<PulseVote?> GetCurrentVoteForUser(string pulseId, string username, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUsernameAsync(username, cancellationToken);
        if (user == null)
        {
            throw new MissingDataException($"Failed to find user {username} when getting Pulse vote for user.");
        }
            
        return await _userRepository.GetCurrentPulseVoteAsync(user.Id, pulseId, cancellationToken);
    }
}