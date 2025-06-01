import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { authGuard } from './features/auth/auth.guard';

export const routes: Routes = [
    {
        path: 'login',
        loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
    },
    {
        path: 'register',
        loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)
    },
    {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full'
    },
    {
        path: '',
        component: MainLayoutComponent,
        canActivate: [authGuard], // ป้องกันการเข้าถึงเมื่อไม่ได้เข้าสู่ระบบ
        children: [
            { path: 'dashboard', loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) },
            { path: 'objectives', loadComponent: () => import('./features/objectives/objectives.component').then(m => m.ObjectivesComponent) },
            { path: 'document', loadComponent: () => import('./features/document/document.component').then(m => m.DocumentComponent) },
        ]
    },
];
