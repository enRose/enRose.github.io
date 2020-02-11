using DocUploadApi.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace DocUploadApi.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class FileUploadController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Multipart()
        {
            var file = HttpContext.Current.Request.Files.Count > 0 ?
            HttpContext.Current.Request.Files[0] : null;

            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);

                var path = Path.Combine(
                    HttpContext.Current.Server.MapPath("~/uploads"),
                    fileName
                );

                file.SaveAs(path);
            }

            return Ok();
        }

        [HttpPost]
        public void InB64([FromBody]FileInB64 v)
        {
            Debug.WriteLine("Woohoo: " + v);

            var path = Path.Combine(
                HttpContext.Current.Server.MapPath("~/uploads"),
                v.Name
            );

            var r = Convert.FromBase64String(v.B64Str);

            File.WriteAllBytes(path, r);
        }

        // base64 seems to be the only encoding works with pdf
        // if we don't use multi-part.
        [HttpPost]
        public void Pdf([FromBody]string v)
        {
            var r = Convert.FromBase64String(v);
            var path = Path.Combine(
                HttpContext.Current.Server.MapPath("~/uploads"),
                Util.RandomString() + ".pdf"
            );
            File.WriteAllBytes(path, r);
        }

        [HttpPost]
        public void Txt([FromBody]string v)
        {
            var r = Encoding.UTF8.GetBytes(v);
            var path = Path.Combine(
                HttpContext.Current.Server.MapPath("~/uploads"),
                Util.RandomString() + ".txt"
            );
            File.WriteAllBytes(path, r);
        }
    }
}
