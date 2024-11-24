namespace PulseService.Domain.Exceptions;

public class MissingDataException : PulseException
{
    public MissingDataException()
    {
    }

    public MissingDataException(string message)
        : base(message)
    {
    }

    public MissingDataException(string message, Exception inner)
        : base(message, inner)
    {
    }
}