import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { JobModel } from '../../shared/models/job.model';
import { TruckLinkService } from '../services/truckLink.service';
import { CommonService } from '../../shared/services/common.service';
import { Router } from '@angular/router';
import { Observable, Subject } from 'rxjs';

@Component({
  selector: 'app-add-job',
  templateUrl: './add-job.component.html',
  styleUrl: './add-job.component.scss'
})
export class AddJobComponent {

  jobForm: FormGroup;
  unsavedForm = false;
  unsavedModelMessage = "You have unsaved changes. Do you really want to leave?"
  private confirmLeaveSubject: Subject<boolean> | null = null;

  constructor(private fb: FormBuilder, private truckLinkService:TruckLinkService, private commonService:CommonService,private router:Router) {
    this.jobForm = this.fb.group({
      loadItem: ['', Validators.required],
      description: ['', Validators.required],
      contactInfo: ['', Validators.required],
      startLocation: ['', Validators.required],
      destination: ['', Validators.required],
      earnings: [null, [Validators.required, Validators.min(0)]],
      distanceKm: [null, [Validators.required, Validators.min(0)]],
      mapUrl: ['', [Validators.required, Validators.pattern(/^https?:\/\/.+/)]],
    });
  }

  onSubmit(): void {
    if (this.jobForm.valid) {
      console.log(this.jobForm.valid)
      const jobData: JobModel = this.jobForm.value;
      this.truckLinkService.postJobs(jobData)
      .subscribe({
        next: (data)=>{
          this.commonService.showToast('success','Success','Job added successfully')
          setTimeout(() => {
            this.router.navigate(['/home'])
          }, 3000);
        },
        error:(err)=>{
          this.commonService.showToast('error','Error','Something went wrong')
        }
      })
    } else {
      this.jobForm.markAllAsTouched();
    }
  }
  
  canDeactivate():Observable<boolean> | boolean {
    if (!this.jobForm.dirty) return true;
    this.unsavedForm = true;
    this.confirmLeaveSubject = new Subject<boolean>();
    return this.confirmLeaveSubject.asObservable();
  }

  // This will be triggered by <confirm> component
  onConfirmResponse(res: string) {
    console.log(res)
    const allowLeave = res === 'yes';
    this.confirmLeaveSubject?.next(allowLeave);
    this.confirmLeaveSubject?.complete();
    this.unsavedForm = false;
  }

  navigateToHome(){
    this.router.navigate(['/home'])
  }
}
