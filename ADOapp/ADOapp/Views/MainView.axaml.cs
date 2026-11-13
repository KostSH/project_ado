using System;
using System.Linq;
using ADOapp.Data;
using Avalonia.Controls;
using Avalonia.Input;
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
            ClientMessageTextBlock.Text = "Добавлен клиент";
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
            ClientMessageTextBlock.Text = "Выбери клиента в списке";
            return;
        }

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
            ClientMessageTextBlock.Text = "Сохранен клиент";
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
            ClientMessageTextBlock.Text = "Сначала выбери клиента для удаления";
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
            ClientMessageTextBlock.Text = "Удален клиент";
        }
        catch (Exception ex)
        {
            ClientMessageTextBlock.Text = $"В удалении ошибка: {ex.Message}";
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
            GameMessageTextBlock.Text = "Количество игроков должно быть ЧИСЛОМ";
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
            GameMessageTextBlock.Text = "Добавлена игра";
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
            GameMessageTextBlock.Text = "Сначала выбери игру для удаления";
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
            GameMessageTextBlock.Text = $"В удалении ошибка: {ex.Message}";
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
            RentalMessageTextBlock.Text = "Цена только ЧИСЛО";
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
            RentalMessageTextBlock.Text = "Добавлена аренда";
        }
        catch (Exception ex)
        {
            RentalMessageTextBlock.Text = $"В сохранении: {ex.Message}";
        }
    }

    private void DeleteRentalButton_Click(object? sender, RoutedEventArgs e)
    {
        RentalMessageTextBlock.Text = "";

        var selectedRental = RentalsListBox.SelectedItem as Rentals;
        if (selectedRental == null)
        {
            RentalMessageTextBlock.Text = "Сначала выбери аренду, чтобы удалить";
            return;
        }

        using var db = new AppDbContext();

        var rentalFromDb = db.Rentals.FirstOrDefault(r => r.Id == selectedRental.Id);
        if (rentalFromDb == null)
        {
            RentalMessageTextBlock.Text = "Аренды нет в базе.";
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
            RentalMessageTextBlock.Text = $"В удалении ошибка: {ex.Message}";
        }
    }
    private void ClientFullNameTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ClientPhoneTextBox.Focus();
            e.Handled = true;
        }
    }

    private void ClientPhoneTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ClientFavoriteGenreTextBox.Focus();
            e.Handled = true;
        }
    }

    private void ClientFavoriteGenreTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ClientIsStudentCheckBox.Focus();
            e.Handled = true;
        }
    }

    private void GameTitleTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            GameGenreTextBox.Focus();
            e.Handled = true;
        }
    }

    private void GameGenreTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            GamePlayerCountTextBox.Focus();
            e.Handled = true;
        }
    }

    private void GamePlayerCountTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            GameIsPartyGameCheckBox.Focus();
            e.Handled = true;
        }
    }

    private void RentalPriceRubTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            RentalIsReturnedCheckBox.Focus();
            e.Handled = true;
        }
    }
}