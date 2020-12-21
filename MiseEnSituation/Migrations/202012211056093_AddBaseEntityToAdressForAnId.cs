namespace MiseEnSituation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBaseEntityToAdressForAnId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", new[] { "PersonalAdress_Number", "PersonalAdress_Street", "PersonalAdress_City", "PersonalAdress_ZipCode", "PersonalAdress_Country" }, "dbo.Addresses");
            DropForeignKey("dbo.Companies", new[] { "Adress_Number", "Adress_Street", "Adress_City", "Adress_ZipCode", "Adress_Country" }, "dbo.Addresses");
            DropIndex("dbo.Users", new[] { "PersonalAdress_Number", "PersonalAdress_Street", "PersonalAdress_City", "PersonalAdress_ZipCode", "PersonalAdress_Country" });
            DropIndex("dbo.Companies", new[] { "Adress_Number", "Adress_Street", "Adress_City", "Adress_ZipCode", "Adress_Country" });
            RenameColumn(table: "dbo.Users", name: "PersonalAdress_Number", newName: "PersonalAdress_Id");
            RenameColumn(table: "dbo.Companies", name: "Adress_Number", newName: "Adress_Id");
            DropPrimaryKey("dbo.Addresses");
            AddColumn("dbo.Addresses", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Addresses", "Id");
            CreateIndex("dbo.Users", "PersonalAdress_Id");
            CreateIndex("dbo.Companies", "Adress_Id");
            AddForeignKey("dbo.Users", "PersonalAdress_Id", "dbo.Addresses", "Id");
            AddForeignKey("dbo.Companies", "Adress_Id", "dbo.Addresses", "Id", cascadeDelete: true);
            DropColumn("dbo.Users", "PersonalAdress_Street");
            DropColumn("dbo.Users", "PersonalAdress_City");
            DropColumn("dbo.Users", "PersonalAdress_ZipCode");
            DropColumn("dbo.Users", "PersonalAdress_Country");
            DropColumn("dbo.Companies", "Adress_Street");
            DropColumn("dbo.Companies", "Adress_City");
            DropColumn("dbo.Companies", "Adress_ZipCode");
            DropColumn("dbo.Companies", "Adress_Country");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Companies", "Adress_Country", c => c.String(nullable: false, maxLength: 200));
            AddColumn("dbo.Companies", "Adress_ZipCode", c => c.Int(nullable: false));
            AddColumn("dbo.Companies", "Adress_City", c => c.String(nullable: false, maxLength: 200));
            AddColumn("dbo.Companies", "Adress_Street", c => c.String(nullable: false, maxLength: 200));
            AddColumn("dbo.Users", "PersonalAdress_Country", c => c.String(maxLength: 200));
            AddColumn("dbo.Users", "PersonalAdress_ZipCode", c => c.Int());
            AddColumn("dbo.Users", "PersonalAdress_City", c => c.String(maxLength: 200));
            AddColumn("dbo.Users", "PersonalAdress_Street", c => c.String(maxLength: 200));
            DropForeignKey("dbo.Companies", "Adress_Id", "dbo.Addresses");
            DropForeignKey("dbo.Users", "PersonalAdress_Id", "dbo.Addresses");
            DropIndex("dbo.Companies", new[] { "Adress_Id" });
            DropIndex("dbo.Users", new[] { "PersonalAdress_Id" });
            DropPrimaryKey("dbo.Addresses");
            DropColumn("dbo.Addresses", "Id");
            AddPrimaryKey("dbo.Addresses", new[] { "Number", "Street", "City", "ZipCode", "Country" });
            RenameColumn(table: "dbo.Companies", name: "Adress_Id", newName: "Adress_Number");
            RenameColumn(table: "dbo.Users", name: "PersonalAdress_Id", newName: "PersonalAdress_Number");
            CreateIndex("dbo.Companies", new[] { "Adress_Number", "Adress_Street", "Adress_City", "Adress_ZipCode", "Adress_Country" });
            CreateIndex("dbo.Users", new[] { "PersonalAdress_Number", "PersonalAdress_Street", "PersonalAdress_City", "PersonalAdress_ZipCode", "PersonalAdress_Country" });
            AddForeignKey("dbo.Companies", new[] { "Adress_Number", "Adress_Street", "Adress_City", "Adress_ZipCode", "Adress_Country" }, "dbo.Addresses", new[] { "Number", "Street", "City", "ZipCode", "Country" }, cascadeDelete: true);
            AddForeignKey("dbo.Users", new[] { "PersonalAdress_Number", "PersonalAdress_Street", "PersonalAdress_City", "PersonalAdress_ZipCode", "PersonalAdress_Country" }, "dbo.Addresses", new[] { "Number", "Street", "City", "ZipCode", "Country" });
        }
    }
}
