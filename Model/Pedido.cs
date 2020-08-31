using System;

namespace DotNet_DurableFunctions.Model
{
    public class Pedido
    {
        public string Numero
        {
            get
            {
                return Guid.NewGuid().ToString().Substring(0, 4);
            }
        }
        public string EmailCliente { get; set; }
        public string NumCartao { get; set; }
        public int Parcelas { get; set; }
        public string NomeNoCartao { get; set; }
        public string Validade { get; set; }
        public string CodSeguranca { get; set; }
        public Produto Produto { get; set; }
        public string Status
        {
            get
            {
                if (string.IsNullOrEmpty(NumCartao))
                    return "Reprovado";

                if (NumCartao.Substring(NumCartao.Length - 2) == "26")
                    return "Aprovado";

                return "Reprovado";
            }
        }
    }
}
