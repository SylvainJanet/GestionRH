namespace MiseEnSituation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignKeyInEmployeeForPostAndCompany : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Users", name: "Company_Id", newName: "CompagnyId");
            RenameColumn(table: "dbo.Users", name: "Post_Id1", newName: "PostId");
            RenameIndex(table: "dbo.Users", name: "IX_Company_Id", newName: "IX_CompagnyId");
            RenameIndex(table: "dbo.Users", name: "IX_Post_Id1", newName: "IX_PostId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Users", name: "IX_PostId", newName: "IX_Post_Id1");
            RenameIndex(table: "dbo.Users", name: "IX_CompagnyId", newName: "IX_Company_Id");
            RenameColumn(table: "dbo.Users", name: "PostId", newName: "Post_Id1");
            RenameColumn(table: "dbo.Users", name: "CompagnyId", newName: "Company_Id");
        }
    }
}
