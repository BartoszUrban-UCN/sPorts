using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class PersonTablesTest5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateIndex(
                name: "IX_MarinaOwner_PersonId",
                table: "MarinaOwner",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoatOwner_PersonId",
                table: "BoatOwner",
                column: "PersonId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BoatOwner_Person_PersonId",
                table: "BoatOwner",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_MarinaOwner_Person_PersonId",
                table: "MarinaOwner",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "PersonId");
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
    }
}
