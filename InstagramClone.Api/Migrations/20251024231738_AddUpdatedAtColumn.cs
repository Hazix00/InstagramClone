using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstagramClone.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedAtColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_post_likes",
                table: "post_likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_follows",
                table: "follows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_comment_likes",
                table: "comment_likes");

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "posts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "post_likes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "post_likes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "follows",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "follows",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "comments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "comment_likes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "comment_likes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_post_likes",
                table: "post_likes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_follows",
                table: "follows",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_comment_likes",
                table: "comment_likes",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_post_likes_post_id_user_id",
                table: "post_likes",
                columns: new[] { "post_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_follows_follower_id_followee_id",
                table: "follows",
                columns: new[] { "follower_id", "followee_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_comment_likes_comment_id_user_id",
                table: "comment_likes",
                columns: new[] { "comment_id", "user_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_post_likes",
                table: "post_likes");

            migrationBuilder.DropIndex(
                name: "IX_post_likes_post_id_user_id",
                table: "post_likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_follows",
                table: "follows");

            migrationBuilder.DropIndex(
                name: "IX_follows_follower_id_followee_id",
                table: "follows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_comment_likes",
                table: "comment_likes");

            migrationBuilder.DropIndex(
                name: "IX_comment_likes_comment_id_user_id",
                table: "comment_likes");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "id",
                table: "post_likes");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "post_likes");

            migrationBuilder.DropColumn(
                name: "id",
                table: "follows");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "follows");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "id",
                table: "comment_likes");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "comment_likes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_post_likes",
                table: "post_likes",
                columns: new[] { "post_id", "user_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_follows",
                table: "follows",
                columns: new[] { "follower_id", "followee_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_comment_likes",
                table: "comment_likes",
                columns: new[] { "comment_id", "user_id" });
        }
    }
}
