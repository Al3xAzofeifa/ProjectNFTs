using System;
using System.Collections.Generic;

namespace ProjectNFTs.Infraestructure.Models;

public partial class RolUsuario
{
    public int Id { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Usuario> Usuario { get; set; } = new List<Usuario>();
}
