using ProjectNFTs.Application.DTOs;
using ProjectNFTs.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Application.Services.Interfaces
{
    public interface IServiceReporte
    {
        Task<byte[]> ClienteReport();
        Task<byte[]> NFTReport();
        //Task<byte[]> ClienteReportByNFTName(string nombre);
        Task<byte[]> FacturasReportByFechas(DateTime fecha1, DateTime fecha2);
        Task<ICollection<EncabezadoFacturaDTO>> BillsByFechasAsync(DateTime fecha1, DateTime fecha2);
        Task<ICollection<EncabezadoFactura>> ListFechasVenta(DateTime fecha1, DateTime fecha2);
    }
}
