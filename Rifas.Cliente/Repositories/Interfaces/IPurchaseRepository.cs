using GCIT.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Rifas.Client.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Rifas.Client.Repositories.Interfaces
{
    public interface IPurchaseRepository: IRepository<PurchaseEntity>
    {
    }
}
