using System.Collections.Generic;
using FormsDemo.Models;
using Xamarin.Forms;

namespace FormsDemo.Views
{
    public partial class MenuPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;
        public MenuPage()
        {
            InitializeComponent();

            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem { Id = MenuItemType.Browse, Title="Browse" },
                new HomeMenuItem { Id = MenuItemType.About, Title="About" },
                new HomeMenuItem { Id = MenuItemType.Docs, Title = "Docs" }
            };

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                await RootPage.NavigateFromMenu(id);
            };
        }
    }
}
