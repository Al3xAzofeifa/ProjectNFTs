using ProjectNFTs.Infraestructure.Models;

namespace ProjectNFTs.Web.Models;

public class IndexViewModel
{
    public List<Pais> Paises { get; set; }
    public int PageSize { get; set; }
    public int PageIndex { get; set; }
    public int TotalPages { get; set; }
}