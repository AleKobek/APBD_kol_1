using kolokwium_1.Models;

namespace kolokwium_1.DTO;

public record MuzykDTO(string imie, string nazwisko, string? pseudonim, UtworDTO? utwor);