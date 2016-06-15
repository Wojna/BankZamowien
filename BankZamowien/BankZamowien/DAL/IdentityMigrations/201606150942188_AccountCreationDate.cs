namespace BankZamowien.DAL.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountCreationDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "CreationDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "CreationDate");
        }
    }
}
