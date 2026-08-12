import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountContextService } from '../../core/services/account-context.service';
import { AccountService, AccountType, CreateAccountRequest } from '../../core/services/account.service';

@Component({
  selector: 'app-account-creation',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './account-creation.component.html',
  styleUrls: ['./account-creation.component.scss']
})
export class AccountCreationComponent {
  private readonly accountService = inject(AccountService);
  private readonly accountContext = inject(AccountContextService);
  private readonly router = inject(Router);
  readonly name = signal('');
  readonly type = signal<AccountType>(AccountType.Checking);
  readonly initialBalance = signal(0);
  readonly currency = signal('USD');
  readonly status = signal('');

  readonly accountTypes = Object.values(AccountType) as AccountType[];

  submit(): void {
    const request: CreateAccountRequest = {
      name: this.name(),
      type: this.type(),
      initialBalance: this.initialBalance(),
      currency: this.currency() || 'USD'
    };

    if (!request.name || request.initialBalance < 0) {
      this.status.set('Please enter a valid name and a non-negative balance.');
      return;
    }

    this.accountService.create(request).subscribe({
      next: account => {
        this.accountContext.select(account.id);
        this.status.set(`Account "${account.name}" created and selected.`);
        this.name.set('');
        this.type.set(AccountType.Checking);
        this.initialBalance.set(0);
        this.currency.set('USD');
        this.router.navigate(['/dashboard']);
      },
      error: () => {
        this.status.set('Unable to create account. Try again later.');
      }
    });
  }
}
