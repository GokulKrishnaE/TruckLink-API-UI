import { Component } from '@angular/core';
import { AuthService } from '../../shared/services/auth.service';
import { CommonService } from '../../shared/services/common.service';
import { TruckLinkService } from '../services/truckLink.service';
import { JobModel } from '../../shared/models/job.model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {

  userName$
  jobs!:JobModel[]
  jobDetail:JobModel | null = null;
  interested = false
  userMobNumber!:string

  constructor(
    private authService: AuthService,
    private commonService:CommonService,
    private truckLinkService:TruckLinkService,
  ) {
      this.userName$ = this.authService.userName$;
  }

  ngOnInit(){
    this.getJobs()
  }
  
  getJobs(){
    this.truckLinkService.getJobs()
    .subscribe({
      next:(res:any)=>{
        this.jobs = res.data.jobs
        console.log(this.jobs)
      },
      error: (e)=>{
        console.log(e)
        this.commonService.showToast('error','Error','Something went wrong')
      }
    })
  }

  seeJob(id:number){
    if(id){
      console.log(this.jobs)
      this.jobDetail = this.jobs.find(x=>x.id===id) as JobModel
    }
    console.log(this.jobDetail)
  }
  closeJobModal(){
    this.jobDetail = null
  }
  getUserDetails(){
    this.interested = true
  }
  sendInterest(){
    if(this.userMobNumber){
        this.truckLinkService.requestJob(this.jobDetail?.id,this.userMobNumber)
        .subscribe({
          next:(res)=>{
            console.log(res)
          },
          error:(e)=>{
            console.log(e)
          }
        })
        this.interested = false
        this.jobDetail = null
    }
  }
}
