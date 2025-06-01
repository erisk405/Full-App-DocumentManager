import { CommonModule } from '@angular/common';
import { Component, ViewChild, ChangeDetectorRef, inject } from '@angular/core';
import { FormsModule, ReactiveFormsModule, FormGroup, FormControl, Validators, AbstractControl, ValidatorFn } from '@angular/forms';
import { catchError, finalize, tap } from 'rxjs/operators';

// PrimeNG Imports
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { DialogModule } from 'primeng/dialog';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { MultiSelectModule } from 'primeng/multiselect';
import { SelectModule } from 'primeng/select';
import { TableLazyLoadEvent, TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { FloatLabelModule } from 'primeng/floatlabel';
import { AvatarModule } from 'primeng/avatar';
import { AvatarGroupModule } from 'primeng/avatargroup';
import { ProgressBarModule } from 'primeng/progressbar';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
// Models & Services
import { sortBy, UserRole } from '../../model/dashboard.type';
import { DashboardService } from './dashboard.service';

import { NgxTypedJsModule } from 'ngx-typed-js';
import { Role } from '../../model/role.type';
import { ToastModule } from 'primeng/toast';
import { ConfirmationService, MessageService } from 'primeng/api';
import { HttpEvent, HttpEventType } from '@angular/common/http';
import { User } from '../../model/user.type';
import { Subscription } from 'rxjs';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    // Angular Modules
    CommonModule,
    FormsModule,
    ReactiveFormsModule,

    // PrimeNG Modules
    ButtonModule,
    CheckboxModule,
    DialogModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    MultiSelectModule,
    SelectModule,
    TableModule,
    TagModule,
    FloatLabelModule,
    AvatarModule,
    AvatarGroupModule,
    ToastModule,
    ProgressBarModule,
    ConfirmDialogModule,

    // typing module
    NgxTypedJsModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',

})
export class DashboardComponent {
  // ตัวแปรสำหรับเก็บ subscription เพื่อ unsubscribe เมื่อ component ถูกทำลาย
  private authService = inject(AuthService);
  private userSubscription?: Subscription;
  // ตัวแปรสำหรับเก็บข้อมูล User
  currentUser!: User;
  // Make UserRole accessible in the template
  UserRole = UserRole;

  // =====================
  // PROPERTIES
  // =====================
  // Add these properties to your component
  totalRecords: number = 0;
  first: number = 0;
  rows: number = 5;
  // For ProgressBar
  visibleProgressBar: boolean = false;
  progress: number = 0;
  interval = null;

  // Sort options
  sortie: sortBy[] | undefined;
  selectedDirection: sortBy | undefined;

  // Data properties
  user!: User[];

  currentRoles!: Role[]; // ข้อมูลที่กำลังแสดงและอาจมีการแก้ไข
  originalRoles!: Role[]; // ข้อมูลต้นฉบับเพื่อเปรียบเทียบ

  isLoading: boolean = false;
  isLoadingRoleBtn: boolean = false;

  isUpdateLoading: boolean = false;
  // Image upload properties
  imagePreview: string | null = null;
  selectedFile: File | null = null;

  // Dialog control
  visible: boolean = false;
  visibleEditUser: boolean = false;
  selectRole: UserRole | undefined;

  // ใช้ตอนแก้ไข user
  selectedUser: User | null = null;

  // Edit user properties
  editImagePreview: string | null = null;
  selectedEditFile: File | null = null;


  // Form fields
  userForm!: FormGroup;
  rolePermissionForm!: FormGroup;
  editUserForm!: FormGroup;
  changePasswordForm!: FormGroup;



  // =====================
  // LIFECYCLE HOOKS
  // =====================

  constructor(private confirmationService: ConfirmationService, private messageService: MessageService, private dashboardService: DashboardService) { }

  ngOnInit() {
    this.initializeData();
    this.initializeForms();
    // Subscribe ข้อมูล User จาก AuthService
    this.userSubscription = this.authService.currentUser$.subscribe(
      user => {
        this.currentUser = user!;
      }
    );
  }
  /**
    * Initialize all component data
    */
  private initializeData(): void {
    this.initializeSortOptions();
    this.loadUserData();
    this.loadRoleData();
  }

