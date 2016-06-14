namespace BankZamowien.DAL.BankMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Imie = c.String(nullable: false),
                        Nazwisko = c.String(nullable: false),
                        Telefon = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Inquiries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RefNumber = c.String(),
                        Priority = c.Int(nullable: false),
                        ExpireDate = c.DateTime(nullable: false),
                        CreateInquiryDate = c.DateTime(nullable: false),
                        ClientID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientID, cascadeDelete: true)
                .Index(t => t.ClientID);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PreviousMessage = c.Int(),
                        Content = c.String(),
                        CreateMessageDate = c.DateTime(nullable: false),
                        InquiryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Inquiries", t => t.InquiryID, cascadeDelete: true)
                .Index(t => t.InquiryID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "InquiryID", "dbo.Inquiries");
            DropForeignKey("dbo.Inquiries", "ClientID", "dbo.Clients");
            DropIndex("dbo.Messages", new[] { "InquiryID" });
            DropIndex("dbo.Inquiries", new[] { "ClientID" });
            DropTable("dbo.Messages");
            DropTable("dbo.Inquiries");
            DropTable("dbo.Clients");
        }
    }
}
