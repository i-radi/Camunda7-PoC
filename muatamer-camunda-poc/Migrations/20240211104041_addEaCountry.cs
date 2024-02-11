using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace muatamer_camunda_poc.Migrations
{
    public partial class addEaCountry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "ExternalAgents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalAgents_CountryId",
                table: "ExternalAgents",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalAgents_Countries_CountryId",
                table: "ExternalAgents",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExternalAgents_Countries_CountryId",
                table: "ExternalAgents");

            migrationBuilder.DropIndex(
                name: "IX_ExternalAgents_CountryId",
                table: "ExternalAgents");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "ExternalAgents");
        }
    }
}
