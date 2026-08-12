import { Component, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CreateTransactionRequest } from '../../core/services/transaction.service';

@Component({
  selector: 'app-transaction-form',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './transaction-form.component.html',
  styleUrls: ['./transaction-form.component.scss']
})
export class TransactionFormComponent {
  readonly transactionAdded = output<Omit<CreateTransactionRequest, 'accountId'>>();

  readonly description = signal('');
  readonly amount = signal(0);
  readonly type = signal<'Income' | 'Expense'>('Income');
  readonly category = signal('');
  readonly currency = signal('USD');

  submit() {
    if (!this.description() || this.amount() <= 0 || !this.category()) return;

    this.transactionAdded.emit({
      description: this.description(),
      amount: this.amount(),
      type: this.type(),
      category: this.category(),
      currency: this.currency()
    });

    this.description.set('');
    this.amount.set(0);
    this.type.set('Income');
    this.category.set('');
  }
}
