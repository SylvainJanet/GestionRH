namespace MiseEnSituation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNameTrainingCourse : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrainingCourses", "Name", c => c.String(nullable: false, maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrainingCourses", "Name");
        }
    }
}
