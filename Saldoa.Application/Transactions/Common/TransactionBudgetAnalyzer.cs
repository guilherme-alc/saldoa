using Saldoa.Application.Budgets.Abstractions;
using Saldoa.Application.Transactions.Abstractions;
using Saldoa.Domain.Enums;

namespace Saldoa.Application.Transactions.Common
{
    public sealed class TransactionBudgetAnalyzer
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly ITransactionRepository _transactionRepository;
        public TransactionBudgetAnalyzer(IBudgetRepository budgetRepository, ITransactionRepository transactionRepository)
        {
            _budgetRepository = budgetRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<List<BudgetAlert>> AnalyzeAsync(
            long categoryId,
            List<InstallmentDraft> installments,
            IReadOnlyCollection<long> excludeTransactionIds,
            CancellationToken ct)
        {
            var result = new List<BudgetAlert>();
            if (installments.Count == 0)
                return result;

            var installmentsStart = installments.Min(i => i.Date);
            var installmentsEnd = installments.Max(i => i.Date);

            var affectedBudgets = await _budgetRepository.GetActiveForPeriodAsync(
                categoryId,
                installmentsStart,
                installmentsEnd,
                ct);

            if (affectedBudgets.Count == 0)
                return result;

            var budgetsStart = affectedBudgets.Min(b => b.PeriodStart);
            var budgetsEnd = affectedBudgets.Max(b => b.PeriodEnd);

            var categoryTotalsByDate = new Dictionary<DateOnly, decimal>();
            var generalTotalsByDate = new Dictionary<DateOnly, decimal>();

            if (affectedBudgets.Any(b => b.Scope == BudgetScope.Category))
            {
                categoryTotalsByDate =
                    await _transactionRepository.GetTotalsByDateExcludingAsync(
                        categoryId,
                        budgetsStart,
                        budgetsEnd,
                        excludeTransactionIds,
                        ct,
                        TransactionType.Expense);
            }

            if (affectedBudgets.Any(b => b.Scope == BudgetScope.General))
            {
                generalTotalsByDate =
                    await _transactionRepository.GetTotalsByDateExcludingAsync(
                        null,
                        budgetsStart,
                        budgetsEnd,
                        excludeTransactionIds,
                        ct,
                        TransactionType.Expense);
            }

            foreach (var budget in affectedBudgets)
            {
                var totalsByDate = budget.Scope == BudgetScope.General
                    ? generalTotalsByDate
                    : categoryTotalsByDate;

                var installmentsPerBudget = installments
                    .Where(i => i.Date >= budget.PeriodStart && i.Date <= budget.PeriodEnd)
                    .ToList();

                if (installmentsPerBudget.Count == 0)
                    continue;

                var spent = totalsByDate
                    .Where(t => 
                        t.Key >= budget.PeriodStart && 
                        t.Key <= budget.PeriodEnd)
                    .Sum(t => t.Value);

                var amount = installmentsPerBudget.Sum(i => i.Amount);

                var totalProjected = spent + amount;

                if (totalProjected > budget.LimitAmount)
                {
                    result.Add(new BudgetAlert(
                        budget.Id,
                        budget.Scope,
                        spent,
                        totalProjected,
                        budget.LimitAmount,
                        $"Orçamento excedido para o período " +
                        $"{budget.PeriodStart:dd/MM/yyyy} - {budget.PeriodEnd:dd/MM/yyyy}"
                    ));
                }
            }

            return result;
        }
    }
}
