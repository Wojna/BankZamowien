namespace BankZamowien.DAL.BankMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PhoneNumberFormat : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Clients", "Telefon", c => c.String(maxLength: 12));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Clients", "Telefon", c => c.String());
        }
    }
}
