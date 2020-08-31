//using System.Threading.Tasks;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.DurableTask;
//using System;
//using Newtonsoft.Json;
//using Microsoft.Extensions.Logging;
//using DotNet_DurableFunctions.Model;

//namespace ANP.Functions
//{
//    public static class ChainPatternExample
//    {
//        [FunctionName("StartOrchestration")]
//        public static async Task Run(
//            [QueueTrigger("chaining-1", Connection = "AzureWebJobsStorage")] string queueItem,
//            [DurableClient] IDurableOrchestrationClient starter)
//        {
//            Console.BackgroundColor = ConsoleColor.Blue;
//            Console.ForegroundColor = ConsoleColor.White;
//            Console.Out.WriteLine("Recebeu mensagem da Fila");
//            Console.ResetColor();

//            await starter.StartNewAsync("Orchestration", string.Empty, queueItem);
//        }

//        [FunctionName("Orchestration")]
//        public static async Task Orchestration([OrchestrationTrigger] IDurableOrchestrationContext ctx)
//        {
//            Console.BackgroundColor = ConsoleColor.Blue;
//            Console.ForegroundColor = ConsoleColor.White;
//            Console.Out.WriteLine("Iniciando Fluxo");
//            Console.ResetColor();

//            var input = ctx.GetInput<string>();
//            var pedido = JsonConvert.DeserializeObject<Pedido>(input);

//            if (await ctx.CallActivityAsync<bool>("SalvarPedido", pedido))
//            {
//                if (await ctx.CallActivityAsync<bool>("AtualizarEstoque", pedido))
//                {
//                    await ctx.CallActivityAsync<Produto>("EnviarEmailConfirmacao", pedido);
//                }
//            }
//        }

//        [FunctionName("SalvarPedido")]
//        public static async Task<bool> SalvarPedido([ActivityTrigger] IDurableActivityContext ctx, ILogger log)
//        {
//            Console.BackgroundColor = ConsoleColor.Blue;
//            Console.ForegroundColor = ConsoleColor.White;
//            Console.Out.WriteLine("Entrou no fluxo de Salvar Pedido no base");
//            Console.ResetColor();

//            var pedido = ctx.GetInput<Pedido>();
//            log.LogInformation(pedido.Numero);
//            var storageHelper = new PedidoHelper();
//            return await storageHelper.SalvarPedido(pedido);
//        }

//        [FunctionName("AtualizarEstoque")]
//        public static async Task<bool> AtualizarEstoque([ActivityTrigger] IDurableActivityContext ctx, ILogger log)
//        {
//            Console.BackgroundColor = ConsoleColor.Blue;
//            Console.ForegroundColor = ConsoleColor.White;
//            Console.Out.WriteLine("Entrou no fluxo de Atualizar Estoque");
//            Console.ResetColor();

//            var pedido = ctx.GetInput<Pedido>();
//            var storageHelper = new PedidoHelper();
//            var result = await storageHelper.AtualizarEstoque(pedido);
//            return result;
//        }

//        [FunctionName("EnviarEmailConfirmacao")]
//        public static async Task EnviarEmailConfirmacao([ActivityTrigger] IDurableActivityContext ctx, ILogger log)
//        {
//            Console.BackgroundColor = ConsoleColor.Blue;
//            Console.ForegroundColor = ConsoleColor.White;
//            Console.Out.WriteLine("Entrou no fluxo de Enviar Email Confirmação");
//            Console.ResetColor();

//            var pedido = ctx.GetInput<Pedido>();
//            var storageHelper = new PedidoHelper();
//            await storageHelper.EnviarConfirmacaoCompra(pedido.EmailCliente);
//        }
//    }
//}