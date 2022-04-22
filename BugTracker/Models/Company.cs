﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Company Name")]
        public string Name { get; set; }

        [DisplayName("Company Description")]
        public string? Description { get; set; }


        // Navigation properties
        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();
        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();
        public virtual ICollection<Invite> Invites { get; set; } = new HashSet<Invite>();
    }
}
