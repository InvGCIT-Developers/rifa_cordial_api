using GCIT.Core.Data;
using Rifas.Client.Entities;
using Rifas.Client.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rifas.Client.Repositories
{
    public class RaffleRepository : Repository<RaffleEntity>, IRaffleRepository
    {
        public RaffleRepository(DefaultDBContext context) : base(context)
        {
        }
    }
}
