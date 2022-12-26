using BillingAPI;
using BillingAPI.Middlewares;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddBillingClient();
builder.Services.AddAutoMapper(typeof(AppMappingProfile));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

app.UseBillingErrorHandling();

// if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

// ReSharper disable once ClassNeverInstantiated.Global
public class BillingApiProgram
{
}