import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, of, tap } from 'rxjs';

export interface Account {
  id: string;
  name: string;
  type:  AccountType;
  balance: number;
  currency: string;
}

export enum AccountType {
  Checking = 'Checking',
  Savings = 'Savings',
  CreditCard = 'CreditCard',
  Investment = 'Investment'
}

export interface CreateAccountRequest {
  name: string;
  type: AccountType;
  initialBalance: number;
  currency?: string;
}

const API_BASE = 'http://localhost:5265/api/v1';

@Injectable({ providedIn: 'root' })
export class AccountService {
  private readonly http = inject(HttpClient);

  private readonly _accounts = signal<Account[]>([]);
  readonly accounts = this._accounts.asReadonly();


  list(): Observable<Account[]> {
    return this.http.get<Account[]>(`${API_BASE}/accounts`).pipe(
      tap(accounts => this._accounts.set(accounts)),
      catchError(err => {
        console.error('Failed to list accounts', err);
        return of([]);
      })
    );
  }

  create(request: CreateAccountRequest): Observable<Account> {
    const payload = { ...request };
    return this.http.post<Account>(`${API_BASE}/accounts`, payload).pipe(
      tap(created => this._accounts.update(current => [created, ...current]))
    );
  }

  get(id: string): Observable<Account> {
    return this.http.get<Account>(`${API_BASE}/accounts/${id}`);
  }
}
