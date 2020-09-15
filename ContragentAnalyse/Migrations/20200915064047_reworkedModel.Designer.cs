﻿// <auto-generated />
using System;
using ContragentAnalyse.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ContragentAnalyse.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20200915064047_reworkedModel")]
    partial class ReworkedModel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.AccountStates", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AccountStates");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.Actualization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Client_Id")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateActEKS")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateEKS")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status_Id")
                        .HasColumnType("int");

                    b.Property<int>("Type_Agreement_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Client_Id");

                    b.HasIndex("Status_Id");

                    b.HasIndex("Type_Agreement_Id");

                    b.ToTable("Actualization");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.BankProduct", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("BankProduct");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AdditionalBIN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AddressPrime")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BIN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("BecomeClientDate")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("CardOP")
                        .HasColumnType("bit");

                    b.Property<string>("ClientManager")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Client_type_Id")
                        .HasColumnType("int");

                    b.Property<int>("CoordinatingEmployee_Id")
                        .HasColumnType("int");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("CurrencyLicence")
                        .HasColumnType("bit");

                    b.Property<string>("EnglName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("INN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LicenceEstDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LicenceNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Mnemonic")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OGRN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("OGRN_Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("RKC_BIK")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RegDate_RP")
                        .HasColumnType("datetime2");

                    b.Property<string>("RegName_RP")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegStruct_Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegistrationRegion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ResponsibleUnit_Id")
                        .HasColumnType("int");

                    b.Property<string>("ShortName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Client_type_Id");

                    b.HasIndex("CoordinatingEmployee_Id");

                    b.HasIndex("ResponsibleUnit_Id");

                    b.ToTable("Client");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.ContactType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ContactType");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.Contacts", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Client_Id")
                        .HasColumnType("int");

                    b.Property<string>("ContactFIO")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ContactType_Id")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Client_Id");

                    b.HasIndex("ContactType_Id");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.Contracts", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Client_Id")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Client_Id");

                    b.ToTable("Contracts");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.Criteria", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("Criteria");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.CriteriaToScoring", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("Criterion_Id")
                        .HasColumnType("int");

                    b.Property<int>("PrescoringScoring_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Criterion_Id");

                    b.HasIndex("PrescoringScoring_Id");

                    b.ToTable("CriteriaToScoring");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.Currency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Currency");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.Employees", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Position_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Position_Id");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.Level", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Level");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.Positions", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Positions");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.PrescoringScoring", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Client_Id")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DateNextScoring")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DatePresScor")
                        .HasColumnType("datetime2");

                    b.Property<double?>("NO_Score")
                        .HasColumnType("float");

                    b.Property<double?>("Nostro_Score")
                        .HasColumnType("float");

                    b.Property<int>("ScoringType_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Client_Id");

                    b.HasIndex("ScoringType_Id");

                    b.ToTable("PrescoringScoring");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.ProductToScoring", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool?>("Inconsistent")
                        .HasColumnType("bit");

                    b.Property<int>("Product_Id")
                        .HasColumnType("int");

                    b.Property<int>("Scoring_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Product_Id");

                    b.HasIndex("Scoring_Id");

                    b.ToTable("ProductToScoring");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.Request", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Client_Id")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RecieveDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("SendDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Client_Id");

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.ResponsibleUnit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ResponsibleUnit");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.RestrictedAccounts", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccountNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("AccountState_Id")
                        .HasColumnType("int");

                    b.Property<int>("Client_Id")
                        .HasColumnType("int");

                    b.Property<int>("Currency_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AccountState_Id");

                    b.HasIndex("Client_Id");

                    b.HasIndex("Currency_Id");

                    b.ToTable("RestrictedAccounts");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.RiskCodes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RiskCodes");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.ScoringType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ScoringType");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.Status", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Status");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.StatusActualization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("StatusActualization");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.StopFactors", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Client_Id")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Measure")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Client_Id");

                    b.ToTable("StopFactors");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.TypeAgreement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TypeAgreement");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.TypeClient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TypeClient");
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.Actualization", b =>
                {
                    b.HasOne("ContragentAnalyse.Model.Entities.Client", "Client")
                        .WithMany("Actualizations")
                        .HasForeignKey("Client_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ContragentAnalyse.Model.Entities.Status", "Status")
                        .WithMany()
                        .HasForeignKey("Status_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ContragentAnalyse.Model.Entities.TypeAgreement", "TypeAgreement")
                        .WithMany()
                        .HasForeignKey("Type_Agreement_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.Client", b =>
                {
                    b.HasOne("ContragentAnalyse.Model.Entities.TypeClient", "TypeClient")
                        .WithMany()
                        .HasForeignKey("Client_type_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ContragentAnalyse.Model.Entities.Employees", "Employees")
                        .WithMany()
                        .HasForeignKey("CoordinatingEmployee_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ContragentAnalyse.Model.Entities.ResponsibleUnit", "ResponsibleUnit")
                        .WithMany()
                        .HasForeignKey("ResponsibleUnit_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.Contacts", b =>
                {
                    b.HasOne("ContragentAnalyse.Model.Entities.Client", "Client")
                        .WithMany("Contacts")
                        .HasForeignKey("Client_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ContragentAnalyse.Model.Entities.ContactType", "ContactType")
                        .WithMany()
                        .HasForeignKey("ContactType_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.Contracts", b =>
                {
                    b.HasOne("ContragentAnalyse.Model.Entities.Client", "Client")
                        .WithMany("Contracts")
                        .HasForeignKey("Client_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.CriteriaToScoring", b =>
                {
                    b.HasOne("ContragentAnalyse.Model.Entities.Criteria", "Criteria")
                        .WithMany("CriteriaToScoring")
                        .HasForeignKey("Criterion_Id");

                    b.HasOne("ContragentAnalyse.Model.Entities.PrescoringScoring", "PrescoringScoring")
                        .WithMany("CriteriaToScoring")
                        .HasForeignKey("PrescoringScoring_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.Employees", b =>
                {
                    b.HasOne("ContragentAnalyse.Model.Entities.Positions", "Position")
                        .WithMany()
                        .HasForeignKey("Position_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.PrescoringScoring", b =>
                {
                    b.HasOne("ContragentAnalyse.Model.Entities.Client", "Client")
                        .WithMany("PrescoringScoring")
                        .HasForeignKey("Client_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ContragentAnalyse.Model.Entities.ScoringType", "ScoringType")
                        .WithMany()
                        .HasForeignKey("ScoringType_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.ProductToScoring", b =>
                {
                    b.HasOne("ContragentAnalyse.Model.Entities.BankProduct", "BankProduct")
                        .WithMany()
                        .HasForeignKey("Product_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ContragentAnalyse.Model.Entities.PrescoringScoring", "PrescoringScoring")
                        .WithMany()
                        .HasForeignKey("Scoring_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.Request", b =>
                {
                    b.HasOne("ContragentAnalyse.Model.Entities.Client", "Client")
                        .WithMany("Requests")
                        .HasForeignKey("Client_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.RestrictedAccounts", b =>
                {
                    b.HasOne("ContragentAnalyse.Model.Entities.AccountStates", "AccountState")
                        .WithMany()
                        .HasForeignKey("AccountState_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ContragentAnalyse.Model.Entities.Client", "Client")
                        .WithMany("RestrictedAccounts")
                        .HasForeignKey("Client_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ContragentAnalyse.Model.Entities.Currency", "Currency")
                        .WithMany()
                        .HasForeignKey("Currency_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ContragentAnalyse.Model.Entities.StopFactors", b =>
                {
                    b.HasOne("ContragentAnalyse.Model.Entities.Client", "Client")
                        .WithMany("StopFactors")
                        .HasForeignKey("Client_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}