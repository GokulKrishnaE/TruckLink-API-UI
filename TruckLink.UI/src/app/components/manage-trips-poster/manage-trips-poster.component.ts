import { Component } from '@angular/core';
import { TruckLinkService } from '../services/truckLink.service';
import { CommonService } from '../../shared/services/common.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';


@Component({
  selector: 'app-manage-trips-poster',
  templateUrl: './manage-trips-poster.component.html',
  styleUrl: './manage-trips-poster.component.scss'
})
export class ManageTripsPosterComponent {


  posterJobs!:any[]
  jobDetail:any
  acceptDriverId!:number
  confirmModalMessage:string = ''
  jobForm!: FormGroup;
  isEditing = false;
  confirmItem = '';

  constructor(
    private truckLinkService:TruckLinkService,
    private commonService:CommonService,
    private fb:FormBuilder
  ){
    this.jobForm = this.fb.group({
      loadItem: ['', Validators.required],
      startLocation: ['', Validators.required],
      destination: ['', Validators.required],
      earnings: [0, Validators.required],
      distanceKm: [0, Validators.required],
      mapUrl: ['']
    });
  }

  ngOnInit(){
    this.getPosterJobs()
  }


  setJobsInForm() {
    this.jobForm.patchValue({
      loadItem: this.jobDetail[0].loadItem,
      startLocation: this.jobDetail[0].startLocation,
      destination: this.jobDetail[0].destination,
      earnings: this.jobDetail[0].earnings,
      distanceKm: this.jobDetail[0].distanceKm,
      mapUrl: this.jobDetail[0].mapUrl
    });
  }


  getPosterJobs(){
    this.truckLinkService.getJobsForPoster()
    .subscribe({
      next: (res:any)=>{
        this.posterJobs = res.data.jobs
      },
      error: (e)=>{
        console.log(e)
      }
    })
  }

  openRequests(jobId:any){
    if(jobId){
      this.jobDetail = this.posterJobs.filter(x=>x.jobId===jobId)
      this.setJobsInForm()
    }
    console.log(this.jobDetail)
  }

  closeRequestView(){
    this.jobDetail = undefined
    this.isEditing = false
  }


  editJobSubmit() {
    if (this.jobForm.valid) {
      console.log('Updated Job:', this.jobForm.value);
      this.isEditing = false;
      this.truckLinkService.updateJob(this.jobDetail[0].jobId,this.jobForm.value)
      .subscribe({
        next: (res)=>{
          console.log(res)
          this.commonService.showToast('success','Success','Job has been updated')
        },
        error:(e)=>{
          console.log(e)
          this.commonService.showToast('error','Error','Something went wrong. Please try again')
        }
      })
   
      // Call API here to update job
    }
  }

  confirmModalResponse(response:string){
    if(this.confirmItem==='accept'){
      this.confirmItem = ''
      this.acceptDriverId = 0
      if(!this.jobDetail[0].isAccepted){
        if(response==='yes'){
          this.truckLinkService.acceptRequest(this.jobDetail[0].jobId,this.acceptDriverId)
          .subscribe({
            next:(res)=>{
              this.commonService.showToast('success','Success','Driver has been accepted for this job')
            },
            error:(e)=>{
              this.commonService.showToast('error','Error','Something went wrong. Please try again')
            }
          })
        }
      }
      else{
          this.commonService.showToast('info','Job Already Accepted','This job has already been accepted')
      }
    }
    if(this.confirmItem==='delete'){
      this.confirmItem = ''
      if(response==='yes'){
        this.truckLinkService.deleteJob(this.jobDetail[0].jobId)
        .subscribe({
          next: (res)=>{
            console.log(res)
            this.commonService.showToast('success','Success','Job has been deleted successfully')
            this.jobDetail = undefined
            this.getPosterJobs()
          },
          error: (e)=>{
            console.log(e)
            this.commonService.showToast('error','Error',e.error.messaeg)
          }
        })
      }
    }
    if(this.confirmItem==='completeJob'){
      this.confirmItem = ''
      this.truckLinkService.completeJob(this.jobDetail[0].jobId)
      .subscribe({
        next: (res)=>{
          console.log(res)
          this.commonService.showToast('success','Success','Job has been marked complete')
          this.jobDetail = undefined
          this.getPosterJobs()
        },
        error: (e)=>{
          console.log(e)
           this.commonService.showToast('error','Error',e.error)
        }
      })
    }
    
}

  routeToMap(url: string): void {
    const isGoogleMapsUrl = /^https?:\/\/(www\.)?google\.(com|[a-z]{2})(\.[a-z]{2})?\/maps/.test(url);

    if (isGoogleMapsUrl) {
      window.open(url, '_blank');
    } else{
      this.commonService.showToast('error','Invalid URL','Provide a valid google maps url')
    }
  }


  acceptJobConfirm(driverId:number){
    this.confirmItem  = 'accept'
    this.acceptDriverId = driverId
    this.confirmModalMessage = 'Are you sure want to accept this driver?'
  }

  editJob(){
     this.isEditing = true
  }

  deleteJobConfirm(){
    this.confirmItem  = 'delete'
    this.confirmModalMessage = 'Are you sure want to delete the job?'
  }

  completeJobConfirm(){
    this.confirmItem  = 'completeJob'
    this.confirmModalMessage = 'Are you sure want to complete the job?'
  }
}
