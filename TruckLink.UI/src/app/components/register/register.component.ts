import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TruckLinkService } from '../services/truckLink.service';

import { RegisterModel } from '../../shared/models/auth.model';
import { CommonService } from '../../shared/services/common.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {

   registerForm: FormGroup;
   registerModel!:RegisterModel

  constructor(private fb: FormBuilder, private truckLinkService:TruckLinkService,private commonService:CommonService) {
    this.registerForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      role: ['Driver', Validators.required]  // 'Driver' or 'Poster'
    });
  }

  onSubmit() {
    if (this.registerForm.valid) {
      this.registerModel = this.registerForm.value
      this.truckLinkService.register(this.registerModel).subscribe({
        next: (data:any)=>{
          this.commonService.showToast('success','Success',data.message)
        },
        error: (error:any)=>{
          console.log(error)
        }
      })
    } else {
      this.registerForm.markAllAsTouched();
    }
  }
}
