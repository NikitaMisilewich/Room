namespace ChatRoom.Components.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Fio { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
    }
}
