using ProjectNFTs.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Infraestructure.Repository.Interfaces;

public interface IRepositoryFactura
{
    Task<int> AddAsync(EncabezadoFactura entity);
    Task UpdateAsync(int id, EncabezadoFactura entity);
    Task CancelAsync(int id);
    Task<int> GetNextReceiptNumber();
    Task<EncabezadoFactura> FindByIdAsync(int id);
    Task<ICollection<EncabezadoFactura>> ListAsync();
    Task<ICollection<EncabezadoFactura>> BillsByClientIdAsync(Guid id);
    Task<ICollection<EncabezadoFactura>> FacturasByFechasAsync(DateTime fecha1, DateTime fecha2);
    Task<List<EncabezadoFactura>> GetFacturas();
}
