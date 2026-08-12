import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { firstValueFrom, map } from 'rxjs';
import { AccountContextService } from '../services/account-context.service';
import { AccountService } from '../services/account.service';

export const accountSelectionGuard: CanActivateFn = async () => {
  const router = inject(Router);
  const accountContext = inject(AccountContextService);
  const accountService = inject(AccountService);

  const selectedAccountId = accountContext.selectedAccountId();
  if (selectedAccountId) {
    return true;
  }

  const accounts = await firstValueFrom(accountService.list().pipe(
    map(accounts => accounts ?? [])
  ));

  if (accounts.length === 0) {
    void router.navigate(['accounts']);
    return false;
  }

  accountContext.select(accounts[0].id);
  return true;
};
