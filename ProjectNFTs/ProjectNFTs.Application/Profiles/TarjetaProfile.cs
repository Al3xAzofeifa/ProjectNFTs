using AutoMapper;
using ProjectNFTs.Application.DTOs;
using ProjectNFTs.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Application.Profiles;

public class TarjetaProfile : Profile
{
    public TarjetaProfile()
    {
        CreateMap<TarjetaDTO, Tarjeta>().ReverseMap();
    }
}
