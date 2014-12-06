namespace ProjectDONE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddKeys : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Dialogs",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByUserId = c.Long(nullable: false),
                        TransactionID = c.Guid(nullable: false),
                        Bid_ID = c.Long(),
                        Job_ID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Bids", t => t.Bid_ID)
                .ForeignKey("dbo.Jobs", t => t.Job_ID)
                .Index(t => t.Bid_ID)
                .Index(t => t.Job_ID);
            
            CreateTable(
                "dbo.Demographics",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByUserId = c.Long(nullable: false),
                        TransactionID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByUserId = c.Long(nullable: false),
                        TransactionID = c.Guid(nullable: false),
                        Demographics_ID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Demographics", t => t.Demographics_ID)
                .Index(t => t.Demographics_ID);
            
            CreateTable(
                "dbo.Emails",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByUserId = c.Long(nullable: false),
                        TransactionID = c.Guid(nullable: false),
                        Demographics_ID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Demographics", t => t.Demographics_ID)
                .Index(t => t.Demographics_ID);
            
            CreateTable(
                "dbo.PhoneNumbers",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByUserId = c.Long(nullable: false),
                        TransactionID = c.Guid(nullable: false),
                        Demographics_ID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Demographics", t => t.Demographics_ID)
                .Index(t => t.Demographics_ID);
            
            AddColumn("dbo.Bids", "Job_ID", c => c.Long());
            AddColumn("dbo.Bids", "Job_ID1", c => c.Long());
            AddColumn("dbo.Jobs", "AcceptedBid_ID", c => c.Long());
            AddColumn("dbo.Jobs", "Demographics_ID", c => c.Long());
            AddColumn("dbo.Media", "Job_ID", c => c.Long());
            CreateIndex("dbo.Bids", "Job_ID");
            CreateIndex("dbo.Bids", "Job_ID1");
            CreateIndex("dbo.Jobs", "AcceptedBid_ID");
            CreateIndex("dbo.Jobs", "Demographics_ID");
            CreateIndex("dbo.Media", "Job_ID");
            AddForeignKey("dbo.Jobs", "AcceptedBid_ID", "dbo.Bids", "ID");
            AddForeignKey("dbo.Bids", "Job_ID", "dbo.Jobs", "ID");
            AddForeignKey("dbo.Jobs", "Demographics_ID", "dbo.Demographics", "ID");
            AddForeignKey("dbo.Media", "Job_ID", "dbo.Jobs", "ID");
            AddForeignKey("dbo.Bids", "Job_ID1", "dbo.Jobs", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bids", "Job_ID1", "dbo.Jobs");
            DropForeignKey("dbo.Media", "Job_ID", "dbo.Jobs");
            DropForeignKey("dbo.Dialogs", "Job_ID", "dbo.Jobs");
            DropForeignKey("dbo.Jobs", "Demographics_ID", "dbo.Demographics");
            DropForeignKey("dbo.PhoneNumbers", "Demographics_ID", "dbo.Demographics");
            DropForeignKey("dbo.Emails", "Demographics_ID", "dbo.Demographics");
            DropForeignKey("dbo.Addresses", "Demographics_ID", "dbo.Demographics");
            DropForeignKey("dbo.Bids", "Job_ID", "dbo.Jobs");
            DropForeignKey("dbo.Jobs", "AcceptedBid_ID", "dbo.Bids");
            DropForeignKey("dbo.Dialogs", "Bid_ID", "dbo.Bids");
            DropIndex("dbo.Media", new[] { "Job_ID" });
            DropIndex("dbo.PhoneNumbers", new[] { "Demographics_ID" });
            DropIndex("dbo.Emails", new[] { "Demographics_ID" });
            DropIndex("dbo.Addresses", new[] { "Demographics_ID" });
            DropIndex("dbo.Jobs", new[] { "Demographics_ID" });
            DropIndex("dbo.Jobs", new[] { "AcceptedBid_ID" });
            DropIndex("dbo.Dialogs", new[] { "Job_ID" });
            DropIndex("dbo.Dialogs", new[] { "Bid_ID" });
            DropIndex("dbo.Bids", new[] { "Job_ID1" });
            DropIndex("dbo.Bids", new[] { "Job_ID" });
            DropColumn("dbo.Media", "Job_ID");
            DropColumn("dbo.Jobs", "Demographics_ID");
            DropColumn("dbo.Jobs", "AcceptedBid_ID");
            DropColumn("dbo.Bids", "Job_ID1");
            DropColumn("dbo.Bids", "Job_ID");
            DropTable("dbo.PhoneNumbers");
            DropTable("dbo.Emails");
            DropTable("dbo.Addresses");
            DropTable("dbo.Demographics");
            DropTable("dbo.Dialogs");
        }
    }
}
