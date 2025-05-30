using System.ComponentModel.DataAnnotations;

namespace SharedKernel.Dtos
{
    public class BuyerDto
    {
        [Required(ErrorMessage = "Nombre del comprador es requerido.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Apellido del comprador es requerido.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Numero de documento del comprador es requerido.")]
        public string DocumentNumber { get; set; }

        [Required(ErrorMessage = "Telefono del comprador es requerido.")]
        public string Phone { get; set; }
    }
}
