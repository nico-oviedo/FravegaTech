using System.ComponentModel.DataAnnotations;

namespace SharedKernel.Dtos.Requests
{
    public class OrderRequestDto
    {
        [Required(ErrorMessage = "Referencia externa de la orden es requerida.")]
        public string ExternalReferenceId { get; set; }

        [Required(ErrorMessage = "Canal de la orden es requerido.")]
        public string Channel { get; set; }

        [Required(ErrorMessage = "Fecha de la orden es requerida.")]
        public DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "Valor total de la orden es requerido.")]
        [Range(0.01, 999999999, ErrorMessage = "Valor total de la orden debe estar comprendido entre 0,01 y 999.999.999")]
        public decimal TotalValue { get; set; }

        [Required(ErrorMessage = "Comprador de la orden es requerido.")]
        public BuyerDto Buyer { get; set; }

        [Required(ErrorMessage = "Productos de la orden son requeridos.")]
        public List<OrderProductDto> Products { get; set; }
    }
}
