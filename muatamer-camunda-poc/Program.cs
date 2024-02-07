using Microsoft.EntityFrameworkCore;
using muatamer_camunda_poc.Context;
using muatamer_camunda_poc.Init;
using muatamer_camunda_poc.Services.MuatamerProcess;
using muatamer_camunda_poc.Services.PaymentProcess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:dbconnection"]);
},ServiceLifetime.Singleton);
builder.Services.AddDbInitializer();

builder.Services.AddSingleton<IMuatamerProcessService, MuatamerProcessService>();
builder.Services.AddSingleton<IPaymentProcessService, PaymentProcessService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();



app.Run();
