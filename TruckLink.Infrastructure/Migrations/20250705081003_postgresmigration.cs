using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TruckLink.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class postgresmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    load_item = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    contact_info = table.Column<string>(type: "text", nullable: false),
                    start_location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    destination = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    earnings = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    distance_km = table.Column<double>(type: "double precision", nullable: false),
                    map_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_accepted = table.Column<bool>(type: "boolean", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    accepted_by_driver_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_jobs", x => x.id);
                    table.ForeignKey(
                        name: "FK_jobs_users_accepted_by_driver_id",
                        column: x => x.accepted_by_driver_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_jobs_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "job_interests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    mobile_number = table.Column<string>(type: "text", nullable: false),
                    requested_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_accepted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_job_interests", x => x.id);
                    table.ForeignKey(
                        name: "f_k_job_interests__users_driver_id",
                        column: x => x.driver_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_job_interests_jobs_job_id",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_job_interests_driver_id",
                table: "job_interests",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "i_x_job_interests_job_id",
                table: "job_interests",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_accepted_by_driver_id",
                table: "jobs",
                column: "accepted_by_driver_id");

            migrationBuilder.CreateIndex(
                name: "IX_jobs_created_by_user_id",
                table: "jobs",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "job_interests");

            migrationBuilder.DropTable(
                name: "jobs");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
