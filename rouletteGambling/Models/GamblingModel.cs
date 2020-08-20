﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rouletteGambling.Models
{
    public class GamblingModel
    {
        public int RouletteId { get; set; }
        public string GamblerId { get; set; }
        public int BetType { get; set; }
        public int BetNumber { get; set; }
        public int BetColor { get; set; }
    }
}