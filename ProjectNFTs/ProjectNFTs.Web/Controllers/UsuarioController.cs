using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectNFTs.Application.DTOs;
using ProjectNFTs.Application.Services.Implementations;
using ProjectNFTs.Application.Services.Interfaces;
using X.PagedList;
using System.Security.Cryptography;

namespace ProjectNFTs.Web.Controllers;

//[Authorize(Roles = "Admin")]
[Authorize(Roles = "Admin")]
public class UsuarioController : Controller
{
    private readonly IServiceUsuario _serviceUsuario;
    private IServiceRol _serviceRol;

    public UsuarioController(IServiceUsuario serviceUsuario, IServiceRol serviceRol)
    {
        _serviceUsuario = serviceUsuario;
        _serviceRol = serviceRol;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? page)
    {
        //Codigo usado para interceptar una nueva clave Secret
        //using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        //{
        //    byte[] secretKey = new byte[32]; // 256 bits
        //    rng.GetBytes(secretKey);
        //    string secret = BitConverter.ToString(secretKey).Replace("-", "");
        //    Console.WriteLine("Secret: " + secret);
        //}

        var collection = await _serviceUsuario.ListAsync();
        return View(collection.ToPagedList(page ?? 1, 5));
    }

    public async Task<IActionResult> List(string nombre)
    {
        if (string.IsNullOrEmpty(nombre))
            return RedirectToAction("Index");

        var collection = await _serviceUsuario.FindByUsernameAsync(nombre);
        return View("Index", collection.ToPagedList(1, 5));
    }


    // GET: UsuarioController/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.ListRols = await _serviceRol.ListAsync();
        return View();
    }

    // GET:  
    public async Task<IActionResult> Login(string id, string password)
    {
        var @object = await _serviceUsuario.LoginAsync(id, password);
        if (@object == null)
        {
            ViewBag.Message = "Error en Login o Password";
            return View("Login");
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }


    }


    // POST: UsuarioController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioDTO dto)
    {

        ViewBag.ListRols = await _serviceRol.ListAsync();

        if (!ModelState.IsValid)
        {
            // Lee del ModelState todos los errores que
            // vienen para el lado del server
            string errors = string.Join("; ", ModelState.Values
                               .SelectMany(x => x.Errors)
                               .Select(x => x.ErrorMessage));
            return BadRequest(errors);
        }

        await _serviceUsuario.AddAsync(dto);

        // 5- Sleep de 2 segundos para el efecto del loading/progress
        Thread.Sleep(1000);

        return RedirectToAction("Index");
    }

    // GET: UsuarioController/Details/5
    public async Task<IActionResult> _Details(string id)
    {
        var @object = await _serviceUsuario.FindByIdAsync(id);

        // Verificar si IdPais es nulo
        if (@object.IdRol != null)
        {
            // Obtener la descripción del país utilizando el servicio de país
            @object.DescripcionRol = await _serviceRol.ObtenerDescripcionRol(@object.IdRol);
        }
        else
        {
            @object.DescripcionRol = "Rol No Especificado"; // O cualquier valor predeterminado
        }


        return PartialView(@object);
    }


    // GET: UsuarioController/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        var @object = await _serviceUsuario.FindByIdAsync(id);
        ViewBag.ListRols = await _serviceRol.ListAsync();
        return View(@object);
    }

    // POST: UsuarioController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, UsuarioDTO dto)
    {
        await _serviceUsuario.UpdateAsync(id, dto);
        ViewBag.ListRols = await _serviceRol.ListAsync();
        return RedirectToAction("Index");
    }

    // GET: UsuarioController/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        var @object = await _serviceUsuario.FindByIdAsync(id);
        return View(@object);
    }

    // POST: UsuarioController/Delete/5
    [HttpPost]
    public async Task<IActionResult> DeleteConfirm(string id)
    {
        await _serviceUsuario.DeleteAsync(id);

        return RedirectToAction("Index");
    }

    // POST: UsuarioController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id, IFormCollection collection)
    {
        await _serviceUsuario.DeleteAsync(id);
        return RedirectToAction("Index");
    }

    public async Task<JsonResult> GetUsuarioById(string id)
    {
        var tarjeta = await _serviceUsuario.FindByIdAsync(id);
        return Json(tarjeta);
    }
}
