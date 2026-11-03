using ShoesApp.entities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ShoesApp.pages
{
    public partial class OrdersPage : Page
    {
        public OrdersPage()
        {
            InitializeComponent();
            App.Current.MainWindow.Title = "Чудо обувь | Заказы";
            UserBlock.Text = $"{App.currentUser.first_name} {App.currentUser.middle_name} {App.currentUser.last_name}";
            ApplyRole();
            LoadOrders();
        }

        private void ApplyRole()
        {
            if (App.currentUser.role == 2 || App.currentUser.role == 1)
            {
                AddOrderBtn.Visibility = Visibility.Visible;
                DeleteOrderBtn.Visibility = Visibility.Visible;
            }

            if (App.currentUser.role == 1)
                EditOrderBtn.Visibility = Visibility.Visible;
        }

        private void LoadOrders()
        {
            try
            {
                var query = App.context.Orders.AsQueryable();

                if (App.currentUser.role == 3)
                    query = query.Where(o => o.client == App.currentUser.id);

                var list = query.OrderByDescending(o => o.date_order).ToList();

                OrdersLV.ItemsSource = list;
                PositionsLV.ItemsSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OrdersLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersLV.SelectedItem == null)
            {
                PositionsLV.ItemsSource = null;
                return;
            }

            int orderId = ((Order)OrdersLV.SelectedItem).id;

            try
            {
                PositionsLV.ItemsSource = App.context.Order_Positions.Where(op => op.id_order == orderId).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PreviousBtnBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        private void AddOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        private void EditOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersLV.SelectedItem == null)
            {
                MessageBox.Show("Выберите заказ", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var order = (Order)OrdersLV.SelectedItem;
            NavigationService.Navigate(new OrderEditPage(order.id));
        }

        private void DeleteOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersLV.SelectedItem == null)
            {
                MessageBox.Show("Выберите заказ", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var order = (Order)OrdersLV.SelectedItem;

            if (MessageBox.Show("Вы действительно хотите удалить заказ?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    var positions = App.context.Order_Positions.Where(p => p.id_order == order.id).ToList();

                    foreach (Order_Positions op in positions) {
                        var stock = App.context.Sizes_Of_Product.FirstOrDefault(sp => sp.id_product == op.id_product && sp.id_size == op.id_size);

                        if (stock != null) stock.amount += op.amount;

                        App.context.Order_Positions.Remove(op);
                    }

                    App.context.Orders.Remove(order);
                    App.context.SaveChanges();
                    LoadOrders();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
