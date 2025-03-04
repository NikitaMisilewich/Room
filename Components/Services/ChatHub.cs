using ChatRoom.Components.DB;
using ChatRoom.Components.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

public class ChatHub : Hub
{
    private readonly AppDbContext _context;

    public ChatHub(AppDbContext context)
    {
        _context = context;
    }

    public async Task SendMessage(int senderId, int receiverId, string text, int? fileId)
    {
        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Text = text,
            SentAt = DateTime.UtcNow,
            FileId = fileId
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        await Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", senderId, text, fileId);
    }
}
