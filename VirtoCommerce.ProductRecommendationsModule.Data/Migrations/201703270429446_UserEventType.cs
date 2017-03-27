namespace VirtoCommerce.ProductRecommendationsModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserEventType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserEvent", "EventType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserEvent", "EventType", c => c.String());
        }
    }
}
