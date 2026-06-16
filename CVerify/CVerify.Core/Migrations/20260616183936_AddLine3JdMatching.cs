using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVerify.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLine3JdMatching : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "desired_salary",
                table: "career_preferences",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "minimum_acceptable_salary",
                table: "career_preferences",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "cv_repository_mappings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_code_repository_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reference_source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    reference_entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    indexed_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cv_repository_mappings", x => x.id);
                    table.ForeignKey(
                        name: "fk_cv_repository_mappings_source_code_repositories_source_code",
                        column: x => x.source_code_repository_id,
                        principalTable: "source_code_repositories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_cv_repository_mappings_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "standardized_jds",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    owner_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    seniority = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    salary_min = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    salary_max = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    structured_json = table.Column<string>(type: "jsonb", nullable: false),
                    human_readable_text = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_standardized_jds", x => x.id);
                    table.ForeignKey(
                        name: "fk_standardized_jds_users_owner_user_id",
                        column: x => x.owner_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_cv_repository_mappings_repo_id",
                table: "cv_repository_mappings",
                column: "source_code_repository_id");

            migrationBuilder.CreateIndex(
                name: "idx_cv_repository_mappings_user_id",
                table: "cv_repository_mappings",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_standardized_jds_owner_user_id_created_at",
                table: "standardized_jds",
                columns: new[] { "owner_user_id", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cv_repository_mappings");

            migrationBuilder.DropTable(
                name: "standardized_jds");

            migrationBuilder.DropColumn(
                name: "desired_salary",
                table: "career_preferences");

            migrationBuilder.DropColumn(
                name: "minimum_acceptable_salary",
                table: "career_preferences");
        }
    }
}
