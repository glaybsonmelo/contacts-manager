﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class GetAllPersons_StoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"
                CREATE PROCEDURE [dbo].[GetAllPersons] 
                    AS BEGIN
                        SELECT * FROM [dbo].[Persons]
                    END
            ";
            migrationBuilder.Sql(sp_GetAllPersons);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"
                DROP PROCEDURE [dbo].[GetAllPersons]
            ";
            migrationBuilder.Sql(sp_GetAllPersons);
        }
    }
}