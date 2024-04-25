using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectNFTs.Application.DTOs;
using ProjectNFTs.Application.Services.Interfaces;
using ProjectNFTs.Infraestructure.Models;
using ProjectNFTs.Infraestructure.Repository.Interfaces;
using X.PagedList;

namespace ProjectNFTs.Web.Controllers;

[Authorize(Roles = "Admin,Processes")]
public class PaisController : Controller
{
    private readonly IServicePais _servicePais;

    public PaisController(IServicePais servicePais)
    {
        _servicePais = servicePais;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? page)
    {
        var collection = await _servicePais.ListAsync();            
        return View(collection.ToPagedList(page ?? 1, 5));
    }

    // GET: PaisController/Create
    public IActionResult Create()
    {
        return View();
    }

    public async Task<IActionResult> List(string nombre)
    {
        if (string.IsNullOrEmpty(nombre))
            return RedirectToAction("Index");

        var collection = await _servicePais.FindByDescriptionAsync(nombre);
        return View("Index", collection.ToPagedList(1, 5));
    }

    // POST: PaisController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PaisDTO dto)
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

        await _servicePais.AddAsync(dto);

        // 5- Sleep de 2 segundos para el efecto del loading/progress
        Thread.Sleep(1000);

        return RedirectToAction("Index");

    }


    // GET: PaisController/Details/5
    public async Task<IActionResult> _Details(int id)
    {
        var @object = await _servicePais.FindByIdAsync(id);

        return View(@object);
    }

    // GET: PaisController/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var @object = await _servicePais.FindByIdAsync(id);

        return View(@object);
    }

    // POST: PaisController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PaisDTO dto)
    {
        Thread.Sleep(1000);
        await _servicePais.UpdateAsync(id, dto);

        return RedirectToAction("Index");

    }

    // GET: PaisController/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        //Codigo original
        var @object = await _servicePais.FindByIdAsync(id);
        return View(@object);
    }

    // POST: PaisController/Delete/5
    [HttpPost]        
    public async Task<IActionResult> DeleteConfirm(int id)
    {
        await _servicePais.DeleteAsync(id);

        return RedirectToAction("Index");
    }

    public async Task<JsonResult> GetPaisById(int id)
    {
        var pais = await _servicePais.FindByIdAsync(id);
        return Json(pais);
    }
}
