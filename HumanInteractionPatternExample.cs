using DotNet_DurableFunctions.Model.HumanInteraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet_DurableFunctions
{
    public static class HumanInteractionPatternExample
    {
        [FunctionName("HumanPatternExample_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var descontoRequest = await req.Content.ReadAsAsync<DescontoRequest>();

            string instanceId = await starter.StartNewAsync("HumanPatternExample_Orchestrator", null, (descontoRequest: descontoRequest, requestUri: req.RequestUri));

            log.LogInformation($"Orchestration foi inciiado com o Id = '{instanceId}'.");

            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted)
            {
                Content = new StringContent("Sua solicitação de desconto foi enviada e estamos aguardando uma aprovação.")
            };
        }

        [FunctionName("HumanPatternExample_Orchestrator")]
        public static async Task RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            log.LogInformation($"************** RunOrchestrator method executing ********************");

            // Using tuples but could also define a class for this data
            var (descontoRequest, requestUri) = context.GetInput<Tuple<DescontoRequest, Uri>>();
            var moderationRequest = new DescontoModeracaoRequest
            {
                DescontoRequest = descontoRequest,
                ApproveRequestUrl = $"{requestUri.Scheme}://{requestUri.Host}:{requestUri.Port}/api/HumanPatternExample_Approve?id={context.InstanceId}",
                DeclineRequestUrl = $"{requestUri.Scheme}://{requestUri.Host}:{requestUri.Port}/api/HumanPatternExample_Decline?id={context.InstanceId}",
            };

            await context.CallActivityAsync("HumanPatternExample_RequestApproval", moderationRequest);

            using (var timeout = new CancellationTokenSource()) // Defino o Timeout para aprovação do workflow
            {
                DateTime moderationDeadline = context.CurrentUtcDateTime.AddSeconds(10);

                Task durableTimeout = context.CreateTimer(moderationDeadline, timeout.Token);

                Task<bool> moderatedEvent = context.WaitForExternalEvent<bool>("Moderation");

                if (moderatedEvent == await Task.WhenAny(moderatedEvent, durableTimeout))
                {
                    timeout.Cancel();

                    bool isApproved = moderatedEvent.Result;

                    if (isApproved)
                    {
                        log.LogInformation($"************** Desconto de  { descontoRequest.ValorDesconto } aprovado com sucesso. ********************");
                        //CRIAR REGRA DE NEGOCIO PARA APROVAR
                    }
                    else
                    {
                        log.LogInformation($"************** Desconto de  { descontoRequest.ValorDesconto } reprovado. ********************");
                        //CRIAR REGRA DE NEGOCIO PARA NEGAR DESCONTO
                    }
                }
                else
                {
                    log.LogInformation($"************** Desconto de  { descontoRequest.ValorDesconto } não recebeu intervenção...  ********************");
                    //CRIAR REGRA DE NEGOCIO QUANDO NAO RECEBER INTERVENCAO
                }
            }

            log.LogInformation($"************** Orchestration complete ********************");
        }

        [FunctionName("HumanPatternExample_RequestApproval")]
        public static void RequestApproval([ActivityTrigger] DescontoModeracaoRequest moderationRequest, ILogger log)
        {
            log.LogInformation($"ENVIAR MENSAGEM AO USUARIO COM OS LINKS DE APROVAÇÂO");
            log.LogInformation($"=> { moderationRequest.ApproveRequestUrl }");
            log.LogInformation($"=> { moderationRequest.DeclineRequestUrl }");
        }

        [FunctionName("HumanPatternExample_Approve")]
        public static async Task<IActionResult> HumanPatternExample_Approve(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            var id = req.Query["id"];

            var status = await client.GetStatusAsync(id);

            if (status.RuntimeStatus == OrchestrationRuntimeStatus.Running)
            {
                await client.RaiseEventAsync(id, "Moderation", true);
                return new OkObjectResult("Desconto foi aprovado com sucesso.");
            }

            return new NotFoundResult();
        }

        [FunctionName("HumanPatternExample_Decline")]
        public static async Task<IActionResult> HumanPatternExample_Decline(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            var id = req.Query["id"];

            var status = await client.GetStatusAsync(id);
            if (status.RuntimeStatus == OrchestrationRuntimeStatus.Running)
            {
                await client.RaiseEventAsync(id, "Moderation", false);
                return new OkObjectResult("Desconto foi negado com sucesso.");
            }

            return new NotFoundResult();
        }
    }
}