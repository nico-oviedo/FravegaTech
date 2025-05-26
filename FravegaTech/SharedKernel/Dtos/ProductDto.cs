using System.ComponentModel.DataAnnotations;

namespace SharedKernel.Dtos
{
    public class ProductDto
    {
        [Required(ErrorMessage = "SKU del producto es obligatorio.")]
        public string SKU { get; set; }

        [Required(ErrorMessage = "Nombre del producto es obligatorio.")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "Descripcion del producto excede el maximo de caracteres permitidos.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Precio del producto es obligatorio.")]
        [Range(0.01, 999999999.99, ErrorMessage = "Precio del producto debe estar comprendido entre 0.01 y 999999999.99")]
        public decimal Price { get; set; }
    }
}
