import { HttpClient, HttpEvent, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Role } from '../../model/role.type';
import { Observable } from 'rxjs';
import { User } from '../../model/user.type';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  private roleApiUrl = `${environment.apiUrl}/role`;
  private userApiUrl = `${environment.apiUrl}/user`;

  constructor(private http: HttpClient) { }

  getUser(): Observable<User[]> {
    return this.http.get<User[]>(this.userApiUrl);
  }

  // We need to update this to something like:
  getUsers(pageNumber: number, pageSize: number, sortField?: string, sortOrder?: string, search?: string) {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString())
      .set('search', search || '')
      .set('orderBy', sortField || '')
      .set('orderDirection', sortOrder || '');

    return this.http.get<{ users: User[], totalCount: number }>(this.userApiUrl, { params });
  }

  getUserById(id: number): Observable<User> {
    return this.http.get<User>(`${this.userApiUrl}/${id}`);
  }

  getRole(): Observable<Role[]> {
    return this.http.get<Role[]>(this.roleApiUrl);
  }

  /**
 * Update role permissions
 * @param role The role with updated permissions
 * @returns Observable with the updated role data
 */
  updateRole(role: Role): Observable<Role> {
    return this.http.put<Role>(this.roleApiUrl, role);
  }
  /**
   * Update multiple roles at once (batch update)
   * @param roles Array of roles with updated permissions
   * @returns Observable with the updated roles data
   */
  updateMultipleRoles(roles: Role[]): Observable<Role[]> {
    return this.http.put<Role[]>(`${this.roleApiUrl}/batch`, roles);
  }

  /**
   * สร้างผู้ใช้ใหม่ผ่าน API พร้อมรองรับ progress event
   * @param userData ข้อมูลผู้ใช้รวมถึงรูปโปรไฟล์
   * @returns Observable<HttpEvent<any>> สำหรับเช็ค progress และผลลัพธ์
   */
  addUser(userData: FormData): Observable<HttpEvent<any>> {
    return this.http.post<any>(this.userApiUrl, userData, {
      reportProgress: true,
      observe: 'events'
    });
  }

  putUser(userId: string, userData: FormData): Observable<User> {
    return this.http.put<User>(`${this.userApiUrl}/${userId}`, userData);
  }

  deleteUser(userId: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.userApiUrl}/${userId}`);
  }

  changePassword(userData: FormData): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.userApiUrl}/password`, userData);
  }

}
