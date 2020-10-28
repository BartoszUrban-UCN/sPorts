using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class PersonTablesTest1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MarinaOwner_PersonId",
                table: "MarinaOwner",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_BoatOwner_PersonId",
                table: "BoatOwner",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoatOwner_Person_PersonId",
                table: "BoatOwner",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarinaOwner_Person_PersonId",
                table: "MarinaOwner",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoatOwner_Person_PersonId",
                table: "BoatOwner");

            migrationBuilder.DropForeignKey(
                name: "FK_MarinaOwner_Person_PersonId",
                table: "MarinaOwner");

            migrationBuilder.DropIndex(
                name: "IX_MarinaOwner_PersonId",
                table: "MarinaOwner");

            migrationBuilder.DropIndex(
                name: "IX_BoatOwner_PersonId",
                table: "BoatOwner");
        }
    }
}
