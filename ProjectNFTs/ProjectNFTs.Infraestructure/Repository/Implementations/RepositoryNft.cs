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

public class RepositoryNft : IRepositoryNft
{
    private readonly ProjectNFTsContext _context;

    public RepositoryNft(ProjectNFTsContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddAsync(Nft entity)
    {
        await _context.Set<Nft>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity.Id;

    }

    public async Task DeleteAsync(Guid id)
    {
        int rowAffected = _context.Database.ExecuteSql($"Delete Nft Where Id = {id}");
        await Task.FromResult(1);
    }

    public async Task<ICollection<Nft>> FindByAutorAsync(string description)
    {
        var collection = await _context
                                     .Set<Nft>()
                                     .Where(p => p.Autor.Contains(description))
                                     .ToListAsync();
        return collection;
    }
    public async Task<ICollection<Nft>> FindByDescriptionAsync(string description)
    {
         var collection = await _context
                                     .Set<Nft>()
                                     .Where(p => p.Nombre.Contains(description))
                                     .ToListAsync();
        return collection;
    }

    public async Task<Nft> FindByIdAsync(Guid id)
    {
        var @object = await _context.Set<Nft>().FindAsync(id);
        return @object!;

    }

    public async Task<ICollection<Nft>> ListAsync()
    {
        var collection = await _context.Set<Nft>().AsNoTracking().ToListAsync();
        return collection;

    }


    public async Task UpdateAsync(Guid id, Nft entity)
    {
        var @object = await FindByIdAsync(id);
        // Verificar si se encontró el objeto en la base de datos
        if (@object != null)
        {
            // Asignar los valores de la entidad recibida a la entidad recuperada
            @object.Nombre = entity.Nombre;
            @object.Autor = entity.Autor;
            @object.Valor = entity.Valor;
            @object.CantidadInventario = entity.CantidadInventario;
            @object.Imagen = entity.Imagen;

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();
        }
        else
        {
            // Manejar el caso en el que no se encontró el objeto en la base de datos
            throw new Exception("No se encontró el objeto con el ID proporcionado.");
        }

    }
}
