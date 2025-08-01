using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settlements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TripId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FromMemberId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FromMemberName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ToMemberId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ToMemberName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SettledAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settlements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpendingSplits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TransactionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TripId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SplitAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    SplitType = table.Column<int>(type: "INTEGER", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    PayerId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PayerName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpendingSplits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "TEXT", maxLength: 7, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SplitParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SpendingSplitId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MemberId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    MemberName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ShareAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    SharePercentage = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    IsPaid = table.Column<bool>(type: "INTEGER", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SplitParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SplitParticipants_SpendingSplits_SpendingSplitId",
                        column: x => x.SpendingSplitId,
                        principalTable: "SpendingSplits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SplitTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SpendingSplitId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TagId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SplitTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SplitTags_SpendingSplits_SpendingSplitId",
                        column: x => x.SpendingSplitId,
                        principalTable: "SpendingSplits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SplitTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SplitParticipants_SpendingSplitId",
                table: "SplitParticipants",
                column: "SpendingSplitId");

            migrationBuilder.CreateIndex(
                name: "IX_SplitTags_SpendingSplitId",
                table: "SplitTags",
                column: "SpendingSplitId");

            migrationBuilder.CreateIndex(
                name: "IX_SplitTags_TagId",
                table: "SplitTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settlements");

            migrationBuilder.DropTable(
                name: "SplitParticipants");

            migrationBuilder.DropTable(
                name: "SplitTags");

            migrationBuilder.DropTable(
                name: "SpendingSplits");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
