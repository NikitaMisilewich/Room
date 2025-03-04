namespace ChatRoom.Components.Services
{
    public class UserSessionData

    {
        public string Username { get; set; }
        public string Role { get; set; }

        // Метод для очистки сессии
        public void ClearSession()
        {
            Username = string.Empty;
            Role = string.Empty;
        }
    }
}
