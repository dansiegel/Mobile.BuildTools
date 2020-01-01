using Mobile.BuildTools.Configuration;
using Xamarin.Forms;

namespace E2EApp.Views
{
    public partial class AppConfig
    {
        public AppConfig()
        {
            InitializeComponent();
            fooLabel.Text = ConfigurationManager.AppSettings["foo"];
            barLabel.Text = ConfigurationManager.AppSettings["bar"];
        }
    }
}
