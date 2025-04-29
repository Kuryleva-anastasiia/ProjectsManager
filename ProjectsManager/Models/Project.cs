using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProjectsManager.Models;

public partial class Project
{
    [Key]
    [HiddenInput(DisplayValue = false)]
    public int Id { get; set; }

    [StringLength(100)]
    [DisplayName("Название")]
    [Required]
    public string? Name { get; set; }

    [Column(TypeName = "ntext")]
    [DisplayName("Описание")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    [DisplayName("Руководитель проекта")]
    public int? UserId { get; set; }

    [DisplayName("Дата")]
    [DataType(DataType.Date)]
    [Required]
    public DateOnly? CreationDate { get; set; }

    [InverseProperty("Project")]
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    [InverseProperty("Project")]
    public virtual ICollection<TeamsProject> TeamsProjects { get; set; } = new List<TeamsProject>();

    [ForeignKey("UserId")]
    [InverseProperty("Projects")]
    public virtual User? User { get; set; }
}
