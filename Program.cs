using System;
using Microsoft.Azure.CognitiveServices.ContentModerator;
using Microsoft.Azure.CognitiveServices.ContentModerator.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Net.Http;

// URL guia:
// https://docs.microsoft.com/pt-br/azure/cognitive-services/Content-Moderator/client-libraries?pivots=programming-language-csharp

namespace AzureContentModarator
{
    class Program
    {
        // Credenciais
        private static readonly string SubscriptionKey = "CHAVEDEAPI";
        private static readonly string Endpoint = "ENDPOINT";


        // TEXT MODERATION
        private static readonly string TxtEntrada = "Entrada.txt";
        private static string TxtSaida = "Saida.txt";

        static void Main(string[] args)
        {
            // Create an image review client
            ContentModeratorClient clientImage = Authenticate(SubscriptionKey, Endpoint);
            // Create a text review client
            ContentModeratorClient clientText = Authenticate(SubscriptionKey, Endpoint);
            // Create a human reviews client
            ContentModeratorClient clientReviews = Authenticate(SubscriptionKey, Endpoint);
            
            Console.WriteLine("Resultado:");

            // Moderate text from text in a file
            ModerateText(clientText, TxtEntrada, TxtSaida);
        }

		public static ContentModeratorClient Authenticate(string key, string endpoint)
		{
			ContentModeratorClient client = new ContentModeratorClient(new ApiKeyServiceClientCredentials(key));
			client.Endpoint = endpoint;
			return client;
		}

        /*
        * TEXT MODERATION
        * This example moderates text from file.
        */
        public static void ModerateText(ContentModeratorClient client, string inputFile, string outputFile)
        {
            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("MODERADOR DE TEXTOS");
            Console.WriteLine();
            // Load the input text.
            string text = File.ReadAllText(inputFile);

            // Remove carriage returns
            text = text.Replace(Environment.NewLine, " ");
            // Convert string to a byte[], then into a stream (for parameter in ScreenText()).
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            MemoryStream stream = new MemoryStream(textBytes);

            Console.WriteLine("Screening {0}...", inputFile);
            // Format text

            // Save the moderation results to a file.
            using (StreamWriter outputWriter = new StreamWriter(outputFile, false))
            {
                using (client)
                {
                    // Screen the input text: check for profanity, classify the text into three categories,
                    // do autocorrect text, and check for personally identifying information (PII)
                    outputWriter.WriteLine("Autocorrect typos, check for matching terms, PII, and classify.");

                    // Moderate the text
                    var screenResult = client.TextModeration.ScreenText("text/plain", stream, "por", false, false, null, true);
                    outputWriter.WriteLine(JsonConvert.SerializeObject(screenResult, Formatting.Indented));
                }

                outputWriter.Flush();
                outputWriter.Close();
            }

            Console.WriteLine("Results written to {0}", outputFile);
            Console.WriteLine();
        }
    }
}
