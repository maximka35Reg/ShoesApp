using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ShoesApp.pages
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
            App.Current.MainWindow.Title = "Чудо обувь | Авторизация";
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = App.context.Users.FirstOrDefault(u => u.login == LoginBox.Text.Trim());

                if (user != null)
                {
                    App.currentUser = user;
                    NavigationService.Navigate(new MainPage());
                }
                else
                    MessageBox.Show("Пользователь с указанным логином не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PreviousBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }
    }
}
