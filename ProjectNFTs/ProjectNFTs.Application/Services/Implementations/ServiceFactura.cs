using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using ProjectNFTs.Application.Config;
using ProjectNFTs.Application.DTOs;
using ProjectNFTs.Application.Services.Interfaces;
using ProjectNFTs.Infraestructure.Models;
using ProjectNFTs.Infraestructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using static iTextSharp.text.pdf.AcroFields;

namespace ProjectNFTs.Application.Services.Implementations;

public class ServiceFactura : IServiceFactura
{
    private readonly IRepositoryFactura _repositoryFactura;
    private readonly IRepositoryCliente _repositoryCliente;
    private readonly IRepositoryNft _repositoryNft;
    private readonly IMapper _mapper;
    private readonly IOptions<AppConfig> _options;
    private readonly ILogger<ServiceFactura> _logger;

    public ServiceFactura(IRepositoryFactura repositoryFactura,
                         IRepositoryCliente repositoryCliente,
                         IRepositoryNft repositoryNft,
                         IMapper mapper,
                         IOptions<AppConfig> options,
                         ILogger<ServiceFactura> logger)
    {
        _repositoryFactura = repositoryFactura;
        _repositoryCliente = repositoryCliente;
        _repositoryNft = repositoryNft;
        _mapper = mapper;
        _options = options;
        _logger = logger;
    }

