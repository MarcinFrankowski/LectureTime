using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Lecture_Time.Startup))]
namespace Lecture_Time
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
