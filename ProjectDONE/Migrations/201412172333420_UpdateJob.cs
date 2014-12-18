namespace ProjectDONE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateJob : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "AddressNotes", c => c.String(nullable:true));
            AddColumn("dbo.Jobs", "Earliest", c => c.DateTime(nullable:true));
            AddColumn("dbo.Jobs", "DoneBy", c => c.DateTime(nullable:true));
            AddColumn("dbo.Jobs", "MaxPay", c => c.Decimal(nullable: true, precision: 18, scale: 2));
            AddColumn("dbo.Jobs", "Address_ID", c => c.Long());
            AddColumn("dbo.Addresses", "Name", c => c.String());
            AddColumn("dbo.Addresses", "Line1", c => c.String());
            AddColumn("dbo.Addresses", "Line2", c => c.String());
            AddColumn("dbo.Addresses", "City", c => c.String());
            AddColumn("dbo.Addresses", "State", c => c.String());
            AddColumn("dbo.Addresses", "Zip", c => c.String());
            AddForeignKey("dbo.Jobs", "Address_ID", "dbo.Addresses", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Jobs", "Address_ID", "dbo.Addresses");
            DropIndex("dbo.Jobs", new[] { "Address_ID" });
            DropColumn("dbo.Addresses", "Zip");
            DropColumn("dbo.Addresses", "State");
            DropColumn("dbo.Addresses", "City");
            DropColumn("dbo.Addresses", "Line2");
            DropColumn("dbo.Addresses", "Line1");
            DropColumn("dbo.Addresses", "Name");
            DropColumn("dbo.Jobs", "Address_ID");
            DropColumn("dbo.Jobs", "MaxPay");
            DropColumn("dbo.Jobs", "DoneBy");
            DropColumn("dbo.Jobs", "Earliest");
            DropColumn("dbo.Jobs", "AddressNotes");
        }
    }
}
