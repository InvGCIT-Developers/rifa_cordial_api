using GCIT.Core.Data;
using Microsoft.EntityFrameworkCore;
using Rifas.Client.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rifas.Client.Data
{
    public class RifasContext: DefaultDBContext
    {
        public RifasContext(DbContextOptions<RifasContext> options) : base(options)
        {
        }
        public RifasContext():base()
        {
            
        }


        public virtual DbSet<TransactionsEntity> Transactions { get; set; }
        public virtual DbSet<RaffleEntity> Raffles { get; set; }
        public virtual DbSet<TicketsEntity> Tickets { get; set; }
        public virtual DbSet<ResultsEntity> Results { get; set; }
        public virtual DbSet<PurchaseEntity> Purchases { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {             
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RifasContext).Assembly);
        }
    }
}
