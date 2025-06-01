import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, EMPTY, Observable, of, tap } from 'rxjs';
import { LoginRequest, LoginResponse } from '../../model/login.type';
import { User } from '../../model/user.type';
import { environment } from '../../../environments/environment';



@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = `${environment.apiUrl}/auth`;
  private userApiUrl = `${environment.apiUrl}/user`;

  // เพิ่ม  BehaviorSubject สำหรับเก็บข้อมูล User
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  // Observable สำหรับ component ต่างๆ ใช้ subscribe
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) { 
    // โหลดข้อมูล User เมื่อ service ถูกสร้าง (เมื่อมีผู้ login แล้ว)
    if (this.isLoggedIn()) {
      this.loadUser();
    }
  }
  // เมธอดสำหรับโหลดข้อมูล User และอัพเดท BehaviorSubject
  private loadUser(): void {
    this.http.get<User>(`${this.userApiUrl}/me`)
      .pipe(
        tap(user => {
          this.currentUserSubject.next(user);// บันทึกข้อมูลที่ได้
        }),
        catchError(error => {
          console.error('Error loading user data:', error);
          return EMPTY;
        })
      ).subscribe();
  }

  // api
    
  // เพิ่มเมธอดเพื่อรับค่า User ปัจจุบัน (แบบไม่ต้อง subscribe)
  getCurrentUser(): User | null {
    return this.currentUserSubject.getValue();
  }
  
  // เพิ่มเมธอดสาธารณะสำหรับโหลดข้อมูล User ใหม่
  refreshUserData(): void {
    this.loadUser();
  }

  login(data: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.baseUrl}/login`, data).pipe(
      tap(response => {
        // บันทึก token ลงใน localStorage เมื่อ login สำเร็จ
        if (response && response.token) {
          localStorage.setItem('token', response.token);
          this.loadUser(); // โหลดข้อมูล User หลัง login สำเร็จ
        }
      })
    );
  }

  register(data: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/register`, data);
  }
  
  logout(): void {
    // รีเซ็ต User data เมื่อ logout
    this.currentUserSubject.next(null);
    // ลบ token ออกจาก localStorage
    localStorage.removeItem('token');
    // นำผู้ใช้กลับไปยังหน้า login
    this.router.navigate(['/login']);
  }
  
  // ตรวจสอบว่ามี token อยู่ใน localStorage หรือไม่
  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }
  
  // รับค่า token จาก localStorage
  getToken(): string | null {
    return localStorage.getItem('token');
  }
  
  // ตรวจสอบการเข้าสู่ระบบและนำทางกลับไปยังหน้า login ถ้าไม่ได้เข้าสู่ระบบ
  checkAuthStatus(): boolean {
    if (!this.isLoggedIn()) {
      this.router.navigate(['/login']);
      return false;
    }
    return true;
  }
}
