using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace muatamer_camunda_poc.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntersectionQuotaTracking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Entity1Type = table.Column<int>(type: "int", nullable: false),
                    Entity1Id = table.Column<int>(type: "int", nullable: false),
                    Entity2Type = table.Column<int>(type: "int", nullable: false),
                    Entity2Id = table.Column<int>(type: "int", nullable: false),
                    PeriodType = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false),
                    Used = table.Column<int>(type: "int", nullable: false),
                    Reserved = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntersectionQuotaTracking", x => x.Id);
                    table.CheckConstraint("CK_IntersectionQuotaTracking_Reserved", "Reserved <= Total - Used");
                    table.CheckConstraint("CK_IntersectionQuotaTracking_Used", "Used <= Total");
                });

            migrationBuilder.CreateTable(
                name: "Nationalities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nationalities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StandaloneQuotaTracking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    PeriodType = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false),
                    Used = table.Column<int>(type: "int", nullable: false),
                    Reserved = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandaloneQuotaTracking", x => x.Id);
                    table.CheckConstraint("CK_StandaloneQuotaTracking_Reserved", "Reserved <= Total - Used");
                    table.CheckConstraint("CK_StandaloneQuotaTracking_Used", "Used <= Total");
                });

            migrationBuilder.CreateTable(
                name: "UmrahOperators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UmrahOperators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExternalAgents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalAgents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalAgents_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UmrahGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromCountry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    HasVoucher = table.Column<bool>(type: "bit", nullable: false),
                    VisaIssued = table.Column<bool>(type: "bit", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UmrahGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UmrahGroups_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalAgentUmrahOperator",
                columns: table => new
                {
                    ExternalAgentsId = table.Column<int>(type: "int", nullable: false),
                    UmrahOperatorsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalAgentUmrahOperator", x => new { x.ExternalAgentsId, x.UmrahOperatorsId });
                    table.ForeignKey(
                        name: "FK_ExternalAgentUmrahOperator_ExternalAgents_ExternalAgentsId",
                        column: x => x.ExternalAgentsId,
                        principalTable: "ExternalAgents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExternalAgentUmrahOperator_UmrahOperators_UmrahOperatorsId",
                        column: x => x.UmrahOperatorsId,
                        principalTable: "UmrahOperators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MuatamerInformations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalityId = table.Column<int>(type: "int", nullable: false),
                    PassportType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassportNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassportIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PassportExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuatamerInformations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MuatamerInformations_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MuatamerInformations_Nationalities_NationalityId",
                        column: x => x.NationalityId,
                        principalTable: "Nationalities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MuatamerInformations_UmrahGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "UmrahGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalAgents_CountryId",
                table: "ExternalAgents",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalAgentUmrahOperator_UmrahOperatorsId",
                table: "ExternalAgentUmrahOperator",
                column: "UmrahOperatorsId");

            migrationBuilder.CreateIndex(
                name: "IX_MuatamerInformations_CountryId",
                table: "MuatamerInformations",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_MuatamerInformations_GroupId",
                table: "MuatamerInformations",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_MuatamerInformations_NationalityId",
                table: "MuatamerInformations",
                column: "NationalityId");

            migrationBuilder.CreateIndex(
                name: "IX_UmrahGroups_CountryId",
                table: "UmrahGroups",
                column: "CountryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalAgentUmrahOperator");

            migrationBuilder.DropTable(
                name: "IntersectionQuotaTracking");

            migrationBuilder.DropTable(
                name: "MuatamerInformations");

            migrationBuilder.DropTable(
                name: "StandaloneQuotaTracking");

            migrationBuilder.DropTable(
                name: "ExternalAgents");

            migrationBuilder.DropTable(
                name: "UmrahOperators");

            migrationBuilder.DropTable(
                name: "Nationalities");

            migrationBuilder.DropTable(
                name: "UmrahGroups");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
