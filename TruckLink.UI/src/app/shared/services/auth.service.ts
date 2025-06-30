import { Injectable } from "@angular/core";
import { BehaviorSubject, Subject } from "rxjs";
import {jwtDecode} from 'jwt-decode';
import { Router } from "@angular/router";

@Injectable({
    providedIn: 'root'
})


export class AuthService{

   private accessTokenKey = 'access_token';

  private isDriverSubject = new BehaviorSubject<boolean>(false);
  private isPosterSubject = new BehaviorSubject<boolean>(false);
  private userNameSubject = new BehaviorSubject<string | null>(null);
  private isAuthorized = new BehaviorSubject<boolean>(false);

  isDriver$ = this.isDriverSubject.asObservable();
  isPoster$ = this.isPosterSubject.asObservable();
  userName$ = this.userNameSubject.asObservable();
  isAuthorized$ = this.isAuthorized.asObservable();

  constructor(private router:Router) {
    // Optionally restore from token at app init
    const token = localStorage.getItem(this.accessTokenKey);
    if (token) this.configureUser(token);
  }

  configureUser(token: string): boolean {
    try {
      localStorage.setItem(this.accessTokenKey, token);
      const decoded = jwtDecode<any>(token);

      const role = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
      const name = decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];

      this.userNameSubject.next(name || 'User');

      this.isDriverSubject.next(role === 'Driver');
      this.isPosterSubject.next(role === 'Poster');

      this.isAuthorized.next(true)
      return true
    } catch (e) {
      console.error('Failed to decode token', e);
      this.isAuthorized.next(false)
      return false
    }
  }

  getToken(): string | null {
    return localStorage.getItem(this.accessTokenKey);
  }


  logout(): void {
    localStorage.removeItem(this.accessTokenKey);
    this.userNameSubject.next(null);
    this.isDriverSubject.next(false);
    this.isPosterSubject.next(false);
    this.isAuthorized.next(false);
    this.router.navigate(['login'])
  }
    
}