  /**
   * Initialize sorting options
   */
  private initializeSortOptions(): void {
    this.sortie = [
      { name: 'Firstname', value: 'firstname' },
      { name: 'Lastname', value: 'lastname' },
      { name: 'Email', value: 'email' },
      { name: 'Role', value: 'role' },
      { name: 'Created At', value: 'createdat' },
    ];
  }
  onSortFieldChange(): void {
    const event: TableLazyLoadEvent = {
      first: 0,
      rows: this.rows,
      sortField: this.selectedDirection?.value,
      sortOrder: 1, // asc (หรือ -1 สำหรับ desc)
    };

    this.loadUserData(event);
  }

  // Form validation
  passwordMatchValidator: ValidatorFn = (control: AbstractControl) => {
    const formGroup = control as FormGroup;
    const password = formGroup.get('password')?.value;
    const confirmPass = formGroup.get('confirmPass')?.value;

    if (password && confirmPass && password !== confirmPass) {
      formGroup.get('confirmPass')?.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return null;
  };
  /**
   * Initialize all forms used in the component
   */  // Password is optional in edit form
  private initializeForms(): void {
    // Main user form with basic information and validation
    this.userForm = new FormGroup({
      firstName: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      lastName: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      email: new FormControl('', [
        Validators.required,
        Validators.email,
        Validators.pattern('^[a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$')
      ]),
      phone: new FormControl('', [
        Validators.required,
        Validators.pattern('^[0-9]{10}$') // 10 digits only
      ]),
      username: new FormControl('', [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(20),
        Validators.pattern('^[a-zA-Z0-9_]+$') // Only allow letters, numbers, and underscore
      ]),
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$') // At least one lowercase, one uppercase, one digit
      ]),
      confirmPass: new FormControl(''),
      role: new FormControl(null, [Validators.required]),
      profileImage: new FormControl(null)
    }, { validators: this.passwordMatchValidator });

    // Edit user form - similar structure but with different validation for password (optional)
    this.editUserForm = new FormGroup({
      firstName: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      lastName: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      email: new FormControl('', [
        Validators.required,
        Validators.email,
        Validators.pattern('^[a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$')
      ]),
      phone: new FormControl('', [
        Validators.required,
        Validators.pattern('^[0-9]{10}$') // 10 digits only
      ]),
      username: new FormControl('', [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(20),
        Validators.pattern('^[a-zA-Z0-9_]+$') // Only allow letters, numbers, and underscore
      ]),

      role: new FormControl(null, [Validators.required]),
      profileImage: new FormControl(null),
      userId: new FormControl(null) // To store the user ID being edited
    });

    // Empty form for role permissions
    this.rolePermissionForm = new FormGroup({});

