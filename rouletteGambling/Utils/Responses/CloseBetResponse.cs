using System.Collections.Generic;

namespace rouletteGambling.Utils.Responses
{
    public class CloseBetResponse
    {
        public BetResultResponse BetResult { get; set; }
        public List<GamblingResultResponse> GamblingResult { get; set; }
    }
}
