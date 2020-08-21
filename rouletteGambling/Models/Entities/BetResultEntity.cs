using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rouletteGambling.Models.Entities
{
    public class BetResultEntity
    {
        public int BetId { get; set; }
        public int Number { get; set; }
        public int Color { get; set; }
    }
}
