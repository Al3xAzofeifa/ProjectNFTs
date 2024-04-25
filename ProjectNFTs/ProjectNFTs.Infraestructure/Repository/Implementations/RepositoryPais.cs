using Microsoft.EntityFrameworkCore;
using ProjectNFTs.Infraestructure.Data;
using ProjectNFTs.Infraestructure.Models;
using ProjectNFTs.Infraestructure.Repository.Interfaces;

namespace ProjectNFTs.Infraestructure.Repository.Implementations;

public class RepositoryPais : IRepositoryPais
{
    private readonly ProjectNFTsContext _context;

    public RepositoryPais(ProjectNFTsContext context)
    {
        _context = context;
    }

    public async Task<int> AddAsync(Pais entity)
    {
        await _context.Set<Pais>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity.IdPais;

    }

    public async Task DeleteAsync(int id)
    {
        int rowAffected = _context.Database.ExecuteSql($"Delete Pais Where IdPais = {id}");
        await Task.FromResult(1);
    }

    public async Task<ICollection<Pais>> FindByDescriptionAsync(string description)
    {
        var collection = await _context
                                     .Set<Pais>()
                                     .Where(p => p.Descripcion.Contains(description))
                                     .ToListAsync();
        return collection;

    }

    public async Task<Pais> FindByIdAsync(int id)
    {
        var @object = await _context.Set<Pais>().FindAsync(id);
        return @object!;

    }

    public async Task<ICollection<Pais>> ListAsync()
    {
        var collection = await _context.Set<Pais>().AsNoTracking().ToListAsync();
        return collection;

    }


    public async Task UpdateAsync(int id, Pais entity)
    {
        var @object = await FindByIdAsync(id);

        // Asignar los valores de la entidad recibida a la entidad recuperada
        @object.IdPais = entity.IdPais;        
        @object.Descripcion = entity.Descripcion;

        await _context.SaveChangesAsync();
    }
}
