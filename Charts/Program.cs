using Charts.Controllers;
using Charts.Hubs;
using Charts.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

// our data source, could be a database
builder.Services.AddSingleton(_ => {
    var buffer = new Buffer<Point>(10);
    // start with something that can grow
    for (var i = 0; i < 7; i++) 
        buffer.AddNewRandomPoint();

    return buffer;
});


// Conversion du JSON en objet de la classe Score
builder.Services.AddHostedService<ChartValueGenerator>();

var app = builder.Build();
app.MapPost("/vote", ExternalParticipationController.Vote);
app.MapPost("/reset", ExternalParticipationController.Reset);
app.MapGet("/vote", ExternalParticipationController.ValidateToken);

app.UseStaticFiles();
app.MapRazorPages();
app.MapHub<ChartHub>(ChartHub.Url);

app.Run();