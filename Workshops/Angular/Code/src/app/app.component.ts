import { Component } from '@angular/core';
import { DemoService } from './demo.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'workshop';
  // Variables
  dateOfBirth: Date = new Date('23-June-1912');
  grinningFace: string = 'https://openmoji.org/data/color/svg/1F600.svg';
  contrast: string[] = ['inverted', 'large'];
  fontSize: number = 20;
  styling: string = 'highlighted';
  selected: boolean = false;
  styles: Record<string, string> = {};
  model: string = '';
  isShown: boolean = false;
  items: string[] = ['Hello', 'World'];
  values: any[] = [
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
  constructor(public demoService: DemoService) { }
  // Methods
  showMessage() {
    let message: string = 'Hello World';
    alert(message);
  }
  
  getMessage() {
    let message: string = 'Hello Again!';
    return message;
  }

  show(message: string) {
    alert(message);
  }

  setStyle() {
    this.selected = !this.selected;
    this.styles = {
      'font-weight': this.selected ? 'bold' : 'normal'
    };
  }

  toggle() {
    this.isShown = !this.isShown;
  }
    
}