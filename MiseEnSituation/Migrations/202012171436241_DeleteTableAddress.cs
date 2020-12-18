namespace MiseEnSituation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteTableAddress : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Companies", new[] { "Adress_Number", "Adress_Street", "Adress_City", "Adress_ZipCode", "Adress_Country" }, "dbo.Addresses");
            DropForeignKey("dbo.Users", new[] { "PersonalAdress_Number", "PersonalAdress_Street", "PersonalAdress_City", "PersonalAdress_ZipCode", "PersonalAdress_Country" }, "dbo.Addresses");
            DropIndex("dbo.Users", new[] { "PersonalAdress_Number", "PersonalAdress_Street", "PersonalAdress_City", "PersonalAdress_ZipCode", "PersonalAdress_Country" });
            DropIndex("dbo.Companies", new[] { "Adress_Number", "Adress_Street", "Adress_City", "Adress_ZipCode", "Adress_Country" });
            DropTable("dbo.Addresses");
        }
        
        public override void Down()
        {
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
            
            CreateIndex("dbo.Companies", new[] { "Adress_Number", "Adress_Street", "Adress_City", "Adress_ZipCode", "Adress_Country" });
            CreateIndex("dbo.Users", new[] { "PersonalAdress_Number", "PersonalAdress_Street", "PersonalAdress_City", "PersonalAdress_ZipCode", "PersonalAdress_Country" });
            AddForeignKey("dbo.Users", new[] { "PersonalAdress_Number", "PersonalAdress_Street", "PersonalAdress_City", "PersonalAdress_ZipCode", "PersonalAdress_Country" }, "dbo.Addresses", new[] { "Number", "Street", "City", "ZipCode", "Country" });
            AddForeignKey("dbo.Companies", new[] { "Adress_Number", "Adress_Street", "Adress_City", "Adress_ZipCode", "Adress_Country" }, "dbo.Addresses", new[] { "Number", "Street", "City", "ZipCode", "Country" }, cascadeDelete: true);
        }
    }
}
