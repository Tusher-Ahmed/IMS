namespace IMS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbInitializer : DbMigration
    {
        public override void Up()
        {
            // Add roles to the AspNetRoles table
            Sql("INSERT INTO dbo.AspNetRoles (Id, Name) VALUES ('1', 'Admin')");
            Sql("INSERT INTO dbo.AspNetRoles (Id, Name) VALUES ('2', 'Customer')");
            Sql("INSERT INTO dbo.AspNetRoles (Id, Name) VALUES ('3', 'Manager')");
            Sql("INSERT INTO dbo.AspNetRoles (Id, Name) VALUES ('4', 'Staff')");
            Sql("INSERT INTO dbo.AspNetRoles (Id, Name) VALUES ('5', 'Garments')");
        }

        public override void Down()
        {
            // Remove roles if needed
            Sql("DELETE FROM dbo.AspNetRoles WHERE Name IN ('Admin', 'Customer', 'Manager', 'Staff', 'Garments')");
        }
    }
}
