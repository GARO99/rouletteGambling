using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rouletteGambling.Models
{
    public class GamblingModel
    {
        private readonly IDistributedCache distributedCache;

        public GamblingModel(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }
    }
}
