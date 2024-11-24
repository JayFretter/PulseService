using System;

namespace PulseService.Domain.Models;

public class Discussion
{
    public IEnumerable<OpinionThread> OpinionThreads { get; set; } = new List<OpinionThread>();
}