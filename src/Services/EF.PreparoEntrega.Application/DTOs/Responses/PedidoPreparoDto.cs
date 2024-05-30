using EF.PreparoEntrega.Domain.Models;

namespace EF.PreparoEntrega.Application.DTOs.Responses;

public class PedidoPreparoDto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; }
    public DateTime DataCriacao { get; set; }
    public StatusPreparo Status { get; set; }
    public List<ItemPreparoDto> Itens { get; set; }
}