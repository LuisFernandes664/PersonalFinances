using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PersonalFinances.BLL.Entities.Models.Notification.Senders;
using PersonalFinances.BLL.Interfaces.Analytics;
using PersonalFinances.BLL.Interfaces.Calendar;
using PersonalFinances.BLL.Interfaces.Currency;
using PersonalFinances.BLL.Interfaces.Notification.Senders;
using PersonalFinances.BLL.Interfaces.Repository;
using PersonalFinances.BLL.Interfaces.SavingPlan.Budget;
using PersonalFinances.BLL.Interfaces.SavingPlan.Goal;
using PersonalFinances.BLL.Interfaces.Transaction;
using PersonalFinances.BLL.Interfaces.User;
using PersonalFinances.BLL.Interfaces.Utilities;
using PersonalFinances.BLL.Services.Analytics;
using PersonalFinances.BLL.Services.SavingPlan.Budget;
using PersonalFinances.BLL.Services.SavingPlan.Goal;
using PersonalFinances.BLL.Services.Transaction;
using PersonalFinances.DAL;
using PersonalFinances.DAL.Calendar;
using PersonalFinances.DAL.Currency;
using PersonalFinances.DAL.Helpers;
using PersonalFinances.DAL.SavingPlan.Budget;
using PersonalFinances.DAL.SavingPlan.Goal;
using PersonalFinances.DAL.Transaction;
using PersonalFinances.DAL.User;
using PersonalFinances.DAL.Utilities;
using System.Text;

//var options = new WebApplicationOptions
//{
//    WebRootPath = "wwwroot"
//};

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

builder.Services.AddScoped<IRecurringTransactionRepository, RecurringTransactionRepository>();

builder.Services.AddScoped<IRecurringTransactionService, RecurringTransactionService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IFinancialHealthService, FinancialHealthService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IDataImportExportService, DataImportExportService>();
builder.Services.AddScoped<ICalendarService, CalendarService>();

// Adicionar serviços de background para processamento
builder.Services.AddHostedService<RecurringTransactionProcessor>();
builder.Services.AddHostedService<ExchangeRateUpdateService>();


// Adicionar memory cache para taxas de câmbio
builder.Services.AddMemoryCache();

// Adicionar HTTP client factory para chamadas de API externas
builder.Services.AddHttpClient();

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
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
