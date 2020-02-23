namespace CleanArchTemplate.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddRoleClaimsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetRoleClaims",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    RoleId = c.String(nullable: false, maxLength: 128),
                    ClaimType = c.String(),
                    ClaimValue = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId);

        }

        public override void Down()
        {
            DropTable("dbo.AspNetRoleClaims");
        }
    }
}
