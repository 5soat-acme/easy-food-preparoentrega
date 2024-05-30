using System.ComponentModel.DataAnnotations;

namespace EF.PreparoEntrega.Application.DTOs.Requests;

public class ConfirmarEntregaDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid PedidoId { get; set; }
}