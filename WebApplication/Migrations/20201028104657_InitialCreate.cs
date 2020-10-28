using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    AddressId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Street = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.AddressId);
                });

            migrationBuilder.CreateTable(
                name: "BoatOwner",
                columns: table => new
                {
                    BoatOwnerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoatOwner", x => x.BoatOwnerId);
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    BookingId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingReferenceNo = table.Column<int>(nullable: false),
                    TotalPrice = table.Column<double>(nullable: false),
                    PaymentStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.BookingId);
                });

            migrationBuilder.CreateTable(
                name: "MarinaOwner",
                columns: table => new
                {
                    MarinaOwnerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarinaOwner", x => x.MarinaOwnerId);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    PersonId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    AddressId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.PersonId);
                    table.ForeignKey(
                        name: "FK_Person_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Boat",
                columns: table => new
                {
                    BoatId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    RegistrationNo = table.Column<int>(nullable: false),
                    Width = table.Column<double>(nullable: false),
                    Length = table.Column<double>(nullable: false),
                    Depth = table.Column<double>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    BoatOwnerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boat", x => x.BoatId);
                    table.ForeignKey(
                        name: "FK_Boat_BoatOwner_BoatOwnerId",
                        column: x => x.BoatOwnerId,
                        principalTable: "BoatOwner",
                        principalColumn: "BoatOwnerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Marina",
                columns: table => new
                {
                    MarinaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Facilities = table.Column<string>(nullable: true),
                    MarinaOwnerId = table.Column<int>(nullable: false),
                    AddressId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marina", x => x.MarinaId);
                    table.ForeignKey(
                        name: "FK_Marina_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Marina_MarinaOwner_MarinaOwnerId",
                        column: x => x.MarinaOwnerId,
                        principalTable: "MarinaOwner",
                        principalColumn: "MarinaOwnerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    ReviewId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Starts = table.Column<byte>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    MarinaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_Review_Marina_MarinaId",
                        column: x => x.MarinaId,
                        principalTable: "Marina",
                        principalColumn: "MarinaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Spot",
                columns: table => new
                {
                    SpotId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpotNumber = table.Column<int>(nullable: false),
                    Available = table.Column<bool>(nullable: false),
                    MaxWidth = table.Column<double>(nullable: false),
                    MaxLength = table.Column<double>(nullable: false),
                    MaxDepth = table.Column<double>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    MarinaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spot", x => x.SpotId);
                    table.ForeignKey(
                        name: "FK_Spot_Marina_MarinaId",
                        column: x => x.MarinaId,
                        principalTable: "Marina",
                        principalColumn: "MarinaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingLine",
                columns: table => new
                {
                    BookingLineId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginalTotalPrice = table.Column<double>(nullable: false),
                    AppliedDiscounts = table.Column<double>(nullable: false),
                    DiscountedTotalPrice = table.Column<double>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Confirmed = table.Column<bool>(nullable: false),
                    BookingId = table.Column<int>(nullable: false),
                    SpotId = table.Column<int>(nullable: false),
                    BoatId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingLine", x => x.BookingLineId);
                    table.ForeignKey(
                        name: "FK_BookingLine_Boat_BoatId",
                        column: x => x.BoatId,
                        principalTable: "Boat",
                        principalColumn: "BoatId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingLine_Booking_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingLine_Spot_SpotId",
                        column: x => x.SpotId,
                        principalTable: "Spot",
                        principalColumn: "SpotId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boat_BoatOwnerId",
                table: "Boat",
                column: "BoatOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingLine_BoatId",
                table: "BookingLine",
                column: "BoatId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingLine_BookingId",
                table: "BookingLine",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingLine_SpotId",
                table: "BookingLine",
                column: "SpotId");

            migrationBuilder.CreateIndex(
                name: "IX_Marina_AddressId",
                table: "Marina",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Marina_MarinaOwnerId",
                table: "Marina",
                column: "MarinaOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Person_AddressId",
                table: "Person",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_MarinaId",
                table: "Review",
                column: "MarinaId");

            migrationBuilder.CreateIndex(
                name: "IX_Spot_MarinaId",
                table: "Spot",
                column: "MarinaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingLine");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "Boat");

            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "Spot");

            migrationBuilder.DropTable(
                name: "BoatOwner");

            migrationBuilder.DropTable(
                name: "Marina");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "MarinaOwner");
        }
    }
}
