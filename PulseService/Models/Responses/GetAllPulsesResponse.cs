namespace PulseService.Models.Responses
{
    public class GetAllPulsesResponse
    {
        public IEnumerable<PulseExternal> Pulses { get; set; } = new List<PulseExternal>();
    }
}
