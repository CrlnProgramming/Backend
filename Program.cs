using Backend.Models;
using Backend.Services;

var builder = WebApplication.CreateBuilder(args);
//builder.WebHost.UseUrls();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CORSPolicy", builder => builder
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials().
    SetIsOriginAllowed((hosts) => true));
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UsePathBase("/api");

app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthorization();

app.MapControllers();

app.UseCors("CORSPolicy");

app.MapPost("/email", (EmailDto email, IEmailService emailService, ILogger<EmailService> logger) =>
{
    logger.LogInformation("Received request to send email.");
    try
    {
        emailService.SendEmailAsync(email);
        logger.LogInformation("Email sent successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while sending the email.");
        // Возвращаем соответствующий статусный код или сообщение об ошибке
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
    return Results.Ok();
});


app.Run();