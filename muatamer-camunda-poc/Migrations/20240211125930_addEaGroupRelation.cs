using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace muatamer_camunda_poc.Migrations
{
    public partial class addEaGroupRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExternalAgentId",
                table: "UmrahGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UmrahGroups_ExternalAgentId",
                table: "UmrahGroups",
                column: "ExternalAgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_UmrahGroups_ExternalAgents_ExternalAgentId",
                table: "UmrahGroups",
                column: "ExternalAgentId",
                principalTable: "ExternalAgents",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UmrahGroups_ExternalAgents_ExternalAgentId",
                table: "UmrahGroups");

            migrationBuilder.DropIndex(
                name: "IX_UmrahGroups_ExternalAgentId",
                table: "UmrahGroups");

            migrationBuilder.DropColumn(
                name: "ExternalAgentId",
                table: "UmrahGroups");
        }
    }
}
