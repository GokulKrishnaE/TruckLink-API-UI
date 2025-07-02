import { NgModule } from '@angular/core';

import { ToastModule } from 'primeng/toast';
import { SkeletonModule } from 'primeng/skeleton';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { CommonModule } from '@angular/common';
import { TabsModule } from 'primeng/tabs';
import { MenuModule } from 'primeng/menu';
import { Menu } from 'primeng/menu';



@NgModule({
  declarations: [
  ],
  imports: [
    CommonModule,
    ToastModule,
    SkeletonModule,
    ProgressSpinnerModule,
    TabsModule,
    MenuModule,
    Menu
  ],
  exports:[
    CommonModule,
    ToastModule,
    SkeletonModule,
    ProgressSpinnerModule,
    TabsModule,
    MenuModule,
    Menu
  ]
})
export class SharedModule { }
