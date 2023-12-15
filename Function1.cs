using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace VulnerableFunctionApp
{
    public static class BinaryFormatterFunction
    {
        [FunctionName("BinaryFormatter")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string responseHtml = GetHtmlPage();

            if (req.Method == HttpMethods.Post)
            {
                string textToEncode = req.Form["textToEncode"];
                string dataToDecode = req.Form["dataToDecode"];

                if (!string.IsNullOrEmpty(textToEncode))
                {
                    var bytes = Encoding.UTF8.GetBytes(textToEncode);
                    var formatter = new BinaryFormatter();
                    using (var ms = new MemoryStream())
                    {
                        formatter.Serialize(ms, textToEncode);
                        string base64EncodedData = Convert.ToBase64String(ms.ToArray());
                        responseHtml += $"<p>Encoded Base64 Data: {base64EncodedData}</p>";
                    }
                }
                else if (!string.IsNullOrEmpty(dataToDecode))
                {
                    byte[] decodedBytes = Convert.FromBase64String(dataToDecode);
                    using (var ms = new MemoryStream(decodedBytes))
                    {
                        var formatter = new BinaryFormatter();
                        string deserializedText = (string)formatter.Deserialize(ms);
                        responseHtml += $"<p>Decoded Text: {deserializedText}</p>";
                    }
                }
            }

            return new ContentResult { Content = responseHtml, ContentType = "text/html" };
        }

        private static string GetHtmlPage()
        {
            return @"
                <html>
                <body>
                    <h2>Enter the text to serialize and encode</h2>
                    <form method='post'>
                        <input type='text' name='textToEncode' />
                        <input type='submit' value='Encode' />
                    </form>
                    <br />
                    <h2>Enter the base64 encoded data to decode and deserialize</h2>
                    <form method='post'>
                        <input type='text' name='dataToDecode' />
                        <input type='submit' value='Decode' />
                    </form>
                </body>
                </html>";
        }
    }
}
