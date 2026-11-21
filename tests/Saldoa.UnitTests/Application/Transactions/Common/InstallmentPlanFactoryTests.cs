using Saldoa.Application.Transactions.Common;

namespace Saldoa.UnitTests.Application.Transactions.Common
{
    public class InstallmentPlanFactoryTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(1)]
        public void GivenInstallmentCountIsNullOrAtMostOne_WhenCreatingPlan_ThenReturnsSinglePaymentDraft(int? totalInstallments)
        {
            var totalAmount = 89.99m;
            var firstDate = new DateOnly(2026, 11, 20);

            List<InstallmentDraft> drafts = InstallmentPlanFactory.Create(totalAmount, firstDate, totalInstallments);

            var item = Assert.Single(drafts);
            Assert.Equal(totalAmount, item.Amount);
            Assert.Equal(firstDate, item.Date);
            Assert.Equal(1, item.InstallmentInfo.TotalInstallments);
            Assert.Equal(1, item.InstallmentInfo.InstallmentNumber);
            Assert.Null(item.InstallmentInfo.InstallmentGroupId);
            Assert.False(item.InstallmentInfo.IsInstallment);
        }

        [Fact]
        public void GivenMultipleInstallments_WhenCreatingPlan_ThenReturnsMonthlyDrafts()
        {
            var totalAmount = 229.99m;
            var firstDate = new DateOnly(2027, 01, 31);
            var totalInstallments = 3;

            List<InstallmentDraft> drafts = InstallmentPlanFactory.Create(totalAmount, firstDate, totalInstallments);

            var firstDraft = drafts[0];
            var expectedSchedule = new[]
            {
                (new DateOnly(2027, 1, 31), 1),
                (new DateOnly(2027, 2, 28), 2),
                (new DateOnly(2027, 3, 31), 3),
            };

            Assert.Equal(expectedSchedule, 
                drafts.Select(d => (d.Date, d.InstallmentInfo.InstallmentNumber)).ToArray());
            Assert.Equal(totalInstallments, drafts.Count);
            Assert.DoesNotContain(drafts, i => !i.InstallmentInfo.IsInstallment);
            Assert.DoesNotContain(drafts, i => i.InstallmentInfo.InstallmentGroupId == null);
            Assert.DoesNotContain(drafts, i => i.InstallmentInfo.InstallmentGroupId != firstDraft.InstallmentInfo.InstallmentGroupId);
            Assert.DoesNotContain(drafts, i => i.InstallmentInfo.TotalInstallments != totalInstallments);
        }

        [Theory]
        [InlineData(299.99, 99.99, 99.99, 100.01)]
        [InlineData(99.99, 33.33, 33.33, 33.33)]
        [InlineData(100, 33.33, 33.33, 33.34)]
        [InlineData(300, 100.00, 100.00, 100.00)]
        public void GivenMultipleInstallments_WhenCreatingPlan_ThenAddsAnyRemainderToLastInstallment(
            decimal totalAmount, 
            decimal firstInstallmentValue, 
            decimal secondInstallmentValue, 
            decimal lastInstallmentValue)
        {
            var firstDate = new DateOnly(2026, 11, 30);
            var totalInstallments = 3;

            List<InstallmentDraft> drafts = InstallmentPlanFactory.Create(totalAmount, firstDate, totalInstallments);

            var firstDraft = drafts[0];
            var secondDraft = drafts[1];
            var lastDraft = drafts.Last();

            Assert.Equal(totalAmount, drafts.Sum(i => i.Amount));
            Assert.Equal(firstInstallmentValue, firstDraft.Amount);
            Assert.Equal(secondInstallmentValue, secondDraft.Amount);
            Assert.Equal(lastInstallmentValue, lastDraft.Amount);
        }
    }
}
