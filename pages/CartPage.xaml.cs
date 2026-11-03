using ShoesApp.entities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ShoesApp.pages
{
    public partial class CartPage : Page
    {
        public CartPage()
        {
            InitializeComponent();
            App.Current.MainWindow.Title = "Чудо обувь | Корзина";
            UserBlock.Text = $"{App.currentUser.first_name} {App.currentUser.middle_name} {App.currentUser.last_name}";

            ApplyRole();
            LoadCart();
        }

        private void ApplyRole()
        {
            try
            {
                if (App.currentUser.role == 1 || App.currentUser.role == 2)
                {
                    ClientPanel.Visibility = Visibility.Visible;
                    ClientCB.ItemsSource = App.context.Users.Where(u => u.role == 3).ToList();
                    ClientCB.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCart()
        {
            CartLV.ItemsSource = null;
            CartLV.ItemsSource = App.Cart;
        }

        private void CreateOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.Cart.Count < 1)
            {
                MessageBox.Show("Корзина пуста. Добавьте товары, чтобы оформить заказ", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int clientId = App.currentUser.id;
            if (App.currentUser.role == 1 || App.currentUser.role == 2)
            {
                if (ClientCB.SelectedItem == null)
                {
                    MessageBox.Show("Выберите клиента", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                clientId = (int)ClientCB.SelectedValue;
            }

            if (MessageBox.Show("Вы действительно хотите оформить заказ?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    var order = new Order
                    {
                        client = clientId,
                        date_order = DateTime.Now
                    };

                    App.context.Orders.Add(order);
                    App.context.SaveChanges();

                    foreach (var item in App.Cart)
                    {
                        App.context.Order_Positions.Add(new Order_Positions
                        {
                            id_order = order.id,
                            id_product = item.ProductId,
                            id_size = item.SizeId,
                            amount = item.Amount,
                            price_per_unit = item.PricePerUnit,
                        });

                        var stock = App.context.Sizes_Of_Product.FirstOrDefault(sp => sp.id_product == item.ProductId && sp.id_size == item.SizeId);

                        if (stock != null) stock.amount -= item.Amount;
                    }

                    App.context.SaveChanges();
                    App.Cart.Clear();

                    MessageBox.Show("Заказ оформлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.Navigate(new OrdersPage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }  
        }

        private void CleanCartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.Cart.Count < 1) return;

            if (MessageBox.Show("Вы действительно хотите очистить корзину?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                App.Cart.Clear();
                LoadCart();
            }
        }

        private void CatalogLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CartLV.SelectedItem is App.CartItem item)
            {
                if (MessageBox.Show($"Вы действительно хотите удалить выбранный товар из корзины?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    App.Cart.Remove(item);
                    LoadCart();
                }
            }
        }

        private void PreviousBtnBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }
    }
}
