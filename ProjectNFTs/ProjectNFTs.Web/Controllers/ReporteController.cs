using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using ProjectNFTs.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using ProjectNFTs.Infraestructure.Repository.Interfaces;

namespace ProjectNFTs.Web.Controllers;

[Authorize(Roles = "Admin,Reports")]
public class ReporteController : Controller
{
    private readonly IServiceReporte _servicioReporte;
    private readonly IRepositoryCliente _repositoryCliente;
    private readonly IRepositoryNft _repositoryNFT;
    public ReporteController(IServiceReporte servicioReporte, IRepositoryCliente repositoryCliente, IRepositoryNft repositoryNFT)
    {
        _servicioReporte = servicioReporte;
        _repositoryCliente = repositoryCliente;
        _repositoryNFT = repositoryNFT;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult NFTReport()
    {
        return View();
    }

    [HttpPost]
    [RequireAntiforgeryToken]
    public async Task<FileResult> NFTReportPDF()
    {
        byte[] bytes = await _servicioReporte.NFTReport();
        return File(bytes, "text/plain", "file.pdf");
    }

    public IActionResult ClienteReport()
    {
        return View();
    }

    [HttpPost]
    [RequireAntiforgeryToken]
    public async Task<FileResult> ClienteReportPDF()
    {

        byte[] bytes = await _servicioReporte.ClienteReport();
        return File(bytes, "text/plain", "file.pdf");

    }

    public IActionResult ClienteByNFTNameReport()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ClienteReportByNFTNamePDF(string descripcion)
    {
        if (string.IsNullOrEmpty(descripcion))
        {
            ViewBag.Message = "El nombre es requerido";
            return View("ClienteByNFTNameReport");
        }

        var nft = await _repositoryNFT.FindByDescriptionAsync(descripcion);
        if (nft == null)
        {
            ViewBag.Message = "El NFT no existe";
            return View("ClienteByNFTNameReport");
        }

        var cliente = await _repositoryCliente.FindByNFTNameAsync(descripcion);
        if (cliente == null)
        {
            ViewBag.Message = "El NFT no tiene ningún cliente asociado";
            return View("ClienteByNFTNameReport");
        }

        ViewBag.Cliente = cliente;
        ViewBag.NFT = nft.FirstOrDefault();

        return View("ClienteByNFTNameReport");
    }

    public IActionResult FacturasReport()
    {
        return View();
    }

    [HttpPost]
    [RequireAntiforgeryToken]
    public async Task<IActionResult> FacturasReportByFechasPDF(DateTime fecha1, DateTime fecha2)
    {
        if (fecha1 == Convert.ToDateTime("1/1/0001 00:00:00"))
        {
            ViewBag.Message = "Las fecha 1 NO puede estar vacía.";
            return View("FacturasReport");
        }

        if (fecha1 >= fecha2)
        {
            ViewBag.Message = "La fecha inicial debe ser menor que la fecha final.";
            return View("FacturasReport");
        }

        byte[] bytes = await _servicioReporte.FacturasReportByFechas(fecha1, fecha2);

        if (bytes.Length > 0 && bytes[0] != '%') // Verifica si los primeros bytes no son un porcentaje, lo que indicaría que es un archivo PDF válido
        {
            ViewBag.Message = Encoding.UTF8.GetString(bytes); // Asigna el mensaje a la variable ViewBag.Message
            return View("FacturasReport"); // Retorna la vista con el mensaje
        }
        else
        {
            return File(bytes, "application/pdf", "file.pdf"); // Cambio en el tipo de archivo devuelto
        }
    }

    public IActionResult VentasGrafico()
    {
        return View();
    }

    public async Task<IActionResult> VentasXFechas(DateTime fecha1, DateTime fecha2)
    {
        if (fecha1 == Convert.ToDateTime("1/1/0001 00:00:00"))
        {
            ViewBag.Message = "Las fecha 1 NO puede estar vacía.";
            return View("VentasGrafico");
        }

        if (fecha1 > fecha2)
        {
            ViewBag.Message = "La fecha inicial debe ser menor o igual que la fecha final.";
            return View("VentasGrafico");
        }

        string etiquetas = "";
        string precios = "";
        decimal total = 0M;

        var lista = await _servicioReporte.ListFechasVenta(fecha1, fecha2.AddDays(1));

        if (lista.Count == 0)
        {
            ViewBag.Message = $"No hay reportes de ventas entre las fechas {fecha1.ToShortDateString()} y {fecha2.ToShortDateString()}";
            return View("VentasGrafico");
        }

        var ventasPorFecha = lista.GroupBy(item => item.FechaFacturacion)
                                  .Select(group => new { FechaFactura = group.Key, TotalVentas = group.Sum(item => item.Total) });

        foreach (var venta in ventasPorFecha)
        {
            etiquetas += $"{venta.FechaFactura!.Value.ToShortDateString()},";
            total += venta.TotalVentas!.Value;
            precios += venta.TotalVentas!.Value.ToString("##") + ",";
        }

        etiquetas = etiquetas.Substring(0, etiquetas.Length - 1); // ultima coma
        precios = precios.Substring(0, precios.Length - 1);

        ViewBag.GraphTitle = $"Ventas entre  {fecha1.ToShortDateString()} - {fecha2.ToShortDateString()} -  Total de Ventas {total.ToString("###,###.00")} ";
        ViewBag.Etiquetas = etiquetas;
        ViewBag.Valores = precios;
        return View("VentasGrafico");
    }
}
