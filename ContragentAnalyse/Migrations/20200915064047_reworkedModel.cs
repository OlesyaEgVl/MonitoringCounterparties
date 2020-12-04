using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContragentAnalyse.Migrations
{
    public partial class ReworkedModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountStates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankProduct",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankProduct", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Criteria",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Weight = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Criteria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Level",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Level", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResponsibleUnit",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponsibleUnit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RiskCodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScoringType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoringType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusActualization",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusActualization", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeAgreement",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeAgreement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeClient",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeClient", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Position_Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Positions_Position_Id",
                        column: x => x.Position_Id,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Mnemonic = table.Column<string>(nullable: true),
                    ClientManager = table.Column<string>(nullable: true),
                    CardOP = table.Column<bool>(nullable: true),
                    Client_type_Id = table.Column<int>(nullable: false),
                    BecomeClientDate = table.Column<DateTime>(nullable: false),
                    ResponsibleUnit_Id = table.Column<int>(nullable: false),
                    CoordinatingEmployee_Id = table.Column<int>(nullable: false),
                    BIN = table.Column<string>(nullable: true),
                    AdditionalBIN = table.Column<string>(nullable: true),
                    ShortName = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    EnglName = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    LicenceNumber = table.Column<string>(nullable: true),
                    LicenceEstDate = table.Column<DateTime>(nullable: true),
                    RKC_BIK = table.Column<string>(nullable: true),
                    INN = table.Column<string>(nullable: true),
                    OGRN = table.Column<string>(nullable: true),
                    OGRN_Date = table.Column<DateTime>(nullable: true),
                    RegName_RP = table.Column<string>(nullable: true),
                    RegDate_RP = table.Column<DateTime>(nullable: true),
                    RegStruct_Name = table.Column<string>(nullable: true),
                    CurrencyLicence = table.Column<bool>(nullable: true),
                    RegistrationRegion = table.Column<string>(nullable: true),
                    AddressPrime = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Client_TypeClient_Client_type_Id",
                        column: x => x.Client_type_Id,
                        principalTable: "TypeClient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Client_Employees_CoordinatingEmployee_Id",
                        column: x => x.CoordinatingEmployee_Id,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Client_ResponsibleUnit_ResponsibleUnit_Id",
                        column: x => x.ResponsibleUnit_Id,
                        principalTable: "ResponsibleUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Actualization",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Client_Id = table.Column<int>(nullable: false),
                    Status_Id = table.Column<int>(nullable: false),
                    DateEKS = table.Column<DateTime>(nullable: false),
                    DateActEKS = table.Column<DateTime>(nullable: false),
                    Type_Agreement_Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actualization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Actualization_Client_Client_Id",
                        column: x => x.Client_Id,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Actualization_Status_Status_Id",
                        column: x => x.Status_Id,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Actualization_TypeAgreement_Type_Agreement_Id",
                        column: x => x.Type_Agreement_Id,
                        principalTable: "TypeAgreement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContactType_Id = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    ContactFIO = table.Column<string>(nullable: true),
                    Client_Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_Client_Client_Id",
                        column: x => x.Client_Id,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contacts_ContactType_ContactType_Id",
                        column: x => x.ContactType_Id,
                        principalTable: "ContactType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Client_Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_Client_Client_Id",
                        column: x => x.Client_Id,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrescoringScoring",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatePresScor = table.Column<DateTime>(nullable: false),
                    ScoringType_Id = table.Column<int>(nullable: false),
                    Client_Id = table.Column<int>(nullable: false),
                    NO_Score = table.Column<float>(nullable: true),
                    Nostro_Score = table.Column<float>(nullable: true),
                    DateNextScoring = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescoringScoring", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrescoringScoring_Client_Client_Id",
                        column: x => x.Client_Id,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrescoringScoring_ScoringType_ScoringType_Id",
                        column: x => x.ScoringType_Id,
                        principalTable: "ScoringType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Client_Id = table.Column<int>(nullable: false),
                    SendDate = table.Column<DateTime>(nullable: false),
                    RecieveDate = table.Column<DateTime>(nullable: true),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requests_Client_Client_Id",
                        column: x => x.Client_Id,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestrictedAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Client_Id = table.Column<int>(nullable: false),
                    Currency_Id = table.Column<int>(nullable: false),
                    AccountNumber = table.Column<string>(nullable: true),
                    AccountState_Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestrictedAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestrictedAccounts_AccountStates_AccountState_Id",
                        column: x => x.AccountState_Id,
                        principalTable: "AccountStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestrictedAccounts_Client_Client_Id",
                        column: x => x.Client_Id,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestrictedAccounts_Currency_Currency_Id",
                        column: x => x.Currency_Id,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StopFactors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    Client_Id = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    Measure = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StopFactors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StopFactors_Client_Client_Id",
                        column: x => x.Client_Id,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriteriaToScoring",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Criterion_Id = table.Column<int>(nullable: true),
                    PrescoringScoring_Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriteriaToScoring", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CriteriaToScoring_Criteria_Criterion_Id",
                        column: x => x.Criterion_Id,
                        principalTable: "Criteria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CriteriaToScoring_PrescoringScoring_PrescoringScoring_Id",
                        column: x => x.PrescoringScoring_Id,
                        principalTable: "PrescoringScoring",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductToScoring",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Product_Id = table.Column<int>(nullable: false),
                    Scoring_Id = table.Column<int>(nullable: false),
                    Inconsistent = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductToScoring", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductToScoring_BankProduct_Product_Id",
                        column: x => x.Product_Id,
                        principalTable: "BankProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductToScoring_PrescoringScoring_Scoring_Id",
                        column: x => x.Scoring_Id,
                        principalTable: "PrescoringScoring",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actualization_Client_Id",
                table: "Actualization",
                column: "Client_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Actualization_Status_Id",
                table: "Actualization",
                column: "Status_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Actualization_Type_Agreement_Id",
                table: "Actualization",
                column: "Type_Agreement_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Client_Client_type_Id",
                table: "Client",
                column: "Client_type_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Client_CoordinatingEmployee_Id",
                table: "Client",
                column: "CoordinatingEmployee_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Client_ResponsibleUnit_Id",
                table: "Client",
                column: "ResponsibleUnit_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_Client_Id",
                table: "Contacts",
                column: "Client_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_ContactType_Id",
                table: "Contacts",
                column: "ContactType_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_Client_Id",
                table: "Contracts",
                column: "Client_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CriteriaToScoring_Criterion_Id",
                table: "CriteriaToScoring",
                column: "Criterion_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CriteriaToScoring_PrescoringScoring_Id",
                table: "CriteriaToScoring",
                column: "PrescoringScoring_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Position_Id",
                table: "Employees",
                column: "Position_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PrescoringScoring_Client_Id",
                table: "PrescoringScoring",
                column: "Client_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PrescoringScoring_ScoringType_Id",
                table: "PrescoringScoring",
                column: "ScoringType_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductToScoring_Product_Id",
                table: "ProductToScoring",
                column: "Product_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductToScoring_Scoring_Id",
                table: "ProductToScoring",
                column: "Scoring_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Client_Id",
                table: "Requests",
                column: "Client_Id");

            migrationBuilder.CreateIndex(
                name: "IX_RestrictedAccounts_AccountState_Id",
                table: "RestrictedAccounts",
                column: "AccountState_Id");

            migrationBuilder.CreateIndex(
                name: "IX_RestrictedAccounts_Client_Id",
                table: "RestrictedAccounts",
                column: "Client_Id");

            migrationBuilder.CreateIndex(
                name: "IX_RestrictedAccounts_Currency_Id",
                table: "RestrictedAccounts",
                column: "Currency_Id");

            migrationBuilder.CreateIndex(
                name: "IX_StopFactors_Client_Id",
                table: "StopFactors",
                column: "Client_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actualization");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "CriteriaToScoring");

            migrationBuilder.DropTable(
                name: "Level");

            migrationBuilder.DropTable(
                name: "ProductToScoring");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "RestrictedAccounts");

            migrationBuilder.DropTable(
                name: "RiskCodes");

            migrationBuilder.DropTable(
                name: "StatusActualization");

            migrationBuilder.DropTable(
                name: "StopFactors");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "TypeAgreement");

            migrationBuilder.DropTable(
                name: "ContactType");

            migrationBuilder.DropTable(
                name: "Criteria");

            migrationBuilder.DropTable(
                name: "BankProduct");

            migrationBuilder.DropTable(
                name: "PrescoringScoring");

            migrationBuilder.DropTable(
                name: "AccountStates");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "ScoringType");

            migrationBuilder.DropTable(
                name: "TypeClient");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "ResponsibleUnit");

            migrationBuilder.DropTable(
                name: "Positions");
        }
    }
}
