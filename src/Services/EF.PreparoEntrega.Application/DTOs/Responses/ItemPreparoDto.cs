namespace EF.PreparoEntrega.Application.DTOs.Responses;

public class ItemPreparoDto
{
    public int Quantidade { get; set; }
    public Guid ProdutoId { get; set; }
    public string NomeProduto { get; set; }
    public int TempoPreparoEstimado { get; set; }
}