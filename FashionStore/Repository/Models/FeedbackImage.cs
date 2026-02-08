using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Repository.Models;

public partial class FeedbackImage
{
    [Key]
    public int ImageId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public int FeedbackId { get; set; }

    [ForeignKey("FeedbackId")]
    [InverseProperty("FeedbackImages")]
    public virtual CustomerFeedback Feedback { get; set; } = null!;
}
