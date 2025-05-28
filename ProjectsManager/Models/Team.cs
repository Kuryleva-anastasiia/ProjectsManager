using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProjectsManager.Models;

public partial class Team
{
    [Key]
    [HiddenInput(DisplayValue = false)]
    public int Id { get; set; }

    [DisplayName("Название")]
    [StringLength(100, ErrorMessage = "Максимальная длина 100 символов")]
    [Required(ErrorMessage = "Название обязательно для заполнения")]
    public string? Name { get; set; }

    [DisplayName("Дата создания")]
    [DataType(DataType.Date)]
    [Required(ErrorMessage = "Дата обязательна для заполнения")]
    public DateOnly? CreationDate { get; set; }

    [InverseProperty("Team")]
    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

    [InverseProperty("Team")]
    public virtual ICollection<TeamsProject> TeamsProjects { get; set; } = new List<TeamsProject>();
}
