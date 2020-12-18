namespace MiseEnSituation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignKeyForReportInCheckUp : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.CheckUps", name: "Report_Id", newName: "ReportId");
            RenameIndex(table: "dbo.CheckUps", name: "IX_Report_Id", newName: "IX_ReportId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.CheckUps", name: "IX_ReportId", newName: "IX_Report_Id");
            RenameColumn(table: "dbo.CheckUps", name: "ReportId", newName: "Report_Id");
        }
    }
}
