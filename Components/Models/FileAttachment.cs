namespace ChatRoom.Components.Models
{
    public class FileAttachment
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty; // Тип файла (image/png, application/pdf и т. д.)
        public byte[] Data { get; set; } = Array.Empty<byte>(); // Содержимое файла
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
