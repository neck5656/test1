namespace StudentInformationManagementSystem.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Navigation property for users with this role
        public virtual ICollection<User> Users { get; set; }
    }
}