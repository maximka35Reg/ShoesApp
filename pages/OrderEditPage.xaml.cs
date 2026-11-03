using ShoesApp.entities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ShoesApp.pages
{
    public partial class OrderEditPage : Page
    {
        private int orderId;
        public OrderEditPage(int orderId)
        {
            InitializeComponent();
            App.Current.MainWindow.Title = $"Чудо обувь | Редактирование заказа №{orderId}";
            UserBlock.Text = $"{App.currentUser.first_name} {App.currentUser.middle_name} {App.currentUser.last_name}";

            this.orderId = orderId;
            LoadOrder();
        }

        private void LoadOrder()
        {
            try
            {
                var order = App.context.Orders.FirstOrDefault(o => o.id == orderId);

                if (order != null)
                {
                    DateDp.SelectedDate = order.date_order;
                    LoadPositions();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPositions()
        {
            try
            {
                PositionsLV.ItemsSource = App.context.Order_Positions.Where(p => p.id_order == orderId).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PreviousBtnBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void DeletePosBtn_Click(object sender, RoutedEventArgs e)
        {
            
            if (PositionsLV.SelectedItem == null)
            {
                MessageBox.Show("Выберите позицию заказа", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var pos = (Order_Positions)PositionsLV.SelectedItem;

            if (MessageBox.Show("Вы действительно хотите удалить выбранную позицию заказа?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    var stock = App.context.Sizes_Of_Product.FirstOrDefault(sp => sp.id_product == pos.id_product && sp.id_size == pos.id_size);

                    if (stock != null) stock.amount += pos.amount;

                    var del = App.context.Order_Positions.FirstOrDefault(op => op.id_order == orderId && op.id_product == pos.id_product && op.id_size == pos.id_size);
                    if (del != null) App.context.Order_Positions.Remove(del);

                    App.context.SaveChanges();
                    LoadPositions();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }        
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DateDp.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var order = App.context.Orders.First(o => o.id == orderId);
                order.date_order = DateDp.SelectedDate.Value;
                App.context.SaveChanges();

                MessageBox.Show("Все изменения успешно применены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new OrdersPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrdersPage());
        }
    }
}
