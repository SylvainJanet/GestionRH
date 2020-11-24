namespace MiseEnSituation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAnnotations : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "Company_Id", "dbo.Companies");
            DropForeignKey("dbo.Users", "Post_Id1", "dbo.Posts");
            DropForeignKey("dbo.Posts", "Company_Id", "dbo.Companies");
            DropIndex("dbo.Users", new[] { "Email" });
            DropIndex("dbo.Posts", new[] { "Company_Id" });
            RenameColumn(table: "dbo.Posts", name: "Manage_Id", newName: "Manager_Id");
            RenameIndex(table: "dbo.Posts", name: "IX_Manage_Id", newName: "IX_Manager_Id");
            CreateTable(
                "dbo.CheckUpReports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CheckUps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Manager_Id = c.Int(),
                        Report_Id = c.Int(),
                        RH_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Manager_Id)
                .ForeignKey("dbo.CheckUpReports", t => t.Report_Id)
                .ForeignKey("dbo.Users", t => t.RH_Id)
                .Index(t => t.Manager_Id)
                .Index(t => t.Report_Id)
                .Index(t => t.RH_Id);
            
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Number = c.Int(nullable: false),
                        Street = c.String(nullable: false, maxLength: 200),
                        City = c.String(nullable: false, maxLength: 200),
                        ZipCode = c.Int(nullable: false),
                        Country = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => new { t.Number, t.Street, t.City, t.ZipCode, t.Country });
            
            AddColumn("dbo.Users", "CheckUp_Id", c => c.Int());
            AlterColumn("dbo.Users", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false, maxLength: 150));
            AlterColumn("dbo.Users", "Password", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "PersonalAdress_Street", c => c.String(maxLength: 200));
            AlterColumn("dbo.Users", "PersonalAdress_City", c => c.String(maxLength: 200));
            AlterColumn("dbo.Users", "PersonalAdress_Country", c => c.String(maxLength: 200));
            AlterColumn("dbo.Companies", "Name", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.Companies", "Adress_Street", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.Companies", "Adress_City", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.Companies", "Adress_Country", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.Skills", "Description", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.Posts", "FileForContract", c => c.String(nullable: false));
            AlterColumn("dbo.Posts", "Company_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Users", "Email", unique: true);
            CreateIndex("dbo.Users", new[] { "PersonalAdress_Number", "PersonalAdress_Street", "PersonalAdress_City", "PersonalAdress_ZipCode", "PersonalAdress_Country" });
            CreateIndex("dbo.Users", "CheckUp_Id");
            CreateIndex("dbo.Companies", new[] { "Adress_Number", "Adress_Street", "Adress_City", "Adress_ZipCode", "Adress_Country" });
            CreateIndex("dbo.Posts", "Company_Id");
            AddForeignKey("dbo.Companies", new[] { "Adress_Number", "Adress_Street", "Adress_City", "Adress_ZipCode", "Adress_Country" }, "dbo.Addresses", new[] { "Number", "Street", "City", "ZipCode", "Country" }, cascadeDelete: false);
            AddForeignKey("dbo.Users", new[] { "PersonalAdress_Number", "PersonalAdress_Street", "PersonalAdress_City", "PersonalAdress_ZipCode", "PersonalAdress_Country" }, "dbo.Addresses", new[] { "Number", "Street", "City", "ZipCode", "Country" }, cascadeDelete: false);
            AddForeignKey("dbo.Users", "CheckUp_Id", "dbo.CheckUps", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Users", "Company_Id", "dbo.Companies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Users", "Post_Id1", "dbo.Posts", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Posts", "Company_Id", "dbo.Companies", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Posts", "Company_Id", "dbo.Companies");
            DropForeignKey("dbo.Users", "Post_Id1", "dbo.Posts");
            DropForeignKey("dbo.Users", "Company_Id", "dbo.Companies");
            DropForeignKey("dbo.CheckUps", "RH_Id", "dbo.Users");
            DropForeignKey("dbo.CheckUps", "Report_Id", "dbo.CheckUpReports");
            DropForeignKey("dbo.CheckUps", "Manager_Id", "dbo.Users");
            DropForeignKey("dbo.Users", "CheckUp_Id", "dbo.CheckUps");
            DropForeignKey("dbo.Users", new[] { "PersonalAdress_Number", "PersonalAdress_Street", "PersonalAdress_City", "PersonalAdress_ZipCode", "PersonalAdress_Country" }, "dbo.Addresses");
            DropForeignKey("dbo.Companies", new[] { "Adress_Number", "Adress_Street", "Adress_City", "Adress_ZipCode", "Adress_Country" }, "dbo.Addresses");
            DropIndex("dbo.Posts", new[] { "Company_Id" });
            DropIndex("dbo.Companies", new[] { "Adress_Number", "Adress_Street", "Adress_City", "Adress_ZipCode", "Adress_Country" });
            DropIndex("dbo.Users", new[] { "CheckUp_Id" });
            DropIndex("dbo.Users", new[] { "PersonalAdress_Number", "PersonalAdress_Street", "PersonalAdress_City", "PersonalAdress_ZipCode", "PersonalAdress_Country" });
            DropIndex("dbo.Users", new[] { "Email" });
            DropIndex("dbo.CheckUps", new[] { "RH_Id" });
            DropIndex("dbo.CheckUps", new[] { "Report_Id" });
            DropIndex("dbo.CheckUps", new[] { "Manager_Id" });
            AlterColumn("dbo.Posts", "Company_Id", c => c.Int());
            AlterColumn("dbo.Posts", "FileForContract", c => c.String());
            AlterColumn("dbo.Skills", "Description", c => c.String());
            AlterColumn("dbo.Companies", "Adress_Country", c => c.String());
            AlterColumn("dbo.Companies", "Adress_City", c => c.String());
            AlterColumn("dbo.Companies", "Adress_Street", c => c.String());
            AlterColumn("dbo.Companies", "Name", c => c.String());
            AlterColumn("dbo.Users", "PersonalAdress_Country", c => c.String());
            AlterColumn("dbo.Users", "PersonalAdress_City", c => c.String());
            AlterColumn("dbo.Users", "PersonalAdress_Street", c => c.String());
            AlterColumn("dbo.Users", "Password", c => c.String());
            AlterColumn("dbo.Users", "Email", c => c.String(maxLength: 150));
            AlterColumn("dbo.Users", "Name", c => c.String());
            DropColumn("dbo.Users", "CheckUp_Id");
            DropTable("dbo.Addresses");
            DropTable("dbo.CheckUps");
            DropTable("dbo.CheckUpReports");
            RenameIndex(table: "dbo.Posts", name: "IX_Manager_Id", newName: "IX_Manage_Id");
            RenameColumn(table: "dbo.Posts", name: "Manager_Id", newName: "Manage_Id");
            CreateIndex("dbo.Posts", "Company_Id");
            CreateIndex("dbo.Users", "Email", unique: true);
            AddForeignKey("dbo.Posts", "Company_Id", "dbo.Companies", "Id");
            AddForeignKey("dbo.Users", "Post_Id1", "dbo.Posts", "Id");
            AddForeignKey("dbo.Users", "Company_Id", "dbo.Companies", "Id");
        }
    }
}
