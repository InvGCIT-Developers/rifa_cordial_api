using GCIT.Core.Data;
using Rifas.Client.Entities;
using Rifas.Client.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rifas.Client.Repositories
{
    public class CategoryRepository : Repository<CategoryEntity>, ICategoryRepository
    {
        public CategoryRepository(DefaultDBContext context) : base(context)
        {
        }
    }
}
