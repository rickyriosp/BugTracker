﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class TicketPriority
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Priority Name")]
        public string Name { get; set; }
    }
}
