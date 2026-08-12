import { Component, input } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { Transaction } from '../../core/services/transaction.service';

@Component({
  selector: 'app-transaction-list',
  standalone: true,
  imports: [CurrencyPipe],
  templateUrl: './transaction-list.component.html',
  styleUrl: './transaction-list.component.scss'
})
export class TransactionListComponent {
  readonly transactions = input.required<Transaction[]>();
}
