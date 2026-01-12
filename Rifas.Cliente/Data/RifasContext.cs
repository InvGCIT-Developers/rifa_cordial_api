using GCIT.Core.Data;
using Microsoft.EntityFrameworkCore;
using Rifas.Client.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rifas.Client.Data
{
    public class RifasContext : DefaultDBContext
    {
        public RifasContext(DbContextOptions<RifasContext> options) : base(options)
        {
        }
        public RifasContext() : base()
        {

        }


        public virtual DbSet<TransactionsEntity> Transactions { get; set; }
        public virtual DbSet<RaffleEntity> Raffles { get; set; }
        public virtual DbSet<TicketsEntity> Tickets { get; set; }
        public virtual DbSet<ResultsEntity> Results { get; set; }
        public virtual DbSet<PurchaseEntity> Purchases { get; set; }
        public virtual DbSet<CategoryEntity> Categories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PurchaseEntity>()
            .HasMany(p => p.Tickets)
            .WithOne(t => t.Purchase)
            .HasForeignKey(t => t.PurchaseId)
            .OnDelete(DeleteBehavior.Restrict); // o .Cascade si prefieres borrar tickets al borrar compra

            // índice único: TicketNumber por Raffle
            modelBuilder.Entity<TicketsEntity>()
                .HasIndex(t => new { t.RaffleId, t.TicketNumber })
                .IsUnique();

            // Configurar relación entre Raffle y Category usando la propiedad FK 'Category'
            modelBuilder.Entity<RaffleEntity>()
                .HasOne(r => r.CategoryEntity)
                .WithMany()
                .HasForeignKey(r => r.Category)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RifasContext).Assembly);
        }
    }
}
