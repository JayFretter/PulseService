using PulseService.DatabaseAdapter.Mongo;
using PulseService.Domain.Handlers;
using PulseService.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add application services
builder.Services.AddSecurity(builder.Configuration);
builder.Services.AddMongoService(builder.Configuration);

builder.Services.AddSingleton<IPulseHandler, PulseHandler>();
builder.Services.AddSingleton<IUserHandler, UserHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
