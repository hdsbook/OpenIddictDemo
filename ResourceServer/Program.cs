using OpenIddict.Validation.AspNetCore;
var authServerHost = "https://localhost:7225/";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 加入 Authentication，DefaultScheme 設定為 OpenIddict AuthenticationScheme
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

// 加入 OpenIddict validation
builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        // AuthServer 網址
        options.SetIssuer(authServerHost);

        // 使用前篇設定 introspection 的 clientid/clientsecret
        options.UseIntrospection()
            .SetClientId("resource_server")
            .SetClientSecret("846B62D0-DEF9-4215-A99D-86E6B8DAB342");

        options.UseSystemNetHttp();
        options.UseAspNetCore();
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 加入 Authorization
builder.Services.AddAuthorization();

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
