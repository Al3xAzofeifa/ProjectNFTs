using ProjectNFTs.Application.DTOs;
using ProjectNFTs.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ProjectNFTs.Infraestructure.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectNFTs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NftController : Controller
{
    private readonly IServiceNft _serviceNft;
    private readonly IServicePurchase _servicePurchase;
    private readonly IServiceCliente _serviceCliente;

    public NftController(IServiceNft servicioNft, IServicePurchase servicePurchase, IServiceCliente serviceCliente)
    {
        _serviceNft = servicioNft;
        _servicePurchase = servicePurchase;
        _serviceCliente = serviceCliente;
    }

    [HttpGet("Nft")]
    public async Task<IActionResult> GetNft()
    {
        var collection = await _serviceNft.ListAsync();

        var result = collection.Select(nft => new
        {
            nft.Id,
            nft.Nombre,
            nft.Imagen
        })
        .Where(nft => nft.Id != null && nft.Nombre != null && nft.Imagen != null);

        return Ok(result);
    }

    [HttpGet("Nft/descripcion/{descripcion}")]
    public async Task<IActionResult> GetNftByDescription(string descripcion)
    {
        var collection = await _serviceNft.FindByDescriptionAsync(descripcion);
        var result = new List<object>();

        foreach (var item in collection)
        {
            var purchase = await _servicePurchase.FindOwnerByIdAsync(item.Id);

            if (purchase != null)
            {
                var cliente = await _serviceCliente.FindByIdAsync(purchase.CustomerId);

                if (cliente != null)
                {
                    result.Add(new
                    {
                        item.Id,
                        item.Nombre,
                        item.Imagen,
                        Propietario = new
                        {
                            cliente.IdCliente,
                            cliente.Nombre,
                            cliente.Apellido1,
                            cliente.Apellido2,
                            cliente.IdPais,
                            cliente.Email,
                            cliente.Sexo,
                            cliente.FechaDeNacimiento
                        }
                    });
                }
            }
        }

        if (result.Any())
        {
            return Ok(result);
        }
        else
        {
            return NotFound($"No existen elementos con la descripción {descripcion}");
        }
    }

}