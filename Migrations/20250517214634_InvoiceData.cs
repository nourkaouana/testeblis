using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace testbills.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Libelle = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<string>(type: "TEXT", nullable: false),
                    Montant_HT = table.Column<string>(type: "TEXT", nullable: false),
                    Montant_TTC = table.Column<string>(type: "TEXT", nullable: false),
                    TVA = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TaxAmount = table.Column<string>(type: "TEXT", nullable: false),
                    TaxRate = table.Column<string>(type: "TEXT", nullable: false),
                    InvoiceDataId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxDetails_Invoices_InvoiceDataId",
                        column: x => x.InvoiceDataId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaxDetails_InvoiceDataId",
                table: "TaxDetails",
                column: "InvoiceDataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaxDetails");

            migrationBuilder.DropTable(
                name: "Invoices");
        }
    }
}
