using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProjectsManager.Models;

public partial class TeamsProject
{
    [Key]
    [HiddenInput(DisplayValue = false)]
    public int Id { get; set; }

    [DisplayName("Проект")]
    public int? ProjectId { get; set; }

    [DisplayName("Команда")]
    public int? TeamId { get; set; }

    [DisplayName("Проект")]
    [ForeignKey("ProjectId")]
    [InverseProperty("TeamsProjects")]
    public virtual Project? Project { get; set; }

    [DisplayName("Команда")]
    [ForeignKey("TeamId")]
    [InverseProperty("TeamsProjects")]
    public virtual Team? Team { get; set; }
}
