using AutoMapper;
using ProjectNFTs.Application.DTOs;
using ProjectNFTs.Application.Services.Interfaces;
using ProjectNFTs.Infraestructure.Models;
using ProjectNFTs.Infraestructure.Repository.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Application.Services.Implementations;

public class ServiceReporte : IServiceReporte
{
    private readonly IRepositoryCliente _repositoryCliente;
    private readonly IRepositoryNft _repositoryNFT;
    private readonly IRepositoryFactura _repositoryFactura;
    private readonly IMapper _mapper;


    public ServiceReporte(IRepositoryCliente repositoryCliente, IRepositoryNft repositoryNft, IRepositoryFactura repositoryFactura, IMapper mapper)
    {
        _repositoryCliente = repositoryCliente;
        _repositoryNFT = repositoryNft;
        _repositoryFactura = repositoryFactura;
        _mapper = mapper;
    }

    public async Task<byte[]> ClienteReport()
    {
        // Get Data
        var collection = await _repositoryCliente.ListAsync();

        // License config ******  IMPORTANT ******
        QuestPDF.Settings.License = LicenseType.Community;

        // return ByteArrays
        var pdfByteArray = QuestPDF.Fluent.Document.Create(document =>
        {
            document.Page(page =>
            {

                page.Size(PageSizes.Letter);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.Margin(30);

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().AlignLeft().Text("NFT Sales ").Bold().FontSize(14).Bold();
                        col.Item().AlignLeft().Text($"Fecha: {DateTime.Now} ").FontSize(9);
                        col.Item().LineHorizontal(1f);
                    });

                });


                page.Content().PaddingVertical(10).Column(col1 =>
                {
                    col1.Item().AlignCenter().Text("Reporte de Clientes").FontSize(14).Bold();
                    col1.Item().Text("");
                    col1.Item().LineHorizontal(0.5f);

                    col1.Item().Table(tabla =>
                    {
                        tabla.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();

                        });

                        tabla.Header(header =>
                        {
                            header.Cell().Background("#164753")
                            .Padding(2).AlignCenter().Text("ID").FontColor("#fff");

                            header.Cell().Background("#164753")
                           .Padding(2).AlignCenter().Text("Nombre").FontColor("#fff");

                            header.Cell().Background("#164753")
                           .Padding(2).AlignCenter().Text("Apellido 1").FontColor("#fff");

                            header.Cell().Background("#164753")
                           .Padding(2).AlignCenter().Text("Apellido 2").FontColor("#fff");

                            header.Cell().Background("#164753")
                           .Padding(2).AlignCenter().Text("Email").FontColor("#fff");
                        });

                        foreach (var item in collection)
                        {

                            //var total = item.Cantidad * item.Precio;

                            // Column 1
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#200d25")
                            .Padding(2).Text(item.IdCliente.ToString()).FontSize(10);

                            // Column 2
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#200d25")
                            .Padding(2).Text(item.Nombre?.ToString()).FontSize(10);

                            // Column 3
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#200d25")
                            .Padding(2).Text(item.Apellido1?.ToString()).FontSize(10);

                            // Column 4
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#200d25")
                            .Padding(2).Text(item.Apellido2?.ToString()).FontSize(10);

                            //Column 5
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#200d25")
                            .Padding(2).Text(item.Email?.ToString()).FontSize(10);
                        }

                    });

                });


                page.Footer()
                .AlignRight()
                .Text(txt =>
                {
                    txt.Span("Página ").FontSize(10);
                    txt.CurrentPageNumber().FontSize(10);
                    txt.Span(" de ").FontSize(10);
                    txt.TotalPages().FontSize(10);
                });
            });
        }).GeneratePdf();

        //File.WriteAllBytes(@"C:\temp\ProductReport.pdf", pdfByteArray);
        return pdfByteArray;

    }

    /*public async Task<byte[]> ClienteReportByNFTName(string nombre)
    {
        // Get Data
        var collection = await _repositoryCliente.FindByNFTNameAsync(nombre);
        var collection2 = await _repositoryNFT.FindByDescriptionAsync(nombre);

        // License config ******  IMPORTANT ******
        QuestPDF.Settings.License = LicenseType.Community;

        // return ByteArrays
        var pdfByteArray = QuestPDF.Fluent.Document.Create(document =>
        {
            document.Page(page =>
            {

                page.Size(PageSizes.Letter);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.Margin(30);

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().AlignLeft().Text("NFT Sales ").Bold().FontSize(14).Bold();
                        col.Item().AlignLeft().Text($"Fecha: {DateTime.Now} ").FontSize(9);
                        col.Item().LineHorizontal(1f);
                    });

                });


                page.Content().PaddingVertical(10).Column(col1 =>
                {
                    col1.Item().AlignCenter().Text("Reporte de dueño del NFT").FontSize(14).Bold();
                    col1.Item().Text("");
                    col1.Item().LineHorizontal(0.5f);

                    col1.Item().Table(tabla =>
                    {
                        tabla.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();

                        });

                        tabla.Header(header =>
                        {
                            header.Cell().Background("#164753")
                            .Padding(2).AlignCenter().Text("Nombre").FontColor("#fff");

                            header.Cell().Background("#164753")
                            .Padding(2).AlignCenter().Text("Imagen").FontColor("#fff");

                            header.Cell().Background("#164753")
                           .Padding(2).AlignCenter().Text("Dueño del NFT").FontColor("#fff");

                            header.Cell().Background("#164753")
                           .Padding(2).AlignCenter().Text("Email").FontColor("#fff");
                        });

                        foreach (var item in collection)
                        {
                            foreach(var item2 in collection2)
                            {
                                //Column 1
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#200d25")
                                .Padding(2).Text(item2.Nombre?.ToString()).FontSize(10);

                                // Column 2
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#200d25")
                                .Padding(2).Image(item2.Imagen).UseOriginalImage();

                                // Column 3
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#200d25")
                                .Padding(2).Text(item.Nombre?.ToString() + " "+ item.Apellido1?.ToString() + " "+ item.Apellido2?.ToString()).FontSize(10);

                                //Column 4
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#200d25")
                                .Padding(2).Text(item.Email?.ToString()).FontSize(10);
                            }
                        }

                    });

                });


                page.Footer()
                .AlignRight()
                .Text(txt =>
                {
                    txt.Span("Página ").FontSize(10);
                    txt.CurrentPageNumber().FontSize(10);
                    txt.Span(" de ").FontSize(10);
                    txt.TotalPages().FontSize(10);
                });
            });
        }).GeneratePdf();

        //File.WriteAllBytes(@"C:\temp\ProductReport.pdf", pdfByteArray);
        return pdfByteArray;

    }*/

    public async Task<byte[]> NFTReport()
    {
        // Get Data
        var collection = await _repositoryNFT.ListAsync();

        // License config ******  IMPORTANT ******
        QuestPDF.Settings.License = LicenseType.Community;

        // return ByteArrays
        var pdfByteArray = QuestPDF.Fluent.Document.Create(document =>
        {
            document.Page(page =>
            {

                page.Size(PageSizes.Letter);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.Margin(30);

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().AlignLeft().Text("NFT Sales ").Bold().FontSize(14).Bold();
                        col.Item().AlignLeft().Text($"Fecha: {DateTime.Now} ").FontSize(9);
                        col.Item().LineHorizontal(1f);
                    });

                });


                page.Content().PaddingVertical(10).Column(col1 =>
                {
                    col1.Item().AlignCenter().Text("Reporte de NFT's").FontSize(14).Bold();
                    col1.Item().Text("");
                    col1.Item().LineHorizontal(0.5f);

                    col1.Item().Table(tabla =>
                    {
                        tabla.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();

                        });

                        tabla.Header(header =>
                        {
                            header.Cell().Background("#164753")
                            .Padding(2).AlignCenter().Text("Nombre").FontColor("#fff");

                            header.Cell().Background("#164753")
                           .Padding(2).AlignCenter().Text("Autor").FontColor("#fff");

                            header.Cell().Background("#164753")
                           .Padding(2).AlignCenter().Text("Imagen").FontColor("#fff");

                            header.Cell().Background("#164753")
                           .Padding(2).AlignCenter().Text("Valor").FontColor("#fff");
                        });

                        foreach (var item in collection)
                        {

                            //var total = item.Cantidad * item.Precio;

                            // Column 1
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#200d25")
                            .Padding(2).Text(item.Nombre?.ToString()).FontSize(10);

                            // Column 2
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#200d25")
                            .Padding(2).Text(item.Autor?.ToString()).FontSize(10);

                            // Column 3
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#200d25")
                            .Padding(2).Image(item.Imagen).UseOriginalImage();

                            // Column 4
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#200d25")
                            .Padding(2).AlignRight().Text(item.Valor?.ToString("###,##0.00") + " ETH").FontSize(10);
                        }

                    });

                    var granTotal = collection.Sum(p => p.Valor);

                    col1.Item().AlignRight().Text("Sumatoria de activos: " + granTotal?.ToString("###,##0.00") +" ETH").FontSize(12).Bold();

                });


                page.Footer()
                .AlignRight()
                .Text(txt =>
                {
                    txt.Span("Página ").FontSize(10);
                    txt.CurrentPageNumber().FontSize(10);
                    txt.Span(" de ").FontSize(10);
                    txt.TotalPages().FontSize(10);
                });
            });
        }).GeneratePdf();

        //File.WriteAllBytes(@"C:\temp\ProductReport.pdf", pdfByteArray);
        return pdfByteArray;

    }

    public async Task<byte[]> FacturasReportByFechas(DateTime fecha1, DateTime fecha2)
    {
        // Llamar a la función con la fecha2 modificada
        var collection = await _repositoryFactura.FacturasByFechasAsync(fecha1, fecha2.AddDays(1));
        if (collection.Count() == 0)
        {
            // Aquí enviamos la respuesta en lugar del PDF.
            return Encoding.UTF8.GetBytes("No se encontraron facturas registradas.");
        }
        // License config ******  IMPORTANT ******
        QuestPDF.Settings.License = LicenseType.Community;

        // return ByteArrays
        var pdfByteArray = QuestPDF.Fluent.Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.Margin(30);

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().AlignLeft().Text("NFT Sales ").Bold().FontSize(14).Bold();
                        col.Item().AlignLeft().Text($"Fecha del reporte: {DateTime.Now:dd/MM/yyyy} ").FontSize(9);
                        col.Item().LineHorizontal(1f);
                    });
                });

                page.Content().PaddingVertical(10).Column(col1 =>
                {
                    col1.Item().AlignCenter().Text($"Reporte de encabezados de factura por fechas ({fecha1:dd/MM/yyyy} - {fecha2:dd/MM/yyyy})").FontSize(14).Bold();
                    col1.Item().Text("");
                    col1.Item().LineHorizontal(0.5f);

                    col1.Item().Table(tabla =>
                    {
                        tabla.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        tabla.Header(header =>
                        {
                            header.Cell().Background("#164753").Padding(2).AlignCenter().Text("ID Factura").FontColor("#fff");
                            header.Cell().Background("#164753").Padding(2).AlignCenter().Text("Fecha Facturación").FontColor("#fff");
                            header.Cell().Background("#164753").Padding(2).AlignCenter().Text("Total").FontColor("#fff");
                            header.Cell().Background("#164753").Padding(2).AlignCenter().Text("Cliente").FontColor("#fff");
                        });

                        foreach (var item in collection)
                        {
                            //Column 1
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#200d25").Padding(2).Text(item.IdFactura.ToString()).FontSize(10);

                            // Column 2
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#200d25").Padding(2).Text(item.FechaFacturacion?.ToShortDateString()).FontSize(10);

                            // Column 3
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#200d25").Padding(2).Text(item.Total?.ToString()+ " ETH").FontSize(10);

                            //Column 4
                            var cliente = _repositoryCliente.FindByIdAsync((Guid)item.IdCliente);
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#200d25").Padding(2).Text(cliente?.Result.Nombre +" "+cliente?.Result.Apellido1+" "+cliente.Result.Apellido2).FontSize(10);
                        }
                    });
                });

                page.Footer()
                    .AlignRight()
                    .Text(txt =>
                    {
                        txt.Span("Página ").FontSize(10);
                        txt.CurrentPageNumber().FontSize(10);
                        txt.Span(" de ").FontSize(10);
                        txt.TotalPages().FontSize(10);
                    });
            });
        }).GeneratePdf();

        return pdfByteArray;
    }

    public async Task<ICollection<EncabezadoFacturaDTO>> BillsByFechasAsync(DateTime fecha1, DateTime fecha2)
    {
        var collection = await _repositoryFactura.FacturasByFechasAsync(fecha1, fecha2);
        var collectionMapped = _mapper.Map<ICollection<EncabezadoFacturaDTO>>(collection);
        return collectionMapped;
    }

    public async Task<ICollection<EncabezadoFactura>> ListFechasVenta(DateTime fecha1, DateTime fecha2)
    {
        var factura = _repositoryFactura.GetFacturas().GetAwaiter().GetResult();

        List<EncabezadoFactura> lista = new List<EncabezadoFactura>();
        foreach (var item in factura)
        {
            if (item.FechaFacturacion >= fecha1 && item.FechaFacturacion <= fecha2)
            {
                lista.Add(item);
            }
        }
        return lista;

    }
}
