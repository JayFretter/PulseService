using System.Globalization;
using Microsoft.AspNetCore.Localization;
using PulseService.DatabaseAdapter.Mongo;
using PulseService.Domain.Handlers;
using PulseService.Domain.Validation;
using PulseService.Security;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicyName = "PulseCORSPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsPolicyName,
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

// Add localisation
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Add application services
builder.Services.AddSecurity(builder.Configuration);
builder.Services.AddMongoService(builder.Configuration);

builder.Services.AddSingleton<IPulseHandler, PulseHandler>();
builder.Services.AddSingleton<IUserHandler, UserHandler>();
builder.Services.AddSingleton<IDiscussionHandler, DiscussionHandler>();
builder.Services.AddSingleton<IProfileHandler, ProfileHandler>();
builder.Services.AddSingleton<IUserValidationService, UserValidationService>();
builder.Services.AddSingleton<IPulseValidationService, PulseValidationService>();
builder.Services.AddSingleton<IArgumentValidationService, ArgumentValidationService>();

var app = builder.Build();

// Configure localisation
var supportedCultures = new[] { "en" };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList(),
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(CorsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
