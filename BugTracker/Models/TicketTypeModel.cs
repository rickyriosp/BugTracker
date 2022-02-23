using System.ComponentModel;

namespace BugTracker.Models
{
    public class TicketTypeModel
    {
        public int Id { get; set; }

        [DisplayName("Type Name")]
        public string Name { get; set; }
    }
}
