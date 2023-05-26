using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class InsertPerson_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string insertPerson_sp = @"CREATE PROCEDURE [dbo].[insertPerson]
                (@PersonID uniqueidentifier, @PersonName nvarchar(40),
                @Email nvarchar(50), @DateOfBirth datetime2(7), @Gendrer nvarchar(10),
                @CountryID uniqueidentifier, @Address nvarchar(1000), @ReceiveNewsLetter bit)
                AS BEGIN
                    INSERT INTO [dbo].[Persons](PersonID, PersonName, Email,
                    DateOfBirth, Gender, CountryID, Address, ReceiveNewsLetter)
                    VALUES
                    (@PersonID, @PersonName, @Email, @DateOfBirth, @Gendrer,
                    @CountryID , @Address,  @ReceiveNewsLetter)
                END";

            migrationBuilder.Sql(insertPerson_sp);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string insertPerson_sp = @"DROP PROCEDURE [dbo].[insertPerson]";
            migrationBuilder.Sql(insertPerson_sp);
        }
    }
}
