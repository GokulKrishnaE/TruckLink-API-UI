import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(private auth: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const expectedRole = route.data['expectedRole']; // 'Driver' or 'Poster'

    if (!this.auth.getToken()) {
      this.router.navigate(['/login']);
      return false;
    }

    const isAllowed = (expectedRole === 'Driver' && this.auth['isDriverSubject'].value) ||
                      (expectedRole === 'Poster' && this.auth['isPosterSubject'].value);

    if (!isAllowed) {
      this.router.navigate(['/login']);
      return false;
    }

    return true;
  }
}