namespace BankZamowien.DAL.BankMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClientAnswered : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "IsNonAnsweredInquiry", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "IsNonAnsweredInquiry");
        }
    }
}
