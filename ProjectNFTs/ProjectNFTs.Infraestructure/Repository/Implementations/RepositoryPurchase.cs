using Microsoft.Data.SqlClient;
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

public class RepositoryPurchase :  IRepositoryPurchase
{
    private readonly ProjectNFTsContext _context;

    public RepositoryPurchase(ProjectNFTsContext context)
    {
        _context = context;
    }

    public async Task<Purchase> FindOwnerByIdAsync(Guid id)
    {

        // Ejecutar una consulta SQL personalizada para obtener el propietario del NFT
        //CustomerId, IdNft
        var query = $"SELECT * FROM Purchase WHERE ID_NFT = '{id}'";

        var owner = await _context.Purchase.FromSqlRaw(query).FirstOrDefaultAsync();

        return owner;
    }
}
