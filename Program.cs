using Congratulator.Repositories;
using Congratulator.Services;
using Congratulator.Services.Hosted;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IMongoClient>(s => new MongoClient(builder.Configuration.GetConnectionString("MongoDB")));
builder.Services.AddSingleton<IMongoDatabase>(s => s.GetRequiredService<IMongoClient>().GetDatabase("Congratulator"));
builder.Services.AddSingleton<BirthdayRepository>();
builder.Services.AddHostedService<BotBackgroundService>();
builder.Services.AddSingleton<IBirthdayService, BirthdayService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
} else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});
app.Run();
