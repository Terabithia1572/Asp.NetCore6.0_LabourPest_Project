using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class mig_BlogComment_Blog_relationAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlogsComments",
                columns: table => new
                {
                    BlogCommentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlogCommentUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlogCommentTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlogCommentContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlogImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlogCommentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BlogCommentStatus = table.Column<bool>(type: "bit", nullable: false),
                    BlogID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogsComments", x => x.BlogCommentID);
                    table.ForeignKey(
                        name: "FK_BlogsComments_Blogs_BlogID",
                        column: x => x.BlogID,
                        principalTable: "Blogs",
                        principalColumn: "BlogID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogsComments_BlogID",
                table: "BlogsComments",
                column: "BlogID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogsComments");
        }
    }
}
