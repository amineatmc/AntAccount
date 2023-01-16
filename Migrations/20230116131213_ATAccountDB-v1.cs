using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntalyaTaksiAccount.Migrations
{
    /// <inheritdoc />
    public partial class ATAccountDBv1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Activity = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.CompanyID)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Activity = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentID)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Genders",
                columns: table => new
                {
                    GenderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GenderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Activity = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genders", x => x.GenderID)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Activity = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleID)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Passengers",
                columns: table => new
                {
                    PassengerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MailAdress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfilePhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MailVerify = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ResetPasswordVerify = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2023, 1, 16, 16, 12, 13, 548, DateTimeKind.Local).AddTicks(6737)),
                    GenderID = table.Column<int>(type: "int", nullable: true),
                    PasswordChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2023, 1, 16, 16, 12, 13, 548, DateTimeKind.Local).AddTicks(7289)),
                    PasswordExpiration = table.Column<int>(type: "int", nullable: false, defaultValue: 90),
                    Activity = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passengers", x => x.PassengerID)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_Passengers_Genders_GenderID",
                        column: x => x.GenderID,
                        principalTable: "Genders",
                        principalColumn: "GenderID");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MailAdress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfilePhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MailVerify = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ResetPasswordVerify = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2023, 1, 16, 16, 12, 13, 548, DateTimeKind.Local).AddTicks(3420)),
                    RoleID = table.Column<int>(type: "int", nullable: true),
                    DepartmentID = table.Column<int>(type: "int", nullable: true),
                    GenderID = table.Column<int>(type: "int", nullable: true),
                    CompanyID = table.Column<int>(type: "int", nullable: true),
                    PasswordChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2023, 1, 16, 16, 12, 13, 548, DateTimeKind.Local).AddTicks(3922)),
                    PasswordExpiration = table.Column<int>(type: "int", nullable: false, defaultValue: 90),
                    Activity = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_Users_Companies_CompanyID",
                        column: x => x.CompanyID,
                        principalTable: "Companies",
                        principalColumn: "CompanyID");
                    table.ForeignKey(
                        name: "FK_Users_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "DepartmentID");
                    table.ForeignKey(
                        name: "FK_Users_Genders_GenderID",
                        column: x => x.GenderID,
                        principalTable: "Genders",
                        principalColumn: "GenderID");
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_GenderID",
                table: "Passengers",
                column: "GenderID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CompanyID",
                table: "Users",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentID",
                table: "Users",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_GenderID",
                table: "Users",
                column: "GenderID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleID",
                table: "Users",
                column: "RoleID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Passengers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Genders");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
