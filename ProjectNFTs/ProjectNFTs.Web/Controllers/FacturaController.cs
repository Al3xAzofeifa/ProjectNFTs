using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectNFTs.Application.DTOs;
using ProjectNFTs.Application.Services.Implementations;
using ProjectNFTs.Application.Services.Interfaces;
using ProjectNFTs.Infraestructure.Models;
using System.Text.Json;
using X.PagedList;

namespace ProjectNFTs.Web.Controllers;

[Authorize(Roles = "Admin,Processes")]
public class FacturaController : Controller
{
    private readonly IServiceNft _serviceNft;
    private readonly IServiceTarjeta _serviceTarjeta;
    private readonly IServiceFactura _serviceFactura;
    //private readonly IServiceImpuesto _serviceImpuesto;
    private readonly IServiceCliente _serviceCliente;

    public FacturaController(IServiceNft servicoNft,
                             IServiceTarjeta servicoTarjeta,
                             IServiceFactura serviceFactura,
                             IServiceCliente serviceCliente)
    {
        _serviceNft = servicoNft;
        _serviceTarjeta = servicoTarjeta;
        _serviceFactura = serviceFactura;
        _serviceCliente = serviceCliente;
    }

    public async Task<IActionResult> Index()
    {

        var nextReceiptNumber = await _serviceFactura.GetNextReceiptNumber();
        ViewBag.CurrentReceipt = nextReceiptNumber;
        var collection = await _serviceTarjeta.ListaAsyncValid();
        ViewBag.ListTarjeta = collection;

        // Clear CarShopping
        TempData["CartShopping"] = null;
        TempData.Keep();

        return View();
    }


    public async Task<IActionResult> Anular(int? page)
    {       
        var collection = await _serviceFactura.ListAsync();

        foreach (var item in collection)
        {
            ClienteDTO clientedto = await _serviceCliente.FindByIdAsync((Guid)item.IdCliente);
            item.NombreCliente = clientedto.Nombre + " " + clientedto.Apellido1;

            item.DescripcionTarjeta = await _serviceTarjeta.ObtenerDescripcionTarjeta(item.IdTarjeta.Value);
        }

        return View(collection.ToPagedList(page ?? 1, 5));
    }

    
    [HttpPost]
    public async Task<IActionResult> CancelConfirm(int id)
    {
        await _serviceFactura.CancelAsync(id);
        return RedirectToAction("Index");
    }


