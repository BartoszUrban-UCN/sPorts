using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class PersonTablesTest2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boat_BoatOwner_BoatOwnerId",
                table: "Boat");

            migrationBuilder.DropForeignKey(
                name: "FK_Marina_MarinaOwner_MarinaOwnerId",
                table: "Marina");

            migrationBuilder.DropTable(
                name: "BoatOwner");

            migrationBuilder.DropTable(
                name: "MarinaOwner");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Person",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Boat_Person_BoatOwnerId",
                table: "Boat",
                column: "BoatOwnerId",
                principalTable: "Person",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Marina_Person_MarinaOwnerId",
                table: "Marina",
                column: "MarinaOwnerId",
                principalTable: "Person",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boat_Person_BoatOwnerId",
                table: "Boat");

            migrationBuilder.DropForeignKey(
                name: "FK_Marina_Person_MarinaOwnerId",
                table: "Marina");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Person");

            migrationBuilder.CreateTable(
                name: "BoatOwner",
                columns: table => new
                {
                    BoatOwnerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoatOwner", x => x.BoatOwnerId);
                    table.ForeignKey(
                        name: "FK_BoatOwner_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarinaOwner",
                columns: table => new
                {
                    MarinaOwnerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarinaOwner", x => x.MarinaOwnerId);
                    table.ForeignKey(
                        name: "FK_MarinaOwner_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoatOwner_PersonId",
                table: "BoatOwner",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_MarinaOwner_PersonId",
                table: "MarinaOwner",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Boat_BoatOwner_BoatOwnerId",
                table: "Boat",
                column: "BoatOwnerId",
                principalTable: "BoatOwner",
                principalColumn: "BoatOwnerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Marina_MarinaOwner_MarinaOwnerId",
                table: "Marina",
                column: "MarinaOwnerId",
                principalTable: "MarinaOwner",
                principalColumn: "MarinaOwnerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
