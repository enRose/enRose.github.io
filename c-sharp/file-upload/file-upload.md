# File Upload

Most articles or answers on the net only explain either the client or backend leg. Hardly any resources as far as I could search for, explains the complete end-to-end flow.

So I created a little experiment of an JS client with a C# web api backend to fully explore the possible approaches for file upload.

Source code demo is available [here](https://github.com/enRose/enRose.github.io/tree/master/c-sharp/file-upload).

## Scope

This article is based on react and ASP.NET web api 2 for PDFs upload. For other backends and file formats, the handling might be syntactically different but the basic idea remains the same.

## Setup

First all, we need an html form. Inside that we have a what reactjs refers to as an uncontrolled input because when we specify input type as 'file' react will always treat it as a raw html input. In order to get hold of this input during re-render, we need to give it a ref which is created in the constructor of react component:

```javascript
constructor(props) {
    super(props)
    this.fileInput = React.createRef()
}
```

```javascript
<form encType="multipart/form-data"
name="file-upload-form" onSubmit={this.handleSubmit}>

    <input id='file-upload' type='file' ref={this.fileInput} multiple/>

    <button type="submit">Submit</button>
</form>
```

In the submit handler, we retrieve the files via the reference of the input:

```javascript
const files = this.fileInput.current.files
```

Now we have a form if we npm i and then npm start at the client demo root, we should have a form runs at http://localhost:3000.

Here are several ways to upload a file let's go through them:

## multipart

Multipart in my opinion is the most correct way for file upload as it is specifically designed for file upload.

The easiest way is to use form where we set a button type to submit. On button click, form will pack the files up as multipart and send it off to the endpoint specified at the form action attribute. But we can do it manually which gives us more control.

Here is our submit handler:

```javascript
handleSubmit = e => {
    e.preventDefault()

    const files = this.fileInput.current.files

    let formData = new FormData(e.target)

    for(let i = 0; i < files.length; i++) {
      formData.append( `file-${i}`, files[i])
    }

    fetch('https://localhost:44335/api/FileUpload/Multipart',
      {
        mode: 'cors',
        headers:{
          'Access-Control-Allow-Origin':'*'
        },
        method: "POST", body: formData,
      })
}
```

There are a couple of ways:
1. FormData is a browser API that allows you construct a key-value pair payload for html form.

    new FormData() - empty formData object

    new FormData(form) - pre-populate with a form's input data

2. [Manually construct body](https://developer.mozilla.org/en-US/docs/Learn/Forms/Sending_forms_through_JavaScript)

In the handler, we new a FormData object append files one by one to it then send it off the backend in body.

> Key thing to note is does not matter which approach to initialise a FormData object, when we send it, it will always use multipart which will be [7bit, 8bit or binary](https://www.w3.org/Protocols/rfc1341/7_2_Multipart.html) encoding for the part body.

In the backend, we can either use a multipart media type formatter which will bind it to a complex type in the action parameter. Or we manipulate the files with ```HttpContext.Current.Request.Files``` in the controller action.

Let's go through the second approach here and I will demonstrate the media type formatter in a [separate article]().

```C#
[HttpPost]
public IHttpActionResult Multipart()
{
    if (!Request.Content.IsMimeMultipartContent())
    {
        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
    }

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
```

This is probably the easiest backend code I can give. It can be separated out into a helper class that translates the payload.

## Base64 encoding files in complex type

> For PDF, base64 is the only encoding that I can get it to work. For txt files we simply use the default encode utf-8 when we read the file from disk

Although I don't recommend this approach but if we really want we can base64 encode each file and stick them into an array post them in http body along side with other information.

Below shows posting a single file but the principle is the same for multiple files.

```javascript
var file = this.fileInput.current.files[0]

console.log(file)

let reader = new FileReader()

reader.onload = () => {
    console.log(reader.result)

    const payload = {
        name: file.name,
        type: file.type,
        size: file.size,
        b64Str: reader.result.split('base64,')[1],
    }

    fetch('https://localhost:44335/api/FileUpload/InB64',
    {
        mode: 'cors', method: "POST", body: JSON.stringify(payload),
        headers: new Headers(
        {'content-type': 'application/json'},
        {'Access-Control-Allow-Origin':'*'},
        ),
    })
}

reader.readAsDataURL(file)
```

Okay, a few things to note here:
1. In FileReader API, we need an onload event listener so that when the file is read into the reader it will be ready in onload.

2. reader.readAsDataURL(file) encodes the file binary into a base64 string starts with 'data:application/pdf;base64,' After 'base64,' is the actual base64 encoded string. That's why we do

    ```javascript
    reader.result.split('base64,')[1]
    ```

3. Our payload is a complex type so we need ```JSON.stringify(payload)``` for the http body.

4. Since we are not sending data as multipart and we are using a complex type in http body so we need to let backend know the format of the body is ```application/json```. 

    **This is a subtle but very important setting**, if we don't set the mime type as json, by default it will be ```plain/text```. For plain text media type, C# backend needs a custom formatter to handle plain/text as well as a json deserializer to translate string back to its original complex type.

Now we have our client ready, it works as a normal post except we turn the file content into a base64 string so that it can be sent along as json in http body.

Okay, in the backend, so long as content-type is application/json, we can just instruct ASP.NET to bind the body string into a complex type.

```C#
public class FileInB64
{
    public string Name { get; set; }

    public string Type { get; set; }

    public long? Size { get; set; }

    public string B64Str { get; set; }
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
```

The ```[FromBody]``` data binding attribute will force ASP.NET to read body string into a complex type in our case it is called FileInB64. The base64 encoded file content will be mapped to its property B64Str. Then we convert base64 file content into bytes so we can write the file bytes to disk.

> So long as http request header content-type is set to ```application/json``` and ```[FromBody]``` is present, ASP.NET will always bind the json body payload into a complex type in controller action. Even if we have a custom media type formatter that can read the intended complex type.

## Base64 encode file in simple string

Again I do not recommend this but only to show the possibility here. I am not going through in great details here because I don't think anyone would do file upload this way in real life!

In this approach, we simply encode the PDF into a string then send the string as http body. Since the body is a string, in the backend we need a custom plain text media type formatter.

On the client this time we use ```reader.readAsArrayBuffer(file)```. It allows finer grain of data manipulation.

```javascript
var file = this.fileInput.current.files[0]

console.log(file)

let reader = new FileReader()

reader.onload = () => {
    console.log(reader.result)

    let charEncoded = '',
    binaryData = new Uint8Array(reader.result)

    for (let i = 0; i < binaryData.length; i++) {
        let currentByte = binaryData[i]

        charEncoded += String.fromCharCode(currentByte)
    }

    const payload = btoa(charEncoded)

    fetch('https://localhost:44335/api/FileUpload/Pdf',
    {
        mode: 'cors', type: 'text/plain',
        headers:{'Access-Control-Allow-Origin':'*'},
        method: "POST", body: payload,
    })
}

reader.readAsArrayBuffer(file)
```

```C#
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
```

```C#
// in Application_Start
GlobalConfiguration
    .Configuration
    .Formatters
    .Insert(0, new PlainTextMediaTypeFormatter());
```

```C#
// in controller
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
```

## Demo

Source code can be found [here](https://github.com/enRose/enRose.github.io/tree/master/c-sharp/file-upload).

Use chrome so we can use [Cors extension](https://chrome.google.com/webstore/detail/moesif-orign-cors-changer/digfbfaphojjndkpccljibejjbppifbc) to enable cors.

The client is scaffolded using create-react-app.
Under project root run:

```
npm i
npm start
```

Client on http://localhost:3000

Enable ASP.NET web api cors:

```C#
[EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
public class FileUploadController : ApiController
```

F5 Visual Studio api solution in IIS express.

API on https://localhost:44335

## Reference

[formData() MDN](https://developer.mozilla.org/en-US/docs/Web/API/FormData/FormData)

[manually construct multipart body](https://developer.mozilla.org/en-US/docs/Learn/Forms/Sending_forms_through_JavaScript)