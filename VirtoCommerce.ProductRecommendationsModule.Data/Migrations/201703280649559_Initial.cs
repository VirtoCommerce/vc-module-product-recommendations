namespace VirtoCommerce.ProductRecommendationsModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UsageEvent",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CustomerId = c.String(nullable: false, maxLength: 64),
                        StoreId = c.String(nullable: false, maxLength: 64),
                        ItemId = c.String(nullable: false, maxLength: 64),
                        EventType = c.String(nullable: false, maxLength: 64),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UsageEvent");
        }
    }
}
