using Microsoft.EntityFrameworkCore;
using Mirchi.Services.Email.DBContexts;
using Mirchi.Services.Email.Extensions;
using Mirchi.Services.Email.Messaging;
using Mirchi.Services.Email.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDBContext>(dbContextOptions =>
dbContextOptions.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var optionBuilder = new DbContextOptionsBuilder<ApplicationDBContext>();
optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddSingleton<IEmailRepository>(new EmailRepository(optionBuilder.Options));
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseAzureServiceBusConsumer();
app.Run();
