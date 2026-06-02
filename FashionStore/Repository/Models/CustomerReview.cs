using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Repository.Models;

public partial class CustomerReview
{
    [Key]
    public int ReviewId { get; set; }

    [StringLength(200)]
    public string CustomerName { get; set; } = null!;

    [StringLength(200)]
    public string? Profession { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    public int Rating { get; set; }

    public string ReviewContent { get; set; } = null!;

    [StringLength(500)]
    public string? AvatarImage { get; set; }

    [StringLength(500)]
    public string? SuitImage { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsPublished { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }
}
