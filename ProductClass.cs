using System;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace ShoesApp.entities
{
    public partial class Product
    {

        public BitmapImage ImagePath
        {
            get
            {
                string fileName = string.IsNullOrEmpty(product_image) ? "picture.png" : Path.GetFileName(product_image);

                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);

                if (File.Exists(fullPath))
                    return new BitmapImage(new Uri(fullPath, UriKind.Absolute));

                return null;
            }
        }

        public string Structure
        {
            get
            {
                if (Structures == null || Structures.Count == 0)
                    return "не указан";

                return string.Join(", ", Structures.Select(s =>
                {
                    var mat = s.Material?.name_material ?? "?";
                    var area = s.ShoesArea?.name_area ?? "?";
                    return $"{area} - {mat}";
                }));
            }
        }

        public int CountProducts
        {
            get { return Sizes_Of_Product.Sum(s => s.amount); }
        }


        public bool IsLowCount
        {
            get { return CountProducts <= 3; }
        }

        public string CountDisplay
        {
            get
            {
                int total = Sizes_Of_Product.Sum(s => s.amount);
                return total > 5 ? "много" : "мало";
            }
        }

        public decimal Price
        {
            get
            {
                var end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var start = end.AddMonths(-1);

                bool hasOrders = App.context.Order_Positions.Any(op => op.id_product == this.id && op.Order.date_order >= start && op.Order.date_order < end);

                return hasOrders ? price : price * 0.75m;
            }
        }

        public bool HasDiscount
        {
            get { return Price < price; }
        }

        public string SizeRange
        {
            get
            {
                if (Sizes_Of_Product == null || Sizes_Of_Product.Count == 0) return "-";
                
                var sizes = Sizes_Of_Product.Select(s => s.Size.num_size).OrderBy(n => n).ToList();

                return string.Join("; ", sizes);
            }
        }
    }
}
