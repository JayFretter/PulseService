namespace PulseService.Domain.Exceptions;

public abstract class PulseException : Exception
{
    public PulseException()
    {
    }

    public PulseException(string message)
        : base(message)
    {
    }

    public PulseException(string message, Exception inner)
        : base(message, inner)
    {
    }
}