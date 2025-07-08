import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { HomeComponent } from './components/home/home.component';
import { AddJobComponent } from './components/add-job/add-job.component';
import { MyTripsDriverComponent } from './components/my-trips-driver/my-trips-driver.component';
import { ManageTripsPosterComponent } from './components/manage-trips-poster/manage-trips-poster.component';
import { AuthGuard } from './shared/guards/auth.guard';
import { RoleGuard } from './shared/guards/role.guard';


const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { 
    path: 'home', 
    component: HomeComponent, 
    canActivate: [AuthGuard] 
  },
  { 
    path: 'add-job', 
    component: AddJobComponent, 
    canActivate: [AuthGuard, RoleGuard], 
    data: { expectedRole: 'Poster' } 
  },
  { 
    path: 'my-trips', 
    component: MyTripsDriverComponent, 
    canActivate: [AuthGuard, RoleGuard], 
    data: { expectedRole: 'Driver' } 
  },
  { 
    path: 'manage-trips', 
    component: ManageTripsPosterComponent, 
    canActivate: [AuthGuard, RoleGuard], 
    data: { expectedRole: 'Poster' } 
  },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
