import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class DemoService {

  constructor() { }

  getMessage() {
    return 'Hello Demo!';
  }  
}
