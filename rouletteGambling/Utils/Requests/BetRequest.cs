using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rouletteGambling.Utils.Requests
{
    public class BetRequest
    {
        public int RouletteId { get; set; }
        public decimal CreditsBet { get; set; }
        public int BetType { get; set; }
        public int? BetNumber { get; set; }
        public int? BetColor { get; set; }
    }
}
