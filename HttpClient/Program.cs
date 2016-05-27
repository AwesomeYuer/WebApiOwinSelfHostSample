using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.IO;
namespace Microshaoft
{
    class Program
    {
        const int BufferSize = 1024;

        static readonly string _baseAddress = "http://localhost:50231/";
        static readonly string _filename = "Sample.xml";
        static void Main(string[] args)
        {
            HttpClientHelper
                .PostUploadFile
                    (
                        @"E:\download\PerfView.zip"
                        , @"http://localhost:10281/api/restful/Files/upload"
                        , (x, y, z) =>
                        {
                            Console.WriteLine(z.BytesTransferred);
                        }
                        , false
                        , (x, y, z) =>
                        {
                            return false;
                        }
                        , null

                    );
            Console.ReadLine();
        }
        static async void RunClient()
        {
            // Create a progress notification handler
            ProgressMessageHandler progress = new ProgressMessageHandler();
            progress.HttpSendProgress += ProgressEventHandler;

            // Create an HttpClient and wire up the progress handler
            HttpClient client = HttpClientFactory.Create(progress);

            // Set the request timeout as large uploads can take longer than the default 2 minute timeout
            client.Timeout = TimeSpan.FromMinutes(20);

            // Open the file we want to upload and submit it
            using (FileStream fileStream = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, useAsync: true))
            {
                // Create a stream content for the file
                StreamContent content = new StreamContent(fileStream, BufferSize);

                // Create Multipart form data content, add our submitter data and our stream content
                MultipartFormDataContent formData = new MultipartFormDataContent();
                formData.Add(new StringContent("Me"), "submitter");
                formData.Add(content, "filename", _filename);

                // Post the MIME multipart form data upload with the file
                Uri address = new Uri(_baseAddress + "api/fileupload");
                HttpResponseMessage response = await client.PostAsync(address, formData);

                 //await response.Content.ReadAsAsync();
                //Console.WriteLine("{0}Result:{0}  Filename:  {1}{0}  Submitter: {2}", Environment.NewLine, result.FileNames.FirstOrDefault(), result.Submitter);
            }
        }

        static void ProgressEventHandler(object sender, HttpProgressEventArgs eventArgs)
        {
            // The sender is the originating HTTP request   
            HttpRequestMessage request = sender as HttpRequestMessage;

            // Write different message depending on whether we have a total length or not   
            string message;
            if (eventArgs.TotalBytes != null)
            {
                message = String.Format("  Request {0} uploaded {1} of {2} bytes ({3}%)",
                    request.RequestUri, eventArgs.BytesTransferred, eventArgs.TotalBytes, eventArgs.ProgressPercentage);
            }
            else
            {
                message = String.Format("  Request {0} uploaded {1} bytes",
                    request.RequestUri, eventArgs.BytesTransferred, eventArgs.TotalBytes, eventArgs.ProgressPercentage);
            }

            // Write progress message to console   
            Console.WriteLine(message);
        }
    }
}
