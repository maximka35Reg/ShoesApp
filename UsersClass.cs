namespace ShoesApp.entities
{
    public partial class User
    {
        public string UserFio
        {
            get
            {
                return first_name + " " + middle_name + " " + last_name;
            }
        }
    }
}
