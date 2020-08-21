using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rouletteGambling.Utils.Requests
{
    public class CloseBetRequest
    {
        public int BetResultNumber { get; set; }
        public int BetResultColor { get; set; }
    }
}
