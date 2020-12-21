namespace MiseEnSituation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignKeyInPostForCompany : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Posts", name: "Company_Id", newName: "CompanyId");
            RenameIndex(table: "dbo.Posts", name: "IX_Company_Id", newName: "IX_CompanyId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Posts", name: "IX_CompanyId", newName: "IX_Company_Id");
            RenameColumn(table: "dbo.Posts", name: "CompanyId", newName: "Company_Id");
        }
    }
}