    public async Task<string> GenerarUrlImagen(Guid id)
    {
        var nft = await _serviceNft.FindByIdAsync(id);
        if (nft != null && nft.Imagen != null)
        {
            // La URL generada apunta al controlador desde la base de datos
            return $"/Nft/ObtenerImagen?id={nft.Id}";
        }
        else
        {
            // Cambiar por una Imagen por defecto, que haga referencia a que no se encontró la imagen
            return null;
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var @object = await _serviceFactura.FindByIdAsync(id);
        return View(@object);
    }

    
    public async Task<IActionResult> AddNft(Guid id, int cantidad)
    {
        DetalleFacturaDTO facturaDetalleDTO = new DetalleFacturaDTO();
        List<DetalleFacturaDTO> lista = new List<DetalleFacturaDTO>();
        string json = "";
        decimal impuesto = 0;

        var Nft = await _serviceNft.FindByIdAsync(id);

        // Stock ??
        if (cantidad > Nft.CantidadInventario)
        {
            return BadRequest("No hay inventario suficiente!");
        }

        //impuesto = await _serviceImpuesto.GetImpuesto();

        facturaDetalleDTO.DescripcionNFT = Nft.Nombre;
        facturaDetalleDTO.Cantidad = cantidad;
        facturaDetalleDTO.Precio = Nft.Valor;
        facturaDetalleDTO.IdNft = id;
        facturaDetalleDTO.TotalLinea = (facturaDetalleDTO.Precio * facturaDetalleDTO.Cantidad) + impuesto;
        facturaDetalleDTO.ImagenUrl = await GenerarUrlImagen(Nft.Id);

        //facturaDetalleDTO.Impuesto = (cantidad * Nft.Precio) * (impuesto / 100);
        if (TempData["CartShopping"] == null)
        {
            lista.Add(facturaDetalleDTO);
            // Reenumerate 
            int idx = 1;
            lista.ForEach(p => p.IdDetalle = idx++);
            json = JsonSerializer.Serialize(lista);
            TempData["CartShopping"] = json;
        }
        else
        {
            json = (string)TempData["CartShopping"]!;
            lista = JsonSerializer.Deserialize<List<DetalleFacturaDTO>>(json!)!;
            lista.Add(facturaDetalleDTO);
            // Reenumerate 
            int idx = 1;
            lista.ForEach(p => p.IdDetalle = idx++);
            json = JsonSerializer.Serialize(lista);
            TempData["CartShopping"] = json;
        }

        TempData.Keep();
        return PartialView("_DetailFactura", lista);
    }

    // GET: ClienteController/Details/5
    public async Task<IActionResult> _Details(int id)
    {
        var encabezadoDTO = await _serviceFactura.FindByIdAsync(id);

        // Verificar si IdPais es nulo
        if (encabezadoDTO.IdTarjeta.HasValue)
        {
            // Obtener la descripción del país utilizando el servicio de país
            encabezadoDTO.DescripcionTarjeta = await _serviceTarjeta.ObtenerDescripcionTarjeta(encabezadoDTO.IdTarjeta.Value);
        }
        else
        {
            encabezadoDTO.DescripcionTarjeta = "Tarjeta No Especificada"; // O cualquier valor predeterminado
        }

        if (encabezadoDTO.IdCliente.HasValue)
        {
            // Obtener la descripción del país utilizando el servicio de país
            ClienteDTO clientedto = await _serviceCliente.FindByIdAsync(encabezadoDTO.IdCliente.Value);
            encabezadoDTO.NombreCliente = clientedto.Nombre +" "+ clientedto.Apellido1;
        }
        else
        {
            encabezadoDTO.NombreCliente = "Cliente No Especificado"; // O cualquier valor predeterminado
        }


        return PartialView(encabezadoDTO);
    }

    public IActionResult GetDetailFactura()
    {
        List<DetalleFacturaDTO> lista = new List<DetalleFacturaDTO>();
        string json = "";
        json = (string)TempData["CartShopping"]!;
        lista = JsonSerializer.Deserialize<List<DetalleFacturaDTO>>(json!)!;
        // Reenumerate 
        int idx = 1;
        lista.ForEach(p => p.IdDetalle = idx++);
        json = JsonSerializer.Serialize(lista);
        TempData["CartShopping"] = json;
        TempData.Keep();

        return PartialView("_DetailFactura", lista);
    }

    public IActionResult DeleteNft(int id)
    {
        DetalleFacturaDTO facturaDetalleDTO = new DetalleFacturaDTO();
        List<DetalleFacturaDTO> lista = new List<DetalleFacturaDTO>();
        string json = "";

        if (TempData["CartShopping"] != null)
        {
            json = (string)TempData["CartShopping"]!;
            lista = JsonSerializer.Deserialize<List<DetalleFacturaDTO>>(json!)!;
            // Remove from list by Index
            int idx = lista.FindIndex(p => p.IdDetalle == id);
            lista.RemoveAt(idx);
            json = JsonSerializer.Serialize(lista);
            TempData["CartShopping"] = json;
        }

        TempData.Keep();

        return PartialView("_DetailFactura", lista);

    }


    public async Task<IActionResult> Create(EncabezadoFacturaDTO facturaEncabezadoDTO)
    {
        string json;
        try
        {

            // IdClient exist?
            var cliente = await _serviceCliente.FindByIdAsync((Guid)facturaEncabezadoDTO.IdCliente);

            if (cliente == null)
            {
                TempData.Keep();
                return BadRequest("Cliente No existe");
            }


            json = (string)TempData["CartShopping"]!;

            if (string.IsNullOrEmpty(json))
            {
                return BadRequest("No hay datos por facturar");
            }

            var lista = JsonSerializer.Deserialize<List<DetalleFacturaDTO>>(json!)!;

            if (lista.Count == 0 || lista == null)
            {
                return BadRequest("No hay datos por facturar");
            }

            //Mismo numero de factura para el detalle FK
            lista.ForEach(p => p.IdFactura = facturaEncabezadoDTO.IdFactura);
            facturaEncabezadoDTO.ListFacturaDetalle = lista;
            facturaEncabezadoDTO.Total = lista.Sum(detalle => detalle.TotalLinea);

            foreach (var detalle in facturaEncabezadoDTO.ListFacturaDetalle)
            {
                var nft = await _serviceNft.FindByIdAsync((Guid)detalle.IdNft);
                string ruta = $"c:\\temp\\NFT_{nft.Id}.png";

                // Guardar la imagen del NFT en la ruta especificada
                await GuardarNFT(nft, ruta);
            }

            await _serviceFactura.AddAsync(facturaEncabezadoDTO);

            Thread.Sleep(2000);

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            // Keep Cache data
            TempData.Keep();
            return BadRequest(ex.Message);
        }
    }

    public async Task GuardarNFT(NftDTO nft, string ruta)
    {
        // Como el campo 'Imagen' de tu NFT ya es un Byte[], puedes escribirlo directamente en un archivo.
        byte[] datosNFT = nft.Imagen;

        // Crear el archivo en la ruta especificada y escribir los datos del NFT en el archivo.
        await System.IO.File.WriteAllBytesAsync(ruta, datosNFT);
    }


}
