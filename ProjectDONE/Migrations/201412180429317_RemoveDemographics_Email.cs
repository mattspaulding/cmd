namespace ProjectDONE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveDemographics_Email : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Addresses", "Demographics_ID", "dbo.Demographics");
            DropForeignKey("dbo.Emails", "Demographics_ID", "dbo.Demographics");
            DropForeignKey("dbo.PhoneNumbers", "Demographics_ID", "dbo.Demographics");
            DropForeignKey("dbo.Jobs", "Demographics_ID", "dbo.Demographics");
            DropIndex("dbo.Jobs", new[] { "Demographics_ID" });
            DropIndex("dbo.Addresses", new[] { "Demographics_ID" });
            DropIndex("dbo.Emails", new[] { "Demographics_ID" });
            DropIndex("dbo.PhoneNumbers", new[] { "Demographics_ID" });
            DropColumn("dbo.Jobs", "Demographics_ID");
            DropColumn("dbo.Addresses", "Demographics_ID");
            DropTable("dbo.Demographics");
            DropTable("dbo.Emails");
            DropTable("dbo.PhoneNumbers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PhoneNumbers",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByUserId = c.String(),
                        TransactionID = c.Guid(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        Demographics_ID = c.Long(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Emails",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByUserId = c.String(),
                        TransactionID = c.Guid(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        Demographics_ID = c.Long(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Demographics",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByUserId = c.String(),
                        TransactionID = c.Guid(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Addresses", "Demographics_ID", c => c.Long());
            AddColumn("dbo.Jobs", "Demographics_ID", c => c.Long());
            CreateIndex("dbo.PhoneNumbers", "Demographics_ID");
            CreateIndex("dbo.Emails", "Demographics_ID");
            CreateIndex("dbo.Addresses", "Demographics_ID");
            CreateIndex("dbo.Jobs", "Demographics_ID");
            AddForeignKey("dbo.Jobs", "Demographics_ID", "dbo.Demographics", "ID");
            AddForeignKey("dbo.PhoneNumbers", "Demographics_ID", "dbo.Demographics", "ID");
            AddForeignKey("dbo.Emails", "Demographics_ID", "dbo.Demographics", "ID");
            AddForeignKey("dbo.Addresses", "Demographics_ID", "dbo.Demographics", "ID");
        }
    }
}
