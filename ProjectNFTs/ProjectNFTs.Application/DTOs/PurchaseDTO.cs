using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNFTs.Application.DTOs;

public class PurchaseDTO
{
    public int PurchaseId { get; set; }

    public DateTime Date { get; set; }

    public Guid IdNft { get; set; }

    public Guid CustomerId { get; set; }

    public bool Status { get; set; }
}
