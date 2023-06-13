using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using TranslationSample;
using Microsoft.AspNetCore.Http;
using System.IO;




namespace TranslationSample
{
    public static class TranslationService
    {
        private const string SubscriptionKey = "API_KEY_HERE";
        private const string Endpoint = "https://api.cognitive.microsofttranslator.com/";
        private const string Route = "/translate?api-version=3.0&to=chinese";

        public static async Task<string> TranslateText(string FileContents)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);

            var requestBody = new StringContent("[{\"Text\": \"" + FileContents + "\"}]", Encoding.UTF8, "application/json");

            var response = await client.PostAsync(Endpoint + Route, requestBody);
            var result = await response.Content.ReadAsStringAsync();
            var jsonResult = JArray.Parse(result);

            return jsonResult[0]["translations"][0]["text"].ToString();
        }
    }
}
namespace MyApp.Controllers
{
    public class TranslationController : Controller
    {
        public async Task<IActionResult> DownloadTranslatedText(IFormFile fileInput)
        {
            using (var reader = new StreamReader(fileInput.OpenReadStream()))
            {
                string text = reader.ReadToEnd();
                string translatedText = await TranslationService.TranslateText(text);
                byte[] fileContents = Encoding.UTF8.GetBytes(translatedText);
                return File(fileContents, "text/plain", "translated_text.txt");
            }
        }
    }
}

namespace MyApp.Pages
{
    public class IndexModel : PageModel
    {
        public class DisplayPageModel : PageModel
        {
            public string? FileContents { get; set; }

            public void OnPost(IFormFile fileInput)
            {
                using (var reader = new StreamReader(fileInput.OpenReadStream()))
                {
                    FileContents = reader.ReadToEnd();
                }
            }
        }
    }
}
