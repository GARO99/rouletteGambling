using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rouletteGambling.Models.Entities
{
    public class GamblerEntity
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public decimal Credits { get; set; }
    }
}
