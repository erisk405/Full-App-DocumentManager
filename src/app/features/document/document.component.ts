import { Component, inject, ViewChild } from '@angular/core';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { sortBy } from '../../model/dashboard.type';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, FormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { NgxTypedJsModule } from 'ngx-typed-js';
import { DocumentProp } from '../../model/document.type';
import { DocumentService } from './document.service';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { DialogModule } from 'primeng/dialog';
import { FileUploadModule, FileUpload } from 'primeng/fileupload';
import { ToastModule } from 'primeng/toast';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TextareaModule } from 'primeng/textarea';
import { RadioButtonModule } from 'primeng/radiobutton';
import { ReactiveFormsModule } from '@angular/forms'; // เพิ่มเข้ามา
import { ConfirmDialogModule } from 'primeng/confirmdialog';


import { FloatLabel } from 'primeng/floatlabel';
import { Subscription } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { User } from '../../model/user.type';
@Component({
  selector: 'app-document',
  imports: [
    // angular
    CommonModule,
    FormsModule,

    // primNG
    InputTextModule,
    IconFieldModule,
    InputIconModule,
    SelectModule,
    ButtonModule,
    TableModule,
    TagModule,
    DialogModule,
    FileUploadModule,
    ToastModule,
    TextareaModule,
    FloatLabel,
    RadioButtonModule,
    ReactiveFormsModule,
    ConfirmDialogModule,
    // typing module
    NgxTypedJsModule,
  ],
  templateUrl: './document.component.html',
  styleUrl: './document.component.css',
})
export class DocumentComponent {
  @ViewChild('fu') fileUpload!: FileUpload;
  // ตัวแปรสำหรับเก็บ subscription เพื่อ unsubscribe เมื่อ component ถูกทำลาย
  private userSubscription?: Subscription;
  private authService = inject(AuthService);
  // ตัวแปรสำหรับเก็บข้อมูล User
  currentUser!: User;
  sortie: sortBy[] | undefined;
  selectedCity: sortBy | undefined;
  currentDate: Date = new Date();
  visible: boolean = false;
  document!: DocumentProp[];
  visibleEditDocument: boolean = false;
  formGroup!: FormGroup;
  updateDocument!: FormGroup;
  selectedDocument: DocumentProp | null = null;
  loading: boolean = false;
  docStatus: any[] = [
    { name: 'Available', value: 'AVAILABLE' },
    { name: 'Review', value: 'REVIEW' },
    { name: 'Archived', value: 'ARCHIVED' }
  ];
  constructor(private confirmationService: ConfirmationService, private documentService: DocumentService, private messageService: MessageService) {
  }


  private loadDocument(): void {
    this.documentService.getDocuments().subscribe((docs) => {
      console.log("docs", docs);

      this.document = docs;
    });
  }
  ngOnInit(): void {
    this.loadDocument();
    this.formGroup = new FormGroup({
      filename: new FormControl('', [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(100)
      ]),
      selectedStatus: new FormControl(this.docStatus[1], Validators.required),
      description: new FormControl('', [
        Validators.required,
        Validators.minLength(10),
        Validators.maxLength(500)
      ]),
      createDate: new FormControl(new Date())
    });
    this.updateDocument = new FormGroup({
      filename: new FormControl('', [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(100)
      ]),
      selectedStatus: new FormControl(null, Validators.required),
      description: new FormControl('', [
        Validators.required,
        Validators.minLength(10),
        Validators.maxLength(500)
      ]),
      createDate: new FormControl(new Date())
    });
    // Subscribe ข้อมูล User จาก AuthService
    this.userSubscription = this.authService.currentUser$.subscribe(
      user => {
        this.currentUser = user!;
      }
    );
  }

  showEditDocumentDialog(doc: DocumentProp): void {
    this.visibleEditDocument = true;
    this.selectedDocument = doc;
    const status = this.docStatus.findIndex((item) => item.value === doc.status);
    this.updateDocument.setValue({
      filename: doc.filename.split('.')[0],
      selectedStatus: this.docStatus[status],
      description: doc.description,
      createDate: doc.createAt
    });
  }

