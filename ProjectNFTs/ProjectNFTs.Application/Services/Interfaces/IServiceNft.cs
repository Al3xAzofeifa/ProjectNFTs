﻿using ProjectNFTs.Application.DTOs;
using ProjectNFTs.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Application.Services.Interfaces;

public interface IServiceNft
{
    Task<ICollection<NftDTO>> FindByDescriptionAsync(string description);
    Task<ICollection<NftDTO>> FindByAutorAsync(string description);    
    Task<ICollection<NftDTO>> ListAsync();
    Task<NftDTO> FindByIdAsync(Guid id);
    Task<Guid> AddAsync(NftDTO dto);
    Task DeleteAsync(Guid id);
    Task UpdateAsync(Guid id, NftDTO dto);
}
