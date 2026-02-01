using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Repository.Models;

public partial class News
{
    [Key]
    public int NewsId { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    [StringLength(500)]
    public string? Summary { get; set; }

    public string? Content { get; set; }

    [StringLength(255)]
    public string? ImageUrl { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    public bool IsPublished { get; set; }

    public int? NewsCategoryId { get; set; }

    [ForeignKey("NewsCategoryId")]
    [InverseProperty("News")]
    public virtual NewsCategory? NewsCategory { get; set; }
}
