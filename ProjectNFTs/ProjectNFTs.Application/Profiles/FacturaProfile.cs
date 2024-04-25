using AutoMapper;
using ProjectNFTs.Application.DTOs;
using ProjectNFTs.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Application.Profiles;

public class FacturaProfile : Profile
{
    public FacturaProfile()
    {
        CreateMap<EncabezadoFacturaDTO, EncabezadoFactura>().ReverseMap();
        CreateMap<DetalleFacturaDTO, DetalleFactura>().ReverseMap();

        CreateMap<EncabezadoFacturaDTO, EncabezadoFactura>()
             .ForMember(dest => dest.IdFactura, orig => orig.MapFrom(x => x.IdFactura))
             .ForMember(dest => dest.IdTarjeta, orig => orig.MapFrom(x => x.IdTarjeta))
             .ForMember(dest => dest.IdCliente, orig => orig.MapFrom(x => x.IdCliente))
             .ForMember(dest => dest.DetalleFactura, orig => orig.MapFrom(x => x.ListFacturaDetalle)).ReverseMap();
    }
}
