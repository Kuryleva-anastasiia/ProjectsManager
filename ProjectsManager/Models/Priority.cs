using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProjectsManager.Models;

[Table("Priority")]
public partial class Priority
{
    [Key]
    [HiddenInput(DisplayValue = false)]
    public int Id { get; set; }

    [DisplayName("Приоритет")]
    [StringLength(50)]
    [Required]
    public string? Name { get; set; }

    [InverseProperty("Priority")]
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
