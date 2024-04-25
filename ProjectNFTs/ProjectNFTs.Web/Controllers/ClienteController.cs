using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using ProjectNFTs.Application.DTOs;
using ProjectNFTs.Application.Services.Implementations;
using ProjectNFTs.Application.Services.Interfaces;
using X.PagedList;

namespace ProjectNFTs.Web.Controllers;


public class ClienteController: Controller
{
    private readonly IServiceCliente _serviceCliente;
    private IServicePais _servicePais;

    public ClienteController(IServiceCliente serviceCliente, IServicePais servicePais)
    {
        _serviceCliente = serviceCliente;
        _servicePais = servicePais;
    }

    [Authorize(Roles = "Admin,Processes")]
    [HttpGet]
    public async Task<IActionResult> Index(int? page)
    {
        var collection = await _serviceCliente.ListAsync();
        return View(collection.ToPagedList(page ?? 1, 5));
    }

    [Authorize(Roles = "Admin,Processes")]
    public async Task<IActionResult> List(string nombre)
    {
        if (string.IsNullOrEmpty(nombre))
            return RedirectToAction("Index");

        var collection = await _serviceCliente.FindByDescriptionAsync(nombre);
        return View("Index", collection.ToPagedList(1, 5));
    }

    // GET: ClienteController/Create
    [Authorize(Roles = "Admin,Processes")]
    public async Task<IActionResult> Create()
    {
        ViewBag.ListPais = await _servicePais.ListAsync();
        return View();
    }


    // POST: ClienteController/Create
    [Authorize(Roles = "Admin,Processes")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClienteDTO dto)
    {
        ViewBag.ListPais = await _servicePais.ListAsync();
        
        dto.IdCliente = Guid.NewGuid();

        if (!ModelState.IsValid)
        {
            // Lee del ModelState todos los errores que
            // vienen para el lado del server
            string errors = string.Join("; ", ModelState.Values
                               .SelectMany(x => x.Errors)
                               .Select(x => x.ErrorMessage));
            return BadRequest(errors);
        }
        

        await _serviceCliente.AddAsync(dto);

        // 5- Sleep de 2 segundos para el efecto del loading/progress
        Thread.Sleep(2000);

        return RedirectToAction("Index");

    }


    // GET: ClienteController/Details/5
    [Authorize(Roles = "Admin,Processes")]
    public async Task<IActionResult> _Details(Guid id)
    {
        var clienteDTO = await _serviceCliente.FindByIdAsync(id);

        // Verificar si IdPais es nulo
        if (clienteDTO.IdPais.HasValue)
        {
            // Obtener la descripción del país utilizando el servicio de país
            clienteDTO.DescripcionPais = await _servicePais.ObtenerDescripcionPais(clienteDTO.IdPais.Value);
        }
        else
        {
            clienteDTO.DescripcionPais = "País No Especificado"; // O cualquier valor predeterminado
        }

        return PartialView(clienteDTO);
    }

    // GET: ClienteController/Edit/5
    [Authorize(Roles = "Admin,Processes")]
    public async Task<IActionResult> Edit(Guid id)
    {
        ViewBag.ListPais = await _servicePais.ListAsync();
        var @object = await _serviceCliente.FindByIdAsync(id);

        return View(@object);
    }

    // POST: ClienteController/Edit/5
    [Authorize(Roles = "Admin,Processes")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ClienteDTO dto)
    {

        await _serviceCliente.UpdateAsync(id, dto);

        return RedirectToAction("Index");

    }

    [Authorize(Roles = "Admin,Processes")]
    // GET: ClienteController/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        //Código original generado por la IDE
        var @object = await _serviceCliente.FindByIdAsync(id);
        return View(@object);
    }

    [Authorize(Roles = "Admin,Processes")]
    [HttpPost]
    public async Task<IActionResult> DeleteConfirm(Guid id)
    {
        await _serviceCliente.DeleteAsync(id);
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Admin,Processes")]
    public async Task<JsonResult> GetClienteById(Guid id)
    {
        var cliente = await _serviceCliente.FindByIdAsync(id);
        return Json(cliente);
    }

    [Authorize(Roles = "Admin,Processes,Reports")]
    public async Task<JsonResult> GetClienteByName(string filtro)
    {
        var collection = await _serviceCliente.FindByDescriptionAsync(filtro);
        return Json(collection);
    }
}
