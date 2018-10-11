using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UML2Netbeans
{
    class OCR
    {
        public async Task<string> convertToTextAsync(String imagePath, String language)
        {
            String apiKey = "237ec520dc88957";
            String textResult = "";


            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.Timeout = new TimeSpan(1, 1, 1);

                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent(apiKey), "apikey"); //Added api key in form data
                form.Add(new StringContent(language), "language");
                byte[] imageData = File.ReadAllBytes(imagePath);
                form.Add(new ByteArrayContent(imageData, 0, imageData.Length), "image", "image.jpg");

                HttpResponseMessage response = await httpClient.PostAsync("https://api.ocr.space/Parse/Image", form);

                string strContent = await response.Content.ReadAsStringAsync();
                MessageBox.Show(strContent);


                Rootobject ocrResult = JsonConvert.DeserializeObject<Rootobject>(strContent);

                if (ocrResult.OCRExitCode == 1)
                {
                    for (int i = 0; i < ocrResult.ParsedResults.Count(); i++)
                    {
                        textResult = textResult + ocrResult.ParsedResults[i].ParsedText;
                    }
                }
                else
                {
                    MessageBox.Show("ERROR: " + strContent);
                }
            }
            catch
            {
                MessageBox.Show("Ooops");
                Environment.Exit(1);
            }

            return textResult;
        }
    }
}
