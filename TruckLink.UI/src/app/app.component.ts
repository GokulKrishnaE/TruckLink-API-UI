import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonService } from './shared/services/common.service';
import { AuthService } from './shared/services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'TruckLink.UI';
  loading$
  isAuthorized$

  constructor(private commonService:CommonService,private authService:AuthService) {
      this.loading$ = this.commonService.loading$
      this.isAuthorized$ = this.authService.isAuthorized$
      
  }
}
