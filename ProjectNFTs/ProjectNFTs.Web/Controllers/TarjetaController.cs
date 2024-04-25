using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectNFTs.Application.DTOs;
using ProjectNFTs.Application.Services.Interfaces;
using X.PagedList;

namespace ProjectNFTs.Web.Controllers;

[Authorize(Roles = "Admin,Processes")]
public class TarjetaController : Controller
{
    private readonly IServiceTarjeta _serviceTarjeta;

    public TarjetaController(IServiceTarjeta serviceTarjeta)
    {
        _serviceTarjeta = serviceTarjeta;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? page)
    {
        var collection = await _serviceTarjeta.ListAsync();
        return View(collection.ToPagedList(page ?? 1, 5));
    }

    // GET: TarjetaController/Create
    public IActionResult Create()
    {
        return View();
    }

    public async Task<IActionResult> List(string nombre)
    {
        if (string.IsNullOrEmpty(nombre))
            return RedirectToAction("Index");

        var collection = await _serviceTarjeta.FindByDescriptionAsync(nombre);
        return View("Index", collection.ToPagedList(1, 5));
    }


    // POST: TarjetaController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TarjetaDTO dto)
    {

        if (!ModelState.IsValid)
        {
            // Lee del ModelState todos los errores que
            // vienen para el lado del server
            string errors = string.Join("; ", ModelState.Values
                               .SelectMany(x => x.Errors)
                               .Select(x => x.ErrorMessage));
            return BadRequest(errors);
        }

        // Verificar si el ID ya existe
        var existingTarjeta = await _serviceTarjeta.FindByIdAsync(dto.Id);
        if (existingTarjeta != null)
        {

            // Mensaje de error personalizado
            string errorMessage = "El ID ya existe en la base de datos.";

            // Devuelve un JsonResult con el mensaje de error
            return Json(new { success = false, message = errorMessage });
        }

        await _serviceTarjeta.AddAsync(dto);

        // 5- Sleep de 2 segundos para el efecto del loading/progress
        Thread.Sleep(1000);

        return RedirectToAction("Index");

    }


    // GET: TarjetaController/Details/5
    public async Task<IActionResult> _Details(int id)
    {
        var @object = await _serviceTarjeta.FindByIdAsync(id);

        return PartialView(@object);
    }

    // GET: TarjetaController/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var @object = await _serviceTarjeta.FindByIdAsync(id);

        return View(@object);
    }

    // POST: TarjetaController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TarjetaDTO dto)
    {
        Thread.Sleep(1000);
        await _serviceTarjeta.UpdateAsync(id, dto);

        return RedirectToAction("Index");

    }

    // GET: TarjetaController/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        //Codigo original
        var @object = await _serviceTarjeta.FindByIdAsync(id);
        return View(@object);
    }

    // POST: TarjetaController/Delete/5
    [HttpPost]
    public async Task<IActionResult> DeleteConfirm(int id)
    {
        await _serviceTarjeta.DeleteAsync(id);

        return RedirectToAction("Index");
    }

    public async Task<JsonResult> GetTarjetaById(int id)
    {
        var tarjeta = await _serviceTarjeta.FindByIdAsync(id);
        return Json(tarjeta);
    }
}
