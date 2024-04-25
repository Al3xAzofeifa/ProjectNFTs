using ProjectNFTs.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Infraestructure.Repository.Interfaces;

public interface IRepositoryRol
{
    Task<ICollection<RolUsuario>> FindByDescriptionAsync(string description);
    Task<ICollection<RolUsuario>> ListAsync();
    Task<RolUsuario> FindByIdAsync(int id);
    Task<int> AddAsync(RolUsuario entity);
    Task DeleteAsync(int id);
    Task UpdateAsync(int id, RolUsuario entity);
}
