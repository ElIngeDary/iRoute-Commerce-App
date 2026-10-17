using System.ComponentModel.DataAnnotations;

namespace CommerceApi.Dtos
{
    public class ProcessDateRequestDto
    {
        [Required(ErrorMessage = "La fecha de proceso es requerida.")]
        public DateTime ProcessDate { get; set; }
    }
}