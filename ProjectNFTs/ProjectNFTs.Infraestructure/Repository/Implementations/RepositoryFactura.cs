using Microsoft.EntityFrameworkCore;
using ProjectNFTs.Infraestructure.Data;
using ProjectNFTs.Infraestructure.Models;
using ProjectNFTs.Infraestructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectNFTs.Infraestructure.Repository.Implementations;

public class RepositoryFactura : IRepositoryFactura
{
    private readonly ProjectNFTsContext _context;

    public RepositoryFactura(ProjectNFTsContext context)
    {
        _context = context;

    }

    public async Task UpdateAsync(int id, EncabezadoFactura entity)
    {
        var @object = await FindByIdAsync(id);
        //Se actualiza solo el Estado
        @object.EstadoFactura = entity.EstadoFactura;
        
        await _context.SaveChangesAsync();
    }

    public async Task CancelAsync(int id)
    {
        var @object = await FindByIdAsync(id);

        int rowAffected = _context.Database.ExecuteSql($"Update EncabezadoFactura SET EstadoFactura = 0 Where IdFactura = {id}");
        await Task.FromResult(rowAffected);
    }
    public async Task<int> AddAsync(EncabezadoFactura entity)
    {

        try
        {
            // Get No Receipt
            entity.IdFactura = GetNoReceipt();
            //Asignacion de valores faltantes (De preferencia realizar esto en la DB, no aqui)
            entity.EstadoFactura = 1;
            entity.FechaFacturacion = DateTime.Now;
            // Reenumerate
            entity.DetalleFactura.ToList().ForEach(p => p.IdFactura = entity.IdFactura);
            // Begin Transaction
            await _context.Database.BeginTransactionAsync();
            await _context.Set<EncabezadoFactura>().AddAsync(entity);

            await _context.SaveChangesAsync();
            // Commit
            await _context.Database.CommitTransactionAsync();

            return entity.IdFactura;
        }
        catch (Exception ex)
        {
            Exception exception = ex;
            // Rollback 
            await _context.Database.RollbackTransactionAsync();
            throw;
        }
    }

    /// <summary>
    /// Get current NoReceipt without increment
    /// </summary>
    /// <returns></returns>
    public async Task<int> GetNextReceiptNumber()
    {

        int current = 0;

        string sql = string.Format("SELECT current_value FROM sys.sequences WHERE name = 'ReceiptNumber'");

        System.Data.DataTable dataTable = new System.Data.DataTable();

        System.Data.Common.DbConnection connection = _context.Database.GetDbConnection();
        System.Data.Common.DbProviderFactory dbFactory = System.Data.Common.DbProviderFactories.GetFactory(connection!)!;
        using (var cmd = dbFactory!.CreateCommand())
        {
            cmd!.Connection = connection;
            cmd.CommandText = sql;
            using (System.Data.Common.DbDataAdapter adapter = dbFactory.CreateDataAdapter()!)
            {
                adapter.SelectCommand = cmd;
                adapter.Fill(dataTable);
            }
        }


        current = Convert.ToInt32(dataTable.Rows[0][0].ToString());
        return await Task.FromResult(current);

    }



    public async Task<ICollection<EncabezadoFactura>> ListAsync()
    {
        var collection = await _context.Set<EncabezadoFactura>().AsNoTracking().ToListAsync();
        return collection;
    }


    /// <summary>
    /// Get sequence in order to assign Receipt number.   
    /// Automaticaly INCREMENT ++
    /// http://technet.microsoft.com/es-es/library/ff878091.aspx
    /// CREATE SEQUENCE  ReceiptNumber  START WITH 1 INCREMENT BY 1 ;
    /// </summary>
    /// <returns>Num. de factura</returns>
    private int GetNoReceipt()
    {
        int siguiente = 0;

        string sql = string.Format("SELECT NEXT VALUE FOR ReceiptNumber");

        System.Data.DataTable dataTable = new System.Data.DataTable();

        System.Data.Common.DbConnection connection = _context.Database.GetDbConnection();
        System.Data.Common.DbProviderFactory dbFactory = System.Data.Common.DbProviderFactories.GetFactory(connection!)!;
        using (var cmd = dbFactory!.CreateCommand())
        {
            cmd!.Connection = connection;
            cmd.CommandText = sql;
            using (System.Data.Common.DbDataAdapter adapter = dbFactory.CreateDataAdapter()!)
            {
                adapter.SelectCommand = cmd;
                adapter.Fill(dataTable);
            }
        }


        siguiente = Convert.ToInt32(dataTable.Rows[0][0].ToString());
        return siguiente;

    }


    public async Task<ICollection<EncabezadoFactura>> BillsByClientIdAsync(Guid id)
    {
        var response = await _context.Set<EncabezadoFactura>()
                       .Include(p => p.DetalleFactura)
                       .Where(p => p.IdCliente == id).ToListAsync();

        return response;

    }

    public async Task<EncabezadoFactura> FindByIdAsync(int id)
    {

        var response = await _context.Set<EncabezadoFactura>()
                    .Include(detalle => detalle.DetalleFactura)
                    .ThenInclude(detalle => detalle.IdNftNavigation)
                    .Include(cliente => cliente.IdClienteNavigation)
                    .Where(p => p.IdFactura == id).FirstOrDefaultAsync();

        return response!;
    }

    public async Task<ICollection<EncabezadoFactura>> FacturasByFechasAsync(DateTime fecha1, DateTime fecha2)
    {
        var response = await _context.Set<EncabezadoFactura>()
                       .Include(e => e.DetalleFactura)
                       .Where(e => e.FechaFacturacion >= fecha1 && e.FechaFacturacion <= fecha2)
                       .ToListAsync();

        foreach (var encabezadoFactura in response)
        {
            encabezadoFactura.DetalleFactura = encabezadoFactura.DetalleFactura
                .Where(d => d.IdFactura == encabezadoFactura.IdFactura)
                .ToList();
        }

        return response;
    }

    public async Task<List<EncabezadoFactura>> GetFacturas()
    {
        try
        {
            var facturas = await _context.Set<EncabezadoFactura>()
                .Where(f => f.EstadoFactura == 1)
                .Select(f => new EncabezadoFactura
                {
                    IdCliente = f.IdCliente,
                    IdFactura = f.IdFactura,
                    FechaFacturacion = f.FechaFacturacion,
                    Total = f.Total,
                    EstadoFactura = f.EstadoFactura,
                    IdTarjeta = f.IdTarjeta
                })
                .ToListAsync();

            return facturas;
        }
        catch (Exception ex)
        {
            // Handle or log the exception as needed
            throw new Exception("Error fetching invoices.", ex);
        }
    }
}
