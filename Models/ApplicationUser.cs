using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using testbills.Enums;
using static System.Net.Mime.MediaTypeNames;


namespace testbills.Models
{
    // Classe représentant un utilisateur de l'application
    public class ApplicationUser : IdentityUser
    {
        // Propriété représentant le rôle de l'utilisateur
        public Role Role { get; set; }
        [Column(TypeName = "TEXT")]
        public required string Firstname { get; set; }
        [Column(TypeName = "TEXT")]
        public required string Lastname { get; set; }

        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
    }

}