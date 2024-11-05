using PulseService.DatabaseAdapter.Mongo;
using PulseService.Domain.Handlers;
using PulseService.Security;

var builder = WebApplication.CreateBuilder(args);

const string CORS_POLICY_NAME = "PulseCORSPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CORS_POLICY_NAME,
        policy =>
        {
            policy
                .WithOrigins(builder.Configuration.GetValue<string>("AllowedOrigins")!.Split(','))
                .WithHeaders("Authorization", "Content-Type")
                .AllowAnyMethod();
        });
});


builder.Services.AddControllers().AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add application services
builder.Services.AddSecurity(builder.Configuration);
builder.Services.AddMongoService(builder.Configuration);

builder.Services.AddSingleton<IPulseHandler, PulseHandler>();
builder.Services.AddSingleton<IUserHandler, UserHandler>();
builder.Services.AddSingleton<IDiscussionHandler, DiscussionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(CORS_POLICY_NAME);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
