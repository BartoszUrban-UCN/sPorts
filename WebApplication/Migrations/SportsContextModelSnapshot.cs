﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication.Data;

namespace WebApplication.Migrations
{
    [DbContext(typeof(SportsContext))]
    partial class SportsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("IdentityRoleClaim`1");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("IdentityUserClaim`1");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("IdentityUserLogin`1");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("IdentityUserRole`1");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("IdentityUserToken`1");
                });

            modelBuilder.Entity("WebApplication.Models.Address", b =>
                {
                    b.Property<int>("AddressId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostalCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Street")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AddressId");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("WebApplication.Models.Boat", b =>
                {
                    b.Property<int>("BoatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("BoatOwnerId")
                        .HasColumnType("int");

                    b.Property<double>("Depth")
                        .HasColumnType("float");

                    b.Property<double>("Length")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RegistrationNo")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Width")
                        .HasColumnType("float");

                    b.HasKey("BoatId");

                    b.HasIndex("BoatOwnerId");

                    b.ToTable("Boat");
                });

            modelBuilder.Entity("WebApplication.Models.BoatOwner", b =>
                {
                    b.Property<int>("BoatOwnerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("PersonId")
                        .HasColumnType("int");

                    b.Property<int?>("PersonId1")
                        .HasColumnType("int");

                    b.HasKey("BoatOwnerId");

                    b.HasIndex("PersonId")
                        .IsUnique();

                    b.HasIndex("PersonId1")
                        .IsUnique()
                        .HasFilter("[PersonId1] IS NOT NULL");

                    b.ToTable("BoatOwner");
                });

            modelBuilder.Entity("WebApplication.Models.Booking", b =>
                {
                    b.Property<int>("BookingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("BoatId")
                        .HasColumnType("int");

                    b.Property<int>("BookingReferenceNo")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PaymentStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("TotalPrice")
                        .HasColumnType("float");

                    b.HasKey("BookingId");

                    b.HasIndex("BoatId");

                    b.ToTable("Booking");
                });

            modelBuilder.Entity("WebApplication.Models.BookingLine", b =>
                {
                    b.Property<int>("BookingLineId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<double>("AppliedDiscounts")
                        .HasColumnType("float");

                    b.Property<int>("BookingId")
                        .HasColumnType("int");

                    b.Property<bool>("Confirmed")
                        .HasColumnType("bit");

                    b.Property<double>("DiscountedTotalPrice")
                        .HasColumnType("float");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Ongoing")
                        .HasColumnType("bit");

                    b.Property<double>("OriginalTotalPrice")
                        .HasColumnType("float");

                    b.Property<int>("SpotId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("BookingLineId");

                    b.HasIndex("BookingId");

                    b.HasIndex("SpotId");

                    b.ToTable("BookingLine");
                });

            modelBuilder.Entity("WebApplication.Models.Location", b =>
                {
                    b.Property<int>("LocationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<double>("Radius")
                        .HasColumnType("float");

                    b.HasKey("LocationId");

                    b.ToTable("Location");
                });

            modelBuilder.Entity("WebApplication.Models.Marina", b =>
                {
                    b.Property<int>("MarinaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("AddressId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Facilities")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("LocationId")
                        .HasColumnType("int");

                    b.Property<int>("MarinaOwnerId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MarinaId");

                    b.HasIndex("AddressId");

                    b.HasIndex("LocationId")
                        .IsUnique()
                        .HasFilter("[LocationId] IS NOT NULL");

                    b.HasIndex("MarinaOwnerId");

                    b.ToTable("Marina");
                });

            modelBuilder.Entity("WebApplication.Models.MarinaOwner", b =>
                {
                    b.Property<int>("MarinaOwnerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("PersonId")
                        .HasColumnType("int");

                    b.Property<int?>("PersonId1")
                        .HasColumnType("int");

                    b.HasKey("MarinaOwnerId");

                    b.HasIndex("PersonId")
                        .IsUnique();

                    b.HasIndex("PersonId1")
                        .IsUnique()
                        .HasFilter("[PersonId1] IS NOT NULL");

                    b.ToTable("MarinaOwner");
                });

            modelBuilder.Entity("WebApplication.Models.Payment", b =>
                {
                    b.Property<int>("PaymentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<int>("BookingId")
                        .HasColumnType("int");

                    b.Property<string>("IncomingPaymentReference")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IncomingPaymentStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InvoiceStatus")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PaymentId");

                    b.HasIndex("BookingId");

                    b.ToTable("Payment");
                });

            modelBuilder.Entity("WebApplication.Models.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<int?>("AddressId")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("Person");
                });

            modelBuilder.Entity("WebApplication.Models.Review", b =>
                {
                    b.Property<int>("ReviewId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MarinaId")
                        .HasColumnType("int");

                    b.Property<byte>("Stars")
                        .HasColumnType("tinyint");

                    b.HasKey("ReviewId");

                    b.HasIndex("MarinaId");

                    b.ToTable("Review");
                });

            modelBuilder.Entity("WebApplication.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("WebApplication.Models.Spot", b =>
                {
                    b.Property<int>("SpotId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<bool>("Available")
                        .HasColumnType("bit");

                    b.Property<int?>("LocationId")
                        .HasColumnType("int");

                    b.Property<int?>("MarinaId")
                        .HasColumnType("int");

                    b.Property<double>("MaxDepth")
                        .HasColumnType("float");

                    b.Property<double>("MaxLength")
                        .HasColumnType("float");

                    b.Property<double>("MaxWidth")
                        .HasColumnType("float");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<int>("SpotNumber")
                        .HasColumnType("int");

                    b.HasKey("SpotId");

                    b.HasIndex("LocationId")
                        .IsUnique()
                        .HasFilter("[LocationId] IS NOT NULL");

                    b.HasIndex("MarinaId");

                    b.ToTable("Spot");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("WebApplication.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("WebApplication.Models.Person", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("WebApplication.Models.Person", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.HasOne("WebApplication.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApplication.Models.Person", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("WebApplication.Models.Person", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WebApplication.Models.Boat", b =>
                {
                    b.HasOne("WebApplication.Models.BoatOwner", "BoatOwner")
                        .WithMany("Boats")
                        .HasForeignKey("BoatOwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BoatOwner");
                });

            modelBuilder.Entity("WebApplication.Models.BoatOwner", b =>
                {
                    b.HasOne("WebApplication.Models.Person", "Person")
                        .WithOne()
                        .HasForeignKey("WebApplication.Models.BoatOwner", "PersonId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("WebApplication.Models.Person", null)
                        .WithOne("BoatOwner")
                        .HasForeignKey("WebApplication.Models.BoatOwner", "PersonId1");

                    b.Navigation("Person");
                });

            modelBuilder.Entity("WebApplication.Models.Booking", b =>
                {
                    b.HasOne("WebApplication.Models.Boat", "Boat")
                        .WithMany("Bookings")
                        .HasForeignKey("BoatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Boat");
                });

            modelBuilder.Entity("WebApplication.Models.BookingLine", b =>
                {
                    b.HasOne("WebApplication.Models.Booking", "Booking")
                        .WithMany("BookingLines")
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApplication.Models.Spot", "Spot")
                        .WithMany("BookingLines")
                        .HasForeignKey("SpotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Booking");

                    b.Navigation("Spot");
                });

            modelBuilder.Entity("WebApplication.Models.Marina", b =>
                {
                    b.HasOne("WebApplication.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId");

                    b.HasOne("WebApplication.Models.Location", "Location")
                        .WithOne("Marina")
                        .HasForeignKey("WebApplication.Models.Marina", "LocationId");

                    b.HasOne("WebApplication.Models.MarinaOwner", "MarinaOwner")
                        .WithMany("Marinas")
                        .HasForeignKey("MarinaOwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Address");

                    b.Navigation("Location");

                    b.Navigation("MarinaOwner");
                });

            modelBuilder.Entity("WebApplication.Models.MarinaOwner", b =>
                {
                    b.HasOne("WebApplication.Models.Person", "Person")
                        .WithOne()
                        .HasForeignKey("WebApplication.Models.MarinaOwner", "PersonId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("WebApplication.Models.Person", null)
                        .WithOne("MarinaOwner")
                        .HasForeignKey("WebApplication.Models.MarinaOwner", "PersonId1");

                    b.Navigation("Person");
                });

            modelBuilder.Entity("WebApplication.Models.Payment", b =>
                {
                    b.HasOne("WebApplication.Models.Booking", "Booking")
                        .WithMany()
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Booking");
                });

            modelBuilder.Entity("WebApplication.Models.Person", b =>
                {
                    b.HasOne("WebApplication.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId");

                    b.Navigation("Address");
                });

            modelBuilder.Entity("WebApplication.Models.Review", b =>
                {
                    b.HasOne("WebApplication.Models.Marina", "Marina")
                        .WithMany("Reviews")
                        .HasForeignKey("MarinaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Marina");
                });

            modelBuilder.Entity("WebApplication.Models.Spot", b =>
                {
                    b.HasOne("WebApplication.Models.Location", "Location")
                        .WithOne("Spot")
                        .HasForeignKey("WebApplication.Models.Spot", "LocationId");

                    b.HasOne("WebApplication.Models.Marina", "Marina")
                        .WithMany("Spots")
                        .HasForeignKey("MarinaId");

                    b.Navigation("Location");

                    b.Navigation("Marina");
                });

            modelBuilder.Entity("WebApplication.Models.Boat", b =>
                {
                    b.Navigation("Bookings");
                });

            modelBuilder.Entity("WebApplication.Models.BoatOwner", b =>
                {
                    b.Navigation("Boats");
                });

            modelBuilder.Entity("WebApplication.Models.Booking", b =>
                {
                    b.Navigation("BookingLines");
                });

            modelBuilder.Entity("WebApplication.Models.Location", b =>
                {
                    b.Navigation("Marina");

                    b.Navigation("Spot");
                });

            modelBuilder.Entity("WebApplication.Models.Marina", b =>
                {
                    b.Navigation("Reviews");

                    b.Navigation("Spots");
                });

            modelBuilder.Entity("WebApplication.Models.MarinaOwner", b =>
                {
                    b.Navigation("Marinas");
                });

            modelBuilder.Entity("WebApplication.Models.Person", b =>
                {
                    b.Navigation("BoatOwner");

                    b.Navigation("MarinaOwner");
                });

            modelBuilder.Entity("WebApplication.Models.Spot", b =>
                {
                    b.Navigation("BookingLines");
                });
#pragma warning restore 612, 618
        }
    }
}
