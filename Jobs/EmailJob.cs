using Quartz;
using Microsoft.Extensions.Logging;

public class EmailJob : IJob
{
    private readonly EmailService _emailService;
    private readonly ILogger<EmailJob> _logger;

    public EmailJob(EmailService emailService, ILogger<EmailJob> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            _logger.LogInformation("📧 Sending scheduled email...");
            _emailService.SendEmail("testmail.com", "Test Scheduled Email", "This is an automated email sent every 5 minutes.");
            _logger.LogInformation("✅ Email sent successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error in EmailJob: {ex.Message}");
        }

        await Task.CompletedTask;
    }
}
