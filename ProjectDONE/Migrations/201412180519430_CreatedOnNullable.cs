namespace ProjectDONE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatedOnNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Bids", "CreatedOn", c => c.DateTime());
            AlterColumn("dbo.Dialogs", "CreatedOn", c => c.DateTime());
            AlterColumn("dbo.Jobs", "CreatedOn", c => c.DateTime());
            AlterColumn("dbo.Addresses", "CreatedOn", c => c.DateTime());
            AlterColumn("dbo.Media", "CreatedOn", c => c.DateTime());
            AlterColumn("dbo.Owners", "CreatedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Owners", "CreatedOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Media", "CreatedOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Addresses", "CreatedOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Jobs", "CreatedOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Dialogs", "CreatedOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Bids", "CreatedOn", c => c.DateTime(nullable: false));
        }
    }
}
