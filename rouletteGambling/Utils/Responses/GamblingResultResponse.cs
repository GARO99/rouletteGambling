using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rouletteGambling.Utils.Responses
{
    public class GamblingResultResponse
    {
        public string GamblerId { get; set; }
        public string GamblerFullName { get; set; }
        public decimal CreditsBet { get; set; }
        public string BetType { get; set; }
        public int? BetNumber { get; set; }
        public string BetColor { get; set; }
        public bool WontBet { get; set; }
    }
}
