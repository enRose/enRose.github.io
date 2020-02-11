# File Upload

Most articles or answers on the net only explain either the client or backend leg. Hardly any resources as far as I could search for, explains the complete end-to-end flow.

So I created a little experiment of an JS client with a C# web api backend to fully explore the possible approaches for file upload.

Source code demo is available [here](https://github.com/enRose/enRose.github.io/tree/master/c-sharp/file-upload).

## Scope

This article is based on react and ASP.NET web api 2 for PDFs upload. For other backends and file formats, the handling might be syntactically different but the basic idea remains the same.

There are several ways to upload a file let's go through them:

## Option 1: multipart

I put multipart as the first option because in my opinion, this is the most correct way for file upload as it is specifically designed for file upload.

The easiest way is to use form where we set a button type to submit. On button click, form will pack the files up as multipart and send it off to the endpoint specified at the form action attribute. But we can do it manually which gives us more control.

First all, we need an html form. Inside that we have a what reactjs refers to as an uncontrolled input because when we specify input type as 'file' react will always treat it as a raw html input. In order to get hold of this input during re-render, we need to give it a ref which is created in constructor ```this.fileInput = React.createRef() ```

```javascript
<form encType="multipart/form-data"
name="file-upload-form" onSubmit={this.handleSubmit}>

    <input id='file-upload' type='file' ref={this.fileInput} multiple/>

    <button type="submit">Submit</button>
</form>
```

In the submit handler, we retrieve the files via the reference of the input:

```typescript
const files = this.fileInput.current.files
```


## Demo

The client is scaffolded using create-react-app.

Run npm install under root, then npm start.




## API