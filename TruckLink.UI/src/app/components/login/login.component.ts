import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TruckLinkService } from '../services/truckLink.service';
import { Router } from '@angular/router';
import { LoginModel } from '../../shared/models/auth.model';
import { AuthService } from '../../shared/services/auth.service';
import { CommonService } from '../../shared/services/common.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  loginForm: FormGroup;
  formSubmitted = false;
  loginModel!:LoginModel

  constructor(
    private fb: FormBuilder,
    private truckLinkService:TruckLinkService,
    private authService:AuthService,
    private router:Router,
    private commonService:CommonService
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  ngOnInit(){
     this.authService.logout();  
  }

  onSubmit() {
    this.formSubmitted = true;
    if (this.loginForm.valid) {
      this.loginModel = this.loginForm.value;
      this.truckLinkService.login(this.loginModel).subscribe({
        next: (res:any) => {
          if(res){
            this.router.navigate(['home'])
          }
        },
        error: (err:any) => {
          this.commonService.showToast('error','Error','Login failed')
        }
      });
    }
  }
}
