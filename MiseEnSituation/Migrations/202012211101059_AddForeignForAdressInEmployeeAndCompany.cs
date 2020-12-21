namespace MiseEnSituation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignForAdressInEmployeeAndCompany : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Users", name: "PersonalAdress_Id", newName: "AdresseId");
            RenameColumn(table: "dbo.Companies", name: "Adress_Id", newName: "AdressId");
            RenameIndex(table: "dbo.Users", name: "IX_PersonalAdress_Id", newName: "IX_AdresseId");
            RenameIndex(table: "dbo.Companies", name: "IX_Adress_Id", newName: "IX_AdressId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Companies", name: "IX_AdressId", newName: "IX_Adress_Id");
            RenameIndex(table: "dbo.Users", name: "IX_AdresseId", newName: "IX_PersonalAdress_Id");
            RenameColumn(table: "dbo.Companies", name: "AdressId", newName: "Adress_Id");
            RenameColumn(table: "dbo.Users", name: "AdresseId", newName: "PersonalAdress_Id");
        }
    }
}
