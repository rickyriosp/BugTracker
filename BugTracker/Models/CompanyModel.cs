using System.ComponentModel;

namespace BugTracker.Models
{
    public class CompanyModel
    {
        public int Id { get; set; }

        [DisplayName("Company Name")]
        public string Name { get; set; }

        [DisplayName("Company Description")]
        public string Description { get; set; }


        // Navigation properties
        public virtual ICollection<BTUserModel> Members { get; set; } = new HashSet<BTUserModel>();
        public virtual ICollection<ProjectModel> Projects { get; set; } = new HashSet<ProjectModel>();
    }
}
