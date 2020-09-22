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
            modelBuilder.Entity<Client>().HasMany(i => i.Actualizations).WithOne(i => i.Client);
            modelBuilder.Entity<Client>().HasMany(i => i.PrescoringScoring).WithOne(i => i.Client);
            modelBuilder.Entity<Client>().HasMany(i => i.Contracts).WithOne(i => i.Client);
            modelBuilder.Entity<Client>().HasMany(i => i.RestrictedAccounts).WithOne(i => i.Client);
            modelBuilder.Entity<Client>().HasMany(i => i.Contacts).WithOne(i => i.Client);
            modelBuilder.Entity<Client>().HasMany(i => i.Requests).WithOne(i => i.Client);
            modelBuilder.Entity<Client>().HasMany(i => i.StopFactors).WithOne(i => i.Client);
            modelBuilder.Entity<Client>().HasMany(i => i.ClientToCrineria).WithOne(i => i.Client);


            /* modelBuilder.Entity<PrescoringScoring>().HasMany(i => i.CriteriaToScoring).WithOne(i => i.PrescoringScoring);
             modelBuilder.Entity<Criteria>().HasMany(i => i.CriteriaToScoring).WithOne(i => i.Criteria);*/
        }

        public DatabaseContext() : base()
        {
           
            Database.EnsureCreated();
           /* if (Client.Count() == 0)
            {
                //Создать тестовые денные
                Client.BIN.;
            }*/
        }

        public DbSet<AccountStates> AccountStates { get; set; }
        public DbSet<Actualization> Actualization { get; set; }
        public DbSet<BankProduct> BankProduct { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<ClientToCrineria> ClientToCrineria { get; set; }
        public DbSet<Contacts> Contacts { get; set; }
        public DbSet<Contracts> Contracts { get; set; }
        public DbSet<ContactType> ContactType { get; set; }
        public DbSet<Criteria> Criteria { get; set; }
       
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
