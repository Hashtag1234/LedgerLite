import { Routes } from '@angular/router';
import { accountSelectionGuard } from './core/guards/account-selection.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [accountSelectionGuard]
  },
  {
    path: 'transactions',
    loadComponent: () =>
      import('./features/transactions/transaction-shell.component').then(m => m.TransactionShellComponent),
    canActivate: [accountSelectionGuard]
  },
  {
    path: 'accounts',
    loadComponent: () =>
      import('./features/accounts/account-creation.component').then(m => m.AccountCreationComponent),
  },

  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
