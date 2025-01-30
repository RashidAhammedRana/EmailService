using Quartz;
using Quartz.Spi;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigureServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
ConfigureMiddleware(app);

app.Run();

void ConfigureServices(IServiceCollection services)
{
    // Add Controllers
    services.AddControllers();

    // Configure Swagger
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "MailService API", Version = "v1" });
    });

    // Register Email Service
    services.AddScoped<EmailService>();

    // Configure Quartz Scheduler
    services.AddQuartz(q =>
    {
        var jobKey = new JobKey("EmailJob");
        q.AddJob<EmailJob>(opts => opts.WithIdentity(jobKey));
        q.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity("EmailJob-trigger")
            .WithSimpleSchedule(x => x.WithIntervalInMinutes(5).RepeatForever()));
    });

    services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

    // Register Email Job
    services.AddTransient<EmailJob>();
}

void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
}
