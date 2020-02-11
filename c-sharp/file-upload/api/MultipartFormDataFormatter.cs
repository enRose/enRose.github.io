using DocUploadApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DocUploadApi
{
    public class MultipartFormDataFormatter : MediaTypeFormatter
    {
        public MultipartFormDataFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));
        }

        public override bool CanReadType(Type type)
        {
            return type == typeof(FileInB64);
        }

        public override bool CanWriteType(Type type)
        {
            return false;
        }

        public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var parts = await content.ReadAsMultipartAsync().ConfigureAwait(false);

            var tasks = parts.Contents?.Select(ToViewModel).ToArray();
            
            var files = await Task.WhenAll(tasks).ConfigureAwait(false);

            if (files?.Length == 0)
            {
                throw new ArgumentNullException();
            }

            return files;
        }

        private async Task<FileInB64> ToViewModel(HttpContent httpContent)
        {
            return new FileInB64
            {
                Name = httpContent.Headers.ContentDisposition.FileName.Trim('"'),
                Type = httpContent.Headers.ContentType.MediaType,
                B64Str = await httpContent.ReadAsStringAsync().ConfigureAwait(false)
            };
        }
    }
}