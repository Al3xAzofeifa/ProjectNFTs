using ProjectNFTs.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryPurchase
    {
        Task<Purchase> FindOwnerByIdAsync(Guid id);
    }
}
