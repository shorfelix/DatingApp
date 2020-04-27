namespace DatingApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] passworHash { get; set; }
        public byte[] passwordSalt { get; set; }
    }
}