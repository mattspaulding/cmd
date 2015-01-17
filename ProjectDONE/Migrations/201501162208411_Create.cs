namespace ProjectDONE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bids",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Owner_ID = c.Long(nullable: false),
                        Job_ID = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        Stripe_Transaction_ID = c.Long(nullable: true),
                        CreatedOn = c.DateTime(),
                        CreatedByUserId = c.String(),
                        TransactionID = c.Guid(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Owners", t => t.Owner_ID)
                .ForeignKey("dbo.Jobs", t => t.Job_ID)
                .ForeignKey("dbo.StripeTransactions", t => t.Stripe_Transaction_ID)
                .Index(t => t.Owner_ID)
                .Index(t => t.Job_ID)
                .Index(t => t.Stripe_Transaction_ID);
            
            CreateTable(
                "dbo.Dialogs",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreatedOn = c.DateTime(),
                        CreatedByUserId = c.String(),
                        TransactionID = c.Guid(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        Bid_ID = c.Long(),
                        Job_ID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Bids", t => t.Bid_ID)
                .ForeignKey("dbo.Jobs", t => t.Job_ID)
                .Index(t => t.Bid_ID)
                .Index(t => t.Job_ID);
            
            CreateTable(
                "dbo.Jobs",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Owner_ID = c.Long(nullable: false),
                        Title = c.String(),
                        PublicDescription = c.String(),
                        Latitude = c.Long(nullable: false),
                        Longitude = c.Long(nullable: false),
                        PrivateDescription = c.String(),
                        AcceptedBid_ID = c.Long(),
                        Media_ID = c.Long(nullable: false),
                        AddressNotes = c.String(),
                        Earliest = c.DateTime(),
                        DoneBy = c.DateTime(),
                        MaxPay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedByUserId = c.String(),
                        TransactionID = c.Guid(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        Address_ID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Bids", t => t.AcceptedBid_ID)
                .ForeignKey("dbo.Addresses", t => t.Address_ID)
                .ForeignKey("dbo.Media", t => t.Media_ID)
                .ForeignKey("dbo.Owners", t => t.Owner_ID)
                .Index(t => t.Owner_ID)
                .Index(t => t.AcceptedBid_ID)
                .Index(t => t.Media_ID)
                .Index(t => t.Address_ID);
            
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Line1 = c.String(),
                        Line2 = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Zip = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedByUserId = c.String(),
                        TransactionID = c.Guid(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Media",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        MIME_TYPE = c.String(),
                        URL = c.String(),
                        Title = c.String(),
                        Meta = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedByUserId = c.String(),
                        TransactionID = c.Guid(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        Owner_ID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Owners", t => t.Owner_ID)
                .Index(t => t.Owner_ID);
            
            CreateTable(
                "dbo.Owners",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        IsCorporateEntity = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedByUserId = c.String(),
                        TransactionID = c.Guid(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.StripeTransactions",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Amount = c.Int(),
                        Paid = c.Boolean(),
                        ReciptEmail = c.String(),
                        CustomerId = c.String(),
                        FailureCode = c.String(),
                        FailureMessage = c.String(),
                        RawStripeTransaction = c.String(),
                        MetaData = c.String(),
                        Customer_ID = c.Long(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedByUserId = c.String(),
                        TransactionID = c.Guid(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Owners", t => t.Customer_ID)
                .Index(t => t.Customer_ID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        Owner_ID = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Owners", t => t.Owner_ID)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.Owner_ID);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "Owner_ID", "dbo.Owners");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Bids", "Stripe_Transaction_ID", "dbo.StripeTransactions");
            DropForeignKey("dbo.StripeTransactions", "Customer_ID", "dbo.Owners");
            DropForeignKey("dbo.Bids", "Job_ID", "dbo.Jobs");
            DropForeignKey("dbo.Media", "Owner_ID", "dbo.Owners");
            DropForeignKey("dbo.Jobs", "Owner_ID", "dbo.Owners");
            DropForeignKey("dbo.Bids", "Owner_ID", "dbo.Owners");
            DropForeignKey("dbo.Jobs", "Media_ID", "dbo.Media");
            DropForeignKey("dbo.Dialogs", "Job_ID", "dbo.Jobs");
            DropForeignKey("dbo.Jobs", "Address_ID", "dbo.Addresses");
            DropForeignKey("dbo.Jobs", "AcceptedBid_ID", "dbo.Bids");
            DropForeignKey("dbo.Dialogs", "Bid_ID", "dbo.Bids");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "Owner_ID" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.StripeTransactions", new[] { "Customer_ID" });
            DropIndex("dbo.Media", new[] { "Owner_ID" });
            DropIndex("dbo.Jobs", new[] { "Address_ID" });
            DropIndex("dbo.Jobs", new[] { "Media_ID" });
            DropIndex("dbo.Jobs", new[] { "AcceptedBid_ID" });
            DropIndex("dbo.Jobs", new[] { "Owner_ID" });
            DropIndex("dbo.Dialogs", new[] { "Job_ID" });
            DropIndex("dbo.Dialogs", new[] { "Bid_ID" });
            DropIndex("dbo.Bids", new[] { "Stripe_Transaction_ID" });
            DropIndex("dbo.Bids", new[] { "Job_ID" });
            DropIndex("dbo.Bids", new[] { "Owner_ID" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.StripeTransactions");
            DropTable("dbo.Owners");
            DropTable("dbo.Media");
            DropTable("dbo.Addresses");
            DropTable("dbo.Jobs");
            DropTable("dbo.Dialogs");
            DropTable("dbo.Bids");
        }
    }
}
