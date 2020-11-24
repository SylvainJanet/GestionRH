namespace MiseEnSituation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialClassesBeforeMohammed : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Adress_Number = c.Int(nullable: false),
                        Adress_Street = c.String(),
                        Adress_City = c.String(),
                        Adress_ZipCode = c.Int(nullable: false),
                        Adress_Country = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TrainingCourses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartingDate = c.DateTime(nullable: false),
                        EndingDate = c.DateTime(nullable: false),
                        DurationInHours = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Skills",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        TrainingCourse_Id = c.Int(),
                        Post_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrainingCourses", t => t.TrainingCourse_Id, cascadeDelete:false)
                .ForeignKey("dbo.Posts", t => t.Post_Id, cascadeDelete: false)
                .Index(t => t.TrainingCourse_Id)
                .Index(t => t.Post_Id);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HiringDate = c.DateTime(nullable: false),
                        ContractType = c.Int(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        WeeklyWorkLoad = c.Double(nullable: false),
                        FileForContract = c.String(),
                        Company_Id = c.Int(),
                        Manage_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.Company_Id, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.Manage_Id, cascadeDelete: false)
                .Index(t => t.Company_Id)
                .Index(t => t.Manage_Id);
            
            CreateTable(
                "dbo.TrainingCourseEmployees",
                c => new
                    {
                        TrainingCourse_Id = c.Int(nullable: false),
                        Employee_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TrainingCourse_Id, t.Employee_Id })
                .ForeignKey("dbo.TrainingCourses", t => t.TrainingCourse_Id, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.Employee_Id, cascadeDelete: false)
                .Index(t => t.TrainingCourse_Id)
                .Index(t => t.Employee_Id);
            
            CreateTable(
                "dbo.SkillEmployees",
                c => new
                    {
                        Skill_Id = c.Int(nullable: false),
                        Employee_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Skill_Id, t.Employee_Id })
                .ForeignKey("dbo.Skills", t => t.Skill_Id, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.Employee_Id, cascadeDelete: false)
                .Index(t => t.Skill_Id)
                .Index(t => t.Employee_Id);
            
            AddColumn("dbo.Users", "ProPhone", c => c.String());
            AddColumn("dbo.Users", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "BirthDate", c => c.DateTime());
            AddColumn("dbo.Users", "PersonalPhone", c => c.String());
            AddColumn("dbo.Users", "IsManager", c => c.Boolean());
            AddColumn("dbo.Users", "PersonalAdress_Number", c => c.Int());
            AddColumn("dbo.Users", "PersonalAdress_Street", c => c.String());
            AddColumn("dbo.Users", "PersonalAdress_City", c => c.String());
            AddColumn("dbo.Users", "PersonalAdress_ZipCode", c => c.Int());
            AddColumn("dbo.Users", "PersonalAdress_Country", c => c.String());
            AddColumn("dbo.Users", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Users", "Company_Id", c => c.Int());
            AddColumn("dbo.Users", "Post_Id", c => c.Int());
            AddColumn("dbo.Users", "Post_Id1", c => c.Int());
            CreateIndex("dbo.Users", "Company_Id");
            CreateIndex("dbo.Users", "Post_Id");
            CreateIndex("dbo.Users", "Post_Id1");
            AddForeignKey("dbo.Users", "Company_Id", "dbo.Companies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Users", "Post_Id", "dbo.Posts", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Users", "Post_Id1", "dbo.Posts", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "Post_Id1", "dbo.Posts");
            DropForeignKey("dbo.Skills", "Post_Id", "dbo.Posts");
            DropForeignKey("dbo.Posts", "Manage_Id", "dbo.Users");
            DropForeignKey("dbo.Users", "Post_Id", "dbo.Posts");
            DropForeignKey("dbo.Posts", "Company_Id", "dbo.Companies");
            DropForeignKey("dbo.Skills", "TrainingCourse_Id", "dbo.TrainingCourses");
            DropForeignKey("dbo.SkillEmployees", "Employee_Id", "dbo.Users");
            DropForeignKey("dbo.SkillEmployees", "Skill_Id", "dbo.Skills");
            DropForeignKey("dbo.TrainingCourseEmployees", "Employee_Id", "dbo.Users");
            DropForeignKey("dbo.TrainingCourseEmployees", "TrainingCourse_Id", "dbo.TrainingCourses");
            DropForeignKey("dbo.Users", "Company_Id", "dbo.Companies");
            DropIndex("dbo.SkillEmployees", new[] { "Employee_Id" });
            DropIndex("dbo.SkillEmployees", new[] { "Skill_Id" });
            DropIndex("dbo.TrainingCourseEmployees", new[] { "Employee_Id" });
            DropIndex("dbo.TrainingCourseEmployees", new[] { "TrainingCourse_Id" });
            DropIndex("dbo.Posts", new[] { "Manage_Id" });
            DropIndex("dbo.Posts", new[] { "Company_Id" });
            DropIndex("dbo.Skills", new[] { "Post_Id" });
            DropIndex("dbo.Skills", new[] { "TrainingCourse_Id" });
            DropIndex("dbo.Users", new[] { "Post_Id1" });
            DropIndex("dbo.Users", new[] { "Post_Id" });
            DropIndex("dbo.Users", new[] { "Company_Id" });
            DropColumn("dbo.Users", "Post_Id1");
            DropColumn("dbo.Users", "Post_Id");
            DropColumn("dbo.Users", "Company_Id");
            DropColumn("dbo.Users", "Discriminator");
            DropColumn("dbo.Users", "PersonalAdress_Country");
            DropColumn("dbo.Users", "PersonalAdress_ZipCode");
            DropColumn("dbo.Users", "PersonalAdress_City");
            DropColumn("dbo.Users", "PersonalAdress_Street");
            DropColumn("dbo.Users", "PersonalAdress_Number");
            DropColumn("dbo.Users", "IsManager");
            DropColumn("dbo.Users", "PersonalPhone");
            DropColumn("dbo.Users", "BirthDate");
            DropColumn("dbo.Users", "Type");
            DropColumn("dbo.Users", "ProPhone");
            DropTable("dbo.SkillEmployees");
            DropTable("dbo.TrainingCourseEmployees");
            DropTable("dbo.Posts");
            DropTable("dbo.Skills");
            DropTable("dbo.TrainingCourses");
            DropTable("dbo.Companies");
        }
    }
}
