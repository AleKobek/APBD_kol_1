namespace kolokwium_1.Models;

public class Muzyk
{
    public int id { get; set; }
    public string imie { get; set; }
    public string nazwisko { get; set; }
    public string? pseudonim { get; set; }
    public List<Utwor> utwory { get; set; }
}