using Microsoft.EntityFrameworkCore.Migrations;

namespace ContactManager.Data.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContactForeignKey = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailAddresses_Contacts_ContactForeignKey",
                        column: x => x.ContactForeignKey,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Contacts",
                columns: new[] { "Id", "FirstName", "LastName" },
                values: new object[] { 1, "Bob", "James" });

            migrationBuilder.InsertData(
                table: "Contacts",
                columns: new[] { "Id", "FirstName", "LastName" },
                values: new object[] { 2, "Jerome", "Porter" });

            migrationBuilder.InsertData(
                table: "EmailAddresses",
                columns: new[] { "Id", "Address", "ContactForeignKey", "Type" },
                values: new object[,]
                {
                    { 1, "bjames@kodak.com", 1, 2 },
                    { 2, "bjamespersonal@gmail.com", 1, 1 },
                    { 3, "porterj@steaks.com", 2, 2 },
                    { 4, "jeromeathome@gmail.com", 2, 1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailAddresses");

            migrationBuilder.DropTable(
                name: "Contacts");
        }
    }
}
