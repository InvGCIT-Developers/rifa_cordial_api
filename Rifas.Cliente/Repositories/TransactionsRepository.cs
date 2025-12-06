using GCIT.Core.Data;
using Rifas.Client.Entities;
using Rifas.Client.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rifas.Client.Repositories
{
    public class TransactionsRepository : Repository<TransactionsEntity>, ITransactionsRepository
    {
        public TransactionsRepository(DefaultDBContext context) : base(context)
        {
        }
    }
}
