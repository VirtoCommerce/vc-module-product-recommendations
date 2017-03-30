namespace VirtoCommerce.ProductRecommendationsModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsageEventAuditable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UsageEvent", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.UsageEvent", "ModifiedDate", c => c.DateTime());
            AddColumn("dbo.UsageEvent", "CreatedBy", c => c.String(maxLength: 64));
            AddColumn("dbo.UsageEvent", "ModifiedBy", c => c.String(maxLength: 64));
            DropColumn("dbo.UsageEvent", "Created");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UsageEvent", "Created", c => c.DateTime(nullable: false));
            DropColumn("dbo.UsageEvent", "ModifiedBy");
            DropColumn("dbo.UsageEvent", "CreatedBy");
            DropColumn("dbo.UsageEvent", "ModifiedDate");
            DropColumn("dbo.UsageEvent", "CreatedDate");
        }
    }
}
