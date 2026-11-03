using System.Collections.Generic;
using System.Windows;

namespace ShoesApp
{
    public partial class App : Application
    {
        public static entities.ShoesShopEntities context { get; set; } = new entities.ShoesShopEntities();
        public static entities.User currentUser { get; set; }

        public class CartItem
        {
            public int ProductId { get; set; }
            public int SizeId { get; set; }
            public decimal SizeNum { get; set; }
            public string ProductName { get; set; }
            public decimal PricePerUnit { get; set; }
            public int Amount { get; set; }

            public decimal Total
            {
                get { return PricePerUnit * Amount; }
            }
        }
        public static List<CartItem> Cart = new List<CartItem>();
    }
}
