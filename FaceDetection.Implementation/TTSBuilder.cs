using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FaceDetection.Implementation
{
    public class TTSBuilder
    {
        private IMemoryCache _cache;

        private static readonly string voiceEndpoint = "https://westeurope.tts.speech.microsoft.com/cognitiveservices/v1";
        private static readonly string authEndpoint = "https://westeurope.api.cognitive.microsoft.com/sts/v1.0/issueToken";
        public TTSBuilder(IMemoryCache cache)
        {
            _cache = cache;
        }
        public async Task BuildWavAsync(List<Reply> replies, string wavFileName)
        {
            string accessToken;
            Authentication auth = new Authentication(authEndpoint, "f9fcac82eb164fa5bcfac105aea98bbc", _cache);
            try
            {
                accessToken = await auth.FetchTokenAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to obtain an access token.");
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.Message);
                return;
            }

            var voices = new List<XElement>();
            foreach (var reply in replies)
            {
                switch (reply.Language)
                {
                    case "ro-RO":
                        voices.Add(new XElement("voice",
                                 new XAttribute(XNamespace.Xml + "lang", "ro-RO"),
                                 new XAttribute("name", "ro-RO-Andrei"), // Short name for "Microsoft Server Speech Text to Speech Voice (en-US, Jessa24KRUS)"
                                 reply.Text));
                        break;
                    case "en-US":
                        voices.Add(new XElement("voice",
                                new XAttribute(XNamespace.Xml + "lang", "en-US"),
                                //new XAttribute(XNamespace.Xml + "gender", "Female"),
                                new XAttribute("name", "en-US-JessaNeural"), // Short name for "Microsoft Server Speech Text to Speech Voice (en-US, Jessa24KRUS)"
                                reply.Text));
                        break;
                    default:
                        voices.Add(new XElement("voice",
                                new XAttribute(XNamespace.Xml + "lang", "en-US"),
                                new XAttribute(XNamespace.Xml + "gender", "Female"),
                                new XAttribute("name", "en-US-Jessa24kRUS"), // Short name for "Microsoft Server Speech Text to Speech Voice (en-US, Jessa24KRUS)"
                                reply.Text));
                        break;
                }
            }

            // Create SSML document.
            XDocument body = new XDocument(
                        new XElement("speak",
                            new XAttribute("version", "1.0"),
                            new XAttribute(XNamespace.Xml + "lang", "en-US"),
                            voices));


            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage())
                {
                    // Set the HTTP method
                    request.Method = HttpMethod.Post;
                    // Construct the URI
                    request.RequestUri = new Uri(voiceEndpoint);
                    // Set the content type header
                    request.Content = new StringContent(body.ToString(), Encoding.UTF8, "application/ssml+xml");
                    // Set additional header, such as Authorization and User-Agent
                    request.Headers.Add("Authorization", "Bearer " + accessToken);
                    request.Headers.Add("Connection", "Keep-Alive");
                    // Update your resource name
                    request.Headers.Add("User-Agent", "recognition");
                    // Audio output format. See API reference for full list.
                    request.Headers.Add("X-Microsoft-OutputFormat", "riff-24khz-16bit-mono-pcm");
                    // Create a request

                    using (HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        // Asynchronously read the response
                        using (Stream dataStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        {
                            using (FileStream fileStream = new FileStream($@"{wavFileName}.wav", FileMode.Create, FileAccess.Write, FileShare.Write))
                            {
                                await dataStream.CopyToAsync(fileStream).ConfigureAwait(false);
                                fileStream.Close();
                            }
                        }
                    }
                }
            }
        }

        public async Task BuildWavAsync(Reply reply, string wavFileName)
        {
            string accessToken;
            Authentication auth = new Authentication(authEndpoint, "f9fcac82eb164fa5bcfac105aea98bbc", _cache);
            try
            {
                accessToken = await auth.FetchTokenAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to obtain an access token.");
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.Message);
                return;
            }

            var voices = new List<XElement>();

            switch (reply.Language)
            {
                case "ro-RO":
                    voices.Add(new XElement("voice",
                             new XAttribute(XNamespace.Xml + "lang", "ro-RO"),
                             new XAttribute("name", "ro-RO-Andrei"), // Short name for "Microsoft Server Speech Text to Speech Voice (en-US, Jessa24KRUS)"
                             reply.Text));
                    break;
                case "en-US":
                    voices.Add(new XElement("voice",
                            new XAttribute(XNamespace.Xml + "lang", "en-US"),
                            //new XAttribute(XNamespace.Xml + "gender", "Female"),
                            new XAttribute("name", "en-US-JessaNeural"), // Short name for "Microsoft Server Speech Text to Speech Voice (en-US, Jessa24KRUS)"
                            reply.Text));
                    break;
                default:
                    voices.Add(new XElement("voice",
                            new XAttribute(XNamespace.Xml + "lang", "en-US"),
                            new XAttribute(XNamespace.Xml + "gender", "Female"),
                            new XAttribute("name", "en-US-Jessa24kRUS"), // Short name for "Microsoft Server Speech Text to Speech Voice (en-US, Jessa24KRUS)"
                            reply.Text));
                    break;
            }

            // Create SSML document.
            XDocument body = new XDocument(
                        new XElement("speak",
                            new XAttribute("version", "1.0"),
                            new XAttribute(XNamespace.Xml + "lang", "en-US"),
                            voices));


            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage())
                {
                    // Set the HTTP method
                    request.Method = HttpMethod.Post;
                    // Construct the URI
                    request.RequestUri = new Uri(voiceEndpoint);
                    // Set the content type header
                    request.Content = new StringContent(body.ToString(), Encoding.UTF8, "application/ssml+xml");
                    // Set additional header, such as Authorization and User-Agent
                    request.Headers.Add("Authorization", "Bearer " + accessToken);
                    request.Headers.Add("Connection", "Keep-Alive");
                    // Update your resource name
                    request.Headers.Add("User-Agent", "recognition");
                    // Audio output format. See API reference for full list.
                    request.Headers.Add("X-Microsoft-OutputFormat", "riff-24khz-16bit-mono-pcm");
                    // Create a request

                    using (HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        // Asynchronously read the response
                        using (Stream dataStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        {
                            using (FileStream fileStream = new FileStream($@"{wavFileName}.wav", FileMode.Create, FileAccess.Write, FileShare.Write))
                            {
                                await dataStream.CopyToAsync(fileStream).ConfigureAwait(false);
                                fileStream.Close();
                            }
                        }
                    }
                }
            }
        }
    }
}
