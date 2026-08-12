import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AccountContextService } from './core/services/account-context.service';
import { AccountService } from './core/services/account.service';
import { catchError } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrls: ['./app.scss']
})
export class App {
  private readonly accountService = inject(AccountService);
  private readonly accountContext = inject(AccountContextService);

  readonly accounts = this.accountService.accounts;
  readonly selectedAccountId = this.accountContext.selectedAccountId;

  constructor() {
    this.accountService.list().subscribe();
  }

  onAccountChange(accountId: string): void {
    this.accountContext.select(accountId || null);
  }
}
