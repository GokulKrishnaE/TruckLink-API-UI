import { Component } from '@angular/core';
import { TruckLinkService } from '../services/truckLink.service';
import { JobModel } from '../../shared/models/job.model';

@Component({
  selector: 'app-my-trips-driver',
  templateUrl: './my-trips-driver.component.html',
  styleUrl: './my-trips-driver.component.scss'
})
export class MyTripsDriverComponent {


  interestedJobs!:JobModel[]


  constructor(
    private truckLinkService:TruckLinkService
  ){}

  ngOnInit(){
    this.getInterestedJobs()
  }

  getInterestedJobs(){
    this.truckLinkService.getInterestedJobs()
    .subscribe({
      next: (res:any)=>{
       this.interestedJobs = res.data.jobs
       console.log(this.interestedJobs)
      },
      error: (e)=>{
        console.log(e)
      }
    })
  }
}
