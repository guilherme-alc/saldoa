using Saldoa.Domain.Exceptions;
using Saldoa.Domain.ValueObjects;

namespace Saldoa.UnitTests.Domain.ValueObjects
{
    public class InstallmentInfoTests
    {
        [Fact]
        public void GivenSinglePayment_WhenCreatingInstallmentInfo_ThenInitializesProperties()
        {
            var installmentInfo = InstallmentInfo.SinglePayment();

            Assert.Null(installmentInfo.InstallmentGroupId);
            Assert.Equal(1, installmentInfo.InstallmentNumber);
            Assert.Equal(1, installmentInfo.TotalInstallments);
            Assert.False(installmentInfo.IsInstallment);
        }

        [Theory]
        [InlineData(2, 1)]
        [InlineData(2, 2)]
        [InlineData(6, 3)]
        [InlineData(7, 7)]
        public void GivenMultiplePayments_WhenCreatingInstallmentInfo_ThenInitializesProperties(int totalInstallments, int installmentNumber)
        {
            var installmentGroupId = Guid.NewGuid();
            
            var installmentInfo = InstallmentInfo.Installment(totalInstallments, installmentNumber, installmentGroupId);
            
            Assert.Equal(installmentGroupId, installmentInfo.InstallmentGroupId);
            Assert.Equal(installmentNumber, installmentInfo.InstallmentNumber);
            Assert.Equal(totalInstallments, installmentInfo.TotalInstallments);
            Assert.True(installmentInfo.IsInstallment);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(-2)]
        public void GivenInvalidTotalInstallment_WhenCreatingInstallmentInfo_ThrowsDomainException(int totalInstallments)
        {
            var installmentGroupId = Guid.NewGuid();
            var installmentNumber = 1;
            
            Assert.Throws<DomainException>(() => 
                InstallmentInfo.Installment(totalInstallments, installmentNumber, installmentGroupId));
        
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        [InlineData(7)]
        public void GivenInvalidInstallmentNumber_WhenCreatingInstallmentInfo_ThrowsDomainException(int installmentNumber)
        {
            var installmentGroupId = Guid.NewGuid();
            var totalInstallments = 6;
            
            Assert.Throws<DomainException>(() => 
                InstallmentInfo.Installment(totalInstallments, installmentNumber, installmentGroupId));
        
        }
    }
}
