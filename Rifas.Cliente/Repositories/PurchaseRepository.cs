using GCIT.Core.Data;
using Rifas.Client.Entities;
using Rifas.Client.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using GCIT.Core.Extensions;

namespace Rifas.Client.Repositories
{
    public class PurchaseRepository : Repository<PurchaseEntity>, IPurchaseRepository
    {
        public PurchaseRepository(DefaultDBContext context) : base(context)
        {
        }

        
    }
}
