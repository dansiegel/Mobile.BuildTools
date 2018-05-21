using Xamarin.Forms;

namespace E2E.Core.Views
{
    public partial class DemoPage : ContentPage
    {
        public DemoPage()
        {
            InitializeComponent();
        }

        public Button TransparentButton => transparentButton;

        public Button PrimaryButton => primaryButton;
    }
}