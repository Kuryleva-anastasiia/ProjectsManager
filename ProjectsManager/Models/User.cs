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

    [StringLength(100, ErrorMessage = "Максимальная длина 100 символов")]
    [DisplayName("Фамилия")]
    [Required(ErrorMessage = "Фамилия обязательна для заполнения")]
    public string? LastName { get; set; }

    [StringLength(100, ErrorMessage = "Максимальная длина 100 символов")]
    [DisplayName("Имя")]
    [Required(ErrorMessage = "Имя обязательно для заполнения")]
    public string? Name { get; set; }

    [StringLength(100, ErrorMessage = "Максимальная длина 100 символов")]
    [DisplayName("Отчество")]
    public string? Patronymic { get; set; }

    [StringLength(12, ErrorMessage = "Необходимый формат: +79997776655")]
    [DisplayName("Телефон")]
    [Required(ErrorMessage = "Телефон обязателен для заполнения")]
    [RegularExpression(@"^[+]7+[0-9]+$", ErrorMessage = "Необходимый формат: +79997776655")]
    [DataType(DataType.PhoneNumber, ErrorMessage = "Необходимый формат: +79997776655")]
    public string? Phone { get; set; }

    [DisplayName("Email")]
    [EmailAddress(ErrorMessage = "Необходимый формат : user@yandex.ru")]
    [Required(ErrorMessage = "Почта обязательна для заполнения")]
    public string? Login { get; set; }

    [DisplayName("Пароль")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Пароль обязателен для заполнения")]
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
