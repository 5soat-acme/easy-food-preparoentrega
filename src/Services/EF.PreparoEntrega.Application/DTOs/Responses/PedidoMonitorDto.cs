using EF.PreparoEntrega.Domain.Models;

namespace EF.PreparoEntrega.Application.DTOs.Responses;

public class PedidoMonitorDto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; }
    public StatusPreparo Status { get; set; }
    public string TempoEspera { get; set; }
}