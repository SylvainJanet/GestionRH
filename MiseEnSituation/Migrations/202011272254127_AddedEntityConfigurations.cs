namespace MiseEnSituation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEntityConfigurations : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.CheckUpReportTrainingCourses", newName: "FinishedCoursesReports");
            RenameTable(name: "dbo.CheckUpReportTrainingCourse1", newName: "WishedCoursesReports");
            RenameTable(name: "dbo.EmployeeTrainingCourses", newName: "CoursesEnrolledEmployees");
            RenameTable(name: "dbo.SkillTrainingCourses", newName: "TraninedSkillsCourses");
            RenameTable(name: "dbo.SkillsEmployees", newName: "SkillsEmployees");
            RenameTable(name: "dbo.SkillPosts", newName: "RequiredSkillsPosts");
            RenameColumn(table: "dbo.FinishedCoursesReports", name: "CheckUpReport_Id", newName: "ReportId");
            RenameColumn(table: "dbo.FinishedCoursesReports", name: "TrainingCourse_Id", newName: "CourseId");
            RenameColumn(table: "dbo.WishedCoursesReports", name: "CheckUpReport_Id", newName: "ReportId");
            RenameColumn(table: "dbo.WishedCoursesReports", name: "TrainingCourse_Id", newName: "CourseId");
            RenameColumn(table: "dbo.CoursesEnrolledEmployees", name: "Employee_Id", newName: "EmployeeId");
            RenameColumn(table: "dbo.CoursesEnrolledEmployees", name: "TrainingCourse_Id", newName: "CourseId");
            RenameColumn(table: "dbo.TraninedSkillsCourses", name: "Skill_Id", newName: "SkillId");
            RenameColumn(table: "dbo.TraninedSkillsCourses", name: "TrainingCourse_Id", newName: "TagId");
            RenameColumn(table: "dbo.SkillsEmployees", name: "Skill_Id", newName: "SkillId");
            RenameColumn(table: "dbo.SkillsEmployees", name: "Employee_Id", newName: "EmployeeId");
            RenameColumn(table: "dbo.RequiredSkillsPosts", name: "Skill_Id", newName: "SkillId");
            RenameColumn(table: "dbo.RequiredSkillsPosts", name: "Post_Id", newName: "PostId");
            RenameIndex(table: "dbo.TraninedSkillsCourses", name: "IX_Skill_Id", newName: "IX_SkillId");
            RenameIndex(table: "dbo.TraninedSkillsCourses", name: "IX_TrainingCourse_Id", newName: "IX_TagId");
            RenameIndex(table: "dbo.SkillsEmployees", name: "IX_Skill_Id", newName: "IX_SkillId");
            RenameIndex(table: "dbo.SkillsEmployees", name: "IX_Employee_Id", newName: "IX_EmployeeId");
            RenameIndex(table: "dbo.RequiredSkillsPosts", name: "IX_Skill_Id", newName: "IX_SkillId");
            RenameIndex(table: "dbo.RequiredSkillsPosts", name: "IX_Post_Id", newName: "IX_PostId");
            RenameIndex(table: "dbo.CoursesEnrolledEmployees", name: "IX_TrainingCourse_Id", newName: "IX_CourseId");
            RenameIndex(table: "dbo.CoursesEnrolledEmployees", name: "IX_Employee_Id", newName: "IX_EmployeeId");
            RenameIndex(table: "dbo.FinishedCoursesReports", name: "IX_CheckUpReport_Id", newName: "IX_ReportId");
            RenameIndex(table: "dbo.FinishedCoursesReports", name: "IX_TrainingCourse_Id", newName: "IX_CourseId");
            RenameIndex(table: "dbo.WishedCoursesReports", name: "IX_CheckUpReport_Id", newName: "IX_ReportId");
            RenameIndex(table: "dbo.WishedCoursesReports", name: "IX_TrainingCourse_Id", newName: "IX_CourseId");
            DropPrimaryKey("dbo.CoursesEnrolledEmployees");
            AddPrimaryKey("dbo.CoursesEnrolledEmployees", new[] { "CourseId", "EmployeeId" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.CoursesEnrolledEmployees");
            AddPrimaryKey("dbo.CoursesEnrolledEmployees", new[] { "Employee_Id", "TrainingCourse_Id" });
            RenameIndex(table: "dbo.WishedCoursesReports", name: "IX_CourseId", newName: "IX_TrainingCourse_Id");
            RenameIndex(table: "dbo.WishedCoursesReports", name: "IX_ReportId", newName: "IX_CheckUpReport_Id");
            RenameIndex(table: "dbo.FinishedCoursesReports", name: "IX_CourseId", newName: "IX_TrainingCourse_Id");
            RenameIndex(table: "dbo.FinishedCoursesReports", name: "IX_ReportId", newName: "IX_CheckUpReport_Id");
            RenameIndex(table: "dbo.CoursesEnrolledEmployees", name: "IX_EmployeeId", newName: "IX_Employee_Id");
            RenameIndex(table: "dbo.CoursesEnrolledEmployees", name: "IX_CourseId", newName: "IX_TrainingCourse_Id");
            RenameIndex(table: "dbo.RequiredSkillsPosts", name: "IX_PostId", newName: "IX_Post_Id");
            RenameIndex(table: "dbo.RequiredSkillsPosts", name: "IX_SkillId", newName: "IX_Skill_Id");
            RenameIndex(table: "dbo.SkillsEmployees", name: "IX_EmployeeId", newName: "IX_Employee_Id");
            RenameIndex(table: "dbo.SkillsEmployees", name: "IX_SkillId", newName: "IX_Skill_Id");
            RenameIndex(table: "dbo.TraninedSkillsCourses", name: "IX_TagId", newName: "IX_TrainingCourse_Id");
            RenameIndex(table: "dbo.TraninedSkillsCourses", name: "IX_SkillId", newName: "IX_Skill_Id");
            RenameColumn(table: "dbo.RequiredSkillsPosts", name: "PostId", newName: "Post_Id");
            RenameColumn(table: "dbo.RequiredSkillsPosts", name: "SkillId", newName: "Skill_Id");
            RenameColumn(table: "dbo.SkillsEmployees", name: "EmployeeId", newName: "Employee_Id");
            RenameColumn(table: "dbo.SkillsEmployees", name: "SkillId", newName: "Skill_Id");
            RenameColumn(table: "dbo.TraninedSkillsCourses", name: "TagId", newName: "TrainingCourse_Id");
            RenameColumn(table: "dbo.TraninedSkillsCourses", name: "SkillId", newName: "Skill_Id");
            RenameColumn(table: "dbo.CoursesEnrolledEmployees", name: "CourseId", newName: "TrainingCourse_Id");
            RenameColumn(table: "dbo.CoursesEnrolledEmployees", name: "EmployeeId", newName: "Employee_Id");
            RenameColumn(table: "dbo.WishedCoursesReports", name: "CourseId", newName: "TrainingCourse_Id");
            RenameColumn(table: "dbo.WishedCoursesReports", name: "ReportId", newName: "CheckUpReport_Id");
            RenameColumn(table: "dbo.FinishedCoursesReports", name: "CourseId", newName: "TrainingCourse_Id");
            RenameColumn(table: "dbo.FinishedCoursesReports", name: "ReportId", newName: "CheckUpReport_Id");
            RenameTable(name: "dbo.RequiredSkillsPosts", newName: "SkillPosts");
            RenameTable(name: "dbo.SkillsEmployees", newName: "SkillEmployees");
            RenameTable(name: "dbo.TraninedSkillsCourses", newName: "SkillTrainingCourses");
            RenameTable(name: "dbo.CoursesEnrolledEmployees", newName: "EmployeeTrainingCourses");
            RenameTable(name: "dbo.WishedCoursesReports", newName: "CheckUpReportTrainingCourse1");
            RenameTable(name: "dbo.FinishedCoursesReports", newName: "CheckUpReportTrainingCourses");
        }
    }
}
