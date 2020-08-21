using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rouletteGambling.Models.Entities
{
    public class BetEntity
    {
        public int Id { get; set; }
        public int RouletteId { get; set; }
        public bool Status { get; set; }

    }
}
