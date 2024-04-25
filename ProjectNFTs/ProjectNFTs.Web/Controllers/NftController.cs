using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectNFTs.Application.DTOs;
using ProjectNFTs.Application.Services.Implementations;
using ProjectNFTs.Application.Services.Interfaces;
using ProjectNFTs.Infraestructure.Models;
using X.PagedList;

namespace ProjectNFTs.Web.Controllers;


public class NftController : Controller
{
    private readonly IServiceNft _serviceNft;

    public NftController(IServiceNft serviceNft)
    {
        _serviceNft = serviceNft;
    }

    [Authorize(Roles = "Admin,Processes")]
    [HttpGet]
    public async Task<IActionResult> Index(int? page)
    {           
        var collection = await _serviceNft.ListAsync();

        return View(collection.ToPagedList(page ?? 1,5));
    }

    [Authorize(Roles = "Admin,Processes,Reports")]
    [HttpGet]
    public async Task<IActionResult> _List()
    {
        var collection = await _serviceNft.ListAsync();
        return View(collection);
    }

    [Authorize(Roles = "Admin,Processes")]
    public async Task<IActionResult> List(string nombre)
    {
        if (string.IsNullOrEmpty(nombre))
            return RedirectToAction("Index");

        var collection = await _serviceNft.FindByAutorAsync(nombre);        
        return View("Index", collection.ToPagedList(1, 5));
    }

    [Authorize(Roles = "Admin,Processes")]
    // GET: NftController/Create
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "Admin,Processes,Reports")]
    [HttpGet]
    public async Task<IActionResult> ObtenerImagen(Guid id)
    {
        var nft = await _serviceNft.FindByIdAsync(id);
        if (nft == null || nft.Imagen == null)
        {
            return NotFound(); // Devolver un error 404 si la imagen no se encuentra
        }

        return File(nft.Imagen, "image/jpeg"); // Devolver la imagen como un archivo JPEG
    }

    [Authorize(Roles = "Admin,Processes")]
    // POST: NftController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NftDTO dto, IFormFile imageFile)
    {
        dto.Id = Guid.NewGuid();

        MemoryStream target = new MemoryStream();


        if (dto.Imagen == null)
        {
            if (imageFile != null)
            {
                imageFile.OpenReadStream().CopyTo(target);

                dto.Imagen = target.ToArray();
                ModelState.Remove("Imagen");
            }
        }

        if (!ModelState.IsValid)
        {
            // Lee del ModelState todos los errores que
            // vienen para el lado del server
            string errors = string.Join("; ", ModelState.Values
                               .SelectMany(x => x.Errors)
                               .Select(x => x.ErrorMessage));
            return BadRequest(errors);
        }

   

        await _serviceNft.AddAsync(dto);

        // 5- Sleep de 2 segundos para el efecto del loading/progress
        Thread.Sleep(1000);

        return RedirectToAction("Index");

    }


    // GET: NftController/Details/5
    [Authorize(Roles = "Admin,Processes")]
    public async Task<IActionResult> _Details(Guid id)
    {
        var @object = await _serviceNft.FindByIdAsync(id);

        return View(@object);
    }

    // GET: NftController/Edit/5
    [Authorize(Roles = "Admin,Processes")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var @object = await _serviceNft.FindByIdAsync(id);

        return View(@object);
    }

    // POST: NftController/Edit/5
    [Authorize(Roles = "Admin,Processes")]
    [HttpPost]        
    public async Task<IActionResult> Edit(Guid id, NftDTO dto, IFormFile imageFile)
    {
         MemoryStream target = new MemoryStream();

        if (dto.Imagen == null)
        {
            if (imageFile != null)
            {
                imageFile.OpenReadStream().CopyTo(target);

                dto.Imagen = target.ToArray();
                ModelState.Remove("Imagen");
            }
            else {    
                //Se realiza la busqueda para conservar la imagen, ya que al no existir una etiqueta input del lado
                //de la vista, en el envio del formulario el campo dto.imagen siempre llega nulo.
                var nft = await _serviceNft.FindByIdAsync(id);
                dto.Imagen = nft.Imagen;
            }
        }

        Thread.Sleep(1000);
        await _serviceNft.UpdateAsync(id, dto);

        return RedirectToAction("Index");

    }

    // GET: NftController/Delete/5
    [Authorize(Roles = "Admin,Processes")]
    public async Task<IActionResult> Delete(Guid id)
    {
        //Codigo original
        var @object = await _serviceNft.FindByIdAsync(id);
        return View(@object);
    }

    // POST: NftController/Delete/5
    [Authorize(Roles = "Admin,Processes")]
    [HttpPost]
    public async Task<IActionResult> DeleteConfirm(Guid id)
    {
        await _serviceNft.DeleteAsync(id);

        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Admin,Processes")]
    public async Task<JsonResult> GetNftById(Guid id)
    {
        var nft = await _serviceNft.FindByIdAsync(id);
        return Json(nft);
    }

    [Authorize(Roles = "Admin,Processes,Reports")]
    public async Task<IActionResult> GetNftByName(string filtro)
    {
        var collection = await _serviceNft.FindByAutorAsync(filtro);
        return Json(collection);
    }

}
