namespace Ecommerce.App
{
    public class UserEntity
    {
        //public string? user_id { get; set; }
        public Guid user_id { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }
        public DateTime? date_of_birth { get; set; }
    }
}
