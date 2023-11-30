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
        // ���\ auth server �䴩 client credentials grant
        options.AllowClientCredentialsFlow();

        options
            // �]�w���o access token �� endpoint
            .SetTokenEndpointUris("/connect/token")
            // �]�w introspection endpoint
            .SetIntrospectionEndpointUris("/connect/introspect")
            ;

        options
            // ���Ͷ}�o�Ϊ��[�K���_�Aproduction ��ĳ�Φs�b������ X.509 certificates
            .AddEphemeralEncryptionKey()
            .AddEphemeralSigningKey()
            // ���� access token �[�K�Aproduction ����ĳ�ϥ�
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