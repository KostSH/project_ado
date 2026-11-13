using System;
using System.Linq;
using ADOapp.Data;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
namespace ADOapp.Views;
public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        InitializeDatabase();
        RefreshAllData();
    }
    private TextBox ClientFullNameTextBox => this.FindControl<TextBox>("ClientFullNameTextBox")!;
    private TextBox ClientPhoneTextBox => this.FindControl<TextBox>("ClientPhoneTextBox")!;
    private TextBox ClientFavoriteGenreTextBox => this.FindControl<TextBox>("ClientFavoriteGenreTextBox")!;
    private CheckBox ClientIsStudentCheckBox => this.FindControl<CheckBox>("ClientIsStudentCheckBox")!;
    private ListBox ClientsListBox => this.FindControl<ListBox>("ClientsListBox")!;
    private TextBlock ClientMessageTextBlock => this.FindControl<TextBlock>("ClientMessageTextBlock")!;
    private TextBox GameTitleTextBox => this.FindControl<TextBox>("GameTitleTextBox")!;
    private TextBox GameGenreTextBox => this.FindControl<TextBox>("GameGenreTextBox")!;
    private TextBox GamePlayerCountTextBox => this.FindControl<TextBox>("GamePlayerCountTextBox")!;
    private CheckBox GameIsPartyGameCheckBox => this.FindControl<CheckBox>("GameIsPartyGameCheckBox")!;
    private ListBox GamesListBox => this.FindControl<ListBox>("GamesListBox")!;
    private TextBlock GameMessageTextBlock => this.FindControl<TextBlock>("GameMessageTextBlock")!;
    private ComboBox RentalClientComboBox => this.FindControl<ComboBox>("RentalClientComboBox")!;
    private ComboBox RentalGameComboBox => this.FindControl<ComboBox>("RentalGameComboBox")!;
    private CalendarDatePicker RentalStartDatePicker => this.FindControl<CalendarDatePicker>("RentalStartDatePicker")!;
    private CalendarDatePicker RentalEndDatePicker => this.FindControl<CalendarDatePicker>("RentalEndDatePicker")!;
    private TextBox RentalPriceRubTextBox => this.FindControl<TextBox>("RentalPriceRubTextBox")!;
    private CheckBox RentalIsReturnedCheckBox => this.FindControl<CheckBox>("RentalIsReturnedCheckBox")!;
    private ListBox RentalsListBox => this.FindControl<ListBox>("RentalsListBox")!;
    private TextBlock RentalMessageTextBlock => this.FindControl<TextBlock>("RentalMessageTextBlock")!;
    private void InitializeDatabase()
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();
        DbSeed.Seed(db);
    }
    private void RefreshAllData()
    {
        using var db = new AppDbContext();
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
        ClientsListBox.ItemsSource = clients;
        GamesListBox.ItemsSource = games;
        RentalsListBox.ItemsSource = rentals;
        RentalClientComboBox.ItemsSource = clients;
        RentalGameComboBox.ItemsSource = games;
        ClientsListBox.SelectedItem = null;
        GamesListBox.SelectedItem = null;
        RentalsListBox.SelectedItem = null;
        RentalClientComboBox.SelectedItem = null;
        RentalGameComboBox.SelectedItem = null;
    }
    private void ClearClientInputs()
    {
        ClientFullNameTextBox.Text = "";
        ClientPhoneTextBox.Text = "";
        ClientFavoriteGenreTextBox.Text = "";
        ClientIsStudentCheckBox.IsChecked = false;
    }
    private void ClearGameInputs()
    {
        GameTitleTextBox.Text = "";
        GameGenreTextBox.Text = "";
        GamePlayerCountTextBox.Text = "";
        GameIsPartyGameCheckBox.IsChecked = false;
    }
    private void ClearRentalInputs()
    {
        RentalClientComboBox.SelectedItem = null;
        RentalGameComboBox.SelectedItem = null;
        RentalStartDatePicker.SelectedDate = null;
        RentalEndDatePicker.SelectedDate = null;
        RentalPriceRubTextBox.Text = "";
        RentalIsReturnedCheckBox.IsChecked = false;
    }
    private void ClientsListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var selectedClient = ClientsListBox.SelectedItem as Clients;
        if (selectedClient == null)
        {
            ClearClientInputs();
            return;
        }
        ClientFullNameTextBox.Text = selectedClient.FullName;
        ClientPhoneTextBox.Text = selectedClient.Phone;
        ClientFavoriteGenreTextBox.Text = selectedClient.FavoriteGenre ?? "";
        ClientIsStudentCheckBox.IsChecked = selectedClient.IsStudent;
    }
    private void AddClientButton_Click(object? sender, RoutedEventArgs e)
    {
        ClientMessageTextBlock.Text = "";
        var fullName = ClientFullNameTextBox.Text?.Trim() ?? "";
        var phone = ClientPhoneTextBox.Text?.Trim() ?? "";
        var favoriteGenre = ClientFavoriteGenreTextBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(fullName))
        {
            ClientMessageTextBlock.Text = "Введи имя клиента";
            return;
        }
        if (string.IsNullOrWhiteSpace(phone))
        {
            ClientMessageTextBlock.Text = "Введи телефон";
            return;
        }
        using var db = new AppDbContext();
        var client = new Clients
        {
            FullName = fullName,
            Phone = phone,
            IsStudent = ClientIsStudentCheckBox.IsChecked == true,
            FavoriteGenre = string.IsNullOrWhiteSpace(favoriteGenre) ? null : favoriteGenre
        };
        db.Clients.Add(client);
        try
        {
            db.SaveChanges();
            RefreshAllData();
            ClearClientInputs();
            ClientMessageTextBlock.Text = "Клиент в базе";
        }
        catch (Exception ex)
        {
            ClientMessageTextBlock.Text = $"В сохранении ошибка: {ex.Message}";
        }
    }
    private void SaveClientButton_Click(object? sender, RoutedEventArgs e)
    {
        ClientMessageTextBlock.Text = "";
        var selectedClient = ClientsListBox.SelectedItem as Clients;
        if (selectedClient == null)
        {
            ClientMessageTextBlock.Text = "Сначала нужно выбирать клиента из списка";
            return;
        }
        var fullName = ClientFullNameTextBox.Text?.Trim() ?? "";
        var phone = ClientPhoneTextBox.Text?.Trim() ?? "";
        var favoriteGenre = ClientFavoriteGenreTextBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(fullName))
        {
            ClientMessageTextBlock.Text = "Нужно ввести имя клиента";
            return;
        }
        if (string.IsNullOrWhiteSpace(phone))
        {
            ClientMessageTextBlock.Text = "Нужно ввести телефон";
            return;
        }
        using var db = new AppDbContext();
        var clientFromDb = db.Clients.FirstOrDefault(c => c.Id == selectedClient.Id);
        if (clientFromDb == null)
        {
            ClientMessageTextBlock.Text = "Клиента нет в базе";
            return;
        }
        clientFromDb.FullName = fullName;
        clientFromDb.Phone = phone;
        clientFromDb.IsStudent = ClientIsStudentCheckBox.IsChecked == true;
        clientFromDb.FavoriteGenre = string.IsNullOrWhiteSpace(favoriteGenre) ? null : favoriteGenre;
        try
        {
            db.SaveChanges();
            RefreshAllData();
            ClearClientInputs();
            ClientMessageTextBlock.Text = "Клиент сохранился";
        }
        catch (Exception ex)
        {
            ClientMessageTextBlock.Text = $"В сохранении ошибка: {ex.Message}";
        }
    }
    private void DeleteClientButton_Click(object? sender, RoutedEventArgs e)
    {
        ClientMessageTextBlock.Text = "";
        var selectedClient = ClientsListBox.SelectedItem as Clients;
        if (selectedClient == null)
        {
            ClientMessageTextBlock.Text = "Сначала нужно выбрать клиента для удаления";
            return;
        }
        using var db = new AppDbContext();
        var clientFromDb = db.Clients
            .Include(c => c.Rentals)
            .FirstOrDefault(c => c.Id == selectedClient.Id);
        if (clientFromDb == null)
        {
            ClientMessageTextBlock.Text = "Клиента нет в базе";
            return;
        }
        db.Rentals.RemoveRange(clientFromDb.Rentals);
        db.Clients.Remove(clientFromDb);
        try
        {
            db.SaveChanges();
            RefreshAllData();
            ClearClientInputs();
            ClientMessageTextBlock.Text = "Клиент удален из базы";
        }
        catch (Exception ex)
        {
            ClientMessageTextBlock.Text = $"В сохранении ошибка: {ex.Message}";
        }
    }
    private void AddGameButton_Click(object? sender, RoutedEventArgs e)
    {
        GameMessageTextBlock.Text = "";
        var title = GameTitleTextBox.Text?.Trim() ?? "";
        var genre = GameGenreTextBox.Text?.Trim() ?? "";
        var playerCountText = GamePlayerCountTextBox.Text?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(title))
        {
            GameMessageTextBlock.Text = "Введи название игры";
            return;
        }
        if (string.IsNullOrWhiteSpace(genre))
        {
            GameMessageTextBlock.Text = "Введи жанр";
            return;
        }
        if (!int.TryParse(playerCountText, out var playerCount))
        {
            GameMessageTextBlock.Text = "Количество игроков должно быть написано ЧИСЛОМ";
            return;
        }
        using var db = new AppDbContext();
        var game = new Games
        {
            Title = title,
            Genre = genre,
            PlayerCount = playerCount,
            IsPartyGame = GameIsPartyGameCheckBox.IsChecked == true
        };
        db.Games.Add(game);
        try
        {
            db.SaveChanges();
            RefreshAllData();
            ClearGameInputs();
            GameMessageTextBlock.Text = "Игра вставлена";
        }
        catch (Exception ex)
        {
            GameMessageTextBlock.Text = $"В сохранении ошибка: {ex.Message}";
        }
    }
    private void DeleteGameButton_Click(object? sender, RoutedEventArgs e)
    {
        GameMessageTextBlock.Text = "";
        var selectedGame = GamesListBox.SelectedItem as Games;
        if (selectedGame == null)
        {
            GameMessageTextBlock.Text = "Сначала нужно выбрать игру для удаления";
            return;
        }
        using var db = new AppDbContext();
        var gameFromDb = db.Games
            .Include(g => g.Rentals)
            .FirstOrDefault(g => g.Id == selectedGame.Id);
        if (gameFromDb == null)
        {
            GameMessageTextBlock.Text = "Игры нет в базе";
            return;
        }
        db.Rentals.RemoveRange(gameFromDb.Rentals);
        db.Games.Remove(gameFromDb);
        try
        {
            db.SaveChanges();
            RefreshAllData();
            ClearGameInputs();
            GameMessageTextBlock.Text = "Игра удалена";
        }
        catch (Exception ex)
        {
            GameMessageTextBlock.Text = $"В сохранении ошибка: {ex.Message}";
        }
    }
    private void AddRentalButton_Click(object? sender, RoutedEventArgs e)
    {
        RentalMessageTextBlock.Text = "";
        var selectedClient = RentalClientComboBox.SelectedItem as Clients;
        if (selectedClient == null)
        {
            RentalMessageTextBlock.Text = "Выбери клиента";
            return;
        }
        var selectedGame = RentalGameComboBox.SelectedItem as Games;
        if (selectedGame == null)
        {
            RentalMessageTextBlock.Text = "Выбери игру";
            return;
        }
        var startDateValue = RentalStartDatePicker.SelectedDate;
        if (startDateValue == null)
        {
            RentalMessageTextBlock.Text = "Выбери дату начала";
            return;
        }
        var priceText = RentalPriceRubTextBox.Text?.Trim() ?? "";
        if (!int.TryParse(priceText, out var priceRub))
        {
            RentalMessageTextBlock.Text = "Цена должна быть ЧИСЛОМ";
            return;
        }
        var startDate = startDateValue.Value.Date;
        DateTime? endDate = RentalEndDatePicker.SelectedDate?.Date;
        if (endDate != null && endDate < startDate)
        {
            RentalMessageTextBlock.Text = "Дата ОКОНЧАНИЯ не может быть РАНЬШЕ даты начала";
            return;
        }
        using var db = new AppDbContext();
        var rental = new Rentals
        {
            ClientId = selectedClient.Id,
            GameId = selectedGame.Id,
            StartDate = startDate,
            EndDate = endDate,
            PriceRub = priceRub,
            IsReturned = RentalIsReturnedCheckBox.IsChecked == true
        };
        db.Rentals.Add(rental);
        try
        {
            db.SaveChanges();
            RefreshAllData();
            ClearRentalInputs();
            RentalMessageTextBlock.Text = "Аренда добавлена";
        }
        catch (Exception ex)
        {
            RentalMessageTextBlock.Text = $"В сохранении ошибка: {ex.Message}";
        }
    }
    private void DeleteRentalButton_Click(object? sender, RoutedEventArgs e)
    {
        RentalMessageTextBlock.Text = "";
        var selectedRental = RentalsListBox.SelectedItem as Rentals;
        if (selectedRental == null)
        {
            RentalMessageTextBlock.Text = "Выбери аренду для удаления";
            return;
        }
        using var db = new AppDbContext();
        var rentalFromDb = db.Rentals.FirstOrDefault(r => r.Id == selectedRental.Id);
        if (rentalFromDb == null)
        {
            RentalMessageTextBlock.Text = "Аренды нет в базе";
            return;
        }
        db.Rentals.Remove(rentalFromDb);
        try
        {
            db.SaveChanges();
            RefreshAllData();
            ClearRentalInputs();
            RentalMessageTextBlock.Text = "Аренда удалена";
        }
        catch (Exception ex)
        {
            RentalMessageTextBlock.Text = $"В сохранении ошибка: {ex.Message}";
        }
    }
}