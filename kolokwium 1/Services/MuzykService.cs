using kolokwium_1.DTO;
using kolokwium_1.Models;
using kolokwium_1.Repositories;

namespace kolokwium_1.Services;

public class MuzykService : IMuzykService
{
    private IMuzykRepository _repository;

    public MuzykService(IMuzykRepository repository)
    {
        _repository = repository;
    }

    public async Task<Muzyk?> GetMuzyk(int id)
    {
        return await _repository.GetMuzyk(id);
    }

    public async Task<int> CreateMuzyk(MuzykDTO muzyk)
    {
        return await _repository.CreateMuzyk(muzyk);
    }
}