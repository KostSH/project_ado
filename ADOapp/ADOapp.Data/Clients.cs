namespace ADOapp.Data;
public class Clients
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public bool IsStudent { get; set; }
    public string? FavoriteGenre { get; set; }
    public List<Rentals> Rentals { get; set; } = new();
    public override string ToString()
    {
        return $"{FullName}, телефон: {Phone}, студент: {(IsStudent ? "да" : "нет")}, любимый жанр: {FavoriteGenre ?? "не указан"}";
    }
}