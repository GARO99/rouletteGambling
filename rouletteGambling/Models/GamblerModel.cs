using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rouletteGambling.Models
{
    public class GamblerModel
    {
        private readonly IDistributedCache distributedCache;

        public GamblerModel(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }
    }
}
