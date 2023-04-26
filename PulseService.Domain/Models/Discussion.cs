using System;

namespace PulseService.Domain.Models
{
    public class Discussion
    {
        public string Id { get; set; } = string.Empty;
        public string PulseId { get; set; } = string.Empty;
        public IEnumerable<OpinionThread> OpinionThreads { get; set; } = new List<OpinionThread>();
    }
}
