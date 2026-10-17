import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { UploadCsvComponent } from './components/upload-csv/upload-csv.component';
import { ProcessCommerceComponent } from './components/process-commerce/process-commerce.component';
import { QuarantineListComponent } from './components/quarantine-list/quarantine-list.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'upload', component: UploadCsvComponent, canActivate: [authGuard] },
  { path: 'process', component: ProcessCommerceComponent, canActivate: [authGuard] },
  { path: 'quarantine', component: QuarantineListComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: 'login' }
];