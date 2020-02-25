namespace CleanArchTemplate.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddUserTokesnTable : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.AspNetUserTokens",
            //    c => new
            //    {
            //        UserId = c.String(nullable: false, maxLength: 128),
            //        LoginProvider = c.String(nullable: false, maxLength: 128),
            //        Name = c.String(nullable: false, maxLength: 128),
            //        Value = c.String(),
            //    })
            //    .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.Name });

            CreateTable(
                "dbo.AspNetUserTokens",
                c => new
                {
                    UserId = c.String(nullable: false, maxLength: 128),
                    LoginProvider = c.String(nullable: false, maxLength: 128),
                    Name = c.String(nullable: false, maxLength: 128),
                    Value = c.String(),
                })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.Name })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.LoginProvider)
                .Index(t => t.Name);
        }

        public override void Down()
        {
            DropTable("dbo.AspNetUserTokens");
        }
    }
}
