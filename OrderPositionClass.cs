namespace ShoesApp.entities
{
    public partial class Order_Positions
    {
        public string ProductName
        {
            get
            {
                return Sizes_Of_Product.Product.product_name;
            }
        }


        public string ProductionName
        {
            get
            {
                return Sizes_Of_Product.Product.Production1.name_production;
            }
        }

 
        public decimal SizeNum
        {
            get
            {
                return Sizes_Of_Product.Size.num_size;
            }
        }

        public decimal LineTotal
        {
            get
            {
                return (amount * price_per_unit);
            }
        }
    }
}
