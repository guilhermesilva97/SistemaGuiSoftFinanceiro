using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GuiSoft.Startup))]
namespace GuiSoft
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

        }
    }
}
