using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AIchatbot.Migrations
{
    /// <inheritdoc />
    public partial class IntialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FAQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Question = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Answer = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Tags = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    LastName = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    SessionId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChatSessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    IsFromUser = table.Column<bool>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Intent = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Confidence = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    ResponseSource = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    RelatedFAQId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatSessions_ChatSessionId",
                        column: x => x.ChatSessionId,
                        principalTable: "ChatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessages_FAQs_RelatedFAQId",
                        column: x => x.RelatedFAQId,
                        principalTable: "FAQs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "FAQs",
                columns: new[] { "Id", "Answer", "Category", "CreatedAt", "IsActive", "Priority", "Question", "Tags", "UpdatedAt", "ViewCount" },
                values: new object[,]
                {
                    { 1, "You can track your order by logging into your account and visiting the 'Order History' section, or by using the tracking number provided in your order confirmation email.", "Order Tracking", new DateTime(2025, 7, 12, 5, 53, 43, 999, DateTimeKind.Utc).AddTicks(2380), true, 1, "How can I track my order?", "order,tracking,delivery", null, 0 },
                    { 2, "We offer a 30-day return policy for most items. Products must be in original condition with all tags attached. Some items may have different return policies.", "Returns", new DateTime(2025, 7, 12, 5, 53, 43, 999, DateTimeKind.Utc).AddTicks(2383), true, 1, "What is your return policy?", "return,refund,policy", null, 0 },
                    { 3, "Standard delivery takes 3-5 business days. Express delivery (1-2 business days) is available for an additional fee. International shipping may take 7-14 business days.", "Delivery", new DateTime(2025, 7, 12, 5, 53, 43, 999, DateTimeKind.Utc).AddTicks(2384), true, 1, "How long does delivery take?", "delivery,shipping,time", null, 0 },
                    { 4, "We accept all major credit cards (Visa, MasterCard, American Express), PayPal, Apple Pay, Google Pay, and bank transfers.", "Payment", new DateTime(2025, 7, 12, 5, 53, 43, 999, DateTimeKind.Utc).AddTicks(2386), true, 1, "What payment methods do you accept?", "payment,credit card,paypal", null, 0 },
                    { 5, "Orders can be cancelled within 1 hour of placement if they haven't been processed for shipping. Contact our customer service team immediately for assistance.", "Orders", new DateTime(2025, 7, 12, 5, 53, 43, 999, DateTimeKind.Utc).AddTicks(2388), true, 2, "Can I cancel my order?", "cancel,order,modification", null, 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ChatSessionId",
                table: "ChatMessages",
                column: "ChatSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_RelatedFAQId",
                table: "ChatMessages",
                column: "RelatedFAQId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_Timestamp",
                table: "ChatMessages",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_ChatSessions_SessionId",
                table: "ChatSessions",
                column: "SessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatSessions_UserId",
                table: "ChatSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FAQs_Category",
                table: "FAQs",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_FAQs_IsActive",
                table: "FAQs",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ChatSessions");

            migrationBuilder.DropTable(
                name: "FAQs");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
