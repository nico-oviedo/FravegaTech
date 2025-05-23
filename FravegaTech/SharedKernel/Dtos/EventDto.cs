using System.ComponentModel.DataAnnotations;

namespace SharedKernel.Dtos
{
    public class EventDto
    {
        [Required(ErrorMessage = "Id del evento es obligatorio.")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Tipo de evento es obligatorio.")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Fecha del evento es obligatorio.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Usuario del evento es obligatorio.")]
        public string User { get; set; }
    }
}
