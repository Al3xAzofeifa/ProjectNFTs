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

public class RepositoryCliente : IRepositoryCliente
{
    private readonly ProjectNFTsContext _context;

    public RepositoryCliente(ProjectNFTsContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddAsync(Cliente entity)
    {
        await _context.Set<Cliente>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity.IdCliente;

    }

    public async Task DeleteAsync(Guid id)
    {
        int rowAffected = _context.Database.ExecuteSql($"Delete Cliente Where IdCliente = {id}");
        await Task.FromResult(1);
    }

    public async Task<ICollection<Cliente>> FindByDescriptionAsync(string description)
    {
        var collection = await _context
                                     .Set<Cliente>()
                                     .Where(p => p.Nombre.Contains(description))
                                     .ToListAsync();
        return collection;
    }

    public async Task<Cliente> FindByIdAsync(Guid id)
    {
        var @object = await _context.Set<Cliente>().FindAsync(id);
        return @object!;
    }

    public async Task<ICollection<Cliente>> ListAsync()
    {
        var collection = await _context.Set<Cliente>().AsNoTracking().ToListAsync();
        return collection;
    }


    public async Task UpdateAsync(Guid id, Cliente entity)
    {
        var @object = await FindByIdAsync(id);

        // Asignar los valores de la entidad recibida a la entidad recuperada
        @object.Nombre = entity.Nombre;
        @object.Apellido1 = entity.Apellido1;
        @object.Apellido2 = entity.Apellido2;
        @object.Email = entity.Email;
        @object.Sexo = entity.Sexo;
        @object.FechaDeNacimiento = entity.FechaDeNacimiento;
        @object.IdPais = entity.IdPais;

        await _context.SaveChangesAsync();
    }

    public async Task<Cliente> FindByNFTNameAsync(string nombre)
    {
        // Convertir el nombre a minúsculas para la comparación insensible a mayúsculas y minúsculas
        nombre = nombre.ToLower();

        // Buscar el NFT por nombre de manera insensible a mayúsculas y minúsculas
        var nft = await _context.Set<Nft>()
                                .FirstOrDefaultAsync(n => n.Nombre.ToLower() == nombre);

        if (nft == null)
        {
            // Si no se encuentra el NFT, retornar null
            return null;
        }

        // Filtrar las compras relacionadas al NFT encontrado y con Status = 1
        var purchases = await _context.Set<Purchase>()
                                       .Where(p => p.IdNft == nft.Id && p.Status)
                                       .ToListAsync();

        if (purchases.Count == 0)
        {
            // Si no hay compras con Status = 1 para este NFT, retornar null
            return null;
        }

        // Obtener el Customer_ID de la primera compra
        var customerId = purchases.FirstOrDefault()?.CustomerId;

        if (customerId == null)
        {
            // Si no se encuentra Customer_ID, retornar null
            return null;
        }

        // Buscar la información del cliente usando el Customer_ID obtenido
        var client = await _context.Set<Cliente>()
                                   .FirstOrDefaultAsync(c => c.IdCliente == customerId);

        return client;
    }
}
