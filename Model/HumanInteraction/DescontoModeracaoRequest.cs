namespace DotNet_DurableFunctions.Model.HumanInteraction
{
    public class DescontoModeracaoRequest
    {
        public DescontoRequest DescontoRequest { get; set; }
        public string ApproveRequestUrl { get; set; }
        public string DeclineRequestUrl { get; set; }
    }
}
