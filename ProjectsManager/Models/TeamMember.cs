using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProjectsManager.Models
{
    public partial class TeamMember
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [DisplayName("Команда")]
        public int? TeamId { get; set; }

        [DisplayName("Сотрудник")]
        public int? UserId { get; set; }

        [DisplayName("Команда")]
        [ForeignKey("TeamId")]
        [InverseProperty("TeamMembers")]
        public virtual Team? Team { get; set; }

        [DisplayName("Сотрудник")]
        [ForeignKey("UserId")]
        [InverseProperty("TeamMembers")]
        public virtual User? User { get; set; }
    }
}