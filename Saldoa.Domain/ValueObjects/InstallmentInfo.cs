namespace Saldoa.Domain.ValueObjects
{
    public class InstallmentInfo
    {
        public int TotalInstallments { get; }
        public int InstallmentNumber { get; }
        public Guid? InstallmentGroupId { get; } = null;
        public bool IsInstallment => TotalInstallments > 1;

        private InstallmentInfo(int totalInstallments, int installmentNumber, Guid? installmentGroupId) 
        {
            TotalInstallments = totalInstallments;
            InstallmentNumber = installmentNumber;
            InstallmentGroupId = installmentGroupId;
        }

        public static InstallmentInfo Single()
        {
            return new InstallmentInfo(
                totalInstallments: 1,
                installmentNumber: 1,
                installmentGroupId: null
            );
        }

        public static InstallmentInfo Create(int totalInstallments, int installmentNumber, Guid? installmentGroupId)
        {
            if (totalInstallments <= 1)
                throw new ArgumentOutOfRangeException(nameof(totalInstallments), "Parcelamento deve ser maior que 1.");

            if (installmentNumber <= 0 || installmentNumber > totalInstallments)
                throw new ArgumentOutOfRangeException(nameof(installmentNumber), "Número da parcela inválido.");

            return new InstallmentInfo(
                totalInstallments,
                installmentNumber,
                installmentGroupId
            );
        }
    }
}
