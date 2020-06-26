using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SmartViewHealth.Startup))]
namespace SmartViewHealth
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
