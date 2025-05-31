// Models/User.cs
using System;
using System.ComponentModel.DataAnnotations; // <-- Make sure this is here

namespace UserManagementAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")] // <-- *Crucially*, make sure this is here
        public string? Name { get; set; } // <-- Make sure there is *NO* '?' here

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; } // Make sure there is *NO* '?' here

        // ...
    }
}