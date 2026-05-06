using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AuditController: Controller
{
    private readonly AppDbContext _context;

    public AuditController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> AuditLogs()
    {
        if(!User.HasClaim("Permission", "Admin.Access"))
            return Forbid();

        var logs = await _context.AuditLogs
            .OrderByDescending(l => l.CreatedAt)
            .Take(100)
            .ToListAsync(); 

        return View(logs);
    }
    public async Task<IActionResult> ExportAuditLogsCsv()
    {
        if(!User.HasClaim("Permission", "Admin.Access"))
            return Forbid();

        var logs = await _context.AuditLogs
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

        var builder = new System.Text.StringBuilder();


        builder.AppendLine("User,Action,Entity,Details,Date");

        foreach (var log in logs)
        {
            var line = string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                log.Username,
                log.Action,
                log.EntityName,
                log.Details?.Replace("\"", "\"\""),
                log.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            );

            builder.AppendLine(line);
        }

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(builder.ToString());

        return File(bytes, "text/csv", "AuditLogs.csv"); 
    }


}