using System.Collections.Generic;
using Mobile.BuildTools.Configuration;

namespace E2EApp.Views
{
    public partial class AppConfig
    {
        private const string Default = nameof(Default);

        public AppConfig()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            var environments = new List<string> { Default };
            environments.AddRange(ConfigurationManager.Environments);
            environmentsPicker.ItemsSource = environments;
            environmentsPicker.SelectedItem = Default;
            UpdateLabels();
        }

        private void Button_Clicked(object sender, System.EventArgs e)
        {
            var selectedItem = (string)environmentsPicker.SelectedItem;
            if (selectedItem == Default)
            {
                ConfigurationManager.Reset();
            }
            else
            {
                ConfigurationManager.Transform(selectedItem);
            }
            UpdateLabels();
        }

        private void UpdateLabels()
        {
            fooLabel.Text = $"foo: {ConfigurationManager.AppSettings["foo"]}";
            barLabel.Text = $"bar: {ConfigurationManager.AppSettings["bar"]}";
            environmentLabel.Text = $"Environment: {ConfigurationManager.AppSettings["Environment"]}";
            barLabel.Text = $"test: {ConfigurationManager.ConnectionStrings["test"].ConnectionString}";
        }
    }
}
