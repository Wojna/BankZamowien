// <auto-generated />
namespace BankZamowien.DAL.IdentityMigrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.1.3-40302")]
    public sealed partial class AccountCreationDate : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(AccountCreationDate));
        
        string IMigrationMetadata.Id
        {
            get { return "201606150942188_AccountCreationDate"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}