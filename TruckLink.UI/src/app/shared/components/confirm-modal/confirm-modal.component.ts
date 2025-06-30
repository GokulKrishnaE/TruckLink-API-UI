import { Component, EventEmitter, Input, Output, output } from '@angular/core';

@Component({
  selector: 'app-confirm-modal',
  templateUrl: './confirm-modal.component.html',
  styleUrl: './confirm-modal.component.scss'
})
export class ConfirmModalComponent {
 
  @Input() confirmModalMessage:string = ''

  @Output() confirmResponse = new EventEmitter()


  response(res:string){
    this.confirmResponse.emit(res)
  }
}
