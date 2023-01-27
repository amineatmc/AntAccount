using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntalyaTaksiAccount.Migrations
{
    /// <inheritdoc />
    public partial class ATAccountDBv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StationMail",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "StationName",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "StationPhone",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "Mail",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "MailAdress",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "MailVerify",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "Drivers");

            migrationBuilder.AddColumn<int>(
                name: "AllUserID",
                table: "Stations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AllUserID",
                table: "Passengers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AllUserID",
                table: "Drivers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AllUsers",
                columns: table => new
                {
                    AllUserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MailAdress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    UserType = table.Column<int>(type: "int", nullable: false),
                    MailVerify = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Activity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllUsers", x => x.AllUserID)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stations_AllUserID",
                table: "Stations",
                column: "AllUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_AllUserID",
                table: "Passengers",
                column: "AllUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_AllUserID",
                table: "Drivers",
                column: "AllUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_AllUsers_AllUserID",
                table: "Drivers",
                column: "AllUserID",
                principalTable: "AllUsers",
                principalColumn: "AllUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Passengers_AllUsers_AllUserID",
                table: "Passengers",
                column: "AllUserID",
                principalTable: "AllUsers",
                principalColumn: "AllUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Stations_AllUsers_AllUserID",
                table: "Stations",
                column: "AllUserID",
                principalTable: "AllUsers",
                principalColumn: "AllUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_AllUsers_AllUserID",
                table: "Drivers");

            migrationBuilder.DropForeignKey(
                name: "FK_Passengers_AllUsers_AllUserID",
                table: "Passengers");

            migrationBuilder.DropForeignKey(
                name: "FK_Stations_AllUsers_AllUserID",
                table: "Stations");

            migrationBuilder.DropTable(
                name: "AllUsers");

            migrationBuilder.DropIndex(
                name: "IX_Stations_AllUserID",
                table: "Stations");

            migrationBuilder.DropIndex(
                name: "IX_Passengers_AllUserID",
                table: "Passengers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_AllUserID",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "AllUserID",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "AllUserID",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "AllUserID",
                table: "Drivers");

            migrationBuilder.AddColumn<string>(
                name: "StationMail",
                table: "Stations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StationName",
                table: "Stations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StationPhone",
                table: "Stations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Mail",
                table: "Passengers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Passengers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Passengers",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "Passengers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MailAdress",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MailVerify",
                table: "Drivers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Drivers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Drivers",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "Drivers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
