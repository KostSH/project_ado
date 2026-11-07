namespace ADOapp.Data;
public class Rentals
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int GameId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PriceRub { get; set; }
    public bool IsReturned { get; set; }
    public Clients Client { get; set; } = null!;
    public Games Game { get; set; } = null!;
    public override string ToString()
    {
        var endDateText = EndDate?.ToShortDateString() ?? "еще не возвращена";

        return $"{Client.FullName} взял игру \"{Game.Title}\" с {StartDate.ToShortDateString()} до {endDateText}, цена: {PriceRub} руб., возвращена: {(IsReturned ? "да" : "нет")}";
    }
}