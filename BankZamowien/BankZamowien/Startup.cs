using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BankZamowien.Startup))]
namespace BankZamowien
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
