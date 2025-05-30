using System.ComponentModel.DataAnnotations;

namespace SharedKernel.Dtos
{
    public class OrderProductDto : ProductDto
    {
        [Required(ErrorMessage = "Cantidad del producto es requerida.")]
        [Range(1, 999, ErrorMessage = "Cantidad del producto debe estar comprendido entre 1 y 999.")]
        public int Quantity { get; set; }
    }
}
