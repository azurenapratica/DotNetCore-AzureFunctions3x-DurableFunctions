namespace DotNet_DurableFunctions.Model.HumanInteraction
{
    public class DescontoRequest
    {
        public string CodigoProduto { get; set; }
        public decimal ValorProduto { get; set; }
        public decimal ValorDesconto { get; set; }
        public string SolicitadoPor { get; set; }
    }
}
