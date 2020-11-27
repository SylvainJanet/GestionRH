namespace MiseEnSituation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CorrectedManyToManyRelationshipsInvolvingSkillAndTrainingCourse : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Skills", "Post_Id", "dbo.Posts");
            DropForeignKey("dbo.TrainingCourses", "CheckUpReport_Id", "dbo.CheckUpReports");
            DropForeignKey("dbo.TrainingCourses", "CheckUpReport_Id1", "dbo.CheckUpReports");
            DropIndex("dbo.TrainingCourses", new[] { "CheckUpReport_Id" });
            DropIndex("dbo.TrainingCourses", new[] { "CheckUpReport_Id1" });
            DropIndex("dbo.Skills", new[] { "Post_Id" });
            CreateTable(
                "dbo.SkillPosts",
                c => new
                    {
                        Skill_Id = c.Int(nullable: false),
                        Post_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Skill_Id, t.Post_Id })
                .ForeignKey("dbo.Skills", t => t.Skill_Id, cascadeDelete: false)
                .ForeignKey("dbo.Posts", t => t.Post_Id, cascadeDelete: false)
                .Index(t => t.Skill_Id)
                .Index(t => t.Post_Id);
            
            CreateTable(
                "dbo.CheckUpReportTrainingCourses",
                c => new
                    {
                        CheckUpReport_Id = c.Int(nullable: false),
                        TrainingCourse_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CheckUpReport_Id, t.TrainingCourse_Id })
                .ForeignKey("dbo.CheckUpReports", t => t.CheckUpReport_Id, cascadeDelete: false)
                .ForeignKey("dbo.TrainingCourses", t => t.TrainingCourse_Id, cascadeDelete: false)
                .Index(t => t.CheckUpReport_Id)
                .Index(t => t.TrainingCourse_Id);
            
            CreateTable(
                "dbo.CheckUpReportTrainingCourse1",
                c => new
                    {
                        CheckUpReport_Id = c.Int(nullable: false),
                        TrainingCourse_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CheckUpReport_Id, t.TrainingCourse_Id })
                .ForeignKey("dbo.CheckUpReports", t => t.CheckUpReport_Id, cascadeDelete: false)
                .ForeignKey("dbo.TrainingCourses", t => t.TrainingCourse_Id, cascadeDelete: false)
                .Index(t => t.CheckUpReport_Id)
                .Index(t => t.TrainingCourse_Id);
            
            DropColumn("dbo.TrainingCourses", "CheckUpReport_Id");
            DropColumn("dbo.TrainingCourses", "CheckUpReport_Id1");
            DropColumn("dbo.Skills", "Post_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Skills", "Post_Id", c => c.Int());
            AddColumn("dbo.TrainingCourses", "CheckUpReport_Id1", c => c.Int());
            AddColumn("dbo.TrainingCourses", "CheckUpReport_Id", c => c.Int());
            DropForeignKey("dbo.CheckUpReportTrainingCourse1", "TrainingCourse_Id", "dbo.TrainingCourses");
            DropForeignKey("dbo.CheckUpReportTrainingCourse1", "CheckUpReport_Id", "dbo.CheckUpReports");
            DropForeignKey("dbo.CheckUpReportTrainingCourses", "TrainingCourse_Id", "dbo.TrainingCourses");
            DropForeignKey("dbo.CheckUpReportTrainingCourses", "CheckUpReport_Id", "dbo.CheckUpReports");
            DropForeignKey("dbo.SkillPosts", "Post_Id", "dbo.Posts");
            DropForeignKey("dbo.SkillPosts", "Skill_Id", "dbo.Skills");
            DropIndex("dbo.CheckUpReportTrainingCourse1", new[] { "TrainingCourse_Id" });
            DropIndex("dbo.CheckUpReportTrainingCourse1", new[] { "CheckUpReport_Id" });
            DropIndex("dbo.CheckUpReportTrainingCourses", new[] { "TrainingCourse_Id" });
            DropIndex("dbo.CheckUpReportTrainingCourses", new[] { "CheckUpReport_Id" });
            DropIndex("dbo.SkillPosts", new[] { "Post_Id" });
            DropIndex("dbo.SkillPosts", new[] { "Skill_Id" });
            DropTable("dbo.CheckUpReportTrainingCourse1");
            DropTable("dbo.CheckUpReportTrainingCourses");
            DropTable("dbo.SkillPosts");
            CreateIndex("dbo.Skills", "Post_Id");
            CreateIndex("dbo.TrainingCourses", "CheckUpReport_Id1");
            CreateIndex("dbo.TrainingCourses", "CheckUpReport_Id");
            AddForeignKey("dbo.TrainingCourses", "CheckUpReport_Id1", "dbo.CheckUpReports", "Id");
            AddForeignKey("dbo.TrainingCourses", "CheckUpReport_Id", "dbo.CheckUpReports", "Id");
            AddForeignKey("dbo.Skills", "Post_Id", "dbo.Posts", "Id");
        }
    }
}
