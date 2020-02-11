import React from 'react'
import ReactDOM from 'react-dom'
import './index.css'

const encodePDFToBase64 = {
  option1: {
    readAsArrayBuffer: (reader, file) => reader.readAsArrayBuffer(file),

    toPayload: reader => {
      let charEncoded = '', binaryData = new Uint8Array(reader.result)
    
      for (let i = 0; i < binaryData.length; i++) {
        let currentByte = binaryData[i]
    
        charEncoded += String.fromCharCode(currentByte)
      }
    
      return btoa(charEncoded)
    }
  },

  option2: {
    // readAsDataURL returns a string starts with 'data:application/pdf;base64,'
    // after 'base64,' is the actual base64 encoded string
    readAsDataURL: (reader, file) => reader.readAsDataURL(file),
    toPayload: reader => reader.result.split("base64,")[1]
  }
}

// although file.readAsText could read PDFs, but we will not be 
// able to decode it on the server side.
// My guess is pdf content is binary it cannot be encoded by utf8.
// file.readAsText can only be used for txt format.
const txt = {
  readAsText: (reader, file) => reader.readAsText(file),
  toPayload: reader => reader.result
}

class App extends React.Component {
  constructor(props) {
    super(props)
    this.handleSubmit = this.handleSubmit.bind(this)
    this.fileInput = React.createRef()
  }

  handleSubmit = e => {
    e.preventDefault()

    /////////////
    //multi-part
    /////////////
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

    ////////////////////////////////////////////
    // client: wraps a single PDF file in b64 string
    //
    // backend: uses [FromBody] to force model binding to 
    // convert body json into a typed class 
    // so long as request header content-type is application/json 
    ////////////////////////////////////////////
    // var file = document.getElementById('file-upload').files[0]

    // console.log(file)

    // let reader = new FileReader()

    // reader.onload = () => {
    //   console.log(reader.result)

    //   const payload = {
    //     name: file.name,
    //     type: file.type,
    //     size: file.size,
    //     b64Str: reader.result.split('base64,')[1],
    //   }

    //   fetch('https://localhost:44335/api/FileUpload/InB64', 
    //     {
    //       mode: 'cors', method: "POST", body: JSON.stringify(payload),
    //       headers: new Headers(
    //         {'content-type': 'application/json'},
    //         {'Access-Control-Allow-Origin':'*'},
    //       ),
    //     }
    //   )
    // }

    // reader.readAsDataURL(file)

    /////////////////////////////////////
    // base64 encode PDF file in a string
    /////////////////////////////////////
    // var file = document.getElementById('file-upload').files[0]

    // console.log(file)

    // let reader = new FileReader()

    // reader.onload = () => {
    //   console.log(reader.result)

    //   fetch('https://localhost:44335/api/FileUpload/Pdf', 
    //     {
    //       mode: 'cors', type: 'text/plain',
    //       headers:{'Access-Control-Allow-Origin':'*'},
    //       method: "POST", body: reader.result.split("base64,")[1],
    //     }
    //   )
    // }

    // reader.readAsDataURL(file)

    ////////////////////////////////////
    // utf-8 encode txt file in a string
    ////////////////////////////////////
    // var file = document.getElementById('file-upload').files[0]

    // console.log(file)

    // let reader = new FileReader()

    // reader.onload = () => {
    //   console.log(reader.result)

    //   fetch('https://localhost:44335/api/FileUpload/Pdf', 
    //     {
    //       mode: 'cors', type: 'text/plain',
    //       headers:{'Access-Control-Allow-Origin':'*'},
    //       method: "POST", body: reader.result,
    //     }
    //   )
    // }

    // reader.readAsText(file)
  }

  render() {
    return (
      /////////////////////
      // uncontrolled input
      /////////////////////
      <form enctype="multipart/form-data"
        name="file-upload-form" onSubmit={this.handleSubmit}>
        
        <input id='file-upload' type='file' ref={this.fileInput} multiple/>

        <button type="submit">Submit</button>
      </form>
    )
  }
}

ReactDOM.render(<App />, document.getElementById('container'))
