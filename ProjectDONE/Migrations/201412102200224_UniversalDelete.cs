namespace ProjectDONE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniversalDelete : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bids", "Deleted", c => c.Boolean(nullable: false, defaultValueSql: "0"));
            AddColumn("dbo.Dialogs", "Deleted", c => c.Boolean(nullable: false, defaultValueSql: "0"));
            AddColumn("dbo.Jobs", "Deleted", c => c.Boolean(nullable: false, defaultValueSql: "0"));
            AddColumn("dbo.Demographics", "Deleted", c => c.Boolean(nullable: false, defaultValueSql: "0"));
            AddColumn("dbo.Addresses", "Deleted", c => c.Boolean(nullable: false, defaultValueSql: "0"));
            AddColumn("dbo.Emails", "Deleted", c => c.Boolean(nullable: false, defaultValueSql: "0"));
            AddColumn("dbo.PhoneNumbers", "Deleted", c => c.Boolean(nullable: false, defaultValueSql: "0"));
            AddColumn("dbo.Media", "Deleted", c => c.Boolean(nullable: false, defaultValueSql: "0"));
            AddColumn("dbo.Owners", "Deleted", c => c.Boolean(nullable: false, defaultValueSql: "0"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Owners", "Deleted");
            DropColumn("dbo.Media", "Deleted");
            DropColumn("dbo.PhoneNumbers", "Deleted");
            DropColumn("dbo.Emails", "Deleted");
            DropColumn("dbo.Addresses", "Deleted");
            DropColumn("dbo.Demographics", "Deleted");
            DropColumn("dbo.Jobs", "Deleted");
            DropColumn("dbo.Dialogs", "Deleted");
            DropColumn("dbo.Bids", "Deleted");
        }
    }
}
