import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class BaseService {

    baseUrl!:string;

    constructor(private http: HttpClient) {
        
        this.baseUrl = environment.baseApiUrl
    }


    get<T>(url: string, params?: HttpParams): Observable<T> {
        return this.http.get<any>(this.baseUrl+url, { params });
    }

    post<T>(url: string, body: any, headers?: HttpHeaders): Observable<T> {
        return this.http.post<T>(this.baseUrl+url, body, { headers });
    }

    put<T>(url: string, body: any, headers?: HttpHeaders): Observable<T> {
        return this.http.put<T>(this.baseUrl+url, body, { headers });
    }

    patch<T>(url: string, body: any, headers?: HttpHeaders): Observable<T> {
        return this.http.patch<T>(this.baseUrl+url, body, { headers });
    }

    delete<T>(url: string, params?: HttpParams): Observable<T> {
        return this.http.delete<T>(this.baseUrl+url, { params });
    }
}
