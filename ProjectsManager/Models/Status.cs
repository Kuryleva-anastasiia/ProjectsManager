using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProjectsManager.Models;

[Table("Status")]
public partial class Status
{
    [Key]
    [HiddenInput(DisplayValue = false)]
    public int Id { get; set; }

    [DisplayName("Статус")]
    [StringLength(50)]
    [Required(ErrorMessage = "Название обязательно для заполнения")]
    public string? Name { get; set; }

    [InverseProperty("Status")]
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
