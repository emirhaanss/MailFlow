using MailFlow.Context;
using MailFlow.Entities;
using MailFlow.Models;
using MailFlow.Services;
using MailFlow.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// appsettings.json içindeki ConnectionStrings -> SqlConnection okunur
// ve MailContext'e veritabanı bağlantısı olarak verilir.
// Böylece bağlantı bilgisi kod içine yazılmaz, güvenli ve profesyonel olur.
builder.Services.AddDbContext<MailContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<MailContext>().AddErrorDescriber<CustomIdentityValidator>();
//Üyelik sistemini aktif eder (login,register,şifre hashleme,rol sistemi vb),benim appuser sınıfımı kullan. Kullanıcıları ramde tutma benim mailcontext veritabanımda tut. Benim yazdığım CustomIdentityValidator sınıfını kullan ve hataları Türkçe ver.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); //Session ekledik (Kullanıcı tarayıcadayken verilerini geçici saklayan bellek)
builder.Services.AddScoped<EmailService>(); //“Projede EmailService diye bir servis var. Bunu ihtiyaç duyan Controller’lara otomatik ver.”
                                            //AddScoped → Her sayfa isteği için 1 tane servis oluştur.
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings")); //Appsettings.json dosyasındaki EmailSettings bölümünü EmailSettings sınıfına bağla. Yani appsettings.json dosyasındaki EmailSettings bölümündeki bilgileri EmailSettings sınıfının özelliklerine atarız.


var app = builder.Build();
app.UseStaticFiles();

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
app.UseSession(); //Session çalıştırdık
app.UseAuthentication(); //Login işlemleri
app.UseAuthorization();  //Yetkilendirme işlemleri

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
