using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class TicketModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Title")]
        public string Title { get; set; }

        [Required]
        [DisplayName("Description")]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Created")]
        public DateTimeOffset Created { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Updated")]
        public DateTimeOffset? Updated { get; set; }

        [DisplayName("Archived")]
        public bool Archived { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }

        [DisplayName("Ticket Type")]
        public int TicketTypeId { get; set; }

        [DisplayName("Ticket Priority")]
        public int TicketPriorityId { get; set; }

        [DisplayName("Ticket Status")]
        public int TicketStatusId { get; set; }

        [DisplayName("Ticket Owner")]
        public string OwnerUserId { get; set; }

        [DisplayName("Ticket Developer")]
        public string DeveloperUserId { get; set; }


        // Navigation properties
        // virtual allows EF Core to do lazy loading and modify tracking
        public virtual ProjectModel Project { get; set; }
        public virtual TicketTypeModel TicketType { get; set; }
        public virtual TicketPriorityModel TicketPriority { get; set; }
        public virtual TicketStatusModel TicketStatus { get; set; }
        public virtual BTUserModel OwnerUser { get; set; }
        public virtual BTUserModel DeveloperUser { get; set; }

        public virtual ICollection<TicketCommentModel> Comments { get; set; } = new HashSet<TicketCommentModel>();
        public virtual ICollection<TicketAttachmentModel> Attachments { get; set; } = new HashSet<TicketAttachmentModel>();
        public virtual ICollection<TicketHistoryModel> History { get; set; } = new HashSet<TicketHistoryModel>();
        public virtual ICollection<NotificationModel> Notifications { get; set; } = new HashSet<NotificationModel>();
    }
}
