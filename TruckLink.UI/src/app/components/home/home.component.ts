import { Component } from '@angular/core';
import { AuthService } from '../../shared/services/auth.service';
import { CommonService } from '../../shared/services/common.service';
import { TruckLinkService } from '../services/truckLink.service';
import { JobModel } from '../../shared/models/job.model';
import { debounceTime, Subject, switchMap, takeUntil } from 'rxjs';

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

   filters = {
    search: '',
    distance: null,
    startPlace: null,
    endPlace: null
  };

  options = {
    distance: [
      { label: 'Select Distance', value: '' },
      { label: '0-250 km', value: '0-250' },
      { label: '250-1000 km', value: '250-1000' },
      { label: '1000-2000 km', value: '1000-2000' },
      { label: '2000+ km', value: '2000+' }
    ],
    startPlace: [
      { label: 'Select Start Place', value: '' },
      { label: 'Kannur', value: 'Kannur' },
      { label: 'Kozhikode', value: 'Kozhikode' },
      { label: 'Ernakulam', value: 'Ernakulam' },
      { label: 'Trivandrum', value: 'Trivandrum' },
      { label: 'Chennai', value: 'Chennai' },
      { label: 'Mysore', value: 'Mysore' },
      { label: 'Banglore', value: 'Banglore' },
      { label: 'Mumbai', value: 'Mumbai' },
      { label: 'Punjab', value: 'Punjab' },
      { label: 'Kashmir', value: 'Kashmir' }
    ],
    endPlace: [
      { label: 'Select Destination', value: '' },
      { label: 'Kannur', value: 'Kannur' },
      { label: 'Kozhikode', value: 'Kozhikode' },
      { label: 'Ernakulam', value: 'Ernakulam' },
      { label: 'Trivandrum', value: 'Trivandrum' },
      { label: 'Chennai', value: 'Chennai' },
      { label: 'Mysore', value: 'Mysore' },
      { label: 'Banglore', value: 'Banglore' },
      { label: 'Mumbai', value: 'Mumbai' },
      { label: 'Punjab', value: 'Punjab' },
      { label: 'Kashmir', value: 'Kashmir' }
    ]
  };

  private destroy$ = new Subject<void>();
  private filterChanged$ = new Subject<void>();

  constructor(
    private authService: AuthService,
    private commonService:CommonService,
    private truckLinkService:TruckLinkService,
  ) {
      this.userName$ = this.authService.userName$;
  }

  ngOnInit(){
    this.filterChanged$.pipe(
      debounceTime(300),
      switchMap(() => this.truckLinkService.getJobs({
        search: this.filters.search,
        distance: this.filters.distance,
        startPlace: this.filters.startPlace,
        endPlace: this.filters.endPlace
      })),
      takeUntil(this.destroy$)
    ).subscribe((trips:any) => {
      console.log(trips)
      this.jobs = trips.data.jobs
    });
    this.onFilterChange();
  }

   onFilterChange() {
    this.filterChanged$.next();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
  
  // getJobs(){
  //   this.truckLinkService.getJobs()
  //   .subscribe({
  //     next:(res:any)=>{
  //       this.jobs = res.data.jobs
  //       console.log(this.jobs)
  //     },
  //     error: (e)=>{
  //       console.log(e)
  //       this.commonService.showToast('error','Error','Something went wrong')
  //     }
  //   })
  // }

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

  closeInterestModal(){
    this.interested = false
  }
}
