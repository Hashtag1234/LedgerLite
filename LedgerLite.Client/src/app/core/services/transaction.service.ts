import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { toSignal } from '@angular/core/rxjs-interop';
import { effect } from '@angular/core';
import { AccountContextService } from './account-context.service';

export interface Transaction {
  id: string;
  accountId: string;
  type: 'Income' | 'Expense';
  amount: number;
  currency: string;
  category: string;
  description: string;
  timestamp: string;
}

export interface CreateTransactionRequest {
  accountId: string;
  type: 'Income' | 'Expense';
  amount: number;
  currency: string;
  category: string;
  description: string;
}

const API_BASE = 'http://localhost:5265/api/v1';

@Injectable({ providedIn: 'root' })
export class TransactionService {
  private readonly http = inject(HttpClient);
  private readonly accountContext = inject(AccountContextService);
  readonly selectedAccountId = this.accountContext.selectedAccountId;

  private readonly _transactions = signal<Transaction[]>([]);
  readonly transactions = this._transactions.asReadonly();
  private readonly effect = effect(() => {
    const accountId = this.selectedAccountId();
    this.loadTransactions();
  });

  readonly totalIncome = computed(() =>
    this._transactions()
      .filter(t => t.type === 'Income')
      .reduce((sum, t) => sum + t.amount, 0)
  );

  readonly totalExpenses = computed(() =>
    this._transactions()
      .filter(t => t.type === 'Expense')
      .reduce((sum, t) => sum + t.amount, 0)
  );

  readonly balance = computed(() => this.totalIncome() - this.totalExpenses());

  loadTransactions(): void {
    const accountId = this.selectedAccountId();
    if (!accountId) {
      console.error('Invalid account ID');
      return;
    }

    this.http.get<Transaction[]>(`${API_BASE}/transactions?accountId=${accountId }`)
      .subscribe({
        next: data => this._transactions.set(data),
        error: err => console.error('Failed to load transactions', err)
      });
  }

  addTransaction(request: Omit<CreateTransactionRequest, 'accountId'> ): void {
    const accountId = this.selectedAccountId();
    if (!accountId) {
      console.error('Invalid account ID');
      return;
    }

    const payload: CreateTransactionRequest = {
      ...request,
      accountId
    };

    this.http.post<Transaction>(`${API_BASE}/transactions`, payload)
      .subscribe({
        next: created => this._transactions.update(current => [created, ...current]),
        error: err => console.error('Failed to create transaction', err)
      });
  }
}
