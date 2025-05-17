namespace kolokwium_1.Models;

public class Utwor
{
  public int id { get; set; }
  public string nazwa { get; set; }
  public float czasTrwania { get; set; }
  public int? idAlbumu { get; set; }

  public void Deconstruct(out int id, out string nazwa, out float czasTrwania, out int? idAlbumu)
  {
    id = this.id;
    nazwa = this.nazwa;
    czasTrwania = this.czasTrwania;
    idAlbumu = this.idAlbumu;
  }
}