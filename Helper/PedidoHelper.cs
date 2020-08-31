using DotNet_DurableFunctions.Model;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

public class PedidoHelper
{
    public async Task<bool> SalvarPedido(Pedido pedido)
    {
        await Task.Run(() =>
        {
            var pedidoJson = JsonConvert.SerializeObject(pedido, Formatting.Indented);
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Out.Write(pedidoJson);
            Console.ResetColor();
        });

        return await Task.FromResult(false);
    }

    public async Task<bool> AtualizarEstoque(Pedido pedido)
    {
        if (pedido.NumCartao.Substring(pedido.NumCartao.Length - 2) == "26")
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Out.WriteLine($"Produto {pedido.Numero} atualizado com sucesso do estoque atual");
            Console.ResetColor();
            return await Task.FromResult(true);
        }

        return false;
    }

    public async Task EnviarConfirmacaoCompra(string emailComprador)
    {
        await Task.Run(() =>
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Out.WriteLine($"Enviado email para {emailComprador}.");
            Console.ResetColor();
        });
    }
}