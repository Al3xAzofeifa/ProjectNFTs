using Microsoft.EntityFrameworkCore;
using ProjectNFTs.Infraestructure.Data;
using ProjectNFTs.Infraestructure.Models;
using ProjectNFTs.Infraestructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Infraestructure.Repository.Implementations;

public class RepositoryTarjeta : IRepositoryTarjeta
{
    private readonly ProjectNFTsContext _context;

    public RepositoryTarjeta(ProjectNFTsContext context)
    {
        _context = context;
    }

    public async Task<int> AddAsync(Tarjeta entity)
    {
        await _context.Set<Tarjeta>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity.Id;

    }

    public async Task DeleteAsync(int id)
    {
        int rowAffected = _context.Database.ExecuteSql($"Delete Tarjeta Where Id = {id}");
        await Task.FromResult(1);
    }

    public async Task<ICollection<Tarjeta>> FindByDescriptionAsync(string description)
    {
        var collection = await _context
                                     .Set<Tarjeta>()
                                     .Where(p => p.Descripcion.Contains(description))
                                     .ToListAsync();
        return collection;

    }

    public async Task<Tarjeta> FindByIdAsync(int id)
    {
        var @object = await _context.Set<Tarjeta>().FindAsync(id);
        return @object!;

    }

    public async Task<ICollection<Tarjeta>> ListAsync()
    {
        var collection = await _context.Set<Tarjeta>().AsNoTracking().ToListAsync();
        return collection;

    }

    public async Task UpdateAsync(int id, Tarjeta entity)
    {
        var @object = await FindByIdAsync(id);

        // Asignar los valores de la entidad recibida a la entidad recuperada
        @object.Id = entity.Id;
        @object.Descripcion = entity.Descripcion;
        @object.Estado = entity.Estado;

        await _context.SaveChangesAsync();
    }
}
