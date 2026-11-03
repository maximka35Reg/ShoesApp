namespace ShoesApp.entities
{
    public partial class Order
    {
        public string ClientFio
        {
            get
            {
                return User.first_name + " " + User.middle_name+ " " + User.last_name;
            }
        }


        public decimal Total
        {
            get
            {
                if (Order_Positions == null) return 0;

                decimal sum = 0;
                foreach (var pos in Order_Positions)
                    sum += pos.amount * pos.price_per_unit;

                return sum;
            }
        }
    }
}
