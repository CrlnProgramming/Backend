using Backend.Models;
using Backend.Services;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();
app.UsePathBase("/api");

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.UseRouting();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CORSPolicy", builder => builder.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed((hosts) => true));
});


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
