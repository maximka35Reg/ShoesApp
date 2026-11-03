using ShoesApp.entities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ShoesApp.pages
{
    public partial class ShoePage : Page
    {
        private Product product;

        public ShoePage(Product prod)
        {
            InitializeComponent();
            product = prod;
            DataContext = product;

            App.Current.MainWindow.Title = $"Чудо обувь | {prod.product_name}";

            UserBlock.Text = $"{App.currentUser.first_name} {App.currentUser.middle_name} {App.currentUser.last_name}";
            LoadCombo();
        }

        private void PreviousBtnBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        private void LoadCombo()
        {
            try
            {
                var available = App.context.Sizes_Of_Product.Where(sp => sp.id_product == product.id && sp.amount > 0).ToList();
                SizesCB.ItemsSource = available;

                if (available.Count > 0)
                {
                    SizesCB.SelectedIndex = 0;
                    UpdateAmount();
                }
                else AmountBlock.Text = "0";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateAmount()
        {
            if (SizesCB.SelectedItem is Sizes_Of_Product pos)
            {
                var inCart = App.Cart.FirstOrDefault(c => c.ProductId == pos.id_product && c.SizeId == pos.id_size);
                int cartCount = (inCart != null) ? inCart.Amount : 0;

                int amount = pos.amount - cartCount;
                AmountBlock.Text = amount.ToString();
            }
            else
            {
                AmountBlock.Text = "0";
            }
        }



        private void AddToOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SizesCB.SelectedItem == null)
            {
                MessageBox.Show("Выберите размер", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var pos = (Sizes_Of_Product)SizesCB.SelectedItem;
            if (!int.TryParse(CountOrderShoesBox.Text, out int count) || count < 1)
            {
                MessageBox.Show("Введите корретное количество", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var exist = App.Cart.FirstOrDefault(c => c.ProductId == pos.id_product && c.SizeId == pos.id_size);
            int countInCart = exist?.Amount ?? 0;

            if (count + countInCart > pos.amount)
            {
                MessageBox.Show($"Доступное количество товара для заказа {pos.amount} шт.\nВведите корректное количество", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (exist != null) exist.Amount += count;
            else
            {
                App.Cart.Add(new App.CartItem
                {
                    ProductId = pos.id_product,
                    SizeId = pos.id_size,
                    SizeNum = (decimal)pos.Size.num_size,
                    ProductName = product.product_name,
                    PricePerUnit = product.Price,
                    Amount = count
                });
            }

            MessageBox.Show("Товар добавлен в корзину", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            UpdateAmount();
            CountOrderShoesBox.Text = "1";
        }

        private void SizesCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAmount();
        }
    }
}
