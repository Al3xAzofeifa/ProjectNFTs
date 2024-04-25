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

public class RepositoryRol : IRepositoryRol
{
    private readonly ProjectNFTsContext _context;

    public RepositoryRol(ProjectNFTsContext context)
    {
        _context = context;
    }

    public async Task<int> AddAsync(RolUsuario entity)
    {
        await _context.Set<RolUsuario>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity.Id;

    }

    public async Task DeleteAsync(int id)
    {
        int rowAffected = _context.Database.ExecuteSql($"Delete RolUsuario Where IdRolUsuario = {id}");
        await Task.FromResult(1);
    }

    public async Task<ICollection<RolUsuario>> FindByDescriptionAsync(string description)
    {
        var collection = await _context
                                     .Set<RolUsuario>()
                                     .Where(p => p.Descripcion.Contains(description))
                                     .ToListAsync();
        return collection;

    }

    public async Task<RolUsuario> FindByIdAsync(int id)
    {
        var @object = await _context.Set<RolUsuario>().FindAsync(id);
        return @object!;

    }

    public async Task<ICollection<RolUsuario>> ListAsync()
    {
        var collection = await _context.Set<RolUsuario>().AsNoTracking().ToListAsync();
        return collection;

    }


    public async Task UpdateAsync(int id, RolUsuario entity)
    {
        var @object = await FindByIdAsync(id);

        // Asignar los valores de la entidad recibida a la entidad recuperada
        @object.Id = entity.Id;
        @object.Descripcion = entity.Descripcion;

        await _context.SaveChangesAsync();
    }
}
