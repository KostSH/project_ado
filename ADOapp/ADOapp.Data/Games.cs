namespace ADOapp.Data;
public class Games
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Genre { get; set; } = "";
    public int PlayerCount { get; set; }
    public bool IsPartyGame { get; set; }
    public List<Rentals> Rentals { get; set; } = new();
    public override string ToString()
    {
        return $"{Title}, жанр: {Genre}, игроков: {PlayerCount}, пати-игра: {(IsPartyGame ? "да" : "нет")}";
    }
}