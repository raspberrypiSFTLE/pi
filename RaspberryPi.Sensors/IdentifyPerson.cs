using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RaspberryPi.Sensors
{
    public class IdentifyPerson
    {

        public async Task<List<Person>> IdentifyPersonAsync(byte[] imageContent)
        {
            var persons = new List<Person>();

            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            using (var imageStream = new MemoryStream(imageContent))
            {
                HttpContent fileStreamContent = new StreamContent(imageStream);
                {
                    formData.Add(fileStreamContent, "file", "file");
                    var response = await client.PostAsync("https://faceappfront.azurewebsites.net/facerecognition/known-persons", formData).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        persons = JsonConvert.DeserializeObject<List<Person>>(result);
                        return persons;
                    }
                }
            }

            return persons;
        }
    }
}
