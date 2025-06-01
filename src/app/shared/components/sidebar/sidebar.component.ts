import { NgFor } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AvatarModule } from 'primeng/avatar';
import { AvatarGroupModule } from 'primeng/avatargroup';
import { AuthService } from '../../../features/auth/auth.service';
import { catchError, EMPTY, Subscription, tap } from 'rxjs';
import { User } from '../../../model/user.type';
@Component({
  selector: 'app-sidebar',
  imports: [RouterLink, RouterLinkActive, AvatarModule, AvatarGroupModule, NgFor],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent {
  private authService = inject(AuthService);

  // ตัวแปรสำหรับเก็บข้อมูล User
  currentUser!: User;

  // ตัวแปรสำหรับเก็บ subscription เพื่อ unsubscribe เมื่อ component ถูกทำลาย
  private userSubscription?: Subscription;

  ngOnInit(): void {
    // Subscribe ข้อมูล User จาก AuthService
    this.userSubscription = this.authService.currentUser$.subscribe(
      user => {
        this.currentUser = user!;
      }
    );
  } 
  ngOnDestroy(): void {
    // ยกเลิก subscription เมื่อ component ถูกทำลาย เพื่อป้องกัน memory leak
    if (this.userSubscription) {
      this.userSubscription.unsubscribe();
    }
  }
  menuItems = [
    { label: 'Dashboard', path: '/dashboard', icon: 'pi pi-objects-column' }, // ไอคอนบ้าน
    { label: 'Objectives', path: '/objectives', icon: 'pi pi-chart-bar' }, 
    { label: 'Document', path: '/document', icon: 'pi pi-file-o' }, // ไอคอนเอกสาร
    { label: 'Photo', path: '/photo', icon: 'pi pi-image' },
    { label: 'Hierarchy', path: '/hierarchy', icon: 'pi pi-gauge' },
  ];

  menuItems2 = [
    { label: 'Message', path: '/message', icon: 'pi pi-user' }, // ไอคอนผู้ใช้
    { label: 'Help', path: '/help', icon: 'pi pi-question-circle' },
    { label: 'Setting', path: '/setting', icon: 'pi pi-cog' }, 
  ];
}
