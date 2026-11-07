namespace ADOapp.Data;
public static class DbSeed
{
    public static void Seed(AppDbContext db)
    {
        if (db.Clients.Any() || db.Games.Any() || db.Rentals.Any())
        {
            return;
        }
        var client1 = new Clients
        {
            FullName = "Иван Петров",
            Phone = "89110000001",
            IsStudent = true,
            FavoriteGenre = "Стратегия"
        };
        var client2 = new Clients
        {
            FullName = "Мария Смирнова",
            Phone = "89110000002",
            IsStudent = false,
            FavoriteGenre = "Кооператив"
        };
        var client3 = new Clients
        {
            FullName = "Алексей Иванов",
            Phone = "89110000003",
            IsStudent = true,
            FavoriteGenre = null
        };
        db.Clients.AddRange(client1, client2, client3);
        var game1 = new Games
        {
            Title = "Каркассон",
            Genre = "Стратегия",
            PlayerCount = 5,
            IsPartyGame = false
        };
        var game2 = new Games
        {
            Title = "Мафия",
            Genre = "Компания",
            PlayerCount = 10,
            IsPartyGame = true
        };
        var game3 = new Games
        {
            Title = "Codenames",
            Genre = "Слова",
            PlayerCount = 8,
            IsPartyGame = true
        };
        var game4 = new Games
        {
            Title = "Ticket to Ride",
            Genre = "Семейная",
            PlayerCount = 5,
            IsPartyGame = false
        };
        db.Games.AddRange(game1, game2, game3, game4);
        db.SaveChanges();
        var rental1 = new Rentals
        {
            ClientId = client1.Id,
            GameId = game1.Id,
            StartDate = new DateTime(2026, 11, 1),
            EndDate = new DateTime(2026, 11, 3),
            PriceRub = 300,
            IsReturned = true
        };
        var rental2 = new Rentals
        {
            ClientId = client2.Id,
            GameId = game3.Id,
            StartDate = new DateTime(2026, 11, 5),
            EndDate = null,
            PriceRub = 400,
            IsReturned = false
        };
        var rental3 = new Rentals
        {
            ClientId = client3.Id,
            GameId = game2.Id,
            StartDate = new DateTime(2026, 11, 7),
            EndDate = new DateTime(2026, 11, 8),
            PriceRub = 250,
            IsReturned = true
        };
        var rental4 = new Rentals
        {
            ClientId = client1.Id,
            GameId = game4.Id,
            StartDate = new DateTime(2026, 11, 10),
            EndDate = null,
            PriceRub = 500,
            IsReturned = false
        };
        db.Rentals.AddRange(rental1, rental2, rental3, rental4);
        db.SaveChanges();
    }
}