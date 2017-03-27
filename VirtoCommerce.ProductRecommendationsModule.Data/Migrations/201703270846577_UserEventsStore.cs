namespace VirtoCommerce.ProductRecommendationsModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserEventsStore : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserEvent", "StoreId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserEvent", "StoreId");
        }
    }
}
