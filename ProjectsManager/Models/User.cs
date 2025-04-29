using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProjectsManager.Models;

public partial class User
{
    [Key]
    [HiddenInput(DisplayValue = false)]
    public int Id { get; set; }

    [StringLength(100)]
    [DisplayName("Фамилия")]
    public string? LastName { get; set; }

    [StringLength(100)]
    [DisplayName("Имя")]
    public string? Name { get; set; }

    [StringLength(100)]
    [DisplayName("Отчество")]
    public string? Patronymic { get; set; }

    [StringLength(12)]
    [DisplayName("Телефон")]
    [Required]
    [DataType(DataType.PhoneNumber)]
    public string? Phone { get; set; }

    [DisplayName("Email")]
    [DataType(DataType.EmailAddress)]
    [Required]
    public string? Login { get; set; }

    [DisplayName("Пароль")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [StringLength(50)]
    [DisplayName("Роль")]
    public string? Role { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    [InverseProperty("AssignedUser")]
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    [InverseProperty("User")]
    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}
