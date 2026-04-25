using Saldoa.Domain.ValueObjects;

namespace Saldoa.Application.Transactions.Common
{
    public static class InstallmentPlanFactory
    {
        public static List<InstallmentDraft> Create(
            decimal totalAmount,
            DateOnly firstDate,
            int? totalInstallments)
        {
            totalInstallments = totalInstallments.HasValue && totalInstallments.Value > 1
                ? totalInstallments.Value
                : 1;

            if (totalInstallments == 1)
            {
                return new List<InstallmentDraft>{
                    new(totalAmount, firstDate, InstallmentInfo.Single())
                };
            }

            var installments = new List<InstallmentDraft>();
            var (baseValue, remainder) = CalculateInstallments(totalAmount, totalInstallments.Value);
            var groupId = Guid.CreateVersion7();

            for (int i = 1; i <= totalInstallments; i++)

            {
                var amount = i == totalInstallments ? baseValue + remainder : baseValue;
                var date = firstDate.AddMonths(i - 1);

                installments.Add(new InstallmentDraft(
                    amount, 
                    date, 
                    InstallmentInfo.Create(totalInstallments.Value, i, groupId)
                ));
            }

            return installments;
        }

        private static (decimal baseValue, decimal remainder) CalculateInstallments(decimal total, int count)
        {
            var baseValue = Math.Floor((total / count) * 100) / 100;
            var remainder = total - (baseValue * count);
            return (baseValue, remainder);
        }
    }
}
