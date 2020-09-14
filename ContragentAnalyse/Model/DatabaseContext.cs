﻿using ContragentAnalyse.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContragentAnalyse.Model
{
    public class DatabaseContext : DbContext
    {

#if !DevAtHome
        private const string CONNECTION_STRING = "Server=A105512\\A105512;Database=CounterpartyMonitoring;Integrated Security=false;Trusted_Connection=True;MultipleActiveResultSets=True;User Id = CounterPartyMonitoring_user; Password = orppaAdmin123!";
#else
        private const string CONNECTION_STRING = @"Server=IlyaHome\MyDB;Database=CounterpartyMonitoring;Integrated Security=true;Trusted_Connection=True;MultipleActiveResultSets=True;";
#endif
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            optionsBuilder.UseSqlServer(CONNECTION_STRING);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<Bank>().HasMany(i => i.Actualizations).WithOne(i => i.Bank);
            modelBuilder.Entity<Bank>().HasMany(i => i.PrescoringScoring).WithOne(i => i.Bank);
            modelBuilder.Entity<Bank>().HasMany(i => i.Client).WithOne(i => i.Bank);
            modelBuilder.Entity<Bank>().HasMany(i => i.Contracts).WithOne(i => i.Bank);
            modelBuilder.Entity<Bank>().HasMany(i => i.RestrictedAccounts).WithOne(i => i.Bank);
            modelBuilder.Entity<Bank>().HasMany(i => i.Contacts).WithOne(i => i.Bank);
            modelBuilder.Entity<PrescoringScoring>().HasMany(i => i.CriteriaToScoring).WithOne(i => i.PrescoringScoring);
            modelBuilder.Entity<Criteria>().HasMany(i => i.CriteriaToScoring).WithOne(i => i.Criteria);
            modelBuilder.Entity<Client>().HasMany(i => i.Requests).WithOne(i => i.Client);
        }

        public DatabaseContext() : base()
        {
            Database.EnsureCreated();
        }

        public DbSet<AccountStates> AccountStates { get; set; }
        public DbSet<Actualization> Actualization { get; set; }
        public DbSet<BankProduct> BankProduct { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<Contacts> Contacts { get; set; }
        public DbSet<Contracts> Contracts { get; set; }
        public DbSet<ContactType> ContactType { get; set; }
        public DbSet<Criteria> Criteria { get; set; }
        public DbSet<CriteriaToScoring> CriteriaToScoring { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<Level> Level { get; set; }    
        public DbSet<Positions> Positions { get; set; }
        public DbSet<PrescoringScoring> PrescoringScoring { get; set; }
        public DbSet<ProductToScoring> ProductToScoring { get; set; }
        public DbSet<ResponsibleUnit> ResponsibleUnit { get; set; }
        public DbSet<RestrictedAccounts> RestrictedAccounts { get; set; }
        public DbSet<RiskCodes> RiskCodes { get; set; }
        public DbSet<ScoringType> ScoringType { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<StatusActualization> StatusActualization { get; set; }
        public DbSet<StopFactors> StopFactors { get; set; }
        public DbSet<TypeAgreement> TypeAgreement { get; set; }
        public DbSet<TypeClient> TypeClient { get; set; }
      
    }
}
