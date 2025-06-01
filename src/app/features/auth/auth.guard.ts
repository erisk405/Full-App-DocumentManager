import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // ตรวจสอบ token แทนการตรวจสอบ currentUser
  const isAuthenticated = authService.isLoggedIn();
  console.log("isAuthenticated", isAuthenticated);
  
  if (isAuthenticated) {
    // มี token ในระบบ อนุญาตให้เข้าถึง
    // จะเรียก refreshUserData เพื่ออัปเดตข้อมูล user แต่ไม่ต้องรอผลลัพธ์
    authService.refreshUserData();
    return true;
  }

  // ไม่มี token หรือหมดอายุ ให้กลับไปที่หน้า login
  router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
  return false;
};
