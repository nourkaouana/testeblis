using System.ComponentModel.DataAnnotations;


    // Mod�le pour la r�ponse d'authentification
    public class AuthResponse
    {
        // Nom d'utilisateur
        public string? Username { get; set; }
        
        public string? Firstname { get; set; }

        public string? Lastame { get; set; }

        
        // Adresse e-mail de l'utilisateur
        public string? Email { get; set; }
        public DateTime? CreatedDate { get; set; }
        // Jeton d'authentification
        public string? Token { get; set; }
    }

