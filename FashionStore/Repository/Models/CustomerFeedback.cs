using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Repository.Models;

public partial class CustomerFeedback
{
    [Key]
    public int FeedbackId { get; set; }

    [StringLength(100)]
    public string? CustomerName { get; set; }

    public string? ImageUrl { get; set; }

    public string? Content { get; set; }

    [StringLength(200)]
    public string? Tags { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    public bool? IsPublished { get; set; }

    [InverseProperty("Feedback")]
    public virtual ICollection<FeedbackImage> FeedbackImages { get; set; } = new List<FeedbackImage>();
}
