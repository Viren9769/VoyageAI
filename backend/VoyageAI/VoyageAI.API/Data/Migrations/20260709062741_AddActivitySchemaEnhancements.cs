using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoyageAI.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddActivitySchemaEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Travelers_TripId_IsDeleted",
                table: "Travelers");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Travelers",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Activities",
                type: "numeric(11,8)",
                precision: 11,
                scale: 8,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(11,8)",
                oldPrecision: 11,
                oldScale: 8);

            migrationBuilder.AlterColumn<string>(
                name: "LocationName",
                table: "Activities",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Activities",
                type: "numeric(10,8)",
                precision: 10,
                scale: 8,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,8)",
                oldPrecision: 10,
                oldScale: 8);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAiGenerated",
                table: "Activities",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<decimal>(
                name: "EstimatedCost",
                table: "Activities",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Activities",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Activities",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            // First, add a temporary column to hold the converted data
            migrationBuilder.AddColumn<int>(
                name: "Category_temp",
                table: "Activities",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Migrate data from string to int (map string categories to enum values)
            migrationBuilder.Sql(@"
                UPDATE ""Activities"" 
                SET ""Category_temp"" = CASE 
                    WHEN ""Category"" = 'Sightseeing' THEN 0
                    WHEN ""Category"" = 'Restaurant' THEN 1
                    WHEN ""Category"" = 'Museum' THEN 2
                    WHEN ""Category"" = 'Shopping' THEN 3
                    WHEN ""Category"" = 'Transportation' THEN 4
                    WHEN ""Category"" = 'Adventure' THEN 5
                    WHEN ""Category"" = 'Entertainment' THEN 6
                    WHEN ""Category"" = 'Hotel' THEN 7
                    WHEN ""Category"" = 'Flight' THEN 8
                    ELSE 9
                END");

            // Drop the old string column
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Activities");

            // Rename the temp column to the original name
            migrationBuilder.RenameColumn(
                name: "Category_temp",
                table: "Activities",
                newName: "Category");

            migrationBuilder.AlterColumn<string>(
                name: "ActivityName",
                table: "Activities",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualCost",
                table: "Activities",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Activities",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BookingReference",
                table: "Activities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Activities",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Activities",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Activities",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "Activities",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Activities",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Activities",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "Activities",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Activities",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Activities",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Activities",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Activities",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Activities",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Activities",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_Category",
                table: "Activities",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_DayId_IsDeleted",
                table: "Activities",
                columns: new[] { "DayId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_Priority",
                table: "Activities",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_StartTime",
                table: "Activities",
                column: "StartTime");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_Status",
                table: "Activities",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Activities_Category",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_DayId_IsDeleted",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_Priority",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_StartTime",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_Status",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "ActualCost",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "BookingReference",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Activities");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Travelers",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Activities",
                type: "numeric(11,8)",
                precision: 11,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(11,8)",
                oldPrecision: 11,
                oldScale: 8,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "LocationName",
                table: "Activities",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Activities",
                type: "numeric(10,8)",
                precision: 10,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,8)",
                oldPrecision: 10,
                oldScale: 8,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAiGenerated",
                table: "Activities",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "EstimatedCost",
                table: "Activities",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Activities",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Activities",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "ActivityName",
                table: "Activities",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.CreateIndex(
                name: "IX_Travelers_TripId_IsDeleted",
                table: "Travelers",
                columns: new[] { "TripId", "IsDeleted" });
        }
    }
}