    public async Task CancelAsync(int id)
    {
        await _repositoryFactura.CancelAsync(id);
    }
    public async Task<int> AddAsync(EncabezadoFacturaDTO dto)
    {
        // Validate Stock availability
        foreach (var item in dto.ListFacturaDetalle)
        {
            var activoNFT = await _repositoryNft.FindByIdAsync((Guid)item.IdNft);

            if (activoNFT.CantidadInventario - item.Cantidad < 0)
            {
                throw new InvalidOperationException($"No hay stock para el NFT {activoNFT.Nombre}, cantidad en stock {activoNFT.CantidadInventario} ");
            }
        }

        var @object = _mapper.Map<EncabezadoFactura>(dto);
        // Find Customer
        var cliente = await _repositoryCliente.FindByIdAsync((Guid)dto.IdCliente);
        // Save Bill
        dto.IdFactura = await _repositoryFactura.AddAsync(@object);
        dto.NombreCliente = cliente.Nombre + " " + cliente.Apellido1;
        // Create PDF Array
        var pdfBytes = await CreatePDFBill(dto.IdFactura);
         
        // Directory exist?        
        if (!Directory.Exists("c:\temp"))
            Directory.CreateDirectory(@"C:\temp");
        // Save it locally
        await File.WriteAllBytesAsync(@"c:\temp\" + dto.IdFactura.ToString().Trim() + ".pdf", pdfBytes);

        // Send email with PDF as Attachment
        await SendEmail(cliente!.Email!, dto, pdfBytes);
        return dto.IdFactura;

        //return await _repositoryFactura.AddAsync(@object);
    }

    public async Task<int> GetNextReceiptNumber()
    {
        int nextReceipt = await _repositoryFactura.GetNextReceiptNumber();
        return nextReceipt + 1;
    }

    public async Task<EncabezadoFacturaDTO> FindByIdAsync(int id)
    {
        var @object = await _repositoryFactura.FindByIdAsync(id);
        var objectMapped = _mapper.Map<EncabezadoFacturaDTO>(@object);
        return objectMapped;
    }

    public async Task<ICollection<EncabezadoFacturaDTO>> ListAsync()
    {
        // Get data from Repository
        var list = await _repositoryFactura.ListAsync();
        // Map List<Cliente> to ICollection<ClienteDTO>
        var collection = _mapper.Map<ICollection<EncabezadoFacturaDTO>>(list);
        // Return Data
        return collection;
    }

    public async Task<ICollection<EncabezadoFacturaDTO>> ListAsyncValid()
    {
        // Get data from Repository
        var list = await _repositoryFactura.ListAsync();
        var validCards = list.Where(t => t.EstadoFactura == 1);      
        // Map List<Cliente> to ICollection<ClienteDTO>
        var collection = _mapper.Map<ICollection<EncabezadoFacturaDTO>>(validCards);
        // Return Data
        return collection;
    }

    private async Task<bool> SendEmail(string email, EncabezadoFacturaDTO factura, byte[] pdf)
    {
        if (string.IsNullOrEmpty(_options.Value.SmtpConfiguration.Server) || string.IsNullOrEmpty(_options.Value.SmtpConfiguration.PortNumber.ToString()))
        {
            _logger.LogError($"No se encuentra configurado ningun valor para SMPT en {MethodBase.GetCurrentMethod()!.DeclaringType!.FullName}");
            return false;
        }
        if (string.IsNullOrEmpty(_options.Value.SmtpConfiguration.UserName) || string.IsNullOrEmpty(_options.Value.SmtpConfiguration.FromName))
        {
            _logger.LogError($"No se encuentra configurado UserName o FromName en appSettings.json (Dev | Prod) {MethodBase.GetCurrentMethod()!.DeclaringType!.FullName}");
            return false;
        }

        // Construir el cuerpo del correo electrónico como un recibo de compra
        StringBuilder bodyBuilder = new StringBuilder();
        bodyBuilder.AppendLine("<h2>¡Gracias por tu compra, " + factura.NombreCliente + "!</h2>");
        bodyBuilder.AppendLine("<p>Detalle de la factura de tu compra:</p>");
        bodyBuilder.AppendLine("<ul>");
        foreach (var detalle in factura.ListFacturaDetalle)
        {
            // Aquí puedes personalizar cómo se muestra cada detalle del producto
            bodyBuilder.AppendLine($"<li><b>{detalle.DescripcionNFT}</b>: {detalle.Cantidad} unidades a {detalle.Precio} ETH </li>");
        }
        bodyBuilder.AppendLine("</ul>");
        bodyBuilder.AppendLine("<p>Total: " + factura.Total + " ETH </p>");
        bodyBuilder.AppendLine("<p>¡Esperamos que disfrutes de tus nuevos NFTs!</p>");

        // Crear el mensaje de correo electrónico
        var mailMessage = new MailMessage(
            new MailAddress(_options.Value.SmtpConfiguration.UserName, _options.Value.SmtpConfiguration.FromName),
            new MailAddress(email))
        {
            Subject = "Recibo de compra de NFTs",
            Body = bodyBuilder.ToString(),
            IsBodyHtml = true
        };

        // Adjuntar el PDF de la factura al correo electrónico
        Attachment attachment = new Attachment(new MemoryStream(pdf), "factura.pdf");
        mailMessage.Attachments.Add(attachment);

        using var smtpClient = new SmtpClient(_options.Value.SmtpConfiguration.Server,
                                              _options.Value.SmtpConfiguration.PortNumber)
        {
            Credentials = new NetworkCredential(_options.Value.SmtpConfiguration.UserName,
                                                _options.Value.SmtpConfiguration.Password),
            EnableSsl = _options.Value.SmtpConfiguration.EnableSsl,
        };

        await smtpClient.SendMailAsync(mailMessage);
        return true;
    }

    private async Task<byte[]> CreatePDFBill(int id)
    {
        var factura = await _repositoryFactura.FindByIdAsync(id);
        QuestPDF.Infrastructure.Image imagen = null;

        // License config ******  IMPORTANT ******
        QuestPDF.Settings.License = LicenseType.Community;

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
                        col.Item().AlignLeft().Text("NFT's ").Bold().FontSize(14).Bold();
                        col.Item().AlignLeft().Text($"Fecha: {DateTime.Now} ").FontSize(9);
                        col.Item().LineHorizontal(1f);
                    });

                });


                page.Content().PaddingVertical(10).Column(col1 =>
                {
                    col1.Item().AlignLeft().Text($"Factura : {factura.IdFactura}").FontSize(12);
                    col1.Item().AlignLeft().Text($"Cliente : {factura.IdClienteNavigation.Nombre} {factura.IdClienteNavigation.Apellido1}").FontSize(12);
                    col1.Item().AlignLeft().Text($"Fecha   : {factura.FechaFacturacion}").FontSize(12);
                    col1.Item().LineHorizontal(0.5f);
                    col1.Item().Text("");
                    col1.Item().Table(async tabla =>
                    {
                        tabla.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(10);
                            columns.RelativeColumn(5);
                            columns.RelativeColumn(10);
                            columns.RelativeColumn(10);
                            columns.RelativeColumn(10);
                        });

                        tabla.Header(header =>
                        {
                            header.Cell().Background("#164753")
                            .Padding(2).AlignCenter().Text("No").FontColor("#fff");

                            header.Cell().Background("#164753")
                           .Padding(2).AlignCenter().Text("Producto").FontColor("#fff");

                            header.Cell().Background("#164753")
                           .Padding(2).AlignCenter().Text("Cantidad").FontColor("#fff");

                            header.Cell().Background("#164753")
                           .Padding(2).AlignCenter().Text("Precio").FontColor("#fff");

                            header.Cell().Background("#164753")
                            .Padding(2).AlignCenter().Text("Imagen").FontColor("#fff");

                            header.Cell().Background("#164753")
                           .Padding(2).AlignCenter().Text("Total").FontColor("#fff");

                        });


                        foreach (var item in factura.DetalleFactura)
                        {                            
                            var nft = await _repositoryNft.FindByIdAsync(item.IdNft);

                            // Column 1
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9")
                            .Padding(2).AlignCenter().Text(item!.IdDetalle.ToString()).FontSize(10);

                            // Column 2
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9")
                           .Padding(2).Text(item.IdNftNavigation.Nombre.ToString()).FontSize(10);

                            // Column 3
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9")
                           .Padding(2).AlignCenter().Text(item.Cantidad.ToString()).FontSize(10);

                            // Column 4
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9")
                            .Padding(2).AlignRight().Text(string.Format("{0:###,##0.00}", item.Precio) + " ETH").FontSize(10);

                            //Column 5
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9")
                            .Padding(2).AlignRight().Image(nft.Imagen);

                            // Column 6
                            var totalLinea = (item.Cantidad * item.Precio);
                            tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9")
                           .Padding(2).AlignRight().Text(string.Format("{0:###,##0.00}", totalLinea) + " ETH").FontSize(10);
                        }

                    });


                    var granTotal = factura.DetalleFactura.Sum(p => (p.Cantidad * p.Precio));

                    col1.Item().AlignRight().Text("Total " + string.Format("{0:###,##0.00}", granTotal) + " ETH").FontSize(12).Bold();

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

        return pdfByteArray!;
    }

}
