using FreeSql;
using Hangfire;
using Hangfire.Storage.SQLite;
using RequestProcessor;
using WebDashboard.Attributes;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddLogging();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddScoped<AuthMiddleware>();

builder.Services.AddHangfire(configuration => configuration.UseSQLiteStorage());
builder.Services.AddHangfireServer();
builder.Services.AddScoped<IFreeSql>(e =>
{
    var bd = new FreeSqlBuilder();
    bd.UseConnectionString(DataType.Sqlite, "data source=storage.db");
    var freeSql = bd.Build();

    freeSql.Ado.ExecuteScalar("""
                CREATE TABLE If Not Exists "JobStore" (
                  "Id" integer NOT NULL PRIMARY KEY AUTOINCREMENT,
                  "JobId" varchar(36) NOT NULL,
                  "JobDetailId" integer NOT NULL,
                  "Remark" varchar(100),
                  "Cron" varchar(100) not NULL,
                  "Name" varchar(100) not NULL,
                    "Status" integer NOT NULL
                );

                CREATE TABLE If Not Exists "JobDetail" (
                  "Id" integer NOT NULL PRIMARY KEY AUTOINCREMENT,
                  "HttpRequestHeader" varchar(1000),
                  "HttpRequestBody" varchar(1000),
                  "Url" varchar(500) not NULL,
                  "Method" varchar(10) not NULL,
                  "Name" varchar(100) not NULL,
                  "StepJson" text
                );

                CREATE TABLE If Not Exists "JobResult" (
                  "Id" integer NOT NULL PRIMARY KEY AUTOINCREMENT,
                  "JobDetailId" integer Not NULL,
                  "CreationTime" datetime,
                  "Result" text
                );
        """);

    return freeSql;
});
builder.Services.AddScoped<JobDataLogic>();


builder.Services.AddRazorPages();
builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
}



app.UseStaticFiles();

app.UseRouting();

app.UseSession();
// app.UseMiddleware<AuthMiddleware>();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");
app.MapHangfireDashboard();

app.MapFallbackToFile("index.html");

app.Run();
