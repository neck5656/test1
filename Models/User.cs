using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentInformationManagementSystem.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string Salt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastLogin { get; set; }

        public bool IsActive { get; set; }

        // Foreign key for Role
        public int RoleId { get; set; }

        // Navigation property for Role
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        // We'll implement proper relationships after creating the Student class
        // For now, we can comment this out
        // public virtual Student StudentProfile { get; set; }
    }
}