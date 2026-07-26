import { NgModule } from '@angular/core';

import { ToastModule } from 'primeng/toast';
import { SkeletonModule } from 'primeng/skeleton';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { CommonModule } from '@angular/common';
import { TabsModule } from 'primeng/tabs';
import {SelectModule} from 'primeng/select'
import { FormsModule } from '@angular/forms';


@NgModule({
  declarations: [
  ],
  imports: [
    CommonModule,
    FormsModule,
    ToastModule,
    SkeletonModule,
    ProgressSpinnerModule,
    TabsModule,
    SelectModule
  ],
  exports:[
    CommonModule,
    FormsModule,
    ToastModule,
    SkeletonModule,
    ProgressSpinnerModule,
    TabsModule,
    SelectModule
  ]
})
export class SharedModule { }
