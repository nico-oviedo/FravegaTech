using System.ComponentModel.DataAnnotations;

namespace SharedKernel.Dtos
{
    public class EventDto
    {
        [Required(ErrorMessage = "Id del evento es requerido.")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Tipo de evento es requerido.")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Fecha del evento es requerida.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Usuario del evento es requerido.")]
        public string User { get; set; }
    }
}
