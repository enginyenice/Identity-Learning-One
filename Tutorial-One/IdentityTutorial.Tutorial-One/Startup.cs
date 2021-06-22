using IdentityTutorial.Tutorial_One.CustomValidation;
using IdentityTutorial.Tutorial_One.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityTutorial.Tutorial_One
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {



            CookieBuilder cookieBuilder = new CookieBuilder();
            cookieBuilder.Name = "MyBlog"; //Cookie ismi
            cookieBuilder.HttpOnly = false; //Client side tarafında erişilemesin
            cookieBuilder.Expiration = System.TimeSpan.FromDays(60); //Kaç gün kalmasını istiyoruz.
            cookieBuilder.SameSite = SameSiteMode.Lax; // Sadece o site üzerinden ulaşabilirim. (Strict kapatmış olurum) (Lax ayarı kısmış olurum) (None kapatmış olurum) (CSRF ataklarını engellemek için Strict yapılabilir.) Kritik bilgiler taşıyorsan Strict yapabilirsin. (Para transferi vs...)
            cookieBuilder.SecurePolicy = CookieSecurePolicy.SameAsRequest; //Kullanıcı login olduğu zaman cookie oluşurken bu cookienin https üzerinden gönderiliyor.
                                                                           //| Always : eğer browser'a istek sadece https üzerinden gelmişse cookie değerini gönderiyor
                                                                           //| SameAsRequest: Http den gelmişse httpden https den gelmişse https den cookie bilgisini gönderiyor.
                                                                           //| None nerden gösterirseniz gönderin http üzerinden cookie gönderiyor.


            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/Home/Login"); //Kullanıcıların göreceği bir sayfaya cookiesiz bir istek geldiğinde nereye yönlendirileceğini belirtiyoruz.
                options.Cookie = cookieBuilder; // Tasarladığımız cookieBuilder'i verdik
                options.SlidingExpiration = true;//Kullanıcıya verdiğimiz cookie nin ömrünü vermiştik.
                                                 //|SlidingExpiration = true kullanıcı cookieBuilder.Expiration tarihinin yarısa geldiğinde tekrar Expiration kadar gün cookie oluşturma isteği verir.
                                                 //|-->Örneğin: 60 / 2 = 30. gün siteye girdiğinde 60 günlük daha cookie oluşturur.
                                                 //|SlidingExpiration = false cookie ömrü otomatik uzatılmaz.
                

            });

            services.AddControllersWithViews();
            services.AddDbContext<AppIdentityDbContext>(options =>
            {

                options.UseSqlServer(Configuration.GetConnectionString("SqlServer"));
            });
            services.AddIdentity<AppUser, AppRole>(options =>
            {

                //User Default Validaton
                options.User.RequireUniqueEmail = true; //Uniq email olsun
                options.User.AllowedUserNameCharacters = "abcçdefgğhıijklmnoöpqrstuüvwxyzABCÇDEFGĞHIİJKLMNOÖPQRSŞTUÜVWXYZ0123456789-._"; //Kullanıcı adında hangi karakterler girilebilir. Defualt: abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+


                //Password Default Validaton
                options.Password.RequiredLength = 4; // En az 4 karakter olmalı
                options.Password.RequireNonAlphanumeric = false; // Alfanumeric karakter girmesine gerek yok (*.?-....)
                options.Password.RequireLowercase = false; // Küçük karakter girme zorunluluğuna gerek yok
                options.Password.RequireUppercase = false; // Büyük karakter girme zorunluluğuna gerek yok
                options.Password.RequireDigit = false;  // Sayı girme zorunluluğuna gerek yok

            })
                .AddPasswordValidator<CustomPasswordValidator>()//Custom password validator ekledik
                .AddUserValidator<CustomUserValidator>()//Custom user validator ekledik.
                .AddErrorDescriber<CustomIdentityErrorDescriber>() //Hata mesajlarını özelleştirmek.
                .AddEntityFrameworkStores<AppIdentityDbContext>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
