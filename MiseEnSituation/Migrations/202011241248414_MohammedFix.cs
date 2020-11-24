namespace MiseEnSituation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MohammedFix : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TrainingCourseEmployees", newName: "EmployeeTrainingCourses");
            DropForeignKey("dbo.Users", "CheckUp_Id", "dbo.CheckUps");
            DropIndex("dbo.Users", new[] { "CheckUp_Id" });
            DropPrimaryKey("dbo.EmployeeTrainingCourses");
            AddColumn("dbo.CheckUps", "Date", c => c.DateTime(nullable: false));
            AddColumn("dbo.CheckUps", "Employee_Id", c => c.Int());
            AddColumn("dbo.TrainingCourses", "CheckUpReport_Id", c => c.Int());
            AddColumn("dbo.TrainingCourses", "CheckUpReport_Id1", c => c.Int());
            AddPrimaryKey("dbo.EmployeeTrainingCourses", new[] { "Employee_Id", "TrainingCourse_Id" });
            CreateIndex("dbo.TrainingCourses", "CheckUpReport_Id");
            CreateIndex("dbo.TrainingCourses", "CheckUpReport_Id1");
            CreateIndex("dbo.CheckUps", "Employee_Id");
            AddForeignKey("dbo.TrainingCourses", "CheckUpReport_Id", "dbo.CheckUpReports", "Id");
            AddForeignKey("dbo.TrainingCourses", "CheckUpReport_Id1", "dbo.CheckUpReports", "Id");
            AddForeignKey("dbo.CheckUps", "Employee_Id", "dbo.Users", "Id");
            DropColumn("dbo.Users", "CheckUp_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "CheckUp_Id", c => c.Int());
            DropForeignKey("dbo.CheckUps", "Employee_Id", "dbo.Users");
            DropForeignKey("dbo.TrainingCourses", "CheckUpReport_Id1", "dbo.CheckUpReports");
            DropForeignKey("dbo.TrainingCourses", "CheckUpReport_Id", "dbo.CheckUpReports");
            DropIndex("dbo.CheckUps", new[] { "Employee_Id" });
            DropIndex("dbo.TrainingCourses", new[] { "CheckUpReport_Id1" });
            DropIndex("dbo.TrainingCourses", new[] { "CheckUpReport_Id" });
            DropPrimaryKey("dbo.EmployeeTrainingCourses");
            DropColumn("dbo.TrainingCourses", "CheckUpReport_Id1");
            DropColumn("dbo.TrainingCourses", "CheckUpReport_Id");
            DropColumn("dbo.CheckUps", "Employee_Id");
            DropColumn("dbo.CheckUps", "Date");
            AddPrimaryKey("dbo.EmployeeTrainingCourses", new[] { "TrainingCourse_Id", "Employee_Id" });
            CreateIndex("dbo.Users", "CheckUp_Id");
            AddForeignKey("dbo.Users", "CheckUp_Id", "dbo.CheckUps", "Id");
            RenameTable(name: "dbo.EmployeeTrainingCourses", newName: "TrainingCourseEmployees");
        }
    }
}
