using AuthServer.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseOpenIddict();
});

//OpenIddict
builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<AppDbContext>();
    })
    .AddServer(options =>
    {
        // 允許 auth server 支援 client credentials grant
        options.AllowClientCredentialsFlow();

        options
            // 設定取得 access token 的 endpoint
            .SetTokenEndpointUris("/connect/token")
            // 設定 introspection endpoint
            .SetIntrospectionEndpointUris("/connect/introspect")
            ;

        options
            // 產生開發用的加密金鑰，production 建議用存在本機的 X.509 certificates
            .AddEphemeralEncryptionKey()
            .AddEphemeralSigningKey()
            // 停用 access token 加密，production 不建議使用
            .DisableAccessTokenEncryption()
            ;

        options
            .UseAspNetCore()
            .EnableTokenEndpointPassthrough();
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();