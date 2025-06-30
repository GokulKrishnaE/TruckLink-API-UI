import { NgModule } from '@angular/core';

import { ToastModule } from 'primeng/toast';
import { SkeletonModule } from 'primeng/skeleton';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { CommonModule } from '@angular/common';
import { TabsModule } from 'primeng/tabs';



@NgModule({
  declarations: [
  ],
  imports: [
    CommonModule,
    ToastModule,
    SkeletonModule,
    ProgressSpinnerModule,
    TabsModule
  ],
  exports:[
    CommonModule,
    ToastModule,
    SkeletonModule,
    ProgressSpinnerModule,
    TabsModule
  ]
})
export class SharedModule { }
