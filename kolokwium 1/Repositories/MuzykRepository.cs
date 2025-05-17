using kolokwium_1.DTO;
using kolokwium_1.Models;
using Microsoft.Data.SqlClient;

namespace kolokwium_1.Repositories;

public class MuzykRepository : IMuzykRepository
{
    private IConfiguration _configuration;

    public MuzykRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<Muzyk?> GetMuzyk(int id)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await using var command = new SqlCommand();

        command.Connection = connection;
        
        await connection.OpenAsync();

        command.CommandText = "SELECT * FROM MUZYK WHERE IdMuzyk = @id";
        command.Parameters.AddWithValue("id", id);

        await using SqlDataReader dr = await command.ExecuteReaderAsync();

        if (await dr.ReadAsync()) return null;
        
        return new Muzyk
        {
            id = (int) dr["IdMuzyk"],
            imie = dr["Imie"].ToString(),
            nazwisko = dr["Nazwisko"].ToString(),
            pseudonim = dr["Pseudonim"].ToString(),
            utwory = await DajUtworyMuzyka(id)
        };

    } 

    public async Task<int> CreateMuzyk(MuzykDTO muzyk)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await using var command = new SqlCommand();

        command.Connection = connection;
        
        await connection.OpenAsync();



        command.CommandText = "BEGIN TRANSACTON";
        
        await command.ExecuteNonQueryAsync();

        
        (string imie, string nazwisko, string? pseudonim, UtworDTO? utwor) = muzyk;

        if (utwor != null)
        {
            SprwadzIWprowadzUtwor(utwor);
        }

        await WprowadzMuzykaDoTabeli(imie, nazwisko, pseudonim);

        int id = await WezIdMuzyka();
        
        if (utwor != null)
        {
           await WprowadzDoWykonawcyUtworu(id, await WezIdUtworu(utwor.nazwa));
        }

        return id;
    }


    public async Task<List<Utwor>> DajUtworyMuzyka(int id)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await using var command = new SqlCommand();

        command.Connection = connection;
        
        await connection.OpenAsync();
        
        

        command.CommandText = "SELECT * FROM Utwor " +
                              "INNER JOIN WykonawcaUtworu ON WykonawcaUtworu.IdUtwor = Utwor.IdUtwor " +
                              "WHERE IdMuzyk = @id";
        command.Parameters.AddWithValue("id", id); 
        await using SqlDataReader dr= await command.ExecuteReaderAsync();

        List<Utwor> utwory = new List<Utwor>();
        
        bool czyDalej = await dr.ReadAsync();
        while (czyDalej)
        {
            var utwor = new Utwor()
            {
                id = (int)dr["idUtwor"],
                nazwa = dr["NazwaUtworu"].ToString(),
                czasTrwania = (float) dr["CzasTrwania"],
                idAlbumu = (int) dr["IdAlbum"]
            };
            utwory.Add(utwor);
            czyDalej = await dr.ReadAsync(); 
        }

        return utwory;

    }

    private async void SprwadzIWprowadzUtwor(UtworDTO utwor)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await using var command = new SqlCommand();

        command.Connection = connection;
        
        await connection.OpenAsync();

        command.CommandText = "SELECT Count(1) FROM Utwor WHERE NazwaUtworu = @nazwa";
        command.Parameters.AddWithValue("nazwa", utwor.nazwa);
        
        await using SqlDataReader dr= await command.ExecuteReaderAsync();

        await dr.ReadAsync();

        if (dr.GetInt32(0) == 0)
        {
            await WprowadzUtwor(utwor);
        }
    }

    private async Task<int> WprowadzUtwor(UtworDTO utwor)
    {
        (string nazwa, float czas, int? idAlbumu) = utwor;
        
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await using var command = new SqlCommand();

        command.Connection = connection;
        
        await connection.OpenAsync();

        command.CommandText = "INSERT INTO Utwor VALUES (" +
                              nazwa + ", " + czas + ", " + idAlbumu + ")";
        
        return await command.ExecuteNonQueryAsync();
    }

    private async Task<int> WprowadzMuzykaDoTabeli(string imie, string nazwisko, string? pseudonim)
    {
       
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await using var command = new SqlCommand();

        command.Connection = connection;
        
        await connection.OpenAsync();

        command.CommandText = "INSERT INTO Muzyk VALUES (" +
                              imie + ", " + nazwisko + ", " + pseudonim + ")";
        
        return await command.ExecuteNonQueryAsync(); 
        
    }

    private async Task<int> WprowadzDoWykonawcyUtworu(int idMuzyka, int idUtworu)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await using var command = new SqlCommand();

        command.Connection = connection;
        
        await connection.OpenAsync();

        command.CommandText = "INSERT INTO WykonawcaUtworu VALUES (" +
                              idMuzyka + ", " + idUtworu + ")";
        
        return await command.ExecuteNonQueryAsync();  
    }

    private async Task<int> WezIdMuzyka()
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await using var command = new SqlCommand();

        command.Connection = connection;
        
        await connection.OpenAsync();

        command.CommandText = "SELECT Max(IdMuzyk) FROM Muzyk";
        
        await using SqlDataReader dr= await command.ExecuteReaderAsync();

        await dr.ReadAsync();

        return dr.GetInt32(0);

    }
    private async Task<int> WezIdUtworu(string nazwa)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await using var command = new SqlCommand();

        command.Connection = connection;
        
        await connection.OpenAsync();

        command.CommandText = "SELECT IdUtwor FROM Utwor WHERE NazwaUtworu = @nazwa";
        command.Parameters.AddWithValue("nazwa", nazwa);
 
        
        await using SqlDataReader dr= await command.ExecuteReaderAsync();

        await dr.ReadAsync();

        return dr.GetInt32(0);

    }

    private async void Commituj()
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await using var command = new SqlCommand();

        command.Connection = connection;
        
        await connection.OpenAsync();

        command.CommandText = "COMMIT";
        
        await command.ExecuteNonQueryAsync();  
    }
    
}