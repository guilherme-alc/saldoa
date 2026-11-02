using Saldoa.Domain.Entities;
using Saldoa.Domain.Enums;
using Saldoa.Domain.Exceptions;

namespace Saldoa.UnitTests.Domain.Entities
{
    public class BudgetTests
    {
        public static TheoryData<DateOnly, DateOnly> InvalidPeriod => new()
        {
            { new DateOnly(2026, 10, 01), new DateOnly(2026, 10, 01) },
            { new DateOnly(2026, 11, 01), new DateOnly(2026, 10, 01) }
        };

        [Fact]
        public void GivenEmptyWorkspaceId_WhenCreatingBudget_ThenThrowsDomainException()
        {
            var workspaceId = Guid.Empty;
            var periodStart = new DateOnly(2026, 10, 01);
            var periodEnd = new DateOnly(2026, 10,31);
            var limitAmount = 1300.0M;

            Assert.Throws<DomainException>(() => 
                new Budget(null, BudgetScope.General, periodStart, periodEnd, limitAmount, workspaceId));
        }

        [Fact]
        public void GivenValidData_WhenCreatingGeneralBudget_ThenInitializesProperties()
        {
            var workspaceId = Guid.NewGuid();
            var periodStart = new DateOnly(2026, 10, 01);
            var periodEnd = new DateOnly(2026, 10, 31);
            var limitAmount = 1300.0M;

            var budget = new Budget(null, BudgetScope.General, periodStart, periodEnd, limitAmount, workspaceId);

            Assert.Equal(workspaceId, budget.WorkspaceId);
            Assert.Equal(BudgetScope.General, budget.Scope);
            Assert.Equal(periodStart, budget.PeriodStart);
            Assert.Equal(periodEnd, budget.PeriodEnd);
            Assert.Equal(limitAmount, budget.LimitAmount);
            Assert.Null(budget.CategoryId);
        }

        [Fact]
        public void GivenValidData_WhenCreatingBudgetByCategory_ThenInitializesProperties()
        {
            var categoryId = 5L;
            var workspaceId = Guid.NewGuid();
            var periodStart = new DateOnly(2026, 10, 01);
            var periodEnd = new DateOnly(2026, 10, 31);
            var limitAmount = 1300.0M;

            var budget = new Budget(categoryId, BudgetScope.Category, periodStart, periodEnd, limitAmount, workspaceId);

            Assert.Equal(workspaceId, budget.WorkspaceId);
            Assert.Equal(BudgetScope.Category, budget.Scope);
            Assert.Equal(periodStart, budget.PeriodStart);
            Assert.Equal(periodEnd, budget.PeriodEnd);
            Assert.Equal(limitAmount, budget.LimitAmount);
            Assert.Equal(categoryId, budget.CategoryId);
        }

        [Theory]
        [MemberData(nameof(InvalidPeriod))]
        public void GivenInvalidPeriod_WhenCreatingBudget_ThenThrowsDomainException(DateOnly periodStart, DateOnly periodEnd)
        {
            var workspaceId = Guid.NewGuid();
            var limitAmount = 1300.0M;

            Assert.Throws<DomainException>(() => 
                new Budget(null, BudgetScope.General, periodStart, periodEnd, limitAmount, workspaceId)
            );
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-20)]
        public void GivenInvalidLimit_WhenCreatingBudget_ThenThrowsDomainException(decimal limitAmount)
        {
            var workspaceId = Guid.NewGuid();
            var periodStart = new DateOnly(2026, 10, 01);
            var periodEnd = new DateOnly(2026, 10, 31);

            Assert.Throws<DomainException>(() => 
                new Budget(null, BudgetScope.General, periodStart, periodEnd, limitAmount, workspaceId)
            );
        }

        [Fact]
        public void GivenValidPeriod_WhenChangingPeriod_ThenPeriodChanged()
        {
            var workspaceId = Guid.NewGuid();
            var periodStart = new DateOnly(2026, 10, 01);
            var periodEnd = new DateOnly(2026, 10, 31);
            var limitAmount = 1300.0M;
            var newPeriodStart = new DateOnly(2026, 09, 01);
            var newPeriodEnd = new DateOnly(2026, 09, 30);
            var budget = new Budget(null, BudgetScope.General, periodStart, periodEnd, limitAmount, workspaceId);

            budget.ChangePeriod(newPeriodStart, newPeriodEnd);

            Assert.Equal(newPeriodStart, budget.PeriodStart);
            Assert.Equal(newPeriodEnd, budget.PeriodEnd);
        }

