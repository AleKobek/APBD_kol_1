using kolokwium_1.DTO;
using kolokwium_1.Models;

namespace kolokwium_1.Repositories;

public interface IMuzykRepository
{
    public Task<Muzyk?> GetMuzyk(int id);

    public Task<int> CreateMuzyk(MuzykDTO muzyk);
}