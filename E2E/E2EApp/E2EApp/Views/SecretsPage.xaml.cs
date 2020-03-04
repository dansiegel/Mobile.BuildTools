using E2EApp.Helpers;
using Xamarin.Forms;

namespace E2EApp.Views
{
    public partial class SecretsPage
    {
        public SecretsPage()
        {
            InitializeComponent();
            sampleString.Text = Secrets.SampleString;
            sampleBool.Text = $"{Secrets.SampleBool}";
            sampleDouble.Text = $"{Secrets.SampleDouble}";
            sampleInt.Text = $"{Secrets.SampleInt}";
            sampleUri.Text = $"{Secrets.SampleUri}";
            sampleUriType.Text = Secrets.SampleUri.GetType().FullName;
        }
    }
}
