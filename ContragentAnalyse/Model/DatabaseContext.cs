using ContragentAnalyse.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContragentAnalyse.Model
{
    public class DatabaseContext : DbContext
    {

//#if !DevAtHome
        private const string CONNECTION_STRING = "Server=A105512\\A105512;Database=CounterpartyMonitoring;Integrated Security=false;Trusted_Connection=True;MultipleActiveResultSets=True;User Id = CounterPartyMonitoring_user; Password = orppaAdmin123!";
//#else
        //private const string CONNECTION_STRING = @"Server=IlyaHome\MyDB;Database=CounterpartyMonitoringNew;Integrated Security=true;Trusted_Connection=True;MultipleActiveResultSets=True;";
//#endif
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            optionsBuilder.UseSqlServer(CONNECTION_STRING);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<Client>().HasMany(i => i.Actualization).WithOne(i => i.Client);
            modelBuilder.Entity<Client>().HasMany(i => i.RestrictedAccounts).WithOne(i => i.Client);
            modelBuilder.Entity<Client>().HasMany(i => i.Contacts).WithOne(i => i.Client);
            modelBuilder.Entity<Client>().HasMany(i => i.Requests).WithOne(i => i.Client);
            modelBuilder.Entity<Client>().HasMany(i => i.StopFactors).WithOne(i => i.Client);
            modelBuilder.Entity<Client>().HasMany(i => i.ClientToCurrency).WithOne(i => i.Client);
            modelBuilder.Entity<Client>().HasMany(i => i.ClientToContracts).WithOne(i => i.Client);
            modelBuilder.Entity<Client>().HasMany(i => i.Scorings).WithOne(i => i.Client);
            modelBuilder.Entity<ScoringToCriteria>().HasKey(i => new { i.ScoringId, i.CriteriaId });
            modelBuilder.Entity<ScoringToCriteria>().HasOne(i => i.Scoring).WithMany(i => i.Criterias).HasForeignKey(i => i.ScoringId);
            modelBuilder.Entity<ScoringToCriteria>().HasOne(i => i.Criteria).WithMany(i => i.Scorings).HasForeignKey(i => i.CriteriaId);
        }

        public DatabaseContext() : base()
        {
            Database.EnsureCreated();
        }

        public DbSet<AccountStates> AccountStates { get; set; }
        public DbSet<Actualization> Actualization { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<ClientToCurrency> ClientToCurrency { get; set; }
        public DbSet<ClientToContracts> ClientToContracts { get; set; }
        public DbSet<Contacts> Contacts { get; set; }
        public DbSet<Contracts> Contracts { get; set; }
        public DbSet<ContactType> ContactType { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Criteria> Criteria { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<Scoring> Scorings { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Positions> Positions { get; set; }
        public DbSet<ResponsibleUnit> ResponsibleUnit { get; set; }
        public DbSet<RestrictedAccounts> RestrictedAccounts { get; set; }
        public DbSet<RiskCodes> RiskCodes { get; set; }
        public DbSet<StopFactors> StopFactors { get; set; }
        public DbSet<AgreementType> TypeAgreement { get; set; }
        public DbSet<ClientType> TypeClient { get; set; }

      
    }
}