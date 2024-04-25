using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectNFTs.Application.DTOs;
using ProjectNFTs.Application.Services.Interfaces;
using X.PagedList;

namespace ProjectNFTs.Web.Controllers;
[Authorize(Roles = "Admin,Processes")]
public class RolUsuarioController : Controller
{
    private readonly IServiceRol _serviceRol;

    public RolUsuarioController(IServiceRol serviceRol)
    {
        _serviceRol = serviceRol;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? page)
    {
        var collection = await _serviceRol.ListAsync();
        return View(collection.ToPagedList(page ?? 1, 5));
    }

    // GET: RolController/Create
    public IActionResult Create()
    {
        return View();
    }


    // POST: RolController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RolDTO dto)
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

        await _serviceRol.AddAsync(dto);

        // 5- Sleep de 2 segundos para el efecto del loading/progress
        Thread.Sleep(1000);

        return RedirectToAction("Index");

    }

    // GET: RolController/Details/5
    public async Task<IActionResult> _Details(int id)
    {
        var @object = await _serviceRol.FindByIdAsync(id);

        return View(@object);
    }

    // GET: RolController/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var @object = await _serviceRol.FindByIdAsync(id);

        return View(@object);
    }

    // POST: RolController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RolDTO dto)
    {
        Thread.Sleep(1000);
        await _serviceRol.UpdateAsync(id, dto);

        return RedirectToAction("Index");

    }

    // GET: RolController/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        //Codigo original
        var @object = await _serviceRol.FindByIdAsync(id);
        return View(@object);
    }

    // POST: RolController/Delete/5
    [HttpPost]
    public async Task<IActionResult> DeleteConfirm(int id)
    {
        await _serviceRol.DeleteAsync(id);

        return RedirectToAction("Index");
    }

    public async Task<JsonResult> GetRolById(int id)
    {
        var Rol = await _serviceRol.FindByIdAsync(id);
        return Json(Rol);
    }
}

