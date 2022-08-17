import React from 'react';
import { useState } from 'react';
import {useEffect } from 'react';
import './App.css';
// Classes
class Message extends React.Component {
  render() {
      return <h2>{this.props.value}</h2>;
  }
}

class Change extends React.Component {
  change = event => {
      this.setState(
          { value: event.target.value }
      );
  }

  constructor(props) {
      super(props);
      this.state = {
          value: props.value
      }
  }

  render() {
      return (
          <div>
              <input type="text" onChange={this.change}/>
              <h2>{this.state.value}</h2>
          </div>
      );
  }
}

class Elements extends React.Component {
  render() {
    const display = (value) => {
      switch(value.status)
      {
        case 'red':
          return <span style={{backgroundColor: 'red'}}>Danger</span>
        case 'yellow':
          return <span style={{backgroundColor: 'yellow'}}>Warning</span>
        case 'green':
          return <span style={{backgroundColor: 'green'}}>Proceed</span>
        default:
          return <span>None</span>
      }
    }
    const elements = (values) =>
    {
      return values.map((item) => 
        <li key={item.name}>{display(item)}</li>
      );
    };
    return (
      <ul style={{textAlign:'left'}}>
        {elements(this.props.value)}
      </ul>);
  }
}

// Variables
const message = 'Hello World';
const dateOfBirth = new Date('23-June-1912');
const contrast = ['inverted', 'large'].join(' ');
const style = { backgroundColor: 'yellow' };
const ImageContext = React.createContext('');
const inputMessage = React.createRef();
const items = ['Hello', 'World'];
const itemElements = items.map((item) => 
  <li key={item}>{item}</li>
);
const values = [
  {
    name: 'None',
    status: ''
  },
  {
    name: 'Danger',
    status: 'red'
  },
  {
    name: 'Warning',
    status: 'yellow'
  },
  {
    name: 'Proceed',
    status: 'green'
  }
];

// Methods
function AsDate(props) {
  return <div>{props.value.toDateString()}</div>
}

function Image() {
  let image = React.useContext(ImageContext);
  return <img src={image} alt="React" height="150" width="150"/>
}

function showMessage() {
  let message = 'Hello World';
  alert(message);
}

function ToggleStyle() {
  const [isSelected, selected] = useState(false);
  return (
  <div>
      <button style={{fontWeight: isSelected ? 'bold' : 'normal' }} 
      onClick={() => selected((value) => value = !value)}>Toggle Style</button>
  </div>
  );
}

function Sizer(props) {
  const [size, change] = useState(props.value);
  const resize = (delta) => change(() => Math.min(40, Math.max(8, + size + delta)));
  const decrease = () => {  
    resize(-1); 
  }
  const increase = () => {  
    resize(+1); 
  }

  useEffect(() => {
      document.getElementsByTagName('h1')[0].style.fontSize = size + 'px';
  })

  return (
    <div>
      <button type="button" onClick={decrease} title="Decrease">-</button>
      <button type="button" onClick={increase} title="Increase">+</button>
      <span style={{fontSize: size + 'px'}}>Font Size: {size}px</span>
    </div>
  );
}

function MessageInput()
{
  const show = () => {
    alert(inputMessage.current.value);
  }

  return (
    <div>
      <input type="text" ref={inputMessage}/>
      <button type="button" onClick={show}>Show</button>
    </div>
  );
}

function Toggle() {
  const [isShown, toggle] = useState(false);
  let message = '';
  if(isShown)
  {
    message = <h2>Hello World!</h2>
  }
  return (
  <div>
      <button onClick={() => toggle((value) => value = !value)}>Click Here</button>
      {message}
  </div>
  );
}

function Controlled() {
  const [name, setName] = useState('');
  const handleSubmit = (event) =>
  {
    event.preventDefault();
    alert(name);
  }

  return (
    <form onSubmit={handleSubmit}>
      <input id="name" type="text" 
      onChange={(event) => setName(event.target.value)}/>
      <input type="submit" value="Controlled"/>
    </form>
  )
}

function Uncontrolled() {
  let value = React.createRef();
  const handleSubmit = (event) =>
  {
    event.preventDefault();
    alert(value.current.value);
  }

  return (
    <form onSubmit={handleSubmit}>
      <input id="name" type="text" ref={value}/>
      <input type="submit" value="Uncontrolled"/>
    </form>
  )
}

// Application
function App() {
  return (
    <div className="App">
        <h1>{message}</h1>
        <Message value="Hello Again!"/>
        <AsDate value={dateOfBirth}/>
        <div><span className={contrast}>Contrast</span></div>
        <div><span style={style}>Highlighted</span></div>
        <ImageContext.Provider value="https://openmoji.org/data/color/svg/1F600.svg">
          <Image/>
        </ImageContext.Provider>
        <Change/>
        <button type="button" onClick={showMessage}>Show Message</button>
        <ToggleStyle/>
        <Sizer value="30"/>
        <MessageInput/>
        <Toggle/>
        <ul style={{textAlign:'left'}}>{itemElements}</ul>
        <Elements value={values}/>
        <Controlled/>
        <Uncontrolled/>
    </div>
  );
}

export default App;
