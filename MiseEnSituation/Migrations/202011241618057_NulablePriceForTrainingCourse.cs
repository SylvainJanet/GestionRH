namespace MiseEnSituation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NulablePriceForTrainingCourse : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TrainingCourses", "Price", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TrainingCourses", "Price", c => c.Double(nullable: false));
        }
    }
}
