using ADOapp.Data;
using Microsoft.EntityFrameworkCore;
Console.WriteLine($"Путь к базе: {AppDbContext.DbPath}");
using var db = new AppDbContext();
db.Database.EnsureCreated();
DbSeed.Seed(db);
Console.WriteLine();
Console.WriteLine("Clients:");
foreach (var client in db.Clients)
{
    Console.WriteLine(client);
}
Console.WriteLine();
Console.WriteLine("Games:");
foreach (var game in db.Games)
{
    Console.WriteLine(game);
}
Console.WriteLine();
Console.WriteLine("Rentals:");
var rentals = db.Rentals
    .Include(r => r.Client)
    .Include(r => r.Game);

foreach (var rental in rentals)
{
    Console.WriteLine(rental);
}