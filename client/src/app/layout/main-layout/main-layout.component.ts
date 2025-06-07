import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar.component';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
@Component({
  selector: 'app-main-layout',
  standalone: true,
  providers: [MessageService, ConfirmationService],
  imports: [
    RouterOutlet,
    SidebarComponent,
    HeaderComponent,
    ToastModule,
  ],
  template: `
  <div class=" text-zinc-100">
    <div class="bg-gray-950 sticky z-50 top-0 py-2 border-b-2 border-gray-800">
      <div class="max-w-7xl mx-auto ">
        <app-header></app-header>
      </div>
    </div> 
    <div class="grid lg:flex lg:flex-row-reverse h-screen max-w-7xl my-6 mx-auto">
      <div class="flex-4 flex flex-col ">
        <!-- Header -->
        <!-- Main Content Area -->
        <main class=" lg:flex-1 px-6">
          <p-toast></p-toast>
          <router-outlet></router-outlet>
        </main>
      </div>
      <!-- Sidebar -->
      <app-sidebar class="lg:flex-1 lg:bg-transparent lg:relative fixed bg-slate-950 bottom-0 w-full py-4"></app-sidebar>
    </div>
  </div>
     
    `
})
export class MainLayoutComponent { }