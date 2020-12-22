namespace MiseEnSituation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteRequiredFromCompanyPostEmployee : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "CompagnyId", "dbo.Companies");
            DropForeignKey("dbo.Users", "PostId", "dbo.Posts");
            DropForeignKey("dbo.Companies", "AdressId", "dbo.Addresses");
            DropForeignKey("dbo.Posts", "CompanyId", "dbo.Companies");
            DropIndex("dbo.Companies", new[] { "AdressId" });
            DropIndex("dbo.Posts", new[] { "CompanyId" });
            AlterColumn("dbo.Companies", "AdressId", c => c.Int());
            AlterColumn("dbo.Posts", "CompanyId", c => c.Int());
            CreateIndex("dbo.Companies", "AdressId");
            CreateIndex("dbo.Posts", "CompanyId");
            AddForeignKey("dbo.Users", "CompagnyId", "dbo.Companies", "Id");
            AddForeignKey("dbo.Users", "PostId", "dbo.Posts", "Id");
            AddForeignKey("dbo.Companies", "AdressId", "dbo.Addresses", "Id");
            AddForeignKey("dbo.Posts", "CompanyId", "dbo.Companies", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Posts", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Companies", "AdressId", "dbo.Addresses");
            DropForeignKey("dbo.Users", "PostId", "dbo.Posts");
            DropForeignKey("dbo.Users", "CompagnyId", "dbo.Companies");
            DropIndex("dbo.Posts", new[] { "CompanyId" });
            DropIndex("dbo.Companies", new[] { "AdressId" });
            AlterColumn("dbo.Posts", "CompanyId", c => c.Int(nullable: false));
            AlterColumn("dbo.Companies", "AdressId", c => c.Int(nullable: false));
            CreateIndex("dbo.Posts", "CompanyId");
            CreateIndex("dbo.Companies", "AdressId");
            AddForeignKey("dbo.Posts", "CompanyId", "dbo.Companies", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Companies", "AdressId", "dbo.Addresses", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Users", "PostId", "dbo.Posts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Users", "CompagnyId", "dbo.Companies", "Id", cascadeDelete: true);
        }
    }
}
