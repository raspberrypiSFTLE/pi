﻿using Newtonsoft.Json;
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

        public async Task<string> IdentifyPersonAsync(byte[] imageContent)
        {
            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            using (var imageStream = new MemoryStream(imageContent))
            {
                HttpContent fileStreamContent = new StreamContent(imageStream);
                {
                    formData.Add(fileStreamContent, "file", "file");
                    var response = await client.PostAsync("https://faceappfront.azurewebsites.net/facerecognition/known-persons", formData).ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        return string.Empty;
                    }

                    var result = await response.Content.ReadAsStringAsync();
                    var responseJson = JsonConvert.DeserializeObject<List<object>>(result);

                    foreach (var item in responseJson)
                    {
                        if (item is JObject)
                        {
                            foreach (var property in (item as JObject))
                            {
                                if (property.Key == "match")
                                {
                                    var personName = property.Value.Value<string>();
                                    if (!string.IsNullOrWhiteSpace(personName))
                                    {
                                        Console.WriteLine($"Person identified: {personName}");
                                        return personName;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }
    }
}
