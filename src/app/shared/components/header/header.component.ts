import { Component, inject } from '@angular/core';
import { AvatarModule } from 'primeng/avatar';
import { AvatarGroupModule } from 'primeng/avatargroup';
import { AuthService } from '../../../features/auth/auth.service';
import { User } from '../../../model/user.type';
import { Subscription } from 'rxjs';
import { MenuItem } from 'primeng/api';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { RouterModule } from '@angular/router';
@Component({
  selector: 'app-header',
  imports: [AvatarModule, AvatarGroupModule, MenuModule, ButtonModule, RouterModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  private authService = inject(AuthService);
  items: MenuItem[] | undefined;
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
    this.items = [
      {
        label: 'Dashboard',
        icon: 'pi pi-objects-column',
        routerLink: '/dashboard'
      },
      {
        label: 'Document',
        icon: 'pi pi-file-o',
        routerLink: '/document'
      },
      {
        label: 'Objectives',
        icon: 'pi pi-chart-bar',
        routerLink: '/objectives'
      },
      {
        label: 'Logout',
        icon: 'pi pi-sign-out',
        command: () => this.logout()
      }
    ];
  }

  ngOnDestroy(): void {
    // ยกเลิก subscription เมื่อ component ถูกทำลาย เพื่อป้องกัน memory leak
    if (this.userSubscription) {
      this.userSubscription.unsubscribe();
    }
  }
  logout() {
    this.authService.logout();
  }
}
