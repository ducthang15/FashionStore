using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Repository.Models;

[Index("OldUrl", Name = "IX_Redirects_OldUrl", IsUnique = true)]
public partial class Redirect
{
    [Key]
    public int Id { get; set; }

    [StringLength(500)]
    public string OldUrl { get; set; } = null!;

    [StringLength(500)]
    public string NewUrl { get; set; } = null!;

    public bool IsPermanent { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }
}
