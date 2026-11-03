using ShoesApp.entities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ShoesApp.pages
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            App.Current.MainWindow.Title = "Чудо обувь | Каталог";

            LoadCombo();
            ApplyRole();
            UpdateCatalog();     
        }


        private void ApplyRole()
        {
            if (App.currentUser == null)
            {
                LoginBtn.Visibility = Visibility.Visible;
                LogoutBtn.Visibility = Visibility.Collapsed;
                OrdersBtn.Visibility = Visibility.Collapsed;
                UserInfo.Visibility = Visibility.Collapsed;
                CartBtn.Visibility = Visibility.Collapsed;
                InstrumentsPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                LoginBtn.Visibility = Visibility.Collapsed;
                LogoutBtn.Visibility = Visibility.Visible;
                OrdersBtn.Visibility = Visibility.Visible;
                UserInfo.Visibility = Visibility.Visible;
                CartBtn.Visibility = Visibility.Visible;
                InstrumentsPanel.Visibility = Visibility.Visible;
                UserBlock.Text = $"{App.currentUser.first_name} {App.currentUser.middle_name} {App.currentUser.last_name}";
            }
        }


        private void UpdateCatalog()
        {
            if (SearchBox == null || FilterCB == null || SortCB == null || CatalogLV == null)
                return;

            try
            {
                var query = App.context.Products.Where(p => p.Sizes_Of_Product.Any(sp => sp.amount > 0)).AsQueryable();

                if (!string.IsNullOrEmpty(SearchBox.Text)) query = query.Where(p => p.product_name.ToLower().Contains(SearchBox.Text.Trim().ToLower()) || p.description.ToLower().Contains(SearchBox.Text.Trim().ToLower()));


                if (FilterCB.SelectedItem is string selected && selected != "Все категории") query = query.Where(p => p.Category1.category_name == selected);

                var list = query.ToList();

                if (SortCB.SelectedIndex == 1) list = list.OrderBy(p => p.Price).ToList();
                if (SortCB.SelectedIndex == 2) list = list.OrderByDescending(p => p.Price).ToList();

                CatalogLV.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCombo()
        {
            try
            {
                var filter = App.context.Categories.Select(c => c.category_name).ToList();
                filter.Insert(0, "Все категории");
                FilterCB.ItemsSource = filter;
                FilterCB.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }

        private void OrdersBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrdersPage());
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            App.currentUser = null;
            ApplyRole();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateCatalog();
        }

        private void FilterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCatalog();
        }

        private void SortCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCatalog();
        }

        private void CatalogLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (App.currentUser == null) return;

            if (CatalogLV.SelectedItem is Product selected)
            {
                NavigationService.Navigate(new ShoePage(selected));
            }
        }

        private void CartBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CartPage());
        }
    }
}