        [Fact]
        public void GivenInvalidPeriod_WhenChangingPeriod_ThenThrowsDomainException()
        {
            var workspaceId = Guid.NewGuid();
            var periodStart = new DateOnly(2026, 10, 01);
            var periodEnd = new DateOnly(2026, 10, 31);
            var limitAmount = 1300.0M;
            var newPeriodStart = new DateOnly(2026, 08, 01);
            var newPeriodEnd = newPeriodStart.AddDays(-10);
            var budget = new Budget(null, BudgetScope.General, periodStart, periodEnd, limitAmount, workspaceId);

            Assert.Throws<DomainException>(() => budget.ChangePeriod(newPeriodStart, newPeriodEnd));
            
            Assert.Equal(periodStart, budget.PeriodStart);
            Assert.Equal(periodEnd, budget.PeriodEnd);
        }

        [Fact]
        public void GivenValidLimit_WhenChangingLimit_ThenLimitChanged()
        {
            var workspaceId = Guid.NewGuid();
            var periodStart = new DateOnly(2026, 10, 01);
            var periodEnd = new DateOnly(2026, 10, 31);
            var limitAmount = 1300.0M;
            var newLimitAmount = 2000M;
            var budget = new Budget(null, BudgetScope.General, periodStart, periodEnd, limitAmount, workspaceId);

            budget.ChangeLimit(newLimitAmount);

            Assert.Equal(newLimitAmount, budget.LimitAmount);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-20)]
        public void GivenInvalidLimit_WhenChangingLimit_ThenThrowsDomainException(decimal newLimit)
        {
            var workspaceId = Guid.NewGuid();
            var periodStart = new DateOnly(2026, 10, 01);
            var periodEnd = new DateOnly(2026, 10, 31);
            var limitAmount = 1300.0M;

            var budget = new Budget(null, BudgetScope.General, periodStart, periodEnd, limitAmount, workspaceId);

            Assert.Throws<DomainException>(() => budget.ChangeLimit(newLimit));
            Assert.Equal(limitAmount, budget.LimitAmount);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0L)]
        [InlineData(-2L)]
        public void GivenInvalidCategoryId_WhenCreatingBudgetByCategory_ThenThrowsDomainException(long? categoryId)
        {
            var scope = BudgetScope.Category;
            var workspaceId = Guid.NewGuid();
            var periodStart = new DateOnly(2026, 10, 01);
            var periodEnd = new DateOnly(2026, 10, 31);
            var limitAmount = 250M;

            Assert.Throws<DomainException>(() =>
                new Budget(categoryId!, scope, periodStart, periodEnd, limitAmount, workspaceId)
            );
        }

        [Fact]
        public void GivenCategoryId_WhenCreatingGeneralBudget_ThenThrowsDomainException()
        {
            var scope = BudgetScope.General;
            var categoryId = 7L;
            var workspaceId = Guid.NewGuid();
            var periodStart = new DateOnly(2026, 10, 01);
            var periodEnd = new DateOnly(2026, 10, 31);
            var limitAmount = 250M;

            Assert.Throws<DomainException>(() =>
                new Budget(categoryId, scope, periodStart, periodEnd, limitAmount, workspaceId)
            );
        }

        [Fact]
        public void GivenInvalidScope_WhenCreatingBudget_ThenThrowsDomainException()
        {
            var scope = (BudgetScope) 99;
            var categoryId = 7L;
            var workspaceId = Guid.NewGuid();
            var periodStart = new DateOnly(2026, 10, 01);
            var periodEnd = new DateOnly(2026, 10, 31);
            var limitAmount = 250M;

            Assert.Throws<DomainException>(() =>
                new Budget(categoryId, scope, periodStart, periodEnd, limitAmount, workspaceId)
            );
        }
    }
}
