using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ProjectNFTs.Infraestructure.Models;

public partial class Purchase
{
    [DisplayName("Id")]
    public int PurchaseId { get; set; }

    [DisplayName("Fecha")]
    public DateTime Date { get; set; }

    [DisplayName("NFT")]
    public Guid IdNft { get; set; }

    [DisplayName("Propietario")]
    public Guid CustomerId { get; set; }

    [DisplayName("Estado")]
    public bool Status { get; set; }
}
