import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar.component';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
@Component({
  selector: 'app-main-layout',
  standalone: true,
  providers: [MessageService,ConfirmationService],
  imports: [
    RouterOutlet,
    SidebarComponent,
    HeaderComponent,
    ToastModule,
  ],
  template: `
  <div class=" text-zinc-100">
    <div class="bg-gray-950 sticky z-50 top-0 py-2 border-b-2 border-gray-800">
      <div class="max-w-7xl mx-auto">
        <app-header></app-header>
      </div>
    </div> 
    <div class="flex h-screen max-w-7xl my-6 mx-auto">
      <!-- Sidebar -->
      <app-sidebar class="w-70"></app-sidebar>
      <div class="flex-1 flex flex-col ">
        <!-- Header -->
        <!-- Main Content Area -->
        <main class="flex-1 px-6">
          <p-toast></p-toast>
          <router-outlet></router-outlet>
        </main>
      </div>
    </div>
  </div>
     
    `
})
export class MainLayoutComponent { }