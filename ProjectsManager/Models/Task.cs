using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProjectsManager.Models;

public partial class Task
{
    [Key]
    [HiddenInput(DisplayValue = false)]
    public int Id { get; set; }

    [DisplayName("Название")]
    [StringLength(50)]
    [Required]
    public string? Title { get; set; }

    [Column(TypeName = "ntext")]
    [DisplayName("Описание")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    [DisplayName("Проект")]
    public int? ProjectId { get; set; }

    [DisplayName("Ответственный")]
    public int? AssignedUserId { get; set; }

    [DisplayName("Статус")]
    public int? StatusId { get; set; }

    [DisplayName("Приоритет")]
    public int? PriorityId { get; set; }

    [DisplayName("Дата создания")]
    [DataType(DataType.Date)]
    [Required]
    public DateOnly? CreationDate { get; set; }

    [DisplayName("Дата выполнения")]
    [DataType(DataType.Date)]
    [Required]
    public DateOnly? CompletionDate { get; set; }

    [DisplayName("Ответственный")]
    [ForeignKey("AssignedUserId")]
    [InverseProperty("Tasks")]
    public virtual User? AssignedUser { get; set; }

    [DisplayName("Приоритет")]
    [ForeignKey("PriorityId")]
    [InverseProperty("Tasks")]
    public virtual Priority? Priority { get; set; }

    [DisplayName("Проект")]
    [ForeignKey("ProjectId")]
    [InverseProperty("Tasks")]
    public virtual Project? Project { get; set; }

    [DisplayName("Статус")]
    [ForeignKey("StatusId")]
    [InverseProperty("Tasks")]
    public virtual Status? Status { get; set; }
}
