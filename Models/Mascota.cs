using System.ComponentModel.DataAnnotations;

namespace PawCareMVC.Models
{
    public class Mascota
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la mascota es obligatorio.")]
        [Display(Name = "Nombre de la mascota")]
        public string NombreMascota { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del dueño es obligatorio.")]
        [Display(Name = "Nombre del dueño")]
        public string NombreDueno { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecciona un tipo.")]
        public string Tipo { get; set; } = "Perro";

        [Range(0, 30, ErrorMessage = "La edad debe estar entre 0 y 30 años.")]
        public int Edad { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "El teléfono debe tener 9 dígitos numéricos.")]
        public string Telefono { get; set; } = string.Empty;

        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }
    }
}
