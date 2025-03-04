namespace ChatRoom.Components.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }  // ID отправителя
        public int ReceiverId { get; set; } // ID получателя
        public string Text { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public int? FileId { get; set; } // Идентификатор файла (если есть)
        public FileAttachment? File { get; set; } // Навигационное свойство

        public User Sender { get; set; } // Навигационное свойство
        public User Receiver { get; set; }
    }

}
