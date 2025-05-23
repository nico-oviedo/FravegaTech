using System.ComponentModel.DataAnnotations;

namespace SharedKernel.Dtos
{
    public class BuyerDto
    {
        [Required(ErrorMessage = "Nombre del comprador es obligatorio.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Apellido del comprador es obligatorio.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Numero de documento del comprador es obligatorio.")]
        public string DocumentNumber { get; set; }

        [Required(ErrorMessage = "Telefono del comprador es obligatorio.")]
        public string Phone { get; set; }
    }
}
