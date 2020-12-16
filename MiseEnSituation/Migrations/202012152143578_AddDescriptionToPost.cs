namespace MiseEnSituation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDescriptionToPost : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "Description", c => c.String(nullable: false, maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "Description");
        }
    }
}
