import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-sizer',
  templateUrl: './sizer.component.html',
  styleUrls: ['./sizer.component.css']
})
export class SizerComponent implements OnInit {
  @Input()  size!: number | string;
  @Output() sizeChange = new EventEmitter<number>();
  
  constructor() { }

  resize(delta: number) {
    this.size = Math.min(40, Math.max(8, +this.size + delta));
    this.sizeChange.emit(this.size);
  }
  
  decrease() { 
    this.resize(-1); 
  }
  
  increase() { 
    this.resize(+1); 
  }
  
  
  ngOnInit(): void {
  }

}
