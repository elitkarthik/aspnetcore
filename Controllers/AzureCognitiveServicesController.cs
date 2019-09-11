using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace aspnetcore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AzureCognitiveServicesController : Controller
    {
        string cognitiveServicesKey = "8223549a70db4356bf6b612f0bd6ad2d";
        const string endpoint = "eastus.api.cognitive.microsoft.com";

        string computerVisionURL_analyze = $"https://{endpoint}/vision/v1.0/analyze?{visualFeatures}&{details}&{language}";
        const string visualFeatures = "Categories,Tags,Description,Faces,ImageType,Color";
        const string details = "celebrities,landmarks";
        const string language = "en";

        string computerVisionURL_describe = $"https://{endpoint}/vision/v2.0/describe";

        //string customVisionPredictionURL = $"https://{endpoint}/customvision/v3.0/Prediction/{projectId}/classify/iterations/{publishedName}/image[?application]";        

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost("UploadFiles")]        
        [HttpPost()]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken] //For some reason in Linux, Antiforgery token seems to be throwing errors due to missing DPAPI configuration ******
        public string Post(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cognitiveServicesKey);

                        var memoryStream = new MemoryStream();
                        formFile.CopyTo(memoryStream);

                        using (var content = new ByteArrayContent(memoryStream.ToArray()))
                        {
                            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            ViewData["Response"] = client.PostAsync(computerVisionURL_describe, content).Result.Content.ReadAsStringAsync().Result;                            
                        }
                    }
                }
            }

            return ViewData["Response"].ToString();

            ////Call Azure cognitive Services and submit the image for analysis

            //    // process uploaded files
            //    // Don't rely on or trust the FileName property without validation.
            //    return Ok(new { count = files.Count, size, filePath });
        }
    }
}