    this.changePasswordForm = new FormGroup({
      currentPassword: new FormControl('', [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$') // At least one lowercase, one uppercase, one digit
      ]),
      newPassword: new FormControl('', [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$') // At least one lowercase, one uppercase, one digit
      ]),
    });

  }

  // =====================
  // DATA LOADING METHODS
  // =====================

  /**
   * Load user data from service
   */
  loadUserData(event?: TableLazyLoadEvent): void {
    const pageSize = event?.rows ?? this.rows;
    const pageNumber = (event?.first ?? 0) / pageSize + 1;
    const sortField = typeof event?.sortField === 'string' ? event.sortField : undefined;
    const sortOrder = event?.sortOrder === 1 ? 'asc' : 'desc';
    const globalFilter = typeof event?.globalFilter === 'string' ? event.globalFilter : '';

    this.dashboardService.getUsers(pageNumber, pageSize, sortField, sortOrder, globalFilter)
      .pipe(
        tap(response => {
          this.user = response.users;
          this.totalRecords = response.totalCount;

          // อัปเดต this.rows ด้วยเผื่อใช้ในการ reload
          this.rows = pageSize;
        }),
        catchError(error => {
          console.error('Error loading users:', error);
          return [];
        })
      ).subscribe();
  }


  /**
   * Load permission data from service
   */
  private loadRoleData(): void {
    this.isLoadingRoleBtn = true;
    this.dashboardService.getRole()
      .pipe(
        tap(roles => {
          this.currentRoles = roles;
          // ทำ deep copy 
          this.originalRoles = JSON.parse(JSON.stringify(roles));
          console.log('Roles loaded successfully:', roles);
        }),
        catchError(error => {
          console.error('Error loading roles:', error);
          // You might want to show an error message to the user here
          return [];
        }),
        finalize(() => {
          this.isLoadingRoleBtn = false;
        })
      ).subscribe();
  }

  // =====================
  // FORM VALIDATION METHODS
  // =====================

  /**
   * Check if a form field is invalid and has been touched
   */

  isFieldInvalidUserForm(field: string, isEdit: boolean = false): boolean {
    const control = !isEdit ? this.userForm.get(field) : this.editUserForm.get(field);
    return control ? control.invalid && (control.dirty || control.touched) : false;
  }
  // for change password form
  isFieldInvalidChangePasswordForm(field: string): boolean {
    const control = this.changePasswordForm.get(field);
    return control ? control.invalid && (control.dirty || control.touched) : false;
  }

  /**
 * Get the appropriate error message for a field
 */
  getFieldError(field: string, isEdit: boolean = false): string {
    const control = !isEdit ? this.userForm.get(field) : this.editUserForm.get(field);
    if (!control || !control.errors) return '';

    // Return appropriate error message based on the validation error
    if (control.errors['required']) {
      return 'This field is required';
    }
    if (control.errors['minlength']) {
      return `Minimum length is ${control.errors['minlength'].requiredLength} characters`;
    }
    if (control.errors['maxlength']) {
      return `Maximum length is ${control.errors['maxlength'].requiredLength} characters`;
    }
    if (control.errors['email']) {
      return 'Please enter a valid email address';
    }

    if (control.errors['pattern']) {
      if (field === 'phone') {
        return 'Please enter a valid 10-digit phone number';
      } else if (field === 'username') {
        return 'Username can only contain letters, numbers and underscore';
      } else if (field === 'password') {
        return 'Password must contain at least one uppercase letter, one lowercase letter, and one number';
      } else if (field === 'currentPassword' || field === 'newPassword') {
        return 'Invalid format uppercase, lowercase, number';
      }
      return 'Invalid format';
    }
    if (control.errors['passwordMismatch'] || (this.userForm.errors?.['passwordMismatch'] && field === 'confirmPass')) {
      return 'Passwords do not match';
    }

    return '';
  }

  getFieldErrorChangePassword(field: string): string {
    const control = this.changePasswordForm.get(field);
    if (!control || !control.errors) return '';

    // Return appropriate error message based on the validation error
    if (control.errors['required']) {
      return 'This field is required';
    }
    if (control.errors['minlength']) {
      return `Minimum length is ${control.errors['minlength'].requiredLength} characters`;
    }
    if (control.errors['maxlength']) {
      return `Maximum length is ${control.errors['maxlength'].requiredLength} characters`;
    }
    if (control.errors['pattern']) {
      return 'Invalid format uppercase, lowercase, number';
    }
    return '';
  }

  // =====================
  // FORM SUBMISSION METHODS
  // =====================

  /**
   * Trigger user form submission
   */
  submitUserForm() {
    // จำลองการกดปุ่ม submit ของฟอร์ม
    document.getElementById('userInfoForm')?.dispatchEvent(new Event('submit', { cancelable: true }));
  }

  /**
 * Trigger permission form submission
 */
  submitPermissionForm() {
    // จำลองการกดปุ่ม submit ของฟอร์ม
    document.getElementById('permissionForm')?.dispatchEvent(new Event('submit', { cancelable: true }));
  }

  /**
  * Handle form submission and API call
  */
  onSubmit() {
    if (!this.userForm.valid) {
      this.userForm.markAllAsTouched();
      return;
    }

    this.messageService.add({
      key: 'confirm',
      sticky: true,
      severity: 'custom',
      summary: 'Adding new user.',
      styleClass: 'backdrop-blur-lg rounded-2xl',
    });

    this.visible = true;
    this.progress = 0;
    this.isLoading = true;

    const formData = this.prepareFormData();

    console.log("formData", formData);
    this.dashboardService.addUser(formData).subscribe({
      next: (event: HttpEvent<any>) => {
        switch (event.type) {
          case HttpEventType.UploadProgress:
            if (event.total) {
              this.progress = Math.round((event.loaded / event.total) * 100);
            }
            break;

          case HttpEventType.Response:
            console.log('Upload success:', event.body);
            this.handleSuccessfulSubmission();
            this.isLoading = false;
            break;
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.messageService.clear('confirm'); // ปิด popup confirm
        this.messageService.add({
          severity: 'error',
          summary: 'Upload Failed',
          detail: 'An error occurred while uploading. Please try again.',
          life: 5000,
        });
        console.error('Upload error:', error);
      }
    });

  }

  /**
 * Prepare form data for submission to API
 */
  private prepareFormData(editForm: boolean = false): FormData {
    const formData = new FormData();
    const formValue = editForm ? this.editUserForm.value : this.userForm.value;
    // Add form values to FormData with proper field names
    Object.keys(formValue).forEach(key => {
      if (key !== 'profileImage') {
        // Map field names to match API expectations
        if (key === 'firstName') {
          formData.append('First_name', formValue[key]);
        } else if (key === 'lastName') {
          formData.append('Last_name', formValue[key]);
        } else if (key === 'role') {
          formData.append('RoleId', formValue[key].roleId);
        } else {
          // Other fields - capitalize first letter
          const fieldName = key.charAt(0).toUpperCase() + key.slice(1);
          formData.append(fieldName, formValue[key]);
        }
      }
    });

    // Add profile image if selected
    if (this.selectedFile) {
      formData.append('ProfileImage', this.selectedFile, this.selectedFile.name);
    }
    if (this.selectedEditFile) {
      formData.append('ProfileImage', this.selectedEditFile, this.selectedEditFile.name);
    }


    return formData;
  }


  /**
 * Handle successful form submission
 */
  private handleSuccessfulSubmission(): void {
    this.hideDialog();
    this.loadUserData();
    this.userForm.reset();
    this.resetFileInput();
  }

  /**
* Handle role permission updates
*/
  onRolePermissionSubmit() {
    // console.log('Role permissions currentRoles:', this.currentRoles);
    this.isLoadingRoleBtn = true;

    // Check which roles have changed and update them
    const updatedRoles = this.currentRoles.filter(roleItem => {
      const originalRole = this.originalRoles.find(r => r.roleId === roleItem.roleId);
      return originalRole && this.hasRoleChanged(originalRole, roleItem);
    });

    if (updatedRoles.length === 0) {
      console.log('No roles were changed');
      this.isLoadingRoleBtn = false;
      return;
    }

    // อัปเดตทั้งหมดในคำขอเดียว (batch update)
    // console.log(`Updating ${updatedRoles.length} roles with batch request...`);
    this.dashboardService.updateMultipleRoles(updatedRoles).subscribe({
      next: (response) => {
        console.log('All roles updated successfully:', response);
        // รีโหลดข้อมูลใหม่เพื่อให้แน่ใจว่าข้อมูลที่แสดงเป็นข้อมูลล่าสุด
        this.loadRoleData();
      },
      error: (error) => {
        this.isLoadingRoleBtn = false;
        console.error('Error updating roles:', error);
        // ที่นี่คุณสามารถแสดงข้อความแจ้งเตือนข้อผิดพลาดถ้ามี notification service
      },
      complete: () => {
        this.isLoadingRoleBtn = false;
      }
    });
  }





  /**
   * Check if role permissions have changed
   */
  private hasRoleChanged(original: Role, current: Role): boolean {
    return original.permission.isReadable !== current.permission.isReadable ||
      original.permission.isWriteable !== current.permission.isWriteable ||
      original.permission.isDeleteable !== current.permission.isDeleteable;
  }

  // =====================
  // IMAGE HANDLING METHODS
  // =====================

  /**
   * Handle file selection and validation for user forms
   * Works with both add user and edit user forms
   * @param event File input event
   * @param isEditForm Optional flag to indicate if this is for the edit form
   */
  onFileSelected(event: Event, isEditForm: boolean = false) {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) {
      this.resetFileInput(isEditForm);
      return;
    }

    const file = input.files[0];

    // ตรวจสอบว่าเป็นไฟล์รูปภาพหรือไม่
    if (!file.type.startsWith('image/')) {
      alert('Please select an image file');
      this.resetFileInput(isEditForm);
      return;
    }

    // ตรวจสอบขนาดไฟล์ (ต้องน้อยกว่า 5MB)
    const maxSizeInBytes = 5 * 1024 * 1024; // 5MB
    if (file.size > maxSizeInBytes) {
      alert('Image size must be less than 5MB');
      this.resetFileInput(isEditForm);
      return;
    }

    if (isEditForm) {
      // ถ้าเป็นฟอร์มแก้ไข
      this.selectedEditFile = file;
      this.createImagePreview(true);
      this.editUserForm.patchValue({
        profileImage: file
      });
    } else {
      // ถ้าเป็นฟอร์มเพิ่มผู้ใช้
      this.selectedFile = file;
      this.createImagePreview();
      this.userForm.patchValue({
        profileImage: file
      });
    }
  }

  /**
   * รีเซ็ตการเลือกไฟล์
   * @param isEditForm Optional flag to indicate if this is for the edit form
   */
  private resetFileInput(isEditForm: boolean = false) {
    if (isEditForm) {
      // รีเซ็ตสำหรับฟอร์มแก้ไข
      this.selectedEditFile = null;
      this.editImagePreview = null;
      // รีเซ็ต input element ถ้าจำเป็น
      const fileInput = document.getElementById('editProfileImage') as HTMLInputElement;
      if (fileInput) {
        fileInput.value = '';
      }
    } else {
      // รีเซ็ตสำหรับฟอร์มเพิ่มผู้ใช้
      this.selectedFile = null;
      this.imagePreview = null;
      // รีเซ็ต input element ถ้าจำเป็น
      const fileInput = document.getElementById('profileImage') as HTMLInputElement;
      if (fileInput) {
        fileInput.value = '';
      }
    }
  }

  /**
   * Create image preview from selected file
   * @param isEditForm Optional flag to indicate if this is for the edit form
   */
  private createImagePreview(isEditForm: boolean = false) {
    if (isEditForm) {
      // สร้าง preview สำหรับฟอร์มแก้ไข
      if (!this.selectedEditFile) return;

      const reader = new FileReader();
      reader.onload = () => {
        this.editImagePreview = reader.result as string;
      };
      reader.readAsDataURL(this.selectedEditFile);
    } else {
      // สร้าง preview สำหรับฟอร์มเพิ่มผู้ใช้
      if (!this.selectedFile) return;

      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = reader.result as string;
      };
      reader.readAsDataURL(this.selectedFile);
    }
  }



  // =====================
  // UTILITY METHODS
  // =====================

  /**
   * Convert string to UserRole enum
   */
  stringToUserRole(roleStr: string): UserRole {
    switch (roleStr) {
      case "Admin": return UserRole.Admin;
      case "HR Admin": return UserRole.HRAdmin;
      case "Super Admin": return UserRole.SuperAdmin;
      case "Employee": return UserRole.Employee;
      default: return UserRole.Employee;
    }
  }

  /**
   * Get CSS class for role badge
   */
  getRoleColorClass(role: UserRole): string {
    switch (role) {
      case UserRole.Admin:
      case UserRole.HRAdmin:
      case UserRole.SuperAdmin:
        return 'bg-blue-500';
      case UserRole.Employee:
        return 'bg-zinc-100 text-zinc-500';
      default:
        return 'bg-gray-500';
    }
  }

  // =====================
  // DIALOG METHODS
  // =====================

  /**
   * Show dialog
   */
  showDialog(): void {
    this.visible = true;
  }

  /**
   * Show edit user dialog and populate form with selected user data
   */
  showEditDialog(user: User): void {
    this.selectedUser = user;
    const userRole = this.currentRoles.find(r => r.roleId === user?.roleId) || null;
    console.log("user", user);

    this.editUserForm.setValue({
      firstName: user?.first_name,
      lastName: user?.last_name,
      email: user?.email,
      phone: user?.phone,
      username: user?.username,
      role: userRole,
      profileImage: user?.profileImageUrl,
      userId: user?.userId
    });

    // Initialize image preview with existing profile image URL
    if (user?.profileImageUrl) {
      this.editImagePreview = user.profileImageUrl;
    } else {
      this.editImagePreview = null;
    }

    this.visibleEditUser = true;
  }

  saveUserEdit(): void {
    if (!this.editUserForm.valid) {
      this.editUserForm.markAllAsTouched();
      return;
    }
    console.log(this.editUserForm.value);

    const formData = this.prepareFormData(true);
    // const entries = Array.from(formData.entries());
    // console.log("FormData entries:", entries);
    this.dashboardService.putUser(this.selectedUser?.userId!, formData).subscribe({
      next: (response) => {
        console.log('User updated successfully:', response);
        // รีโหลดข้อมูลใหม่เพื่อให้แน่ใจว่าข้อมูลที่แสดงเป็นข้อมูลล่าสุด
        this.loadUserData();
      },
      error: (error) => {
        console.error('Error updating user:', error);
        // ที่นี่คุณสามารถแสดงข้อความแจ้งเตือนข้อผิดพลาดถ้ามี notification service
      },
      complete: () => {
        this.visibleEditUser = false;
        this.selectedUser = null;
      }
    });


    // Here you would implement the logic to save user edits
    // For now, we'll just close the dialog
    // this.visibleEditUser = false;
    this.selectedUser = null;
  }

  deleteUser(userId: string) {
    this.confirmationService.confirm({
      message: 'Do you want to delete this record?',
      header: 'Danger Zone',
      icon: 'pi pi-info-circle',
      rejectLabel: 'Cancel',
      rejectButtonProps: {
        label: 'Cancel',
        severity: 'secondary',
        outlined: true,
      },
      acceptButtonProps: {
        label: 'Delete',
        severity: 'danger',
      },

      accept: () => {
        console.log("this", userId);

        this.dashboardService.deleteUser(userId).subscribe({
          next: (response) => {
            this.messageService.add({ severity: 'info', summary: 'Confirmed', detail: `${response.message}` });
            this.loadUserData();
          },
          error: (error) => {
            console.error('Error deleting user:', error);
            this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to delete user' });
          }
        })
      },
    });
  }

  onUserPasswordChangeSubmit() {
    const formData = new FormData();
    this.isUpdateLoading = true;
    if (!this.selectedUser?.userId) {
      return this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to change password' });
    }
    formData.append('userId', this.selectedUser.userId);
    formData.append('currentPassword', this.changePasswordForm.value.currentPassword);
    formData.append('newPassword', this.changePasswordForm.value.newPassword);
    this.dashboardService.changePassword(formData).subscribe({
      next: (response) => {
        this.messageService.add({ severity: 'info', summary: 'Confirmed', detail: `${response.message}` });
        this.changePasswordForm.reset();
        this.loadUserData();
      },
      error: (error) => {
        this.isUpdateLoading = false;
        console.error('Error changing password:', error);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to change password' });
      }, complete: () => {
        this.isUpdateLoading = false;
      }
    })
  }
  hideDialog(): void {
    this.visible = false;
  }

  onCloseProgressBar() {
    this.visibleProgressBar = false;
  }

}