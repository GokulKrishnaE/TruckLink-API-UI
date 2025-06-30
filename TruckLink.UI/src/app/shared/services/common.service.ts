import { Injectable } from "@angular/core";
import { MessageService } from "primeng/api";
import { BehaviorSubject } from "rxjs";

@Injectable({
    providedIn:'root'
})

export class CommonService{

    constructor(private messageService: MessageService) {}

    loading$ = new BehaviorSubject(false);

    showToast(severity:string,summary:string,detail:string){
       this.messageService.add({ severity: severity, summary: summary, detail: detail, life: 3000 }); 
    }

    showLoader() {
        this.loading$.next(true);
    }

    hideLoader() {
        this.loading$.next(false);
    }
}