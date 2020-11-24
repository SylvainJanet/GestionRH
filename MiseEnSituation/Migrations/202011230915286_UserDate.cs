namespace MiseEnSituation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "CreationDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "CreationDate");
        }
    }
}
