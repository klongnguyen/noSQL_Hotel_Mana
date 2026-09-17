using HotelManagement.Data;
using HotelManagement.Repositories;

var builder = WebApplication.CreateBuilder(args);

// [TV3 - KAN-17] Đăng ký Repository cho module Quản lý & Xuất Hóa đơn (STORY-302)
builder.Services.AddScoped<HotelManagement.Repositories.InvoiceRepository>();
// [TV3 - KAN-16] Đăng ký Repository cho module Tra cứu lịch sử đặt phòng (STORY-301)
builder.Services.AddScoped<HotelManagement.Repositories.BookingHistoryRepository>();

// Cấu hình Cassandra Astra DB Settings & Context Singleton
builder.Services.Configure<CassandraSettings>(builder.Configuration.GetSection("Cassandra"));
builder.Services.AddSingleton<ICassandraContext, CassandraContext>();

// Đăng ký Repositories
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
