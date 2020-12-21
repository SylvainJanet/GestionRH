namespace MiseEnSituation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveAdressForeignKeyInEmployee : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Users", name: "AdresseId", newName: "PersonalAdress_Id");
            RenameIndex(table: "dbo.Users", name: "IX_AdresseId", newName: "IX_PersonalAdress_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Users", name: "IX_PersonalAdress_Id", newName: "IX_AdresseId");
            RenameColumn(table: "dbo.Users", name: "PersonalAdress_Id", newName: "AdresseId");
        }
    }
}
