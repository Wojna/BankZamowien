namespace BankZamowien.DAL.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PasswordChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "NumberofPasswordResets", c => c.Int());
            DropColumn("dbo.AspNetUsers", "CreationDate");
            DropColumn("dbo.AspNetUsers", "ResetPassword");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "ResetPassword", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "CreationDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.AspNetUsers", "NumberofPasswordResets");
        }
    }
}
