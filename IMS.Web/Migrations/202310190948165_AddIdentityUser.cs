namespace IMS.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIdentityUser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropPrimaryKey("dbo.AspNetRoles");
            DropPrimaryKey("dbo.AspNetUserRoles");
            DropPrimaryKey("dbo.AspNetUsers");
            DropPrimaryKey("dbo.AspNetUserLogins");
            AlterColumn("dbo.AspNetRoles", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.AspNetUserRoles", "UserId", c => c.Long(nullable: false));
            AlterColumn("dbo.AspNetUserRoles", "RoleId", c => c.Long(nullable: false));
            AlterColumn("dbo.AspNetUsers", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.AspNetUserClaims", "UserId", c => c.Long(nullable: false));
            AlterColumn("dbo.AspNetUserLogins", "UserId", c => c.Long(nullable: false));
            AddPrimaryKey("dbo.AspNetRoles", "Id");
            AddPrimaryKey("dbo.AspNetUserRoles", new[] { "UserId", "RoleId" });
            AddPrimaryKey("dbo.AspNetUsers", "Id");
            AddPrimaryKey("dbo.AspNetUserLogins", new[] { "LoginProvider", "ProviderKey", "UserId" });
            CreateIndex("dbo.AspNetUserRoles", "UserId");
            CreateIndex("dbo.AspNetUserRoles", "RoleId");
            CreateIndex("dbo.AspNetUserClaims", "UserId");
            CreateIndex("dbo.AspNetUserLogins", "UserId");
            AddForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            DropColumn("dbo.AspNetUsers", "CreatedBy");
            DropColumn("dbo.AspNetUsers", "CreationDate");
            DropColumn("dbo.AspNetUsers", "ModifyBy");
            DropColumn("dbo.AspNetUsers", "ModificationDate");
            DropColumn("dbo.AspNetUsers", "Rank");
            DropColumn("dbo.AspNetUsers", "Status");
            DropColumn("dbo.AspNetUsers", "VersionNumber");
            DropColumn("dbo.AspNetUsers", "BusinessId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "BusinessId", c => c.String());
            AddColumn("dbo.AspNetUsers", "VersionNumber", c => c.Int());
            AddColumn("dbo.AspNetUsers", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Rank", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "ModificationDate", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "ModifyBy", c => c.Int());
            AddColumn("dbo.AspNetUsers", "CreationDate", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "CreatedBy", c => c.Int());
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropPrimaryKey("dbo.AspNetUserLogins");
            DropPrimaryKey("dbo.AspNetUsers");
            DropPrimaryKey("dbo.AspNetUserRoles");
            DropPrimaryKey("dbo.AspNetRoles");
            AlterColumn("dbo.AspNetUserLogins", "UserId", c => c.Int(nullable: false));
            AlterColumn("dbo.AspNetUserClaims", "UserId", c => c.Int(nullable: false));
            AlterColumn("dbo.AspNetUsers", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.AspNetUserRoles", "RoleId", c => c.Int(nullable: false));
            AlterColumn("dbo.AspNetUserRoles", "UserId", c => c.Int(nullable: false));
            AlterColumn("dbo.AspNetRoles", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.AspNetUserLogins", new[] { "LoginProvider", "ProviderKey", "UserId" });
            AddPrimaryKey("dbo.AspNetUsers", "Id");
            AddPrimaryKey("dbo.AspNetUserRoles", new[] { "UserId", "RoleId" });
            AddPrimaryKey("dbo.AspNetRoles", "Id");
            CreateIndex("dbo.AspNetUserLogins", "UserId");
            CreateIndex("dbo.AspNetUserClaims", "UserId");
            CreateIndex("dbo.AspNetUserRoles", "RoleId");
            CreateIndex("dbo.AspNetUserRoles", "UserId");
            AddForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles", "Id", cascadeDelete: true);
        }
    }
}
