﻿// <auto-generated />
using System;
using AntalyaTaksiAccount.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AntalyaTaksiAccount.Migrations
{
    [DbContext(typeof(ATAccountContext))]
    [Migration("20230116131213_ATAccountDB-v1")]
    partial class ATAccountDBv1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AntalyaTaksiAccount.Models.Company", b =>
                {
                    b.Property<int>("CompanyID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CompanyID"));

                    b.Property<int>("Activity")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<string>("CompanyName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("CompanyID");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("CompanyID"));

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("AntalyaTaksiAccount.Models.Department", b =>
                {
                    b.Property<int>("DepartmentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DepartmentID"));

                    b.Property<int>("Activity")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<string>("DepartmentName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("DepartmentID");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("DepartmentID"));

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("AntalyaTaksiAccount.Models.Gender", b =>
                {
                    b.Property<int>("GenderID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GenderID"));

                    b.Property<int>("Activity")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<string>("GenderName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("GenderID");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("GenderID"));

                    b.ToTable("Genders");
                });

            modelBuilder.Entity("AntalyaTaksiAccount.Models.Passenger", b =>
                {
                    b.Property<int>("PassengerID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PassengerID"));

                    b.Property<int>("Activity")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<int?>("GenderID")
                        .HasColumnType("int");

                    b.Property<DateTime>("InsertedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValue(new DateTime(2023, 1, 16, 16, 12, 13, 548, DateTimeKind.Local).AddTicks(6737));

                    b.Property<string>("MailAdress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MailVerify")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PasswordChangeDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValue(new DateTime(2023, 1, 16, 16, 12, 13, 548, DateTimeKind.Local).AddTicks(7289));

                    b.Property<int>("PasswordExpiration")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(90);

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("nvarchar(11)");

                    b.Property<string>("ProfilePhotoPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ResetPasswordVerify")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("Surname")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("PassengerID");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("PassengerID"));

                    b.HasIndex("GenderID");

                    b.ToTable("Passengers");
                });

            modelBuilder.Entity("AntalyaTaksiAccount.Models.Role", b =>
                {
                    b.Property<int>("RoleID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoleID"));

                    b.Property<int>("Activity")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<string>("RoleName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("RoleID");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("RoleID"));

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("AntalyaTaksiAccount.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserID"));

                    b.Property<int>("Activity")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<int?>("CompanyID")
                        .HasColumnType("int");

                    b.Property<int?>("DepartmentID")
                        .HasColumnType("int");

                    b.Property<int?>("GenderID")
                        .HasColumnType("int");

                    b.Property<DateTime>("InsertedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValue(new DateTime(2023, 1, 16, 16, 12, 13, 548, DateTimeKind.Local).AddTicks(3420));

                    b.Property<string>("MailAdress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MailVerify")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PasswordChangeDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValue(new DateTime(2023, 1, 16, 16, 12, 13, 548, DateTimeKind.Local).AddTicks(3922));

                    b.Property<int>("PasswordExpiration")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(90);

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("nvarchar(11)");

                    b.Property<string>("ProfilePhotoPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ResetPasswordVerify")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<int?>("RoleID")
                        .HasColumnType("int");

                    b.Property<string>("Surname")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("UserID");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("UserID"));

                    b.HasIndex("CompanyID");

                    b.HasIndex("DepartmentID");

                    b.HasIndex("GenderID");

                    b.HasIndex("RoleID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AntalyaTaksiAccount.Models.Passenger", b =>
                {
                    b.HasOne("AntalyaTaksiAccount.Models.Gender", "Gender")
                        .WithMany()
                        .HasForeignKey("GenderID");

                    b.Navigation("Gender");
                });

            modelBuilder.Entity("AntalyaTaksiAccount.Models.User", b =>
                {
                    b.HasOne("AntalyaTaksiAccount.Models.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyID");

                    b.HasOne("AntalyaTaksiAccount.Models.Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentID");

                    b.HasOne("AntalyaTaksiAccount.Models.Gender", "Gender")
                        .WithMany()
                        .HasForeignKey("GenderID");

                    b.HasOne("AntalyaTaksiAccount.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleID");

                    b.Navigation("Company");

                    b.Navigation("Department");

                    b.Navigation("Gender");

                    b.Navigation("Role");
                });
#pragma warning restore 612, 618
        }
    }
}