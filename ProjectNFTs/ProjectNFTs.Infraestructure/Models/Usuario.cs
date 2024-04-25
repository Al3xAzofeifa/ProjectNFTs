using System;
using System.Collections.Generic;

namespace ProjectNFTs.Infraestructure.Models;

public partial class Usuario
{
    public string Login { get; set; } = null!;

    public int? IdRol { get; set; }

    public string? Password { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public bool? Estado { get; set; }

    public virtual RolUsuario? IdRolNavigation { get; set; }
}
