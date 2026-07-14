using Microsoft.EntityFrameworkCore;
using KinetiqueAPI.Data;
using KinetiqueAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowKinetique", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "https://kinetique-vite-react.vercel.app",
            "https://kinetique-vite-react-git-main-sea6580gmailcoms-projects.vercel.app",
            "https://sandbox.payfast.co.za"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Services
builder.Services.AddScoped<EmailService>();

// Add Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowKinetique");
app.UseAuthorization();
app.MapControllers();

app.Run();