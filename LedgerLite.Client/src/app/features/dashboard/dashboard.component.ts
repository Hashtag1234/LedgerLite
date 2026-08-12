import { Component, inject } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { TransactionService } from '../../core/services/transaction.service';
import { AccountCreationComponent } from '../accounts/account-creation.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CurrencyPipe, AccountCreationComponent],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent {
  private readonly transactionService = inject(TransactionService);

  readonly totalIncome = this.transactionService.totalIncome;
  readonly totalExpenses = this.transactionService.totalExpenses;
  readonly balance = this.transactionService.balance;
}
