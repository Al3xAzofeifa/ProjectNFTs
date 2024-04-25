using AutoMapper;
using ProjectNFTs.Application.DTOs;
using ProjectNFTs.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Application.Profiles;

public class UsuarioProfile : Profile
{
    public UsuarioProfile() {
        CreateMap<UsuarioDTO, Usuario>();

        CreateMap<Usuario, UsuarioDTO>()
            .ForMember(dest => dest.Login, orig => orig.MapFrom(x => x.Login))
            .ForMember(dest => dest.Nombre, orig => orig.MapFrom(x => x.Nombre))
            .ForMember(dest => dest.Apellido, orig => orig.MapFrom(x => x.Apellido))
            .ForMember(dest => dest.Estado, orig => orig.MapFrom(x => x.Estado))
            .ForMember(dest => dest.IdRol, orig => orig.MapFrom(x => x.IdRol))
            .ForMember(dest => dest.DescripcionRol, orig => orig.MapFrom(x => x.IdRolNavigation.Descripcion));
    }
}
