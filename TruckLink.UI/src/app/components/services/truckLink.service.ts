import { Injectable } from "@angular/core";
import { catchError, map, Observable, of } from "rxjs";
import { LoginModel, RegisterModel } from "../../shared/models/auth.model";
import { AuthService } from "../../shared/services/auth.service";
import { BaseService } from "../../shared/services/base.service";
import { CommonService } from "../../shared/services/common.service";
import { JobModel } from "../../shared/models/job.model";
import { HttpParams } from "@angular/common/http";

@Injectable({
    providedIn: 'root'
})
export class TruckLinkService {

    constructor(
        private baseService: BaseService,
        private authService: AuthService,
        private commonService: CommonService
    ) {}

    login(loginModel: LoginModel): Observable<boolean> {
        return this.baseService.post('auth/login', loginModel).pipe(
            map((res: any) => {
                const token = res?.data?.token;
                if (res.isSuccess && token) {
                    return this.authService.configureUser(token);
                }
                return false;
            }),
            catchError(err => {
                this.commonService.showToast('error', 'Error', 'Login failed');
                return of(false);
            })
        );
    }

    register(registerModel: RegisterModel) {
        return this.baseService.post('auth/register', registerModel);
    }

   getJobs(filters: { search?: string; distance?: number | null; startPlace?: string | null; endPlace?: string | null }) {
        let params = new HttpParams();

        if (filters.search) params = params.set('search', filters.search);
        if (filters.distance != null) params = params.set('distance', filters.distance.toString());
        if (filters.startPlace) params = params.set('startPlace', filters.startPlace);
        if (filters.endPlace) params = params.set('endPlace', filters.endPlace);

        return this.baseService.get('jobs', { params });
    }

    getInterestedJobs(){
        return this.baseService.get('jobs/interested')
    }
    getJobsForPoster(){
        return this.baseService.get('jobs/with-requests')
    }

    postJobs(postJobModel: JobModel) {
        return this.baseService.post('jobs', postJobModel);
    }

    // New: Driver sends a job request/interest with mobile number
    requestJob(jobId: number|undefined, mobileNumber: string) {
        // mobileNumber is passed as a query param
        return this.baseService.post(`jobs/request/${jobId}?mobileNumber=${encodeURIComponent(mobileNumber)}`, null);
    }

    // New: Poster gets all job interests (driver requests)
    getJobInterestsForPoster() {
        return this.baseService.get('jobs/interests');
    }

    // New: Poster accepts a job request from a driver
    acceptJob(jobId: number, driverId: number) {
        return this.baseService.post(`jobs/accept/${jobId}?driverId=${driverId}`, null);
    }

    // New: Driver gets all jobs accepted by them
    getJobsForDriver(driverId: number) {
        return this.baseService.get(`jobs/driver/${driverId}`);
    }

    acceptRequest(jobId:number,driverId:number){
        return this.baseService.post('jobs/accept-request',{"jobId": jobId,"driverId": driverId})
    }

    updateJob(jobId: number, jobData: any) {
        return this.baseService.put(`jobs/${jobId}`, jobData);
    }
    deleteJob(jobId: number) {
        return this.baseService.delete(`jobs/${jobId}`);
    }
    completeJob(jobId: number) {
        return this.baseService.post(`jobs/${jobId}/complete`, {});
    }
}
