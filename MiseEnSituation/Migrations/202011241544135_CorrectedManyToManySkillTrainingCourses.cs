namespace MiseEnSituation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CorrectedManyToManySkillTrainingCourses : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Skills", "TrainingCourse_Id", "dbo.TrainingCourses");
            DropIndex("dbo.Skills", new[] { "TrainingCourse_Id" });
            CreateTable(
                "dbo.SkillTrainingCourses",
                c => new
                    {
                        Skill_Id = c.Int(nullable: false),
                        TrainingCourse_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Skill_Id, t.TrainingCourse_Id })
                .ForeignKey("dbo.Skills", t => t.Skill_Id, cascadeDelete: true)
                .ForeignKey("dbo.TrainingCourses", t => t.TrainingCourse_Id, cascadeDelete: true)
                .Index(t => t.Skill_Id)
                .Index(t => t.TrainingCourse_Id);
            
            DropColumn("dbo.Skills", "TrainingCourse_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Skills", "TrainingCourse_Id", c => c.Int());
            DropForeignKey("dbo.SkillTrainingCourses", "TrainingCourse_Id", "dbo.TrainingCourses");
            DropForeignKey("dbo.SkillTrainingCourses", "Skill_Id", "dbo.Skills");
            DropIndex("dbo.SkillTrainingCourses", new[] { "TrainingCourse_Id" });
            DropIndex("dbo.SkillTrainingCourses", new[] { "Skill_Id" });
            DropTable("dbo.SkillTrainingCourses");
            CreateIndex("dbo.Skills", "TrainingCourse_Id");
            AddForeignKey("dbo.Skills", "TrainingCourse_Id", "dbo.TrainingCourses", "Id");
        }
    }
}
