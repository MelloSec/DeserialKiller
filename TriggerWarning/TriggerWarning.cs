using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace TriggerWarning
{
    public static class TriggerWarning
    {
        private static readonly ConcurrentQueue<string> _requestLogs = new ConcurrentQueue<string>();
        private const int MaxLogCount = 10;

        [FunctionName("triggerwarning")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation("Received data: " + requestBody);

            // Add request to the log
            _requestLogs.Enqueue(requestBody);
            if (_requestLogs.Count > MaxLogCount)
            {
                _requestLogs.TryDequeue(out _);
            }

            // Create response message with the last 10 requests
            string responseMessage = "Last 10 Requests:\n";
            foreach (var request in _requestLogs)
            {
                responseMessage += $"{request}\n---\n";
            }

            return new OkObjectResult(responseMessage);
        }
    }
}

