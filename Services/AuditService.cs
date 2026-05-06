using System.Text;

public class AuditService
{
    private readonly AppDbContext _context;
    public AuditService(AppDbContext context)
    {
        _context = context;
    }
    public async Task Log (
        int userId,
        string username,
        string action,
        string entity,
        int? entityId = null,
        string? details = null
    )
    {
        var log = new AuditLog
        {
            UserId = userId,
            Username = username,
            Action = action,
            EntityName = entity,
            EntityId = entityId,
            Details = details,
            CreatedAt = DateTime.UtcNow
        }; 
        _context.Add(log);
        await _context.SaveChangesAsync();
    }

}