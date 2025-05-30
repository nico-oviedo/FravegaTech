using System.ComponentModel.DataAnnotations;

namespace SharedKernel.Dtos
{
    public class ProductDto
    {
        [Required(ErrorMessage = "SKU del producto es requerido.")]
        public string SKU { get; set; }

        [Required(ErrorMessage = "Nombre del producto es requerido.")]
        public string Name { get; set; }

        [StringLength(200, ErrorMessage = "Descripción del producto excede el maximo de caracteres permitidos (200 caracteres).")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Precio del producto es requerido.")]
        [Range(0.01, 999999999, ErrorMessage = "Precio del producto debe estar comprendido entre 0,01 y 999.999.999")]
        public decimal Price { get; set; }
    }
}
