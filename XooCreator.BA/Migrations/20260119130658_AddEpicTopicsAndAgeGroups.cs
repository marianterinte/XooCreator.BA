using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace XooCreator.BA.Migrations
{
    /// <inheritdoc />
    public partial class AddEpicTopicsAndAgeGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Regions_RegionId",
                schema: "alchimalia_schema",
                table: "Animals");

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000002"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000002"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000002"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000003"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000003"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000003"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000003"), "horn" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000003"), "horns" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000003"), "legs" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000003"), "tail" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000004"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000004"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000004"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000005"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000005"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000005"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000006"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000006"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000006"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000007"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000007"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000007"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000008"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000008"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000008"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000009"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000009"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000009"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000009"), "horn" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000009"), "horns" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000009"), "legs" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000009"), "tail" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000a"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000a"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000a"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000a"), "legs" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000a"), "tail" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000a"), "wings" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000b"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000b"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000b"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000b"), "legs" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000b"), "tail" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000b"), "wings" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000c"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000c"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000c"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000d"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000d"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000d"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000d"), "legs" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000d"), "tail" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000d"), "wings" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000e"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000e"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000e"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000e"), "legs" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000e"), "tail" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000e"), "wings" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000f"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000f"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000000f"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000010"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000010"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000010"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000010"), "legs" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000010"), "tail" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000010"), "wings" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000011"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000011"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000011"), "tail" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000012"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000012"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000012"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000013"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000013"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000013"), "legs" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000013"), "tail" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000014"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000014"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000014"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000015"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000015"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000015"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000016"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000016"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000016"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000016"), "legs" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000016"), "tail" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000017"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000017"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000017"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000017"), "legs" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000017"), "tail" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000018"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000018"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000018"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000018"), "horn" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000018"), "legs" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000018"), "tail" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000019"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000019"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000019"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001a"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001a"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001a"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001a"), "horns" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001a"), "legs" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001a"), "tail" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001b"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001b"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001b"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001c"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001c"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001c"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001c"), "legs" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001c"), "tail" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001d"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001d"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001d"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001d"), "legs" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001d"), "tail" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001d"), "wings" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001e"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001e"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001e"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001f"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001f"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-00000000001f"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000020"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000020"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000020"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000020"), "legs" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000020"), "tail" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000021"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000021"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000021"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000021"), "legs" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000021"), "tail" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000021"), "wings" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000022"), "arms" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000022"), "body" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                keyColumns: new[] { "AnimalId", "PartKey" },
                keyValues: new object[] { new Guid("00000000-0000-0000-0000-000000000022"), "head" });

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("003614ca-880e-4c87-9497-ba00bb755633"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("0abf406a-0dd7-4310-baf3-80eff206a879"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("0bae418c-cd02-4fe0-8cae-ba0709b27843"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("0f7f5fcf-cdd4-4a2f-aee6-04c11ef21c7e"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("15a3d6f3-3d5d-484c-b5a0-c2b3e3598c44"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("231d5136-a8bd-4e1c-a5ed-6a46156db1f4"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("2f194465-3e7e-4df3-8ada-f3eef714762a"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("2f8fc3a2-3aec-4512-974d-17ecba506217"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("2ff01efe-f322-457c-a93a-ca67b88cfced"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("33bf19be-1d72-49b3-b3c1-e9bb329f4316"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("36d80ca6-8f78-42ad-b970-e2caa6d865a8"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("414d0a9d-70e6-42b5-a45e-42cef178d372"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("514a3b0f-b005-4821-b7a8-d8286495950b"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("59bfa96d-3739-486e-9f06-4f9d1c2160d3"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("5fdbd7d2-d8d7-4758-93aa-23aff9740cfb"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("6d8f580f-92ae-44cf-8114-e6b4676e97dd"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("714399e4-8272-4bd1-92bc-b0e2569214ad"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("727e2e83-2957-47ec-945a-155d080c08e9"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("72d3368c-de7d-4a00-a738-f5ab2fd1487a"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("7d95f51f-5d2d-42e6-8df4-f35a0dcfe4fb"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("826fcdf4-4ac5-4619-b1be-a54d3bdd7513"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("82f383c1-7caa-4c12-88e6-7099bfd4e6a9"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("8473a1b9-5b1c-4377-9e6d-b6d6692b59a1"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("8490cf98-c482-4ed2-9219-030af6962ec1"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("9a519ce6-9b80-4d0f-9830-65815451d93c"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("a7e519df-5e82-4d56-a760-87e5fb1c11bc"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("ad8ea5b4-b3ca-41ef-8af5-a08ef0c9e6c1"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("afcc1664-b026-4eec-92c5-5a25b26a9bb7"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("b42320d3-0800-49f3-95e7-34d8926b77b0"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("bc3d333d-669d-4be8-ae49-6882833af8be"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("de2fd098-ff21-4592-a8ad-2a56c0d1390d"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("f1f0ac42-5fac-4a99-aa34-86cfb2f0a818"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("fd4a9bea-32ad-4a14-9c61-7c5632a8724c"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                keyColumn: "Id",
                keyValue: new Guid("fe6fc993-4542-443b-a6b1-84ff2e228455"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "BodyPartTranslations",
                keyColumn: "Id",
                keyValue: new Guid("07deeccd-b677-4f0e-9d5b-58e937ea6006"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "BodyPartTranslations",
                keyColumn: "Id",
                keyValue: new Guid("274952ca-a936-4639-ab67-1aecbb85bc5b"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "BodyPartTranslations",
                keyColumn: "Id",
                keyValue: new Guid("6c36036c-86b4-4dd7-aed3-bcd915d059f2"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "BodyPartTranslations",
                keyColumn: "Id",
                keyValue: new Guid("7f7e9e04-b282-4d5b-839b-bdebdf262f31"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "BodyPartTranslations",
                keyColumn: "Id",
                keyValue: new Guid("8087c412-bbf8-4101-8823-9df83c8da0dc"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "BodyPartTranslations",
                keyColumn: "Id",
                keyValue: new Guid("9cfcfe92-8082-4951-8d45-7dd52809a26f"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "BodyPartTranslations",
                keyColumn: "Id",
                keyValue: new Guid("d0fcca76-457f-46ad-a4c9-2fc620e6ad82"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "BodyPartTranslations",
                keyColumn: "Id",
                keyValue: new Guid("d6422238-61dc-43e2-89e7-e80e0b7ed26b"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroClickMessages",
                keyColumn: "Id",
                keyValue: new Guid("3e089534-9de3-49a2-a543-449f90ccf8a8"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroClickMessages",
                keyColumn: "Id",
                keyValue: new Guid("b2c18476-7fa8-4ad0-9fb7-494ccf51637f"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroClickMessages",
                keyColumn: "Id",
                keyValue: new Guid("f371c87e-6f7c-4d70-89cd-77461832b974"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroDefinitions",
                keyColumn: "Id",
                keyValue: "grubot");

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroDefinitions",
                keyColumn: "Id",
                keyValue: "linkaro");

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroDefinitions",
                keyColumn: "Id",
                keyValue: "puf-puf");

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("0331af85-a296-40bd-a68b-839c9a8966ee"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("05130a8f-d16d-4efd-bb72-bbd81e0ba497"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("053133ad-b99d-448a-b740-9109bb1b299d"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("095ac8b2-1425-45ad-919d-0b4d770935b1"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("10841b71-ecb0-42a0-b4f1-6424cf753186"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("150e8f7e-2907-4f32-a890-89572085c4d8"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("15228aa7-6688-4b49-bb80-7ba9a19ec0f9"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("1eb45cb0-1aba-43b4-9537-13866e55821b"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("216bbbc1-6392-4776-a3ee-59b75a6e1590"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("32882744-ec76-46e8-bdb7-a28edc668939"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("3d9c4c13-8706-4e7d-aa03-dbbed5ee255c"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("464b15f2-f2f1-4052-82df-b3bc4e4410ea"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("55d7d48a-f52f-41c8-9a9c-bc1f5ebae449"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("569c3eed-aeee-4c00-9413-20097458a66d"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("5e61f8ed-352c-4181-98a7-1f1c4ba14552"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("715613a7-4a7c-4810-9e79-4893abd4979d"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("898eceb1-87ae-4ba4-bcd0-60d036f111df"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("8d9cb436-ce1a-4553-877f-414d832cc078"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("91b8cfe3-b057-4022-a950-b8f04990a9dc"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("a7344331-f07f-45e5-8adc-dc8fe46713bb"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("aacdc921-26ab-48d5-a95a-628bb7fe446a"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("b054b29a-f492-4e60-a07a-9297a11c4344"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("b17263bf-e10b-44ba-9d2d-ef06fc75297a"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("bdc34441-9442-466c-b2b7-ad4f8ba0cf58"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("befceaf3-b75b-4f99-8bce-e2e9d426aa54"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("c480afe9-adbf-40d7-8513-5772872178e0"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("c5670208-640d-43bd-9eb5-ec39f8032f24"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("cd670e6c-4474-4c36-b0fe-f74aaacc50a5"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("d47fc9b2-7595-4433-a8d5-558333e911e9"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("d4de0633-61c4-47f3-b755-0e523af02ed8"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("d9f9f339-43eb-4958-80ec-418b12025177"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("e19aa3c2-440d-46df-b2dd-a9660ad8cb35"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("ef3dd111-bfc1-41aa-8f2a-be75c4e135e5"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("efd682b9-07e3-485e-bea7-a01f68999dae"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                keyColumn: "Id",
                keyValue: new Guid("fe6d4822-e85e-43fa-854e-f2c8c4a67511"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "StoryHeroUnlocks",
                keyColumn: "Id",
                keyValue: new Guid("60660482-e7c8-4811-a62b-ff8b8aef94e5"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "StoryHeroUnlocks",
                keyColumn: "Id",
                keyValue: new Guid("c418d02b-db18-4065-b910-b922a20a454e"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "StoryHeroUnlocks",
                keyColumn: "Id",
                keyValue: new Guid("f27a146c-37ce-425b-8ece-ec9d1b3f42eb"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000000a"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000000b"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000000c"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000000d"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000000e"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000000f"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000015"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000016"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000017"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000018"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000019"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000001a"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000001b"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000001c"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000001d"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000001e"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-00000000001f"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000020"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000021"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000022"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "BodyParts",
                keyColumn: "Key",
                keyValue: "arms");

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "BodyParts",
                keyColumn: "Key",
                keyValue: "body");

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "BodyParts",
                keyColumn: "Key",
                keyValue: "head");

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "BodyParts",
                keyColumn: "Key",
                keyValue: "horn");

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "BodyParts",
                keyColumn: "Key",
                keyValue: "horns");

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "BodyParts",
                keyColumn: "Key",
                keyValue: "legs");

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "BodyParts",
                keyColumn: "Key",
                keyValue: "tail");

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "BodyParts",
                keyColumn: "Key",
                keyValue: "wings");

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "StoryHeroes",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000100"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "StoryHeroes",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111100"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "StoryHeroes",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222200"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                schema: "alchimalia_schema",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"));

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "alchimalia_schema",
                table: "StoryHeroes");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "alchimalia_schema",
                table: "HeroDefinitions");

            migrationBuilder.RenameColumn(
                name: "UnlockConditionJson",
                schema: "alchimalia_schema",
                table: "StoryHeroes",
                newName: "UnlockConditionsJson");

            migrationBuilder.AddColumn<string>(
                name: "ContentHash",
                schema: "alchimalia_schema",
                table: "StoryTileTranslations",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentHash",
                schema: "alchimalia_schema",
                table: "StoryTiles",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                schema: "alchimalia_schema",
                table: "StoryHeroes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewNotes",
                schema: "alchimalia_schema",
                table: "StoryHeroes",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedByUserId",
                schema: "alchimalia_schema",
                table: "StoryHeroes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "alchimalia_schema",
                table: "StoryHeroes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "draft");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                schema: "alchimalia_schema",
                table: "StoryHeroes",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<bool>(
                name: "IsEvaluative",
                schema: "alchimalia_schema",
                table: "StoryDefinitions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPartOfEpic",
                schema: "alchimalia_schema",
                table: "StoryDefinitions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LastPublishedVersion",
                schema: "alchimalia_schema",
                table: "StoryDefinitions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsEvaluative",
                schema: "alchimalia_schema",
                table: "StoryCrafts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPartOfEpic",
                schema: "alchimalia_schema",
                table: "StoryCrafts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LastDraftVersion",
                schema: "alchimalia_schema",
                table: "StoryCrafts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                schema: "alchimalia_schema",
                table: "StoryCraftAnswers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                schema: "alchimalia_schema",
                table: "StoryAnswers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AudioUrl",
                schema: "alchimalia_schema",
                table: "HeroDefinitionTranslations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                schema: "alchimalia_schema",
                table: "HeroDefinitions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentVersionId",
                schema: "alchimalia_schema",
                table: "HeroDefinitions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewNotes",
                schema: "alchimalia_schema",
                table: "HeroDefinitions",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedByUserId",
                schema: "alchimalia_schema",
                table: "HeroDefinitions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "alchimalia_schema",
                table: "HeroDefinitions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "draft");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                schema: "alchimalia_schema",
                table: "HeroDefinitions",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AlterColumn<Guid>(
                name: "RegionId",
                schema: "alchimalia_schema",
                table: "Animals",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "alchimalia_schema",
                table: "Animals",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                schema: "alchimalia_schema",
                table: "Animals",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentVersionId",
                schema: "alchimalia_schema",
                table: "Animals",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewNotes",
                schema: "alchimalia_schema",
                table: "Animals",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedByUserId",
                schema: "alchimalia_schema",
                table: "Animals",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "alchimalia_schema",
                table: "Animals",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "draft");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "alchimalia_schema",
                table: "Animals",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                schema: "alchimalia_schema",
                table: "Animals",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<bool>(
                name: "AutoFilterStoriesByAge",
                schema: "alchimalia_schema",
                table: "AlchimaliaUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string[]>(
                name: "SelectedAgeGroupIds",
                schema: "alchimalia_schema",
                table: "AlchimaliaUsers",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_StoryCrafts_StoryId",
                schema: "alchimalia_schema",
                table: "StoryCrafts",
                column: "StoryId");

            migrationBuilder.CreateTable(
                name: "AnimalCrafts",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PublishedDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Label = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Src = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsHybrid = table.Column<bool>(type: "boolean", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    BaseVersion = table.Column<int>(type: "integer", nullable: false),
                    LastDraftVersion = table.Column<int>(type: "integer", nullable: false),
                    AssignedReviewerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewStartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewEndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewNotes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalCrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnimalCrafts_AlchimaliaUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AnimalCrafts_AlchimaliaUsers_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AnimalCrafts_Regions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnimalDefinitions",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    BaseVersion = table.Column<int>(type: "integer", nullable: false),
                    LastPublishedVersion = table.Column<int>(type: "integer", nullable: false),
                    Label = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Src = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsHybrid = table.Column<bool>(type: "boolean", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PublishedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    PublishedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnimalDefinitions_AlchimaliaUsers_PublishedByUserId",
                        column: x => x.PublishedByUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AnimalDefinitions_Regions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnimalPublishChangeLogs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uuid", nullable: false),
                    DraftVersion = table.Column<int>(type: "integer", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ChangeType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: true),
                    AssetDraftPath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    AssetPublishedPath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalPublishChangeLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnimalPublishJobs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedByEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    LangTag = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ForceFull = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DequeueCount = table.Column<int>(type: "integer", nullable: false),
                    QueuedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalPublishJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnimalVersionJobs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedByEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    BaseVersion = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DequeueCount = table.Column<int>(type: "integer", nullable: false),
                    QueuedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalVersionJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnimalVersionJobs_AlchimaliaUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnimalVersions",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SnapshotJson = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnimalVersions_Animals_AnimalId",
                        column: x => x.AnimalId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "Animals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EpicAssetLinks",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EpicId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DraftVersion = table.Column<int>(type: "integer", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    AssetType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DraftPath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    PublishedPath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ContentHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpicAssetLinks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EpicHero",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    GreetingText = table.Column<string>(type: "text", nullable: true),
                    GreetingAudioUrl = table.Column<string>(type: "text", nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublishedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AssignedReviewerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewNotes = table.Column<string>(type: "text", nullable: true),
                    ReviewStartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewEndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpicHero", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EpicHero_AlchimaliaUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EpicHeroCrafts",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BaseVersion = table.Column<int>(type: "integer", nullable: false),
                    LastDraftVersion = table.Column<int>(type: "integer", nullable: false),
                    AssignedReviewerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewNotes = table.Column<string>(type: "text", nullable: true),
                    ReviewStartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewEndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpicHeroCrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EpicHeroCrafts_AlchimaliaUsers_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EpicHeroCrafts_AlchimaliaUsers_AssignedReviewerUserId",
                        column: x => x.AssignedReviewerUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EpicHeroCrafts_AlchimaliaUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EpicHeroCrafts_AlchimaliaUsers_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EpicHeroDefinitions",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublishedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    BaseVersion = table.Column<int>(type: "integer", nullable: false),
                    LastPublishedVersion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpicHeroDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EpicHeroDefinitions_AlchimaliaUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EpicProgress",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<string>(type: "text", nullable: false),
                    IsUnlocked = table.Column<bool>(type: "boolean", nullable: false),
                    EpicId = table.Column<string>(type: "text", nullable: false),
                    UnlockedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpicProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EpicProgress_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EpicPublishChangeLogs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EpicId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DraftVersion = table.Column<int>(type: "integer", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ChangeType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: true),
                    AssetDraftPath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    AssetPublishedPath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpicPublishChangeLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EpicPublishJobs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EpicId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedByEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    LangTag = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    DraftVersion = table.Column<int>(type: "integer", nullable: false),
                    ForceFull = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DequeueCount = table.Column<int>(type: "integer", nullable: false),
                    QueuedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpicPublishJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EpicStoryProgress",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "text", nullable: false),
                    SelectedAnswer = table.Column<string>(type: "text", nullable: true),
                    TokensJson = table.Column<string>(type: "text", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EpicId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpicStoryProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EpicStoryProgress_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EpicVersionJobs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EpicId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedByEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    BaseVersion = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DequeueCount = table.Column<int>(type: "integer", nullable: false),
                    QueuedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpicVersionJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroAssetLinks",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DraftVersion = table.Column<int>(type: "integer", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    AssetType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DraftPath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    PublishedPath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ContentHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroAssetLinks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroDefinitionCrafts",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PublishedDefinitionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BaseVersion = table.Column<int>(type: "integer", nullable: false),
                    LastDraftVersion = table.Column<int>(type: "integer", nullable: false),
                    AssignedReviewerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewStartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewEndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CourageCost = table.Column<int>(type: "integer", nullable: false),
                    CuriosityCost = table.Column<int>(type: "integer", nullable: false),
                    ThinkingCost = table.Column<int>(type: "integer", nullable: false),
                    CreativityCost = table.Column<int>(type: "integer", nullable: false),
                    SafetyCost = table.Column<int>(type: "integer", nullable: false),
                    PrerequisitesJson = table.Column<string>(type: "jsonb", nullable: false),
                    RewardsJson = table.Column<string>(type: "jsonb", nullable: false),
                    IsUnlocked = table.Column<bool>(type: "boolean", nullable: false),
                    PositionX = table.Column<double>(type: "double precision", nullable: false),
                    PositionY = table.Column<double>(type: "double precision", nullable: false),
                    Image = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewNotes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroDefinitionCrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroDefinitionCrafts_AlchimaliaUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_HeroDefinitionCrafts_AlchimaliaUsers_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HeroDefinitionDefinitions",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    BaseVersion = table.Column<int>(type: "integer", nullable: false),
                    LastPublishedVersion = table.Column<int>(type: "integer", nullable: false),
                    CourageCost = table.Column<int>(type: "integer", nullable: false),
                    CuriosityCost = table.Column<int>(type: "integer", nullable: false),
                    ThinkingCost = table.Column<int>(type: "integer", nullable: false),
                    CreativityCost = table.Column<int>(type: "integer", nullable: false),
                    SafetyCost = table.Column<int>(type: "integer", nullable: false),
                    PrerequisitesJson = table.Column<string>(type: "jsonb", nullable: false),
                    RewardsJson = table.Column<string>(type: "jsonb", nullable: false),
                    IsUnlocked = table.Column<bool>(type: "boolean", nullable: false),
                    PositionX = table.Column<double>(type: "double precision", nullable: false),
                    PositionY = table.Column<double>(type: "double precision", nullable: false),
                    Image = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PublishedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    PublishedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroDefinitionDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroDefinitionDefinitions_AlchimaliaUsers_PublishedByUserId",
                        column: x => x.PublishedByUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HeroDefinitionPublishChangeLogs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DraftVersion = table.Column<int>(type: "integer", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ChangeType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: true),
                    AssetDraftPath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    AssetPublishedPath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroDefinitionPublishChangeLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroDefinitionVersionJobs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedByEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    BaseVersion = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DequeueCount = table.Column<int>(type: "integer", nullable: false),
                    QueuedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroDefinitionVersionJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroDefinitionVersionJobs_AlchimaliaUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HeroDefinitionVersions",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroDefinitionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SnapshotJson = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroDefinitionVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroDefinitionVersions_HeroDefinitions_HeroDefinitionId",
                        column: x => x.HeroDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "HeroDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeroPublishChangeLogs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DraftVersion = table.Column<int>(type: "integer", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ChangeType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: true),
                    AssetDraftPath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    AssetPublishedPath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroPublishChangeLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroPublishJobs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedByEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    LangTag = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ForceFull = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DequeueCount = table.Column<int>(type: "integer", nullable: false),
                    QueuedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroPublishJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroVersionJobs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedByEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    BaseVersion = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DequeueCount = table.Column<int>(type: "integer", nullable: false),
                    QueuedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroVersionJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlatformSettings",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    BoolValue = table.Column<bool>(type: "boolean", nullable: false),
                    StringValue = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformSettings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "RegionAssetLinks",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DraftVersion = table.Column<int>(type: "integer", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    AssetType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DraftPath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    PublishedPath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ContentHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionAssetLinks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegionPublishChangeLogs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DraftVersion = table.Column<int>(type: "integer", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ChangeType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: true),
                    AssetDraftPath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    AssetPublishedPath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionPublishChangeLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegionVersionJobs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedByEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    BaseVersion = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DequeueCount = table.Column<int>(type: "integer", nullable: false),
                    QueuedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionVersionJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoryAssetLinks",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DraftVersion = table.Column<int>(type: "integer", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    AssetType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DraftPath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    PublishedPath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    ContentHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryAssetLinks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoryCraftUnlockedHeroes",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    StoryCraftId = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryCraftUnlockedHeroes", x => new { x.StoryCraftId, x.HeroId });
                    table.ForeignKey(
                        name: "FK_StoryCraftUnlockedHeroes_StoryCrafts_StoryCraftId",
                        column: x => x.StoryCraftId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryDefinitionUnlockedHeroes",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    StoryDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryDefinitionUnlockedHeroes", x => new { x.StoryDefinitionId, x.HeroId });
                    table.ForeignKey(
                        name: "FK_StoryDefinitionUnlockedHeroes_StoryDefinitions_StoryDefinit~",
                        column: x => x.StoryDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryDocumentExportJobs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "text", nullable: false),
                    StoryOwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedByEmail = table.Column<string>(type: "text", nullable: false),
                    Locale = table.Column<string>(type: "text", nullable: false),
                    IsDraft = table.Column<bool>(type: "boolean", nullable: false),
                    Format = table.Column<string>(type: "text", nullable: false),
                    PaperSize = table.Column<string>(type: "text", nullable: false),
                    IncludeCover = table.Column<bool>(type: "boolean", nullable: false),
                    IncludeQuizAnswers = table.Column<bool>(type: "boolean", nullable: false),
                    UseMobileImageLayout = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    DequeueCount = table.Column<int>(type: "integer", nullable: false),
                    QueuedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    OutputBlobPath = table.Column<string>(type: "text", nullable: true),
                    OutputFileName = table.Column<string>(type: "text", nullable: true),
                    OutputSizeBytes = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryDocumentExportJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoryEpicCrafts",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CoverImageUrl = table.Column<string>(type: "text", nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BaseVersion = table.Column<int>(type: "integer", nullable: false),
                    LastDraftVersion = table.Column<int>(type: "integer", nullable: false),
                    AssignedReviewerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewNotes = table.Column<string>(type: "text", nullable: true),
                    ReviewStartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewEndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryEpicCrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryEpicCrafts_AlchimaliaUsers_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_StoryEpicCrafts_AlchimaliaUsers_AssignedReviewerUserId",
                        column: x => x.AssignedReviewerUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_StoryEpicCrafts_AlchimaliaUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryEpicCrafts_AlchimaliaUsers_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "StoryEpicDefinitions",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CoverImageUrl = table.Column<string>(type: "text", nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublishedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    BaseVersion = table.Column<int>(type: "integer", nullable: false),
                    LastPublishedVersion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryEpicDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryEpicDefinitions_AlchimaliaUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryEvaluationResults",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalQuizzes = table.Column<int>(type: "integer", nullable: false),
                    CorrectAnswers = table.Column<int>(type: "integer", nullable: false),
                    ScorePercentage = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryEvaluationResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryEvaluationResults_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryExportJobs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "text", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedByEmail = table.Column<string>(type: "text", nullable: false),
                    Locale = table.Column<string>(type: "text", nullable: false),
                    IsDraft = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    DequeueCount = table.Column<int>(type: "integer", nullable: false),
                    QueuedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    ZipBlobPath = table.Column<string>(type: "text", nullable: true),
                    ZipFileName = table.Column<string>(type: "text", nullable: true),
                    ZipSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    MediaCount = table.Column<int>(type: "integer", nullable: true),
                    LanguageCount = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryExportJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoryForkAssetJobs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceStoryId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    SourceType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    SourceOwnerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    SourceOwnerEmail = table.Column<string>(type: "text", nullable: true),
                    TargetStoryId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TargetOwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetOwnerEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    RequestedByEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DequeueCount = table.Column<int>(type: "integer", nullable: false),
                    QueuedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AttemptedAssets = table.Column<int>(type: "integer", nullable: false),
                    CopiedAssets = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    WarningSummary = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryForkAssetJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoryForkJobs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceStoryId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    SourceType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CopyAssets = table.Column<bool>(type: "boolean", nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedByEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TargetOwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetOwnerEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TargetStoryId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DequeueCount = table.Column<int>(type: "integer", nullable: false),
                    QueuedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    WarningSummary = table.Column<string>(type: "text", nullable: true),
                    AssetJobId = table.Column<Guid>(type: "uuid", nullable: true),
                    AssetJobStatus = table.Column<string>(type: "text", nullable: true),
                    SourceTranslations = table.Column<int>(type: "integer", nullable: false),
                    SourceTiles = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryForkJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoryHeroTranslations",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryHeroId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    GreetingText = table.Column<string>(type: "text", nullable: true),
                    GreetingAudioUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryHeroTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryHeroTranslations_StoryHeroes_StoryHeroId",
                        column: x => x.StoryHeroId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryHeroes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryHeroVersions",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryHeroId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SnapshotJson = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryHeroVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryHeroVersions_StoryHeroes_StoryHeroId",
                        column: x => x.StoryHeroId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryHeroes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryImportJobs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    OriginalStoryId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedByEmail = table.Column<string>(type: "text", nullable: false),
                    Locale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    ZipBlobPath = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ZipFileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ZipSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DequeueCount = table.Column<int>(type: "integer", nullable: false),
                    QueuedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ImportedAssets = table.Column<int>(type: "integer", nullable: false),
                    TotalAssets = table.Column<int>(type: "integer", nullable: false),
                    ImportedLanguagesCount = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    WarningSummary = table.Column<string>(type: "text", nullable: true),
                    IncludeImages = table.Column<bool>(type: "boolean", nullable: false),
                    IncludeAudio = table.Column<bool>(type: "boolean", nullable: false),
                    IncludeVideo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryImportJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoryLikes",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LikedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryLikes_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryPublishChangeLogs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DraftVersion = table.Column<int>(type: "integer", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ChangeType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: true),
                    AssetDraftPath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    AssetPublishedPath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryPublishChangeLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoryPublishJobs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedByEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    LangTag = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    DraftVersion = table.Column<int>(type: "integer", nullable: false),
                    ForceFull = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DequeueCount = table.Column<int>(type: "integer", nullable: false),
                    QueuedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryPublishJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoryQuizAnswers",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TileId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SelectedAnswerId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    AnsweredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryQuizAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryQuizAnswers_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryRegion",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublishedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AssignedReviewerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewNotes = table.Column<string>(type: "text", nullable: true),
                    ReviewStartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewEndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryRegion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryRegion_AlchimaliaUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryRegionCrafts",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BaseVersion = table.Column<int>(type: "integer", nullable: false),
                    LastDraftVersion = table.Column<int>(type: "integer", nullable: false),
                    AssignedReviewerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewNotes = table.Column<string>(type: "text", nullable: true),
                    ReviewStartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewEndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedByUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryRegionCrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryRegionCrafts_AlchimaliaUsers_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_StoryRegionCrafts_AlchimaliaUsers_AssignedReviewerUserId",
                        column: x => x.AssignedReviewerUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_StoryRegionCrafts_AlchimaliaUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryRegionCrafts_AlchimaliaUsers_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "StoryRegionDefinitions",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublishedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    BaseVersion = table.Column<int>(type: "integer", nullable: false),
                    LastPublishedVersion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryRegionDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryRegionDefinitions_AlchimaliaUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryVersionJobs",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedByEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    BaseVersion = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DequeueCount = table.Column<int>(type: "integer", nullable: false),
                    QueuedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryVersionJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TreeOfHeroesConfigCrafts",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PublishedDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Label = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewNotes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeOfHeroesConfigCrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreeOfHeroesConfigCrafts_AlchimaliaUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TreeOfHeroesConfigCrafts_AlchimaliaUsers_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TreeOfHeroesConfigDefinitions",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Label = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PublishedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublishedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeOfHeroesConfigDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreeOfHeroesConfigDefinitions_AlchimaliaUsers_PublishedByUs~",
                        column: x => x.PublishedByUserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UserStoryReadHistory",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TotalTilesRead = table.Column<int>(type: "integer", nullable: false),
                    TotalTiles = table.Column<int>(type: "integer", nullable: false),
                    LastReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStoryReadHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserStoryReadHistory_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnimalCraftPartSupports",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    AnimalCraftId = table.Column<Guid>(type: "uuid", nullable: false),
                    BodyPartKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalCraftPartSupports", x => new { x.AnimalCraftId, x.BodyPartKey });
                    table.ForeignKey(
                        name: "FK_AnimalCraftPartSupports_AnimalCrafts_AnimalCraftId",
                        column: x => x.AnimalCraftId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AnimalCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimalCraftPartSupports_BodyParts_BodyPartKey",
                        column: x => x.BodyPartKey,
                        principalSchema: "alchimalia_schema",
                        principalTable: "BodyParts",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnimalCraftTranslations",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalCraftId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Label = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    AudioUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalCraftTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnimalCraftTranslations_AnimalCrafts_AnimalCraftId",
                        column: x => x.AnimalCraftId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AnimalCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnimalDefinitionPartSupports",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    AnimalDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    BodyPartKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalDefinitionPartSupports", x => new { x.AnimalDefinitionId, x.BodyPartKey });
                    table.ForeignKey(
                        name: "FK_AnimalDefinitionPartSupports_AnimalDefinitions_AnimalDefini~",
                        column: x => x.AnimalDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AnimalDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimalDefinitionPartSupports_BodyParts_BodyPartKey",
                        column: x => x.BodyPartKey,
                        principalSchema: "alchimalia_schema",
                        principalTable: "BodyParts",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnimalDefinitionTranslations",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Label = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    AudioUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalDefinitionTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnimalDefinitionTranslations_AnimalDefinitions_AnimalDefini~",
                        column: x => x.AnimalDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AnimalDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnimalHybridCraftParts",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalCraftId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceAnimalId = table.Column<Guid>(type: "uuid", nullable: false),
                    BodyPartKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalHybridCraftParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnimalHybridCraftParts_AnimalCrafts_AnimalCraftId",
                        column: x => x.AnimalCraftId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AnimalCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimalHybridCraftParts_AnimalDefinitions_SourceAnimalId",
                        column: x => x.SourceAnimalId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AnimalDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnimalHybridCraftParts_BodyParts_BodyPartKey",
                        column: x => x.BodyPartKey,
                        principalSchema: "alchimalia_schema",
                        principalTable: "BodyParts",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnimalHybridDefinitionParts",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceAnimalId = table.Column<Guid>(type: "uuid", nullable: false),
                    BodyPartKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalHybridDefinitionParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnimalHybridDefinitionParts_AnimalDefinitions_AnimalDefinit~",
                        column: x => x.AnimalDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AnimalDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimalHybridDefinitionParts_AnimalDefinitions_SourceAnimalId",
                        column: x => x.SourceAnimalId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AnimalDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnimalHybridDefinitionParts_BodyParts_BodyPartKey",
                        column: x => x.BodyPartKey,
                        principalSchema: "alchimalia_schema",
                        principalTable: "BodyParts",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EpicHeroTranslation",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EpicHeroId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    GreetingText = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpicHeroTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EpicHeroTranslation_EpicHero_EpicHeroId",
                        column: x => x.EpicHeroId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "EpicHero",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EpicHeroCraftTranslations",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EpicHeroCraftId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    GreetingText = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    GreetingAudioUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpicHeroCraftTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EpicHeroCraftTranslations_EpicHeroCrafts_EpicHeroCraftId",
                        column: x => x.EpicHeroCraftId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "EpicHeroCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EpicHeroDefinitionTranslations",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EpicHeroDefinitionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    GreetingText = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    GreetingAudioUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpicHeroDefinitionTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EpicHeroDefinitionTranslations_EpicHeroDefinitions_EpicHero~",
                        column: x => x.EpicHeroDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "EpicHeroDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeroDefinitionCraftTranslations",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroDefinitionCraftId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Story = table.Column<string>(type: "text", nullable: false),
                    AudioUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroDefinitionCraftTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroDefinitionCraftTranslations_HeroDefinitionCrafts_HeroDe~",
                        column: x => x.HeroDefinitionCraftId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "HeroDefinitionCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeroDefinitionDefinitionTranslations",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroDefinitionDefinitionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Story = table.Column<string>(type: "text", nullable: false),
                    AudioUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroDefinitionDefinitionTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroDefinitionDefinitionTranslations_HeroDefinitionDefiniti~",
                        column: x => x.HeroDefinitionDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "HeroDefinitionDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryEpicCraftAgeGroup",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    StoryEpicCraftId = table.Column<string>(type: "character varying(100)", nullable: false),
                    StoryAgeGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryEpicCraftAgeGroup", x => new { x.StoryEpicCraftId, x.StoryAgeGroupId });
                    table.ForeignKey(
                        name: "FK_StoryEpicCraftAgeGroup_StoryAgeGroups_StoryAgeGroupId",
                        column: x => x.StoryAgeGroupId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryAgeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryEpicCraftAgeGroup_StoryEpicCrafts_StoryEpicCraftId",
                        column: x => x.StoryEpicCraftId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryEpicCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryEpicCraftHeroReferences",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpicId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    HeroId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StoryId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryEpicCraftHeroReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryEpicCraftHeroReferences_EpicHeroCrafts_HeroId",
                        column: x => x.HeroId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "EpicHeroCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoryEpicCraftHeroReferences_StoryEpicCrafts_EpicId",
                        column: x => x.EpicId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryEpicCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryEpicCraftRegions",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpicId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RegionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                    IsStartupRegion = table.Column<bool>(type: "boolean", nullable: false),
                    X = table.Column<double>(type: "double precision", nullable: true),
                    Y = table.Column<double>(type: "double precision", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryEpicCraftRegions", x => x.Id);
                    table.UniqueConstraint("AK_StoryEpicCraftRegions_EpicId_RegionId", x => new { x.EpicId, x.RegionId });
                    table.ForeignKey(
                        name: "FK_StoryEpicCraftRegions_StoryEpicCrafts_EpicId",
                        column: x => x.EpicId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryEpicCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryEpicCraftTopic",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    StoryEpicCraftId = table.Column<string>(type: "character varying(100)", nullable: false),
                    StoryTopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryEpicCraftTopic", x => new { x.StoryEpicCraftId, x.StoryTopicId });
                    table.ForeignKey(
                        name: "FK_StoryEpicCraftTopic_StoryEpicCrafts_StoryEpicCraftId",
                        column: x => x.StoryEpicCraftId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryEpicCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryEpicCraftTopic_StoryTopics_StoryTopicId",
                        column: x => x.StoryTopicId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryEpicCraftTranslations",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryEpicCraftId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryEpicCraftTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryEpicCraftTranslations_StoryEpicCrafts_StoryEpicCraftId",
                        column: x => x.StoryEpicCraftId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryEpicCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryEpicCraftUnlockRules",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpicId = table.Column<string>(type: "character varying(100)", nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FromId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ToRegionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ToStoryId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RequiredStoriesCsv = table.Column<string>(type: "text", nullable: true),
                    MinCount = table.Column<int>(type: "integer", nullable: true),
                    StoryId = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryEpicCraftUnlockRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryEpicCraftUnlockRules_StoryEpicCrafts_EpicId",
                        column: x => x.EpicId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryEpicCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EpicReaders",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EpicId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AcquiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AcquisitionSource = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpicReaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EpicReaders_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EpicReaders_StoryEpicDefinitions_EpicId",
                        column: x => x.EpicId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryEpicDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EpicReviews",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EpicId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpicReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EpicReviews_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EpicReviews_StoryEpicDefinitions_EpicId",
                        column: x => x.EpicId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryEpicDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryEpicDefinitionAgeGroup",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    StoryEpicDefinitionId = table.Column<string>(type: "character varying(100)", nullable: false),
                    StoryAgeGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryEpicDefinitionAgeGroup", x => new { x.StoryEpicDefinitionId, x.StoryAgeGroupId });
                    table.ForeignKey(
                        name: "FK_StoryEpicDefinitionAgeGroup_StoryAgeGroups_StoryAgeGroupId",
                        column: x => x.StoryAgeGroupId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryAgeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryEpicDefinitionAgeGroup_StoryEpicDefinitions_StoryEpicD~",
                        column: x => x.StoryEpicDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryEpicDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryEpicDefinitionRegions",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpicId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RegionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                    IsStartupRegion = table.Column<bool>(type: "boolean", nullable: false),
                    X = table.Column<double>(type: "double precision", nullable: true),
                    Y = table.Column<double>(type: "double precision", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryEpicDefinitionRegions", x => x.Id);
                    table.UniqueConstraint("AK_StoryEpicDefinitionRegions_EpicId_RegionId", x => new { x.EpicId, x.RegionId });
                    table.ForeignKey(
                        name: "FK_StoryEpicDefinitionRegions_StoryEpicDefinitions_EpicId",
                        column: x => x.EpicId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryEpicDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryEpicDefinitionTopic",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    StoryEpicDefinitionId = table.Column<string>(type: "character varying(100)", nullable: false),
                    StoryTopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryEpicDefinitionTopic", x => new { x.StoryEpicDefinitionId, x.StoryTopicId });
                    table.ForeignKey(
                        name: "FK_StoryEpicDefinitionTopic_StoryEpicDefinitions_StoryEpicDefi~",
                        column: x => x.StoryEpicDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryEpicDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryEpicDefinitionTopic_StoryTopics_StoryTopicId",
                        column: x => x.StoryTopicId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryEpicDefinitionTranslations",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryEpicDefinitionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryEpicDefinitionTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryEpicDefinitionTranslations_StoryEpicDefinitions_StoryE~",
                        column: x => x.StoryEpicDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryEpicDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryEpicDefinitionUnlockRules",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpicId = table.Column<string>(type: "character varying(100)", nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FromId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ToRegionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ToStoryId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RequiredStoriesCsv = table.Column<string>(type: "text", nullable: true),
                    MinCount = table.Column<int>(type: "integer", nullable: true),
                    StoryId = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryEpicDefinitionUnlockRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryEpicDefinitionUnlockRules_StoryEpicDefinitions_EpicId",
                        column: x => x.EpicId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryEpicDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryEpicHeroReferences",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpicId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    HeroId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StoryId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryEpicHeroReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryEpicHeroReferences_EpicHeroDefinitions_HeroId",
                        column: x => x.HeroId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "EpicHeroDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoryEpicHeroReferences_StoryEpicDefinitions_EpicId",
                        column: x => x.EpicId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryEpicDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFavoriteEpics",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EpicId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoriteEpics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFavoriteEpics_AlchimaliaUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "AlchimaliaUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavoriteEpics_StoryEpicDefinitions_EpicId",
                        column: x => x.EpicId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryEpicDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryRegionTranslation",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryRegionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryRegionTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryRegionTranslation_StoryRegion_StoryRegionId",
                        column: x => x.StoryRegionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryRegion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryRegionCraftTranslations",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryRegionCraftId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryRegionCraftTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryRegionCraftTranslations_StoryRegionCrafts_StoryRegionC~",
                        column: x => x.StoryRegionCraftId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryRegionCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryEpicRegionReferences",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpicId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RegionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    X = table.Column<double>(type: "double precision", nullable: true),
                    Y = table.Column<double>(type: "double precision", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryEpicRegionReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryEpicRegionReferences_StoryEpicDefinitions_EpicId",
                        column: x => x.EpicId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryEpicDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryEpicRegionReferences_StoryRegionDefinitions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryRegionDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StoryRegionDefinitionTranslations",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoryRegionDefinitionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryRegionDefinitionTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryRegionDefinitionTranslations_StoryRegionDefinitions_St~",
                        column: x => x.StoryRegionDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryRegionDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreeOfHeroesConfigCraftEdges",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfigCraftId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromHeroId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ToHeroId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeOfHeroesConfigCraftEdges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreeOfHeroesConfigCraftEdges_HeroDefinitionDefinitions_From~",
                        column: x => x.FromHeroId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "HeroDefinitionDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreeOfHeroesConfigCraftEdges_HeroDefinitionDefinitions_ToHe~",
                        column: x => x.ToHeroId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "HeroDefinitionDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreeOfHeroesConfigCraftEdges_TreeOfHeroesConfigCrafts_Confi~",
                        column: x => x.ConfigCraftId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "TreeOfHeroesConfigCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreeOfHeroesConfigCraftNodes",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfigCraftId = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroDefinitionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PositionX = table.Column<double>(type: "double precision", nullable: false),
                    PositionY = table.Column<double>(type: "double precision", nullable: false),
                    CourageCost = table.Column<int>(type: "integer", nullable: false),
                    CuriosityCost = table.Column<int>(type: "integer", nullable: false),
                    ThinkingCost = table.Column<int>(type: "integer", nullable: false),
                    CreativityCost = table.Column<int>(type: "integer", nullable: false),
                    SafetyCost = table.Column<int>(type: "integer", nullable: false),
                    IsStartup = table.Column<bool>(type: "boolean", nullable: false),
                    PrerequisitesJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeOfHeroesConfigCraftNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreeOfHeroesConfigCraftNodes_HeroDefinitionDefinitions_Hero~",
                        column: x => x.HeroDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "HeroDefinitionDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreeOfHeroesConfigCraftNodes_TreeOfHeroesConfigCrafts_Confi~",
                        column: x => x.ConfigCraftId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "TreeOfHeroesConfigCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreeOfHeroesConfigDefinitionEdges",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfigDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromHeroId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ToHeroId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeOfHeroesConfigDefinitionEdges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreeOfHeroesConfigDefinitionEdges_HeroDefinitionDefinitions~",
                        column: x => x.FromHeroId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "HeroDefinitionDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreeOfHeroesConfigDefinitionEdges_HeroDefinitionDefinition~1",
                        column: x => x.ToHeroId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "HeroDefinitionDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreeOfHeroesConfigDefinitionEdges_TreeOfHeroesConfigDefinit~",
                        column: x => x.ConfigDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "TreeOfHeroesConfigDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreeOfHeroesConfigDefinitionNodes",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfigDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroDefinitionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PositionX = table.Column<double>(type: "double precision", nullable: false),
                    PositionY = table.Column<double>(type: "double precision", nullable: false),
                    CourageCost = table.Column<int>(type: "integer", nullable: false),
                    CuriosityCost = table.Column<int>(type: "integer", nullable: false),
                    ThinkingCost = table.Column<int>(type: "integer", nullable: false),
                    CreativityCost = table.Column<int>(type: "integer", nullable: false),
                    SafetyCost = table.Column<int>(type: "integer", nullable: false),
                    IsStartup = table.Column<bool>(type: "boolean", nullable: false),
                    PrerequisitesJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeOfHeroesConfigDefinitionNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreeOfHeroesConfigDefinitionNodes_HeroDefinitionDefinitions~",
                        column: x => x.HeroDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "HeroDefinitionDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreeOfHeroesConfigDefinitionNodes_TreeOfHeroesConfigDefinit~",
                        column: x => x.ConfigDefinitionId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "TreeOfHeroesConfigDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryEpicCraftStoryNodes",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpicId = table.Column<string>(type: "character varying(100)", nullable: false),
                    StoryId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RegionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RewardImageUrl = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    X = table.Column<double>(type: "double precision", nullable: true),
                    Y = table.Column<double>(type: "double precision", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryEpicCraftStoryNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryEpicCraftStoryNodes_StoryCrafts_StoryId",
                        column: x => x.StoryId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryCrafts",
                        principalColumn: "StoryId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_StoryEpicCraftStoryNodes_StoryDefinitions_StoryId",
                        column: x => x.StoryId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryDefinitions",
                        principalColumn: "StoryId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_StoryEpicCraftStoryNodes_StoryEpicCraftRegions_EpicId_Regio~",
                        columns: x => new { x.EpicId, x.RegionId },
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryEpicCraftRegions",
                        principalColumns: new[] { "EpicId", "RegionId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryEpicCraftStoryNodes_StoryEpicCrafts_EpicId",
                        column: x => x.EpicId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryEpicCrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryEpicDefinitionStoryNodes",
                schema: "alchimalia_schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpicId = table.Column<string>(type: "character varying(100)", nullable: false),
                    StoryId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RegionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RewardImageUrl = table.Column<string>(type: "text", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    X = table.Column<double>(type: "double precision", nullable: true),
                    Y = table.Column<double>(type: "double precision", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryEpicDefinitionStoryNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryEpicDefinitionStoryNodes_StoryDefinitions_StoryId",
                        column: x => x.StoryId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryDefinitions",
                        principalColumn: "StoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoryEpicDefinitionStoryNodes_StoryEpicDefinitionRegions_Ep~",
                        columns: x => new { x.EpicId, x.RegionId },
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryEpicDefinitionRegions",
                        principalColumns: new[] { "EpicId", "RegionId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryEpicDefinitionStoryNodes_StoryEpicDefinitions_EpicId",
                        column: x => x.EpicId,
                        principalSchema: "alchimalia_schema",
                        principalTable: "StoryEpicDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                schema: "alchimalia_schema",
                table: "AlchimaliaUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "AutoFilterStoriesByAge", "CreatedAt", "LastLoginAt", "SelectedAgeGroupIds", "UpdatedAt" },
                values: new object[] { false, new DateTime(2026, 1, 19, 13, 6, 55, 276, DateTimeKind.Utc).AddTicks(9331), new DateTime(2026, 1, 19, 13, 6, 55, 276, DateTimeKind.Utc).AddTicks(9332), null, new DateTime(2026, 1, 19, 13, 6, 55, 276, DateTimeKind.Utc).AddTicks(9334) });

            migrationBuilder.UpdateData(
                schema: "alchimalia_schema",
                table: "CreditWallets",
                keyColumn: "UserId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "UpdatedAt",
                value: new DateTime(2026, 1, 19, 13, 6, 55, 276, DateTimeKind.Utc).AddTicks(9659));

            migrationBuilder.CreateIndex(
                name: "IX_StoryHeroes_Id_Status",
                schema: "alchimalia_schema",
                table: "StoryHeroes",
                columns: new[] { "Id", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryHeroes_Status",
                schema: "alchimalia_schema",
                table: "StoryHeroes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitions_Id_Status",
                schema: "alchimalia_schema",
                table: "HeroDefinitions",
                columns: new[] { "Id", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitions_Status",
                schema: "alchimalia_schema",
                table: "HeroDefinitions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Animals_Id_Status",
                schema: "alchimalia_schema",
                table: "Animals",
                columns: new[] { "Id", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Animals_Status",
                schema: "alchimalia_schema",
                table: "Animals",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalCraftPartSupports_BodyPartKey",
                schema: "alchimalia_schema",
                table: "AnimalCraftPartSupports",
                column: "BodyPartKey");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalCrafts_CreatedByUserId",
                schema: "alchimalia_schema",
                table: "AnimalCrafts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalCrafts_RegionId",
                schema: "alchimalia_schema",
                table: "AnimalCrafts",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalCrafts_ReviewedByUserId",
                schema: "alchimalia_schema",
                table: "AnimalCrafts",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalCrafts_Status",
                schema: "alchimalia_schema",
                table: "AnimalCrafts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalCraftTranslations_AnimalCraftId",
                schema: "alchimalia_schema",
                table: "AnimalCraftTranslations",
                column: "AnimalCraftId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalCraftTranslations_AnimalCraftId_LanguageCode",
                schema: "alchimalia_schema",
                table: "AnimalCraftTranslations",
                columns: new[] { "AnimalCraftId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnimalDefinitionPartSupports_BodyPartKey",
                schema: "alchimalia_schema",
                table: "AnimalDefinitionPartSupports",
                column: "BodyPartKey");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalDefinitions_PublishedByUserId",
                schema: "alchimalia_schema",
                table: "AnimalDefinitions",
                column: "PublishedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalDefinitions_RegionId",
                schema: "alchimalia_schema",
                table: "AnimalDefinitions",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalDefinitions_Status",
                schema: "alchimalia_schema",
                table: "AnimalDefinitions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalDefinitionTranslations_AnimalDefinitionId",
                schema: "alchimalia_schema",
                table: "AnimalDefinitionTranslations",
                column: "AnimalDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalDefinitionTranslations_AnimalDefinitionId_LanguageCode",
                schema: "alchimalia_schema",
                table: "AnimalDefinitionTranslations",
                columns: new[] { "AnimalDefinitionId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnimalHybridCraftParts_AnimalCraftId",
                schema: "alchimalia_schema",
                table: "AnimalHybridCraftParts",
                column: "AnimalCraftId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalHybridCraftParts_BodyPartKey",
                schema: "alchimalia_schema",
                table: "AnimalHybridCraftParts",
                column: "BodyPartKey");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalHybridCraftParts_SourceAnimalId",
                schema: "alchimalia_schema",
                table: "AnimalHybridCraftParts",
                column: "SourceAnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalHybridDefinitionParts_AnimalDefinitionId",
                schema: "alchimalia_schema",
                table: "AnimalHybridDefinitionParts",
                column: "AnimalDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalHybridDefinitionParts_BodyPartKey",
                schema: "alchimalia_schema",
                table: "AnimalHybridDefinitionParts",
                column: "BodyPartKey");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalHybridDefinitionParts_SourceAnimalId",
                schema: "alchimalia_schema",
                table: "AnimalHybridDefinitionParts",
                column: "SourceAnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalPublishChangeLogs_AnimalId",
                schema: "alchimalia_schema",
                table: "AnimalPublishChangeLogs",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalPublishChangeLogs_AnimalId_DraftVersion",
                schema: "alchimalia_schema",
                table: "AnimalPublishChangeLogs",
                columns: new[] { "AnimalId", "DraftVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_AnimalPublishJobs_AnimalId_Status",
                schema: "alchimalia_schema",
                table: "AnimalPublishJobs",
                columns: new[] { "AnimalId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AnimalPublishJobs_QueuedAtUtc",
                schema: "alchimalia_schema",
                table: "AnimalPublishJobs",
                column: "QueuedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalVersionJobs_AnimalId_Status",
                schema: "alchimalia_schema",
                table: "AnimalVersionJobs",
                columns: new[] { "AnimalId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AnimalVersionJobs_OwnerUserId",
                schema: "alchimalia_schema",
                table: "AnimalVersionJobs",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalVersionJobs_QueuedAtUtc",
                schema: "alchimalia_schema",
                table: "AnimalVersionJobs",
                column: "QueuedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalVersions_AnimalId_Version",
                schema: "alchimalia_schema",
                table: "AnimalVersions",
                columns: new[] { "AnimalId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EpicAssetLinks_DraftPath",
                schema: "alchimalia_schema",
                table: "EpicAssetLinks",
                column: "DraftPath",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EpicAssetLinks_EpicId_DraftVersion",
                schema: "alchimalia_schema",
                table: "EpicAssetLinks",
                columns: new[] { "EpicId", "DraftVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_EpicHero_OwnerUserId_Id",
                schema: "alchimalia_schema",
                table: "EpicHero",
                columns: new[] { "OwnerUserId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EpicHeroCrafts_ApprovedByUserId",
                schema: "alchimalia_schema",
                table: "EpicHeroCrafts",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EpicHeroCrafts_AssignedReviewerUserId",
                schema: "alchimalia_schema",
                table: "EpicHeroCrafts",
                column: "AssignedReviewerUserId",
                filter: "[AssignedReviewerUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EpicHeroCrafts_Id_Status",
                schema: "alchimalia_schema",
                table: "EpicHeroCrafts",
                columns: new[] { "Id", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_EpicHeroCrafts_OwnerUserId_Id",
                schema: "alchimalia_schema",
                table: "EpicHeroCrafts",
                columns: new[] { "OwnerUserId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EpicHeroCrafts_ReviewedByUserId",
                schema: "alchimalia_schema",
                table: "EpicHeroCrafts",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EpicHeroCrafts_Status",
                schema: "alchimalia_schema",
                table: "EpicHeroCrafts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EpicHeroCraftTranslations_EpicHeroCraftId",
                schema: "alchimalia_schema",
                table: "EpicHeroCraftTranslations",
                column: "EpicHeroCraftId");

            migrationBuilder.CreateIndex(
                name: "IX_EpicHeroCraftTranslations_EpicHeroCraftId_LanguageCode",
                schema: "alchimalia_schema",
                table: "EpicHeroCraftTranslations",
                columns: new[] { "EpicHeroCraftId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EpicHeroDefinitions_Id_Version",
                schema: "alchimalia_schema",
                table: "EpicHeroDefinitions",
                columns: new[] { "Id", "Version" });

            migrationBuilder.CreateIndex(
                name: "IX_EpicHeroDefinitions_OwnerUserId_Id",
                schema: "alchimalia_schema",
                table: "EpicHeroDefinitions",
                columns: new[] { "OwnerUserId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EpicHeroDefinitions_Status",
                schema: "alchimalia_schema",
                table: "EpicHeroDefinitions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EpicHeroDefinitionTranslations_EpicHeroDefinitionId",
                schema: "alchimalia_schema",
                table: "EpicHeroDefinitionTranslations",
                column: "EpicHeroDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_EpicHeroDefinitionTranslations_EpicHeroDefinitionId_Languag~",
                schema: "alchimalia_schema",
                table: "EpicHeroDefinitionTranslations",
                columns: new[] { "EpicHeroDefinitionId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EpicHeroTranslation_EpicHeroId_LanguageCode",
                schema: "alchimalia_schema",
                table: "EpicHeroTranslation",
                columns: new[] { "EpicHeroId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EpicProgress_UserId_RegionId_EpicId",
                schema: "alchimalia_schema",
                table: "EpicProgress",
                columns: new[] { "UserId", "RegionId", "EpicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EpicPublishChangeLogs_EpicId_DraftVersion",
                schema: "alchimalia_schema",
                table: "EpicPublishChangeLogs",
                columns: new[] { "EpicId", "DraftVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_EpicPublishJobs_EpicId_Status",
                schema: "alchimalia_schema",
                table: "EpicPublishJobs",
                columns: new[] { "EpicId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_EpicPublishJobs_QueuedAtUtc",
                schema: "alchimalia_schema",
                table: "EpicPublishJobs",
                column: "QueuedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_EpicReaders_EpicId",
                schema: "alchimalia_schema",
                table: "EpicReaders",
                column: "EpicId");

            migrationBuilder.CreateIndex(
                name: "IX_EpicReaders_UserId_EpicId",
                schema: "alchimalia_schema",
                table: "EpicReaders",
                columns: new[] { "UserId", "EpicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EpicReviews_EpicId",
                schema: "alchimalia_schema",
                table: "EpicReviews",
                column: "EpicId");

            migrationBuilder.CreateIndex(
                name: "IX_EpicReviews_UserId_EpicId",
                schema: "alchimalia_schema",
                table: "EpicReviews",
                columns: new[] { "UserId", "EpicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EpicStoryProgress_UserId_StoryId_EpicId",
                schema: "alchimalia_schema",
                table: "EpicStoryProgress",
                columns: new[] { "UserId", "StoryId", "EpicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EpicVersionJobs_EpicId_Status",
                schema: "alchimalia_schema",
                table: "EpicVersionJobs",
                columns: new[] { "EpicId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_EpicVersionJobs_QueuedAtUtc",
                schema: "alchimalia_schema",
                table: "EpicVersionJobs",
                column: "QueuedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_HeroAssetLinks_DraftPath",
                schema: "alchimalia_schema",
                table: "HeroAssetLinks",
                column: "DraftPath",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeroAssetLinks_HeroId",
                schema: "alchimalia_schema",
                table: "HeroAssetLinks",
                column: "HeroId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroAssetLinks_HeroId_DraftVersion",
                schema: "alchimalia_schema",
                table: "HeroAssetLinks",
                columns: new[] { "HeroId", "DraftVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitionCrafts_CreatedByUserId",
                schema: "alchimalia_schema",
                table: "HeroDefinitionCrafts",
                column: "CreatedByUserId",
                filter: "[CreatedByUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitionCrafts_ReviewedByUserId",
                schema: "alchimalia_schema",
                table: "HeroDefinitionCrafts",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitionCrafts_Status",
                schema: "alchimalia_schema",
                table: "HeroDefinitionCrafts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitionCraftTranslations_HeroDefinitionCraftId",
                schema: "alchimalia_schema",
                table: "HeroDefinitionCraftTranslations",
                column: "HeroDefinitionCraftId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitionCraftTranslations_HeroDefinitionCraftId_Langu~",
                schema: "alchimalia_schema",
                table: "HeroDefinitionCraftTranslations",
                columns: new[] { "HeroDefinitionCraftId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitionDefinitions_PublishedByUserId",
                schema: "alchimalia_schema",
                table: "HeroDefinitionDefinitions",
                column: "PublishedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitionDefinitions_Status",
                schema: "alchimalia_schema",
                table: "HeroDefinitionDefinitions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitionDefinitionTranslations_HeroDefinitionDefinit~1",
                schema: "alchimalia_schema",
                table: "HeroDefinitionDefinitionTranslations",
                columns: new[] { "HeroDefinitionDefinitionId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitionDefinitionTranslations_HeroDefinitionDefiniti~",
                schema: "alchimalia_schema",
                table: "HeroDefinitionDefinitionTranslations",
                column: "HeroDefinitionDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitionPublishChangeLogs_HeroId",
                schema: "alchimalia_schema",
                table: "HeroDefinitionPublishChangeLogs",
                column: "HeroId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitionPublishChangeLogs_HeroId_DraftVersion",
                schema: "alchimalia_schema",
                table: "HeroDefinitionPublishChangeLogs",
                columns: new[] { "HeroId", "DraftVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitionVersionJobs_HeroId_Status",
                schema: "alchimalia_schema",
                table: "HeroDefinitionVersionJobs",
                columns: new[] { "HeroId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitionVersionJobs_OwnerUserId",
                schema: "alchimalia_schema",
                table: "HeroDefinitionVersionJobs",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitionVersionJobs_QueuedAtUtc",
                schema: "alchimalia_schema",
                table: "HeroDefinitionVersionJobs",
                column: "QueuedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_HeroDefinitionVersions_HeroDefinitionId_Version",
                schema: "alchimalia_schema",
                table: "HeroDefinitionVersions",
                columns: new[] { "HeroDefinitionId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeroPublishChangeLogs_HeroId",
                schema: "alchimalia_schema",
                table: "HeroPublishChangeLogs",
                column: "HeroId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroPublishChangeLogs_HeroId_DraftVersion",
                schema: "alchimalia_schema",
                table: "HeroPublishChangeLogs",
                columns: new[] { "HeroId", "DraftVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_HeroPublishJobs_HeroId_Status",
                schema: "alchimalia_schema",
                table: "HeroPublishJobs",
                columns: new[] { "HeroId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_HeroPublishJobs_QueuedAtUtc",
                schema: "alchimalia_schema",
                table: "HeroPublishJobs",
                column: "QueuedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_HeroVersionJobs_HeroId_Status",
                schema: "alchimalia_schema",
                table: "HeroVersionJobs",
                columns: new[] { "HeroId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_HeroVersionJobs_QueuedAtUtc",
                schema: "alchimalia_schema",
                table: "HeroVersionJobs",
                column: "QueuedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_RegionAssetLinks_DraftPath",
                schema: "alchimalia_schema",
                table: "RegionAssetLinks",
                column: "DraftPath",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegionAssetLinks_RegionId",
                schema: "alchimalia_schema",
                table: "RegionAssetLinks",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_RegionAssetLinks_RegionId_DraftVersion",
                schema: "alchimalia_schema",
                table: "RegionAssetLinks",
                columns: new[] { "RegionId", "DraftVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_RegionPublishChangeLogs_RegionId",
                schema: "alchimalia_schema",
                table: "RegionPublishChangeLogs",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_RegionPublishChangeLogs_RegionId_DraftVersion",
                schema: "alchimalia_schema",
                table: "RegionPublishChangeLogs",
                columns: new[] { "RegionId", "DraftVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_RegionVersionJobs_QueuedAtUtc",
                schema: "alchimalia_schema",
                table: "RegionVersionJobs",
                column: "QueuedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_RegionVersionJobs_RegionId_Status",
                schema: "alchimalia_schema",
                table: "RegionVersionJobs",
                columns: new[] { "RegionId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryAssetLinks_DraftPath",
                schema: "alchimalia_schema",
                table: "StoryAssetLinks",
                column: "DraftPath",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryAssetLinks_StoryId_DraftVersion",
                schema: "alchimalia_schema",
                table: "StoryAssetLinks",
                columns: new[] { "StoryId", "DraftVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicCraftAgeGroup_StoryAgeGroupId",
                schema: "alchimalia_schema",
                table: "StoryEpicCraftAgeGroup",
                column: "StoryAgeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicCraftHeroReferences_EpicId",
                schema: "alchimalia_schema",
                table: "StoryEpicCraftHeroReferences",
                column: "EpicId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicCraftHeroReferences_EpicId_HeroId",
                schema: "alchimalia_schema",
                table: "StoryEpicCraftHeroReferences",
                columns: new[] { "EpicId", "HeroId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicCraftHeroReferences_HeroId",
                schema: "alchimalia_schema",
                table: "StoryEpicCraftHeroReferences",
                column: "HeroId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicCraftRegions_EpicId_RegionId",
                schema: "alchimalia_schema",
                table: "StoryEpicCraftRegions",
                columns: new[] { "EpicId", "RegionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicCrafts_ApprovedByUserId",
                schema: "alchimalia_schema",
                table: "StoryEpicCrafts",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicCrafts_AssignedReviewerUserId",
                schema: "alchimalia_schema",
                table: "StoryEpicCrafts",
                column: "AssignedReviewerUserId",
                filter: "[AssignedReviewerUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicCrafts_OwnerUserId_Id",
                schema: "alchimalia_schema",
                table: "StoryEpicCrafts",
                columns: new[] { "OwnerUserId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicCrafts_ReviewedByUserId",
                schema: "alchimalia_schema",
                table: "StoryEpicCrafts",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicCrafts_Status",
                schema: "alchimalia_schema",
                table: "StoryEpicCrafts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicCraftStoryNodes_EpicId_RegionId",
                schema: "alchimalia_schema",
                table: "StoryEpicCraftStoryNodes",
                columns: new[] { "EpicId", "RegionId" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicCraftStoryNodes_EpicId_StoryId_RegionId",
                schema: "alchimalia_schema",
                table: "StoryEpicCraftStoryNodes",
                columns: new[] { "EpicId", "StoryId", "RegionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicCraftStoryNodes_StoryId",
                schema: "alchimalia_schema",
                table: "StoryEpicCraftStoryNodes",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicCraftTopic_StoryTopicId",
                schema: "alchimalia_schema",
                table: "StoryEpicCraftTopic",
                column: "StoryTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicCraftTranslations_StoryEpicCraftId_LanguageCode",
                schema: "alchimalia_schema",
                table: "StoryEpicCraftTranslations",
                columns: new[] { "StoryEpicCraftId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicCraftUnlockRules_EpicId",
                schema: "alchimalia_schema",
                table: "StoryEpicCraftUnlockRules",
                column: "EpicId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicDefinitionAgeGroup_StoryAgeGroupId",
                schema: "alchimalia_schema",
                table: "StoryEpicDefinitionAgeGroup",
                column: "StoryAgeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicDefinitionRegions_EpicId_RegionId",
                schema: "alchimalia_schema",
                table: "StoryEpicDefinitionRegions",
                columns: new[] { "EpicId", "RegionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicDefinitions_OwnerUserId_Id",
                schema: "alchimalia_schema",
                table: "StoryEpicDefinitions",
                columns: new[] { "OwnerUserId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicDefinitions_Status",
                schema: "alchimalia_schema",
                table: "StoryEpicDefinitions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicDefinitionStoryNodes_EpicId_RegionId",
                schema: "alchimalia_schema",
                table: "StoryEpicDefinitionStoryNodes",
                columns: new[] { "EpicId", "RegionId" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicDefinitionStoryNodes_EpicId_StoryId_RegionId",
                schema: "alchimalia_schema",
                table: "StoryEpicDefinitionStoryNodes",
                columns: new[] { "EpicId", "StoryId", "RegionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicDefinitionStoryNodes_StoryId",
                schema: "alchimalia_schema",
                table: "StoryEpicDefinitionStoryNodes",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicDefinitionTopic_StoryTopicId",
                schema: "alchimalia_schema",
                table: "StoryEpicDefinitionTopic",
                column: "StoryTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicDefinitionTranslations_StoryEpicDefinitionId_Langu~",
                schema: "alchimalia_schema",
                table: "StoryEpicDefinitionTranslations",
                columns: new[] { "StoryEpicDefinitionId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicDefinitionUnlockRules_EpicId",
                schema: "alchimalia_schema",
                table: "StoryEpicDefinitionUnlockRules",
                column: "EpicId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicHeroReferences_EpicId_HeroId",
                schema: "alchimalia_schema",
                table: "StoryEpicHeroReferences",
                columns: new[] { "EpicId", "HeroId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicHeroReferences_HeroId",
                schema: "alchimalia_schema",
                table: "StoryEpicHeroReferences",
                column: "HeroId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicRegionReferences_EpicId_RegionId",
                schema: "alchimalia_schema",
                table: "StoryEpicRegionReferences",
                columns: new[] { "EpicId", "RegionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryEpicRegionReferences_RegionId",
                schema: "alchimalia_schema",
                table: "StoryEpicRegionReferences",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryEvaluationResults_UserId_StoryId_CompletedAt",
                schema: "alchimalia_schema",
                table: "StoryEvaluationResults",
                columns: new[] { "UserId", "StoryId", "CompletedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryEvaluationResults_UserId_StoryId_SessionId",
                schema: "alchimalia_schema",
                table: "StoryEvaluationResults",
                columns: new[] { "UserId", "StoryId", "SessionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryForkAssetJobs_TargetStoryId_Status",
                schema: "alchimalia_schema",
                table: "StoryForkAssetJobs",
                columns: new[] { "TargetStoryId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryForkJobs_TargetStoryId_Status",
                schema: "alchimalia_schema",
                table: "StoryForkJobs",
                columns: new[] { "TargetStoryId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryHeroTranslations_StoryHeroId_LanguageCode",
                schema: "alchimalia_schema",
                table: "StoryHeroTranslations",
                columns: new[] { "StoryHeroId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryHeroVersions_StoryHeroId_Version",
                schema: "alchimalia_schema",
                table: "StoryHeroVersions",
                columns: new[] { "StoryHeroId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryImportJobs_OwnerUserId",
                schema: "alchimalia_schema",
                table: "StoryImportJobs",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryImportJobs_StoryId_Status",
                schema: "alchimalia_schema",
                table: "StoryImportJobs",
                columns: new[] { "StoryId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryLikes_StoryId",
                schema: "alchimalia_schema",
                table: "StoryLikes",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryLikes_UserId_StoryId",
                schema: "alchimalia_schema",
                table: "StoryLikes",
                columns: new[] { "UserId", "StoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryPublishChangeLogs_StoryId_DraftVersion",
                schema: "alchimalia_schema",
                table: "StoryPublishChangeLogs",
                columns: new[] { "StoryId", "DraftVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryPublishJobs_QueuedAtUtc",
                schema: "alchimalia_schema",
                table: "StoryPublishJobs",
                column: "QueuedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPublishJobs_StoryId_Status",
                schema: "alchimalia_schema",
                table: "StoryPublishJobs",
                columns: new[] { "StoryId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryQuizAnswers_SessionId",
                schema: "alchimalia_schema",
                table: "StoryQuizAnswers",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryQuizAnswers_UserId_StoryId",
                schema: "alchimalia_schema",
                table: "StoryQuizAnswers",
                columns: new[] { "UserId", "StoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryQuizAnswers_UserId_StoryId_TileId_SessionId",
                schema: "alchimalia_schema",
                table: "StoryQuizAnswers",
                columns: new[] { "UserId", "StoryId", "TileId", "SessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryRegion_OwnerUserId_Id",
                schema: "alchimalia_schema",
                table: "StoryRegion",
                columns: new[] { "OwnerUserId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryRegionCrafts_ApprovedByUserId",
                schema: "alchimalia_schema",
                table: "StoryRegionCrafts",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryRegionCrafts_AssignedReviewerUserId",
                schema: "alchimalia_schema",
                table: "StoryRegionCrafts",
                column: "AssignedReviewerUserId",
                filter: "[AssignedReviewerUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StoryRegionCrafts_Id_Status",
                schema: "alchimalia_schema",
                table: "StoryRegionCrafts",
                columns: new[] { "Id", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryRegionCrafts_OwnerUserId_Id",
                schema: "alchimalia_schema",
                table: "StoryRegionCrafts",
                columns: new[] { "OwnerUserId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryRegionCrafts_ReviewedByUserId",
                schema: "alchimalia_schema",
                table: "StoryRegionCrafts",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryRegionCrafts_Status",
                schema: "alchimalia_schema",
                table: "StoryRegionCrafts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_StoryRegionCraftTranslations_StoryRegionCraftId",
                schema: "alchimalia_schema",
                table: "StoryRegionCraftTranslations",
                column: "StoryRegionCraftId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryRegionCraftTranslations_StoryRegionCraftId_LanguageCode",
                schema: "alchimalia_schema",
                table: "StoryRegionCraftTranslations",
                columns: new[] { "StoryRegionCraftId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryRegionDefinitions_Id_Version",
                schema: "alchimalia_schema",
                table: "StoryRegionDefinitions",
                columns: new[] { "Id", "Version" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryRegionDefinitions_OwnerUserId_Id",
                schema: "alchimalia_schema",
                table: "StoryRegionDefinitions",
                columns: new[] { "OwnerUserId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryRegionDefinitions_Status",
                schema: "alchimalia_schema",
                table: "StoryRegionDefinitions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_StoryRegionDefinitionTranslations_StoryRegionDefinitionId",
                schema: "alchimalia_schema",
                table: "StoryRegionDefinitionTranslations",
                column: "StoryRegionDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryRegionDefinitionTranslations_StoryRegionDefinitionId_L~",
                schema: "alchimalia_schema",
                table: "StoryRegionDefinitionTranslations",
                columns: new[] { "StoryRegionDefinitionId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryRegionTranslation_StoryRegionId_LanguageCode",
                schema: "alchimalia_schema",
                table: "StoryRegionTranslation",
                columns: new[] { "StoryRegionId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryVersionJobs_QueuedAtUtc",
                schema: "alchimalia_schema",
                table: "StoryVersionJobs",
                column: "QueuedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_StoryVersionJobs_StoryId_Status",
                schema: "alchimalia_schema",
                table: "StoryVersionJobs",
                columns: new[] { "StoryId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TreeOfHeroesConfigCraftEdges_ConfigCraftId_FromHeroId_ToHer~",
                schema: "alchimalia_schema",
                table: "TreeOfHeroesConfigCraftEdges",
                columns: new[] { "ConfigCraftId", "FromHeroId", "ToHeroId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreeOfHeroesConfigCraftEdges_FromHeroId",
                schema: "alchimalia_schema",
                table: "TreeOfHeroesConfigCraftEdges",
                column: "FromHeroId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeOfHeroesConfigCraftEdges_ToHeroId",
                schema: "alchimalia_schema",
                table: "TreeOfHeroesConfigCraftEdges",
                column: "ToHeroId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeOfHeroesConfigCraftNodes_ConfigCraftId_HeroDefinitionId",
                schema: "alchimalia_schema",
                table: "TreeOfHeroesConfigCraftNodes",
                columns: new[] { "ConfigCraftId", "HeroDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreeOfHeroesConfigCraftNodes_HeroDefinitionId",
                schema: "alchimalia_schema",
                table: "TreeOfHeroesConfigCraftNodes",
                column: "HeroDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeOfHeroesConfigCrafts_CreatedByUserId",
                schema: "alchimalia_schema",
                table: "TreeOfHeroesConfigCrafts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeOfHeroesConfigCrafts_ReviewedByUserId",
                schema: "alchimalia_schema",
                table: "TreeOfHeroesConfigCrafts",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeOfHeroesConfigCrafts_Status",
                schema: "alchimalia_schema",
                table: "TreeOfHeroesConfigCrafts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TreeOfHeroesConfigDefinitionEdges_ConfigDefinitionId_FromHe~",
                schema: "alchimalia_schema",
                table: "TreeOfHeroesConfigDefinitionEdges",
                columns: new[] { "ConfigDefinitionId", "FromHeroId", "ToHeroId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreeOfHeroesConfigDefinitionEdges_FromHeroId",
                schema: "alchimalia_schema",
                table: "TreeOfHeroesConfigDefinitionEdges",
                column: "FromHeroId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeOfHeroesConfigDefinitionEdges_ToHeroId",
                schema: "alchimalia_schema",
                table: "TreeOfHeroesConfigDefinitionEdges",
                column: "ToHeroId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeOfHeroesConfigDefinitionNodes_ConfigDefinitionId_HeroDe~",
                schema: "alchimalia_schema",
                table: "TreeOfHeroesConfigDefinitionNodes",
                columns: new[] { "ConfigDefinitionId", "HeroDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreeOfHeroesConfigDefinitionNodes_HeroDefinitionId",
                schema: "alchimalia_schema",
                table: "TreeOfHeroesConfigDefinitionNodes",
                column: "HeroDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeOfHeroesConfigDefinitions_PublishedByUserId",
                schema: "alchimalia_schema",
                table: "TreeOfHeroesConfigDefinitions",
                column: "PublishedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TreeOfHeroesConfigDefinitions_Status",
                schema: "alchimalia_schema",
                table: "TreeOfHeroesConfigDefinitions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteEpics_EpicId",
                schema: "alchimalia_schema",
                table: "UserFavoriteEpics",
                column: "EpicId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteEpics_UserId_EpicId",
                schema: "alchimalia_schema",
                table: "UserFavoriteEpics",
                columns: new[] { "UserId", "EpicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserStoryReadHistory_CompletedAt",
                schema: "alchimalia_schema",
                table: "UserStoryReadHistory",
                column: "CompletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserStoryReadHistory_UserId_StoryId",
                schema: "alchimalia_schema",
                table: "UserStoryReadHistory",
                columns: new[] { "UserId", "StoryId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_Regions_RegionId",
                schema: "alchimalia_schema",
                table: "Animals",
                column: "RegionId",
                principalSchema: "alchimalia_schema",
                principalTable: "Regions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Regions_RegionId",
                schema: "alchimalia_schema",
                table: "Animals");

            migrationBuilder.DropTable(
                name: "AnimalCraftPartSupports",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "AnimalCraftTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "AnimalDefinitionPartSupports",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "AnimalDefinitionTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "AnimalHybridCraftParts",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "AnimalHybridDefinitionParts",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "AnimalPublishChangeLogs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "AnimalPublishJobs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "AnimalVersionJobs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "AnimalVersions",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "EpicAssetLinks",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "EpicHeroCraftTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "EpicHeroDefinitionTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "EpicHeroTranslation",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "EpicProgress",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "EpicPublishChangeLogs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "EpicPublishJobs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "EpicReaders",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "EpicReviews",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "EpicStoryProgress",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "EpicVersionJobs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "HeroAssetLinks",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "HeroDefinitionCraftTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "HeroDefinitionDefinitionTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "HeroDefinitionPublishChangeLogs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "HeroDefinitionVersionJobs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "HeroDefinitionVersions",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "HeroPublishChangeLogs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "HeroPublishJobs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "HeroVersionJobs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "PlatformSettings",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "RegionAssetLinks",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "RegionPublishChangeLogs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "RegionVersionJobs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryAssetLinks",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryCraftUnlockedHeroes",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryDefinitionUnlockedHeroes",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryDocumentExportJobs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryEpicCraftAgeGroup",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryEpicCraftHeroReferences",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryEpicCraftStoryNodes",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryEpicCraftTopic",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryEpicCraftTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryEpicCraftUnlockRules",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryEpicDefinitionAgeGroup",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryEpicDefinitionStoryNodes",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryEpicDefinitionTopic",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryEpicDefinitionTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryEpicDefinitionUnlockRules",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryEpicHeroReferences",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryEpicRegionReferences",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryEvaluationResults",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryExportJobs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryForkAssetJobs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryForkJobs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryHeroTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryHeroVersions",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryImportJobs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryLikes",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryPublishChangeLogs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryPublishJobs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryQuizAnswers",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryRegionCraftTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryRegionDefinitionTranslations",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryRegionTranslation",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryVersionJobs",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "TreeOfHeroesConfigCraftEdges",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "TreeOfHeroesConfigCraftNodes",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "TreeOfHeroesConfigDefinitionEdges",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "TreeOfHeroesConfigDefinitionNodes",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "UserFavoriteEpics",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "UserStoryReadHistory",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "AnimalCrafts",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "AnimalDefinitions",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "EpicHero",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "HeroDefinitionCrafts",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "EpicHeroCrafts",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryEpicCraftRegions",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryEpicDefinitionRegions",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "EpicHeroDefinitions",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryRegionCrafts",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryRegionDefinitions",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryRegion",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "TreeOfHeroesConfigCrafts",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "HeroDefinitionDefinitions",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "TreeOfHeroesConfigDefinitions",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryEpicCrafts",
                schema: "alchimalia_schema");

            migrationBuilder.DropTable(
                name: "StoryEpicDefinitions",
                schema: "alchimalia_schema");

            migrationBuilder.DropIndex(
                name: "IX_StoryHeroes_Id_Status",
                schema: "alchimalia_schema",
                table: "StoryHeroes");

            migrationBuilder.DropIndex(
                name: "IX_StoryHeroes_Status",
                schema: "alchimalia_schema",
                table: "StoryHeroes");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_StoryCrafts_StoryId",
                schema: "alchimalia_schema",
                table: "StoryCrafts");

            migrationBuilder.DropIndex(
                name: "IX_HeroDefinitions_Id_Status",
                schema: "alchimalia_schema",
                table: "HeroDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_HeroDefinitions_Status",
                schema: "alchimalia_schema",
                table: "HeroDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_Animals_Id_Status",
                schema: "alchimalia_schema",
                table: "Animals");

            migrationBuilder.DropIndex(
                name: "IX_Animals_Status",
                schema: "alchimalia_schema",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "ContentHash",
                schema: "alchimalia_schema",
                table: "StoryTileTranslations");

            migrationBuilder.DropColumn(
                name: "ContentHash",
                schema: "alchimalia_schema",
                table: "StoryTiles");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "alchimalia_schema",
                table: "StoryHeroes");

            migrationBuilder.DropColumn(
                name: "ReviewNotes",
                schema: "alchimalia_schema",
                table: "StoryHeroes");

            migrationBuilder.DropColumn(
                name: "ReviewedByUserId",
                schema: "alchimalia_schema",
                table: "StoryHeroes");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "alchimalia_schema",
                table: "StoryHeroes");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "alchimalia_schema",
                table: "StoryHeroes");

            migrationBuilder.DropColumn(
                name: "IsEvaluative",
                schema: "alchimalia_schema",
                table: "StoryDefinitions");

            migrationBuilder.DropColumn(
                name: "IsPartOfEpic",
                schema: "alchimalia_schema",
                table: "StoryDefinitions");

            migrationBuilder.DropColumn(
                name: "LastPublishedVersion",
                schema: "alchimalia_schema",
                table: "StoryDefinitions");

            migrationBuilder.DropColumn(
                name: "IsEvaluative",
                schema: "alchimalia_schema",
                table: "StoryCrafts");

            migrationBuilder.DropColumn(
                name: "IsPartOfEpic",
                schema: "alchimalia_schema",
                table: "StoryCrafts");

            migrationBuilder.DropColumn(
                name: "LastDraftVersion",
                schema: "alchimalia_schema",
                table: "StoryCrafts");

            migrationBuilder.DropColumn(
                name: "IsCorrect",
                schema: "alchimalia_schema",
                table: "StoryCraftAnswers");

            migrationBuilder.DropColumn(
                name: "IsCorrect",
                schema: "alchimalia_schema",
                table: "StoryAnswers");

            migrationBuilder.DropColumn(
                name: "AudioUrl",
                schema: "alchimalia_schema",
                table: "HeroDefinitionTranslations");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "alchimalia_schema",
                table: "HeroDefinitions");

            migrationBuilder.DropColumn(
                name: "ParentVersionId",
                schema: "alchimalia_schema",
                table: "HeroDefinitions");

            migrationBuilder.DropColumn(
                name: "ReviewNotes",
                schema: "alchimalia_schema",
                table: "HeroDefinitions");

            migrationBuilder.DropColumn(
                name: "ReviewedByUserId",
                schema: "alchimalia_schema",
                table: "HeroDefinitions");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "alchimalia_schema",
                table: "HeroDefinitions");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "alchimalia_schema",
                table: "HeroDefinitions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "alchimalia_schema",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "alchimalia_schema",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "ParentVersionId",
                schema: "alchimalia_schema",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "ReviewNotes",
                schema: "alchimalia_schema",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "ReviewedByUserId",
                schema: "alchimalia_schema",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "alchimalia_schema",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "alchimalia_schema",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "alchimalia_schema",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "AutoFilterStoriesByAge",
                schema: "alchimalia_schema",
                table: "AlchimaliaUsers");

            migrationBuilder.DropColumn(
                name: "SelectedAgeGroupIds",
                schema: "alchimalia_schema",
                table: "AlchimaliaUsers");

            migrationBuilder.RenameColumn(
                name: "UnlockConditionsJson",
                schema: "alchimalia_schema",
                table: "StoryHeroes",
                newName: "UnlockConditionJson");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "alchimalia_schema",
                table: "StoryHeroes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                schema: "alchimalia_schema",
                table: "HeroDefinitions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "RegionId",
                schema: "alchimalia_schema",
                table: "Animals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.UpdateData(
                schema: "alchimalia_schema",
                table: "AlchimaliaUsers",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "LastLoginAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 26, 18, 40, 47, 382, DateTimeKind.Utc).AddTicks(8833), new DateTime(2025, 11, 26, 18, 40, 47, 382, DateTimeKind.Utc).AddTicks(8833), new DateTime(2025, 11, 26, 18, 40, 47, 382, DateTimeKind.Utc).AddTicks(8834) });

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
                table: "BodyParts",
                columns: new[] { "Key", "Image", "IsBaseLocked", "Name" },
                values: new object[,]
                {
                    { "arms", "images/bodyparts/hands.webp", false, "Arms" },
                    { "body", "images/bodyparts/body.webp", false, "Body" },
                    { "head", "images/bodyparts/face.webp", false, "Head" },
                    { "horn", "images/bodyparts/horn.webp", true, "Horn" },
                    { "horns", "images/bodyparts/horns.webp", true, "Horns" },
                    { "legs", "images/bodyparts/legs.webp", true, "Legs" },
                    { "tail", "images/bodyparts/tail.webp", true, "Tail" },
                    { "wings", "images/bodyparts/wings.webp", true, "Wings" }
                });

            migrationBuilder.UpdateData(
                schema: "alchimalia_schema",
                table: "CreditWallets",
                keyColumn: "UserId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "UpdatedAt",
                value: new DateTime(2025, 11, 26, 18, 40, 47, 382, DateTimeKind.Utc).AddTicks(8950));

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
                table: "HeroClickMessages",
                columns: new[] { "Id", "AudioUrl", "CreatedAt", "HeroId", "IsActive", "MessageKey", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("3e089534-9de3-49a2-a543-449f90ccf8a8"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_click_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(3879), "puf-puf", true, "hero_puf-puf_click_message", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(3879) },
                    { new Guid("b2c18476-7fa8-4ad0-9fb7-494ccf51637f"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_click_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(3883), "linkaro", true, "hero_linkaro_click_message", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(3883) },
                    { new Guid("f371c87e-6f7c-4d70-89cd-77461832b974"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_click_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(3885), "grubot", true, "hero_grubot_click_message", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(3886) }
                });

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
                table: "HeroDefinitions",
                columns: new[] { "Id", "CourageCost", "CreatedAt", "CreativityCost", "CuriosityCost", "Image", "IsUnlocked", "PositionX", "PositionY", "PrerequisitesJson", "RewardsJson", "SafetyCost", "ThinkingCost", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { "grubot", 0, new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(4120), 0, 0, "images/heroes/grubot.png", false, 0.0, 0.0, "[]", "[]", 0, 0, "STORY_HERO", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(4120) },
                    { "linkaro", 0, new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(4117), 0, 0, "images/heroes/linkaro.png", false, 0.0, 0.0, "[]", "[]", 0, 0, "STORY_HERO", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(4117) },
                    { "puf-puf", 0, new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(4112), 0, 0, "images/heroes/pufpufblink.gif", false, 0.0, 0.0, "[]", "[]", 0, 0, "STORY_HERO", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(4113) }
                });

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
                table: "HeroMessages",
                columns: new[] { "Id", "AudioUrl", "CreatedAt", "HeroId", "IsActive", "MessageKey", "RegionId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0331af85-a296-40bd-a68b-839c9a8966ee"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_neptunia_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(273), "linkaro", true, "hero_linkaro_region_neptunia_message", "neptunia", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(273) },
                    { new Guid("05130a8f-d16d-4efd-bb72-bbd81e0ba497"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_sylvaria_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(206), "puf-puf", true, "hero_puf-puf_region_sylvaria_message", "sylvaria", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(206) },
                    { new Guid("053133ad-b99d-448a-b740-9109bb1b299d"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_aetherion_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(276), "linkaro", true, "hero_linkaro_region_aetherion_message", "aetherion", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(276) },
                    { new Guid("095ac8b2-1425-45ad-919d-0b4d770935b1"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_sylvaria_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(267), "linkaro", true, "hero_linkaro_region_sylvaria_message", "sylvaria", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(267) },
                    { new Guid("10841b71-ecb0-42a0-b4f1-6424cf753186"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_oceanica_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(205), "puf-puf", true, "hero_puf-puf_region_oceanica_message", "oceanica", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(205) },
                    { new Guid("150e8f7e-2907-4f32-a890-89572085c4d8"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_lunaria_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(262), "linkaro", true, "hero_linkaro_region_lunaria_message", "lunaria", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(263) },
                    { new Guid("15228aa7-6688-4b49-bb80-7ba9a19ec0f9"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_sylvaria_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(286), "grubot", true, "hero_grubot_region_sylvaria_message", "sylvaria", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(286) },
                    { new Guid("1eb45cb0-1aba-43b4-9537-13866e55821b"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_pyron_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(251), "puf-puf", true, "hero_puf-puf_region_pyron_message", "pyron", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(251) },
                    { new Guid("216bbbc1-6392-4776-a3ee-59b75a6e1590"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_aetherion_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(296), "grubot", true, "hero_grubot_region_aetherion_message", "aetherion", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(296) },
                    { new Guid("32882744-ec76-46e8-bdb7-a28edc668939"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_mechanika_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(264), "linkaro", true, "hero_linkaro_region_mechanika_message", "mechanika", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(264) },
                    { new Guid("3d9c4c13-8706-4e7d-aa03-dbbed5ee255c"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_oceanica_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(266), "linkaro", true, "hero_linkaro_region_oceanica_message", "oceanica", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(266) },
                    { new Guid("464b15f2-f2f1-4052-82df-b3bc4e4410ea"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_terra_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(201), "puf-puf", true, "hero_puf-puf_region_terra_message", "terra", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(201) },
                    { new Guid("55d7d48a-f52f-41c8-9a9c-bc1f5ebae449"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_crystalia_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(248), "puf-puf", true, "hero_puf-puf_region_crystalia_message", "crystalia", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(248) },
                    { new Guid("569c3eed-aeee-4c00-9413-20097458a66d"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_zephyra_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(271), "linkaro", true, "hero_linkaro_region_zephyra_message", "zephyra", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(271) },
                    { new Guid("5e61f8ed-352c-4181-98a7-1f1c4ba14552"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_zephyra_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(249), "puf-puf", true, "hero_puf-puf_region_zephyra_message", "zephyra", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(250) },
                    { new Guid("715613a7-4a7c-4810-9e79-4893abd4979d"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_crystalia_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(289), "grubot", true, "hero_grubot_region_crystalia_message", "crystalia", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(289) },
                    { new Guid("898eceb1-87ae-4ba4-bcd0-60d036f111df"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_kelo_ketis_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(297), "grubot", true, "hero_grubot_region_kelo_ketis_message", "kelo-ketis", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(297) },
                    { new Guid("8d9cb436-ce1a-4553-877f-414d832cc078"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_oceanica_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(285), "grubot", true, "hero_grubot_region_oceanica_message", "oceanica", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(285) },
                    { new Guid("91b8cfe3-b057-4022-a950-b8f04990a9dc"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_terra_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(260), "linkaro", true, "hero_linkaro_region_terra_message", "terra", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(260) },
                    { new Guid("a7344331-f07f-45e5-8adc-dc8fe46713bb"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_crystalia_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(269), "linkaro", true, "hero_linkaro_region_crystalia_message", "crystalia", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(269) },
                    { new Guid("aacdc921-26ab-48d5-a95a-628bb7fe446a"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_gateway_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(279), "grubot", true, "hero_grubot_region_gateway_message", "gateway", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(279) },
                    { new Guid("b054b29a-f492-4e60-a07a-9297a11c4344"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_terra_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(280), "grubot", true, "hero_grubot_region_terra_message", "terra", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(281) },
                    { new Guid("b17263bf-e10b-44ba-9d2d-ef06fc75297a"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_pyron_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(292), "grubot", true, "hero_grubot_region_pyron_message", "pyron", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(292) },
                    { new Guid("bdc34441-9442-466c-b2b7-ad4f8ba0cf58"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_mechanika_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(283), "grubot", true, "hero_grubot_region_mechanika_message", "mechanika", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(284) },
                    { new Guid("befceaf3-b75b-4f99-8bce-e2e9d426aa54"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_neptunia_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(293), "grubot", true, "hero_grubot_region_neptunia_message", "neptunia", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(293) },
                    { new Guid("c480afe9-adbf-40d7-8513-5772872178e0"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_kelo_ketis_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(256), "puf-puf", true, "hero_puf-puf_region_kelo_ketis_message", "kelo-ketis", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(256) },
                    { new Guid("c5670208-640d-43bd-9eb5-ec39f8032f24"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_gateway_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(198), "puf-puf", true, "hero_puf-puf_region_gateway_message", "gateway", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(199) },
                    { new Guid("cd670e6c-4474-4c36-b0fe-f74aaacc50a5"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_pyron_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(272), "linkaro", true, "hero_linkaro_region_pyron_message", "pyron", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(272) },
                    { new Guid("d47fc9b2-7595-4433-a8d5-558333e911e9"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_lunaria_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(282), "grubot", true, "hero_grubot_region_lunaria_message", "lunaria", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(282) },
                    { new Guid("d4de0633-61c4-47f3-b755-0e523af02ed8"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_lunaria_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(203), "puf-puf", true, "hero_puf-puf_region_lunaria_message", "lunaria", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(203) },
                    { new Guid("d9f9f339-43eb-4958-80ec-418b12025177"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_neptunia_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(252), "puf-puf", true, "hero_puf-puf_region_neptunia_message", "neptunia", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(252) },
                    { new Guid("e19aa3c2-440d-46df-b2dd-a9660ad8cb35"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_kelo_ketis_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(277), "linkaro", true, "hero_linkaro_region_kelo_ketis_message", "kelo-ketis", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(278) },
                    { new Guid("ef3dd111-bfc1-41aa-8f2a-be75c4e135e5"), "audio/ro-ro/tol/hero-speach/grubot/hero_grubot_region_zephyra_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(290), "grubot", true, "hero_grubot_region_zephyra_message", "zephyra", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(291) },
                    { new Guid("efd682b9-07e3-485e-bea7-a01f68999dae"), "audio/ro-ro/tol/hero-speach/linkaro/hero_linkaro_region_gateway_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(258), "linkaro", true, "hero_linkaro_region_gateway_message", "gateway", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(258) },
                    { new Guid("fe6d4822-e85e-43fa-854e-f2c8c4a67511"), "audio/ro-ro/tol/hero-speach/puf-puf/hero_puf-puf_region_aetherion_message.wav", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(254), "puf-puf", true, "hero_puf-puf_region_aetherion_message", "aetherion", new DateTime(2025, 11, 26, 18, 40, 47, 378, DateTimeKind.Utc).AddTicks(255) }
                });

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
                table: "Regions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "Sahara" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "Jungle" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "Farm" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "Savanna" },
                    { new Guid("10000000-0000-0000-0000-000000000005"), "Forest" },
                    { new Guid("10000000-0000-0000-0000-000000000006"), "Wetlands" },
                    { new Guid("10000000-0000-0000-0000-000000000007"), "Mountains" }
                });

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
                table: "StoryHeroes",
                columns: new[] { "Id", "CreatedAt", "HeroId", "ImageUrl", "IsActive", "UnlockConditionJson", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000100"), new DateTime(2025, 11, 26, 18, 40, 47, 377, DateTimeKind.Utc).AddTicks(3522), "puf-puf", "images/tol/stories/seed@alchimalia.com/intro-pufpuf/heroes/pufpufblink.gif", true, "{\"Type\":\"story_completion\",\"RequiredStories\":[\"intro-pufpuf\"],\"MinProgress\":100}", new DateTime(2025, 11, 26, 18, 40, 47, 377, DateTimeKind.Utc).AddTicks(3523) },
                    { new Guid("11111111-1111-1111-1111-111111111100"), new DateTime(2025, 11, 26, 18, 40, 47, 377, DateTimeKind.Utc).AddTicks(3539), "linkaro", "images/tol/stories/seed@alchimalia.com/lunaria-s1/heroes/linkaro.png", true, "{\"Type\":\"story_completion\",\"RequiredStories\":[\"lunaria-s1\"],\"MinProgress\":100}", new DateTime(2025, 11, 26, 18, 40, 47, 377, DateTimeKind.Utc).AddTicks(3539) },
                    { new Guid("22222222-2222-2222-2222-222222222200"), new DateTime(2025, 11, 26, 18, 40, 47, 377, DateTimeKind.Utc).AddTicks(3548), "grubot", "images/tol/stories/seed@alchimalia.com/mechanika-s1/heroes/grubot.png", true, "{\"Type\":\"story_completion\",\"RequiredStories\":[\"mechanika-s1\"],\"MinProgress\":100}", new DateTime(2025, 11, 26, 18, 40, 47, 377, DateTimeKind.Utc).AddTicks(3548) }
                });

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
                table: "Animals",
                columns: new[] { "Id", "Label", "RegionId", "Src" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "Bunny", new Guid("10000000-0000-0000-0000-000000000003"), "images/animals/base/bunny.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Hippo", new Guid("10000000-0000-0000-0000-000000000006"), "images/animals/base/hippo.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "Giraffe", new Guid("10000000-0000-0000-0000-000000000004"), "images/animals/base/giraffe.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "Dog", new Guid("10000000-0000-0000-0000-000000000003"), "images/animals/base/dog.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "Fox", new Guid("10000000-0000-0000-0000-000000000005"), "images/animals/base/fox.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "Cat", new Guid("10000000-0000-0000-0000-000000000003"), "images/animals/base/cat.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "Monkey", new Guid("10000000-0000-0000-0000-000000000002"), "images/animals/base/monkey.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "Camel", new Guid("10000000-0000-0000-0000-000000000001"), "images/animals/base/camel.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "Deer", new Guid("10000000-0000-0000-0000-000000000005"), "images/animals/base/deer.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000000a"), "Duck", new Guid("10000000-0000-0000-0000-000000000006"), "images/animals/base/duck.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000000b"), "Eagle", new Guid("10000000-0000-0000-0000-000000000007"), "images/animals/base/eagle.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000000c"), "Elephant", new Guid("10000000-0000-0000-0000-000000000004"), "images/animals/base/elephant.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000000d"), "Ostrich", new Guid("10000000-0000-0000-0000-000000000004"), "images/animals/base/ostrich.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000000e"), "Parrot", new Guid("10000000-0000-0000-0000-000000000002"), "images/animals/base/parrot.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000000f"), "Jaguar", new Guid("10000000-0000-0000-0000-000000000002"), "images/animals/base/jaguar.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "Toucan", new Guid("10000000-0000-0000-0000-000000000002"), "images/animals/base/toucan.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "Anaconda", new Guid("10000000-0000-0000-0000-000000000002"), "images/animals/base/anaconda.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "Capuchin Monkey", new Guid("10000000-0000-0000-0000-000000000002"), "images/animals/base/capuchin_monkey.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "Poison Dart Frog", new Guid("10000000-0000-0000-0000-000000000002"), "images/animals/base/poison_dart_frog.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "Lion", new Guid("10000000-0000-0000-0000-000000000004"), "images/animals/base/lion.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "African Elephant", new Guid("10000000-0000-0000-0000-000000000004"), "images/animals/base/african_elephant.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "Giraffe", new Guid("10000000-0000-0000-0000-000000000004"), "images/animals/base/giraffe.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "Zebra", new Guid("10000000-0000-0000-0000-000000000004"), "images/animals/base/zebra.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "Rhinoceros", new Guid("10000000-0000-0000-0000-000000000004"), "images/animals/base/rhinoceros.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000019"), "Bison", new Guid("10000000-0000-0000-0000-000000000007"), "images/animals/base/bison.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "Saiga Antelope", new Guid("10000000-0000-0000-0000-000000000004"), "images/animals/base/saiga_antelope.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000001b"), "Gray Wolf", new Guid("10000000-0000-0000-0000-000000000005"), "images/animals/base/gray_wolf.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000001c"), "Przewalski's Horse", new Guid("10000000-0000-0000-0000-000000000007"), "images/animals/base/przewalski_horse.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "Steppe Eagle", new Guid("10000000-0000-0000-0000-000000000007"), "images/animals/base/steppe_eagle.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000001e"), "Cow", new Guid("10000000-0000-0000-0000-000000000003"), "images/animals/base/cow.jpg" },
                    { new Guid("00000000-0000-0000-0000-00000000001f"), "Sheep", new Guid("10000000-0000-0000-0000-000000000003"), "images/animals/base/sheep.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "Horse", new Guid("10000000-0000-0000-0000-000000000003"), "images/animals/base/horse.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "Chicken", new Guid("10000000-0000-0000-0000-000000000003"), "images/animals/base/chicken.jpg" },
                    { new Guid("00000000-0000-0000-0000-000000000022"), "Pig", new Guid("10000000-0000-0000-0000-000000000003"), "images/animals/base/pig.jpg" }
                });

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
                table: "BodyPartTranslations",
                columns: new[] { "Id", "BodyPartKey", "LanguageCode", "Name" },
                values: new object[,]
                {
                    { new Guid("07deeccd-b677-4f0e-9d5b-58e937ea6006"), "horns", "en-us", "Horns" },
                    { new Guid("274952ca-a936-4639-ab67-1aecbb85bc5b"), "tail", "en-us", "Tail" },
                    { new Guid("6c36036c-86b4-4dd7-aed3-bcd915d059f2"), "arms", "en-us", "Arms" },
                    { new Guid("7f7e9e04-b282-4d5b-839b-bdebdf262f31"), "horn", "en-us", "Horn" },
                    { new Guid("8087c412-bbf8-4101-8823-9df83c8da0dc"), "body", "en-us", "Body" },
                    { new Guid("9cfcfe92-8082-4951-8d45-7dd52809a26f"), "wings", "en-us", "Wings" },
                    { new Guid("d0fcca76-457f-46ad-a4c9-2fc620e6ad82"), "legs", "en-us", "Legs" },
                    { new Guid("d6422238-61dc-43e2-89e7-e80e0b7ed26b"), "head", "en-us", "Head" }
                });

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
                table: "StoryHeroUnlocks",
                columns: new[] { "Id", "CreatedAt", "StoryHeroId", "StoryId", "UnlockOrder" },
                values: new object[,]
                {
                    { new Guid("60660482-e7c8-4811-a62b-ff8b8aef94e5"), new DateTime(2025, 11, 26, 18, 40, 47, 377, DateTimeKind.Utc).AddTicks(6283), new Guid("00000000-0000-0000-0000-000000000100"), "intro-pufpuf", 1 },
                    { new Guid("c418d02b-db18-4065-b910-b922a20a454e"), new DateTime(2025, 11, 26, 18, 40, 47, 377, DateTimeKind.Utc).AddTicks(6314), new Guid("11111111-1111-1111-1111-111111111100"), "lunaria-s1", 1 },
                    { new Guid("f27a146c-37ce-425b-8ece-ec9d1b3f42eb"), new DateTime(2025, 11, 26, 18, 40, 47, 377, DateTimeKind.Utc).AddTicks(6339), new Guid("22222222-2222-2222-2222-222222222200"), "mechanika-s1", 1 }
                });

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
                table: "AnimalPartSupports",
                columns: new[] { "AnimalId", "PartKey" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000001"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000001"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "horn" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "horns" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "horn" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "horns" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "tail" },
                    { new Guid("00000000-0000-0000-0000-00000000000a"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000000a"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000000a"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000000a"), "legs" },
                    { new Guid("00000000-0000-0000-0000-00000000000a"), "tail" },
                    { new Guid("00000000-0000-0000-0000-00000000000a"), "wings" },
                    { new Guid("00000000-0000-0000-0000-00000000000b"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000000b"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000000b"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000000b"), "legs" },
                    { new Guid("00000000-0000-0000-0000-00000000000b"), "tail" },
                    { new Guid("00000000-0000-0000-0000-00000000000b"), "wings" },
                    { new Guid("00000000-0000-0000-0000-00000000000c"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000000c"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000000c"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000000d"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000000d"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000000d"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000000d"), "legs" },
                    { new Guid("00000000-0000-0000-0000-00000000000d"), "tail" },
                    { new Guid("00000000-0000-0000-0000-00000000000d"), "wings" },
                    { new Guid("00000000-0000-0000-0000-00000000000e"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000000e"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000000e"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000000e"), "legs" },
                    { new Guid("00000000-0000-0000-0000-00000000000e"), "tail" },
                    { new Guid("00000000-0000-0000-0000-00000000000e"), "wings" },
                    { new Guid("00000000-0000-0000-0000-00000000000f"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000000f"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000000f"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "wings" },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "horn" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000019"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000019"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000019"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "horns" },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "legs" },
                    { new Guid("00000000-0000-0000-0000-00000000001a"), "tail" },
                    { new Guid("00000000-0000-0000-0000-00000000001b"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000001b"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000001b"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000001c"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000001c"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000001c"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000001c"), "legs" },
                    { new Guid("00000000-0000-0000-0000-00000000001c"), "tail" },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "legs" },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "tail" },
                    { new Guid("00000000-0000-0000-0000-00000000001d"), "wings" },
                    { new Guid("00000000-0000-0000-0000-00000000001e"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000001e"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000001e"), "head" },
                    { new Guid("00000000-0000-0000-0000-00000000001f"), "arms" },
                    { new Guid("00000000-0000-0000-0000-00000000001f"), "body" },
                    { new Guid("00000000-0000-0000-0000-00000000001f"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "head" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "legs" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "tail" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "wings" },
                    { new Guid("00000000-0000-0000-0000-000000000022"), "arms" },
                    { new Guid("00000000-0000-0000-0000-000000000022"), "body" },
                    { new Guid("00000000-0000-0000-0000-000000000022"), "head" }
                });

            migrationBuilder.InsertData(
                schema: "alchimalia_schema",
                table: "AnimalTranslations",
                columns: new[] { "Id", "AnimalId", "Label", "LanguageCode" },
                values: new object[,]
                {
                    { new Guid("003614ca-880e-4c87-9497-ba00bb755633"), new Guid("00000000-0000-0000-0000-00000000001a"), "Saiga Antelope", "en-us" },
                    { new Guid("0abf406a-0dd7-4310-baf3-80eff206a879"), new Guid("00000000-0000-0000-0000-000000000003"), "Giraffe", "en-us" },
                    { new Guid("0bae418c-cd02-4fe0-8cae-ba0709b27843"), new Guid("00000000-0000-0000-0000-000000000017"), "Zebra", "en-us" },
                    { new Guid("0f7f5fcf-cdd4-4a2f-aee6-04c11ef21c7e"), new Guid("00000000-0000-0000-0000-000000000001"), "Bunny", "en-us" },
                    { new Guid("15a3d6f3-3d5d-484c-b5a0-c2b3e3598c44"), new Guid("00000000-0000-0000-0000-000000000019"), "Bison", "en-us" },
                    { new Guid("231d5136-a8bd-4e1c-a5ed-6a46156db1f4"), new Guid("00000000-0000-0000-0000-00000000001b"), "Gray Wolf", "en-us" },
                    { new Guid("2f194465-3e7e-4df3-8ada-f3eef714762a"), new Guid("00000000-0000-0000-0000-000000000008"), "Camel", "en-us" },
                    { new Guid("2f8fc3a2-3aec-4512-974d-17ecba506217"), new Guid("00000000-0000-0000-0000-00000000000f"), "Jaguar", "en-us" },
                    { new Guid("2ff01efe-f322-457c-a93a-ca67b88cfced"), new Guid("00000000-0000-0000-0000-00000000000a"), "Duck", "en-us" },
                    { new Guid("33bf19be-1d72-49b3-b3c1-e9bb329f4316"), new Guid("00000000-0000-0000-0000-00000000000c"), "Elephant", "en-us" },
                    { new Guid("36d80ca6-8f78-42ad-b970-e2caa6d865a8"), new Guid("00000000-0000-0000-0000-000000000018"), "Rhinoceros", "en-us" },
                    { new Guid("414d0a9d-70e6-42b5-a45e-42cef178d372"), new Guid("00000000-0000-0000-0000-00000000001c"), "Przewalski's Horse", "en-us" },
                    { new Guid("514a3b0f-b005-4821-b7a8-d8286495950b"), new Guid("00000000-0000-0000-0000-000000000013"), "Poison Dart Frog", "en-us" },
                    { new Guid("59bfa96d-3739-486e-9f06-4f9d1c2160d3"), new Guid("00000000-0000-0000-0000-000000000005"), "Fox", "en-us" },
                    { new Guid("5fdbd7d2-d8d7-4758-93aa-23aff9740cfb"), new Guid("00000000-0000-0000-0000-000000000004"), "Dog", "en-us" },
                    { new Guid("6d8f580f-92ae-44cf-8114-e6b4676e97dd"), new Guid("00000000-0000-0000-0000-000000000016"), "Giraffe", "en-us" },
                    { new Guid("714399e4-8272-4bd1-92bc-b0e2569214ad"), new Guid("00000000-0000-0000-0000-00000000001f"), "Sheep", "en-us" },
                    { new Guid("727e2e83-2957-47ec-945a-155d080c08e9"), new Guid("00000000-0000-0000-0000-000000000021"), "Chicken", "en-us" },
                    { new Guid("72d3368c-de7d-4a00-a738-f5ab2fd1487a"), new Guid("00000000-0000-0000-0000-000000000014"), "Lion", "en-us" },
                    { new Guid("7d95f51f-5d2d-42e6-8df4-f35a0dcfe4fb"), new Guid("00000000-0000-0000-0000-00000000000b"), "Eagle", "en-us" },
                    { new Guid("826fcdf4-4ac5-4619-b1be-a54d3bdd7513"), new Guid("00000000-0000-0000-0000-000000000007"), "Monkey", "en-us" },
                    { new Guid("82f383c1-7caa-4c12-88e6-7099bfd4e6a9"), new Guid("00000000-0000-0000-0000-00000000001d"), "Steppe Eagle", "en-us" },
                    { new Guid("8473a1b9-5b1c-4377-9e6d-b6d6692b59a1"), new Guid("00000000-0000-0000-0000-000000000012"), "Capuchin Monkey", "en-us" },
                    { new Guid("8490cf98-c482-4ed2-9219-030af6962ec1"), new Guid("00000000-0000-0000-0000-00000000001e"), "Cow", "en-us" },
                    { new Guid("9a519ce6-9b80-4d0f-9830-65815451d93c"), new Guid("00000000-0000-0000-0000-000000000006"), "Cat", "en-us" },
                    { new Guid("a7e519df-5e82-4d56-a760-87e5fb1c11bc"), new Guid("00000000-0000-0000-0000-000000000010"), "Toucan", "en-us" },
                    { new Guid("ad8ea5b4-b3ca-41ef-8af5-a08ef0c9e6c1"), new Guid("00000000-0000-0000-0000-000000000009"), "Deer", "en-us" },
                    { new Guid("afcc1664-b026-4eec-92c5-5a25b26a9bb7"), new Guid("00000000-0000-0000-0000-000000000002"), "Hippo", "en-us" },
                    { new Guid("b42320d3-0800-49f3-95e7-34d8926b77b0"), new Guid("00000000-0000-0000-0000-000000000020"), "Horse", "en-us" },
                    { new Guid("bc3d333d-669d-4be8-ae49-6882833af8be"), new Guid("00000000-0000-0000-0000-000000000015"), "African Elephant", "en-us" },
                    { new Guid("de2fd098-ff21-4592-a8ad-2a56c0d1390d"), new Guid("00000000-0000-0000-0000-000000000022"), "Pig", "en-us" },
                    { new Guid("f1f0ac42-5fac-4a99-aa34-86cfb2f0a818"), new Guid("00000000-0000-0000-0000-00000000000e"), "Parrot", "en-us" },
                    { new Guid("fd4a9bea-32ad-4a14-9c61-7c5632a8724c"), new Guid("00000000-0000-0000-0000-00000000000d"), "Ostrich", "en-us" },
                    { new Guid("fe6fc993-4542-443b-a6b1-84ff2e228455"), new Guid("00000000-0000-0000-0000-000000000011"), "Anaconda", "en-us" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_Regions_RegionId",
                schema: "alchimalia_schema",
                table: "Animals",
                column: "RegionId",
                principalSchema: "alchimalia_schema",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
