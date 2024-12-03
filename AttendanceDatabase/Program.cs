using Microsoft.EntityFrameworkCore;
using AttendanceDatabase.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<AttendanceDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

// Add support for controllers and views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// Map routes
app.MapGet("/", () => Results.Redirect("/mainmenu"));

// Define custom routes for various menus and data entry views
app.MapControllerRoute(
    name: "mainmenu",
    pattern: "mainmenu",
    defaults: new { controller = "Menus", action = "MainMenu" });

app.MapControllerRoute(
    name: "manageaccounts",
    pattern: "manageaccounts",
    defaults: new { controller = "Account", action = "ManageAccounts" });
app.MapControllerRoute(
    name: "create",
    pattern: "create",
    defaults: new { controller = "Account", action = "Create" });


app.MapControllerRoute(
    name: "dataentrymenu",
    pattern: "dataentrymenu",
    defaults: new { controller = "Menus", action = "DataEntryMenu" });

app.MapControllerRoute(
    name: "dailyentry",
    pattern: "dailyentry",
    defaults: new { controller = "DataEntry", action = "DailyEntry" });

app.MapControllerRoute(
    name: "editcalendar",
    pattern: "editcalendar",
    defaults: new { controller = "EditCalendar", action = "EditCalendar" });

app.MapControllerRoute(
    name: "editdata",
    pattern: "editdata/{entryDate:datetime}",
    defaults: new { controller = "DataEntry", action = "EditData" });

app.MapControllerRoute(
    name: "eventcreator",
    pattern: "events/eventcreator",
    defaults: new { controller = "Events", action = "EventCreator" });

// Custom routes for "View Categories" and "Generate Report"
app.MapControllerRoute(
    name: "viewcategories",
    pattern: "form/viewcategories",
    defaults: new { controller = "Menu", action = "RP_ViewCategories" });

app.MapControllerRoute(
    name: "viewpastcategories",
    pattern: "form/viewpastcategories",
    defaults: new { controller = "Menu", action = "RP_ViewPastCategories" });

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
