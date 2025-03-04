using ChatRoom.Components.DB;
using ChatRoom.Components.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatRoom.Components.Services;

public class MessageService
{
    private readonly AppDbContext _context;

    public MessageService(AppDbContext context)
    {
        _context = context;
    }

    // Отправка сообщения
    public async Task<Message> SendMessage(int senderId, int receiverId, string text, FileAttachment? file = null)
    {
        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Text = text,
            File = file
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    // Получение всех сообщений между пользователями
    public async Task<List<Message>> GetMessagesAsync(int userId1, int userId2)
    {
        return await _context.Messages
            .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                        (m.SenderId == userId2 && m.ReceiverId == userId1))
            .Include(m => m.File)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    // Поиск сообщений по тексту
    public async Task<List<Message>> SearchMessagesAsync(int userId, string keyword)
    {
        return await _context.Messages
            .Where(m => (m.SenderId == userId || m.ReceiverId == userId) && m.Text.Contains(keyword))
            .Include(m => m.File)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();
    }

    // Загрузка файла по Id
    public async Task<FileAttachment?> GetFileAsync(int fileId)
    {
        return await _context.FileAttachments.FindAsync(fileId);
    }

    public async Task<FileAttachment> UploadFileAsync(string fileName, string contentType, byte[] data)
    {
        var file = new FileAttachment
        {
            FileName = fileName,
            ContentType = contentType,
            Data = data
        };

        _context.FileAttachments.Add(file);
        await _context.SaveChangesAsync();
        return file;
    }

}
