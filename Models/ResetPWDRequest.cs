using System.ComponentModel.DataAnnotations;


    // Modèle pour la demande d'inscription
    public class ResetPWDRequest
    {
        // Adresse e-mail de l'utilisateur (obligatoire)
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Newpassword { get; set; }
        [Required]
        public string? username { get; set; }
    }

