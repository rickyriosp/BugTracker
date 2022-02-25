using System.ComponentModel;

namespace BugTracker.Models
{
    public class TicketCommentModel
    {
        public int Id { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [DisplayName("Member Comment")]
        public string Comment { get; set; }

        [DisplayName("Date Created")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("Date Modified")]
        public DateTimeOffset Updated { get; set; }

        [DisplayName("Team Member")]
        public string UserId { get; set; }


        // Navigation properties
        public virtual Ticket Ticket { get; set; }
        public virtual BTUserModel User { get; set; }
    }
}
