namespace BiscuitService.Models.Responses
{
    public class GetAllBiscuitsResponse
    {
        public IEnumerable<BiscuitExternal> Biscuits { get; set; } = new List<BiscuitExternal>();
    }
}
