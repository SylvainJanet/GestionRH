namespace MiseEnSituation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignKeyCheckUp : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.CheckUps", name: "Employee_Id", newName: "EmployeeId");
            RenameColumn(table: "dbo.CheckUps", name: "Manager_Id", newName: "ManagerId");
            RenameColumn(table: "dbo.CheckUps", name: "RH_Id", newName: "RHId");
            RenameIndex(table: "dbo.CheckUps", name: "IX_Employee_Id", newName: "IX_EmployeeId");
            RenameIndex(table: "dbo.CheckUps", name: "IX_Manager_Id", newName: "IX_ManagerId");
            RenameIndex(table: "dbo.CheckUps", name: "IX_RH_Id", newName: "IX_RHId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.CheckUps", name: "IX_RHId", newName: "IX_RH_Id");
            RenameIndex(table: "dbo.CheckUps", name: "IX_ManagerId", newName: "IX_Manager_Id");
            RenameIndex(table: "dbo.CheckUps", name: "IX_EmployeeId", newName: "IX_Employee_Id");
            RenameColumn(table: "dbo.CheckUps", name: "RHId", newName: "RH_Id");
            RenameColumn(table: "dbo.CheckUps", name: "ManagerId", newName: "Manager_Id");
            RenameColumn(table: "dbo.CheckUps", name: "EmployeeId", newName: "Employee_Id");
        }
    }
}
