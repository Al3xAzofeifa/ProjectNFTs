using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProjectNFTs.Web.Controllers;

[Authorize(Roles = "Admin,Processes,Reports")]
public class CalificacionController : Controller
{
    // GET: CalificacionController
    public ActionResult Index()
    {
        return View();
    }       
}
