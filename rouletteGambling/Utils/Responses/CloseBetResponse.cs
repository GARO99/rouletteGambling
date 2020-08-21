using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rouletteGambling.Utils.Responses
{
    public class CloseBetResponse
    {
        public BetResultResponse BetResult { get; set; }
        public List<GamblingResultResponse> GamblingResult { get; set; }
    }
}
