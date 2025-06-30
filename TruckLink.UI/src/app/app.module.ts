import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';

import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { HomeComponent } from './components/home/home.component';


import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { SharedModule } from './shared/module/shared.module';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { providePrimeNG } from 'primeng/config';
import { Messages } from 'primeng/messages';
import { MessageService } from 'primeng/api';
import Aura from '@primeng/themes/aura';
import { AuthInterceptor } from './shared/interceptors/auth.interceptor';
import { HeaderComponent } from './shared/components/header/header.component';
import { AddJobComponent } from './components/add-job/add-job.component';
import { MyTripsDriverComponent } from './components/my-trips-driver/my-trips-driver.component';
import { ManageTripsPosterComponent } from './components/manage-trips-poster/manage-trips-poster.component';
import { ConfirmModalComponent } from './shared/components/confirm-modal/confirm-modal.component';



@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    HomeComponent,
    HeaderComponent,
    AddJobComponent,
    MyTripsDriverComponent,
    ManageTripsPosterComponent,
    ConfirmModalComponent
  ],
  imports: [
    SharedModule,
    BrowserModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    AppRoutingModule
  ],
  providers: [
    provideAnimationsAsync(),
     providePrimeNG({
          theme: {
              preset: Aura,
              options: {
                  darkModeSelector: false
              }
          }
      }),
      {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    MessageService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
