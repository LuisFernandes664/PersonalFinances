using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PersonalFinances.BLL.Entities.Models.Notification.Senders;
using PersonalFinances.BLL.Interfaces.Notification.Senders;
using PersonalFinances.BLL.Interfaces.Repository;
using PersonalFinances.BLL.Interfaces.SavingPlan.Budget;
using PersonalFinances.BLL.Interfaces.SavingPlan.Goal;
using PersonalFinances.BLL.Interfaces.Transaction;
using PersonalFinances.BLL.Interfaces.User;
using PersonalFinances.BLL.Services.SavingPlan.Budget;
using PersonalFinances.BLL.Services.SavingPlan.Goal;
using PersonalFinances.DAL;
using PersonalFinances.DAL.Helpers;
using PersonalFinances.DAL.SavingPlan.Budget;
using PersonalFinances.DAL.SavingPlan.Goal;
using PersonalFinances.DAL.Transaction;
using PersonalFinances.DAL.User;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Logging para desenvolvimento
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<ISmsSender, SmsSender>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IGoalRepository, GoalRepository>();
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
builder.Services.AddScoped<IBudgetService, BudgetService>();

builder.Services.AddScoped<DatabaseContext>();

await DatabaseHelper.CreateTablesAsync();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull);

var key = Encoding.UTF8.GetBytes(CommonStrings.SecretKey);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Bearer", policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
    });
});

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets(); // Verifica se este método não está a gerar exceção

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("AllowAll");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// **Atenção:** Adiciona o middleware de autenticação antes da autorização.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
