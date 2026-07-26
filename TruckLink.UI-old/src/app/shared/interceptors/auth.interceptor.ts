import { Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpResponse,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, finalize } from 'rxjs';
import { CommonService } from '../services/common.service';


@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private commonService: CommonService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.commonService.showLoader();

    const token = localStorage.getItem('access_token');
    const skipAuth = req.url.includes('/auth/login') || req.url.includes('/auth/register');

    let headers = req.headers
      .set('Content-Type', 'application/json')
      .set('Accept', 'application/json');

    if (token && !skipAuth) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    const clonedReq = req.clone({ headers });

    return next.handle(clonedReq).pipe(
      finalize(() => this.commonService.hideLoader())
    );
  }
}