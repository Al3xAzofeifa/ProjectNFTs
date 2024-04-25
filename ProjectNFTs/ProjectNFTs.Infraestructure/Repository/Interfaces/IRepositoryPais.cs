using ProjectNFTs.Infraestructure.Models;

namespace ProjectNFTs.Infraestructure.Repository.Interfaces;

public interface IRepositoryPais
{
    Task<ICollection<Pais>> FindByDescriptionAsync(string description);
    Task<ICollection<Pais>> ListAsync();
    Task<Pais> FindByIdAsync(int id);
    Task<int> AddAsync(Pais entity);
    Task DeleteAsync(int id);
    Task UpdateAsync(int id, Pais entity);
}
