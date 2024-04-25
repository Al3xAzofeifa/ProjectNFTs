using ProjectNFTs.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Application.Services.Interfaces;

public interface IServiceFactura
{
    Task<int> AddAsync(EncabezadoFacturaDTO dto);
    Task CancelAsync(int id);
    Task<int> GetNextReceiptNumber();
    Task<EncabezadoFacturaDTO> FindByIdAsync(int id);
    Task<ICollection<EncabezadoFacturaDTO>> ListAsync();
    Task<ICollection<EncabezadoFacturaDTO>> ListAsyncValid();
}
