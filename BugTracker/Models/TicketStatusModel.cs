using System.ComponentModel;

namespace BugTracker.Models
{
    public class TicketStatusModel
    {
        public int Id { get; set; }

        [DisplayName("Status Name")]
        public string Name { get; set; }
    }
}
