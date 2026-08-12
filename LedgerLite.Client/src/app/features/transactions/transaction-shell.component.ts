import { Component, OnInit, inject } from '@angular/core';
import { TransactionService } from '../../core/services/transaction.service';
import { TransactionListComponent } from './transaction-list.component';
import { TransactionFormComponent } from './transaction-form.component';

@Component({
  selector: 'app-transaction-shell',
  standalone: true,
  imports: [TransactionListComponent, TransactionFormComponent],
  templateUrl: './transaction-shell.component.html',
  styleUrl: './transaction-shell.component.scss'
})
export class TransactionShellComponent implements OnInit {
  private readonly transactionService = inject(TransactionService);
  readonly transactions = this.transactionService.transactions;

  ngOnInit(): void {
    this.transactionService.loadTransactions();
  }

  onTransactionAdded(transaction: Parameters<TransactionService['addTransaction']>[0]) {
    this.transactionService.addTransaction(transaction);
  }
}
