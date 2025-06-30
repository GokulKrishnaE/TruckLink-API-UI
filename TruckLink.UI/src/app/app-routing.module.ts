import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { HomeComponent } from './components/home/home.component';
import { AddJobComponent } from './components/add-job/add-job.component';
import { MyTripsDriverComponent } from './components/my-trips-driver/my-trips-driver.component';
import { ManageTripsPosterComponent } from './components/manage-trips-poster/manage-trips-poster.component';

const routes: Routes = [
  {path:'login',component:LoginComponent},
  {path:'register',component:RegisterComponent},
  {path:'home',component:HomeComponent},
  {path:'add-job',component:AddJobComponent},
  {path:'my-trips',component:MyTripsDriverComponent},
  {path:'manage-trips',component:ManageTripsPosterComponent},
  {path:'',component:LoginComponent},
  {path:'**',redirectTo:''}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
