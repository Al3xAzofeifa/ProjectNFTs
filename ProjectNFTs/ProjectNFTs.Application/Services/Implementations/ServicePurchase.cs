using AutoMapper;
using ProjectNFTs.Application.DTOs;
using ProjectNFTs.Application.Services.Interfaces;
using ProjectNFTs.Infraestructure.Models;
using ProjectNFTs.Infraestructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Application.Services.Implementations;

public class ServicePurchase : IServicePurchase
{
    private readonly IRepositoryPurchase _repository;
    private readonly IMapper _mapper;

    public ServicePurchase(IRepositoryPurchase repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PurchaseDTO> FindOwnerByIdAsync(Guid id)
    {
        var @object = await _repository.FindOwnerByIdAsync(id);
        var objectMapped = _mapper.Map<PurchaseDTO>(@object);
        return objectMapped;

    }

}