  isFieldInvalid(field: string): boolean {
    const control = this.formGroup.get(field);
    return control ? control.invalid && (control.dirty || control.touched) : false;
  }

  getFieldError(field: string): string {
    const control = this.formGroup.get(field);
    if (!control || !control.errors) return '';

    if (control.errors['required']) {
      return 'This field is required';
    }
    if (control.errors['minlength']) {
      return `Minimum length is ${control.errors['minlength'].requiredLength} characters`;
    }
    if (control.errors['maxlength']) {
      return `Maximum length is ${control.errors['maxlength'].requiredLength} characters`;
    }
    return '';
  }

  myUploadHandler(event: any, isUpdate: boolean = false) {
    const formSelector = isUpdate ? this.updateDocument : this.formGroup
    if (formSelector.invalid) {
      formSelector.markAllAsTouched();
      this.messageService.add({
        severity: 'error',
        summary: 'Validation Error',
        detail: 'Please fill in all required fields correctly'
      });
      return;
    }
    if (!isUpdate) {
      this.loading = true;
      const file: File = event.files[0];
      const formData = new FormData();

      formData.append('file', file);
      formData.append('filename', this.formGroup.value.filename);
      formData.append('description', this.formGroup.value.description);
      formData.append('status', this.formGroup.value.selectedStatus?.value);
      this.documentService.uploadDocument(formData).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'File uploaded successfully' });
          this.visible = false;
          this.formGroup.reset();
          this.fileUpload.clear();
          this.loadDocument();
          this.loading = false;
        },
        error: (err) => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: err.message });
          this.loading = false;
        }
      });
    } else {
      this.loading = true;
      if (!this.selectedDocument) {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Please select a document' });
        return;
      }
      const formData = new FormData();
      formData.append('filename', formSelector.value.filename);
      formData.append('description', formSelector.value.description);
      formData.append('status', formSelector.value.selectedStatus?.value);
      this.documentService.updateDocument(formData, this.selectedDocument.documentId).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'File uploaded successfully' });
          this.visible = false;
          this.loadDocument();
          this.loading = false;
        },
        error: (err) => {
          this.messageService.add({ severity: 'error', summary: 'Error', detail: err.message });
          this.loading = false;
        },
        complete: () => {
          this.visibleEditDocument = false;
        }
      });
    }

  }
  onFileSelect(event: any) {
    console.log('File selected:', event.files);
    // ตรวจสอบว่าไฟล์ถูกเลือกแล้ว
    if (event.files && event.files.length > 0) {
      const file = event.files[0];
      console.log('Selected file:', file.name, 'Size:', file.size);
      if (file.size > 3000000) {
        this.messageService.add({
          severity: 'warn',
          summary: 'File Size Error',
          detail: 'File size must be less than 3MB'
        });
      }
    }
  }

  onFileError(event: any) {
    console.log('File error:', event);
    this.messageService.add({
      severity: 'error',
      summary: 'File Error',
      detail: 'File selection failed. Please check file size and type.'
    });
  }

  downloadDocument() {
    if (!this.selectedDocument) {
      this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Please select a document' });
      return;
    }

    this.documentService.downloadDocument(this.selectedDocument.documentId).subscribe({
      next: (blob) => {
        // สร้าง URL จาก blob
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = this.selectedDocument!.original_name; // ชื่อไฟล์ที่ต้องการให้ผู้ใช้เห็น
        a.click();
        window.URL.revokeObjectURL(url); // ล้าง memory
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error || 'Download failed' });
      }
    });
  }


  OnDeleteDocument(id: string) {
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
        this.documentService.deleteDocument(id).subscribe({
          next: (response) => {
            this.messageService.add({ severity: 'success', summary: 'Success', detail: `${response.message}` });
            this.loadDocument();
          },
          error: (err) => {
            this.messageService.add({ severity: 'error', summary: 'Error', detail: err.message });
          }
        });
      },
    });

  }

  showDialog() {
    this.visible = true;
  }
  getSeverity(status: string) {
    switch (status) {
      case 'AVAILABLE':
        return 'success';
      case 'REVIEW':
        return 'warn';
      case 'ARCHIVED':
        return 'danger';
      default:
        return 'info';
    }
  }

}
