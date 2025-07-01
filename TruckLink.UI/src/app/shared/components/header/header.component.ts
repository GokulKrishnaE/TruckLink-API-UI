import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { combineLatest, map } from 'rxjs';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  
  isDriver$
  isPoster$
  isDriverOrPoster$ 
  isMenuOpen = false;

  constructor(private authService:AuthService){
    this.isDriver$ = this.authService.isDriver$
    this.isPoster$ = this.authService.isPoster$
    this.isDriverOrPoster$ = combineLatest([this.isDriver$, this.isPoster$]).pipe(
    map(([isDriver, isPoster]) => isDriver || isPoster)
  );
  }


  logout(){
    this.authService.logout()
  }

  toggleHamburger(){
    this.isMenuOpen = !this.isMenuOpen
  }

}
