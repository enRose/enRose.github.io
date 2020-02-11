using System.Collections.Generic;
using System.IO;
using System.Web;

namespace DocUploadApi.Models
{
    public class MutipartViewModel
    {
        public string Name { get; set; }
   
        public string ContentType { get; set; }

        public long? ContentSize { get; set; }

        public HttpPostedFileBase Attachment { get; set; }

        public List<HttpPostedFileBase> Attachments { get; set; }

        public Stream Stream { get; set; }
    }

    public class FileInB64
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public long? Size { get; set; }

        public string B64Str { get; set; }
    }
}