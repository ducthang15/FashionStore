using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Repository.Models;

public partial class ProductImage
{
    [Key]
    public int ImageId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductImages")]
    public virtual Product Product { get; set; } = null!;
}
