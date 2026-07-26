import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { combineLatest, map } from 'rxjs';
import { MenuItem } from 'primeng/api';
import { NavigationEnd, Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  
  isDriver$
  isPoster$
  isDriverOrPoster$ 
  userName$
  isMenuOpen = false;
  items:MenuItem[] = [
        {
          items: [
              {
                  label: 'My Profile',
                  icon: 'pi pi-refresh'
              },
              {
                  label: 'Logout',
                  icon: 'pi pi-upload',
                  command: () => {
                    this.logout();
                }
              }
          ]
        }
    ];

  constructor(private authService:AuthService,private router: Router){
    this.isDriver$ = this.authService.isDriver$
    this.isPoster$ = this.authService.isPoster$
    this.userName$ = this.authService.userName$
    this.isDriverOrPoster$ = combineLatest([this.isDriver$, this.isPoster$]).pipe(
    map(([isDriver, isPoster]) => isDriver || isPoster));

    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.isMenuOpen = false;
      }
    });
  }


  logout(){
    this.authService.logout()
  }

  toggleHamburger(){
    this.isMenuOpen = !this.isMenuOpen
  }

}
