using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace DocUploadApi
{
    public class PlainTextMediaTypeFormatter : MediaTypeFormatter
    {
        public PlainTextMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
        }

        public override bool CanReadType(Type type) =>
            type == typeof(string);

        public override bool CanWriteType(Type type) =>
            type == typeof(string);

        public override async Task<object> ReadFromStreamAsync(
            Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var streamReader = new StreamReader(readStream);
            return await streamReader.ReadToEndAsync();
        }

        public override async Task WriteToStreamAsync(
            Type type, object value, Stream writeStream,
            HttpContent content, TransportContext transportContext, CancellationToken cancellationToken)
        {
            var streamReader = new StreamWriter(writeStream);
            await streamReader.WriteAsync((string)value);
        }
    }

    public class TextMediaTypeFormatter : MediaTypeFormatter
    {
        public TextMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            try
            {
                var memoryStream = new MemoryStream();
                readStream.CopyTo(memoryStream);
                var s = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
                taskCompletionSource.SetResult(s);
            }
            catch (Exception e)
            {
                taskCompletionSource.SetException(e);
            }
            return taskCompletionSource.Task;
        }

        public override bool CanReadType(Type type)
        {
            return type == typeof(string);
        }

        public override bool CanWriteType(Type type)
        {
            return false;
        }
    }
}