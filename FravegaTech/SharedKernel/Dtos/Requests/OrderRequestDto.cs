using System.ComponentModel.DataAnnotations;

namespace SharedKernel.Dtos.Requests
{
    public class OrderRequestDto
    {
        [Required(ErrorMessage = "Referencia externa de la orden es obligatoria.")]
        public string ExternalReferenceId { get; set; }

        [Required(ErrorMessage = "Canal de la orden es obligatorio.")]
        public string Channel { get; set; }

        [Required(ErrorMessage = "Fecha de la orden es obligatoria.")]
        public DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "Valor total de la orden es obligatorio.")]
        [Range(0.01, 999999999.99, ErrorMessage = "Valor total de la orden debe estar comprendido entre 0.01 y 999999999.99")]
        public decimal TotalValue { get; set; }

        [Required(ErrorMessage = "Comprador de la orden es obligatorio.")]
        public BuyerDto Buyer { get; set; }

        [Required(ErrorMessage = "Productos de la orden son obligatorios.")]
        public List<OrderProductDto> Products { get; set; }
    }
}
