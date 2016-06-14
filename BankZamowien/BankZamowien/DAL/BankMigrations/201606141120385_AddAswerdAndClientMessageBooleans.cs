namespace BankZamowien.DAL.BankMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAswerdAndClientMessageBooleans : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inquiries", "IsAnswered", c => c.Boolean(nullable: false));
            AddColumn("dbo.Messages", "IsClientMessage", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "IsClientMessage");
            DropColumn("dbo.Inquiries", "IsAnswered");
        }
    }
}
