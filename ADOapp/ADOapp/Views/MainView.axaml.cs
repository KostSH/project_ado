using System.Linq;
using ADOapp.Data;
using Avalonia.Controls;
using Microsoft.EntityFrameworkCore;
namespace ADOapp.Views;
public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        using var db = new AppDbContext();
        db.Database.EnsureCreated();
        DbSeed.Seed(db);
        var clients = db.Clients
            .OrderBy(c => c.FullName)
            .ToList();
        var games = db.Games
            .OrderBy(g => g.Title)
            .ToList();
        var rentals = db.Rentals
            .Include(r => r.Client)
            .Include(r => r.Game)
            .OrderBy(r => r.StartDate)
            .ToList();
        var clientsListBox = this.FindControl<ListBox>("ClientsListBox");
        var gamesListBox = this.FindControl<ListBox>("GamesListBox");
        var rentalsListBox = this.FindControl<ListBox>("RentalsListBox");
        if (clientsListBox != null)
        {
            clientsListBox.ItemsSource = clients;
        }
        if (gamesListBox != null)
        {
            gamesListBox.ItemsSource = games;
        }
        if (rentalsListBox != null)
        {
            rentalsListBox.ItemsSource = rentals;
        }
    }
}