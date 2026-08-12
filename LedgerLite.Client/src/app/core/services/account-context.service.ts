import { Injectable, signal, Signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AccountContextService {
  private readonly _account = signal<string | null>(null);
  readonly selectedAccountId: Signal<string | null> = this._account.asReadonly();

  select(accountId: string | null): void { this._account.set(accountId); }
  clear(): void { this._account.set(null); }
}
