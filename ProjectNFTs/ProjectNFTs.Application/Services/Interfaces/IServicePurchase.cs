using ProjectNFTs.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Application.Services.Interfaces
{
    public interface IServicePurchase
    {
        Task<PurchaseDTO> FindOwnerByIdAsync(Guid id);
    }
}
