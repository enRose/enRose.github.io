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

## Option 1: multipart

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
      }
    )
}
```

FormData is a browser API that allows you manually construct a key-value pair payload for html form.

There are a couple of ways to use it:
1. new FormData() - currently empty
2. new FormData(form) - pre-populate with a form's input data

In the handler, we new a FormData object append files one by one to it then send it off the backend in body.

> Key thing to note is does not matter which approach to initialise a FormData object, when we send it, it will always use multipart which will be [7bit, 8bit or binary](https://www.w3.org/Protocols/rfc1341/7_2_Multipart.html) encoding for the part body.

In the backend, we can either use a multipart media type formatter which will bind it to a complex type in the action parameter. Or we manipulate the files with ```HttpContext.Current.Request.Files``` in the controller action.

Let's go through the second approach here and I will demonstrate the media type formatter in a [separate article]().

```C#
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
```



## Demo

The client is scaffolded using create-react-app.

Run npm install under root, then npm start.




## API

## Reference
[formData() MDN](https://developer.mozilla.org/en-US/docs/Web/API/FormData/FormData)