import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { JobModel } from '../../shared/models/job.model';
import { TruckLinkService } from '../services/truckLink.service';
import { CommonService } from '../../shared/services/common.service';

@Component({
  selector: 'app-add-job',
  templateUrl: './add-job.component.html',
  styleUrl: './add-job.component.scss'
})
export class AddJobComponent {

  jobForm: FormGroup;

  constructor(private fb: FormBuilder, private truckLinkService:TruckLinkService, private commonService:CommonService) {
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
        },
        error:(err)=>{
          this.commonService.showToast('error','Error','Something went wrong')
        }
      })
    } else {
      this.jobForm.markAllAsTouched();
    }
  }
}
