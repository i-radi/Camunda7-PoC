using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace muatamer_camunda_poc.Migrations
{
    public partial class addUoGroupRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UmrahOperatorId",
                table: "UmrahGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UmrahGroups_UmrahOperatorId",
                table: "UmrahGroups",
                column: "UmrahOperatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_UmrahGroups_UmrahOperators_UmrahOperatorId",
                table: "UmrahGroups",
                column: "UmrahOperatorId",
                principalTable: "UmrahOperators",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UmrahGroups_UmrahOperators_UmrahOperatorId",
                table: "UmrahGroups");

            migrationBuilder.DropIndex(
                name: "IX_UmrahGroups_UmrahOperatorId",
                table: "UmrahGroups");

            migrationBuilder.DropColumn(
                name: "UmrahOperatorId",
                table: "UmrahGroups");
        }
    }
}
