using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Repository.Models;

public partial class Order
{
    [Key]
    public int OrderId { get; set; }

    public int? UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? OrderDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalAmount { get; set; }

    [StringLength(500)]
    public string ShippingAddress { get; set; } = null!;

    [StringLength(100)]
    public string ReceiverName { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string ReceiverPhone { get; set; } = null!;

    [StringLength(50)]
    public string? OrderStatus { get; set; }

    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [ForeignKey("UserId")]
    [InverseProperty("Orders")]
    public virtual User? User { get; set; }
}
