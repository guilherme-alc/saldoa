using Saldoa.Domain.Entities;
using Saldoa.Domain.Enums;
using Saldoa.Domain.Exceptions;
using Saldoa.Domain.ValueObjects;

namespace Saldoa.UnitTests.Domain.Entities
{
    public class TransactionTests
    {
        public static readonly TheoryData<string?> BlankStrings = new()
        {
            "",
            (string?)null,
            " ",
            "\t",
            "\n"
        };

        [Fact]
        public void GivenEmptyWorkspaceId_WhenCreatingTransaction_ThenThrowsDomainException()
        {
            var title = "Presente Irmão";
            var totalAmount = 180M;
            var paidAt = new DateOnly(2026, 11, 02);
            var categoryId = 8;
            var installmentInfo = InstallmentInfo.SinglePayment();
            var userId = Guid.NewGuid();

            Assert.Throws<DomainException>(() =>
                new Transaction(
                    Guid.Empty, title, null, TransactionType.Expense, totalAmount, paidAt, categoryId, installmentInfo, userId)
            );
        }

        [Fact]
        public void GivenEmptyUserId_WhenCreatingTransaction_ThenThrowsDomainException()
        {
            var workspaceId = Guid.NewGuid();
            var title = "Presente Irmão";
            var totalAmount = 180M;
            var paidAt = new DateOnly(2026, 11, 02);
            var categoryId = 8;
            var installmentInfo = InstallmentInfo.SinglePayment();

            Assert.Throws<DomainException>(() =>
                new Transaction(
                    workspaceId, title, null, TransactionType.Expense, totalAmount, paidAt, categoryId, installmentInfo, Guid.Empty)
            );
        }

        [Fact]
        public void GivenNullInstallmentInfo_WhenCreatingTransaction_ThenThrowsDomainException()
        {
            var workspaceId = Guid.NewGuid();
            var title = "Presente Irmão";
            var totalAmount = 180M;
            var paidAt = new DateOnly(2026, 11, 02);
            var categoryId = 8;
            var userId = Guid.NewGuid();

            Assert.Throws<DomainException>(() =>
                new Transaction(
                    workspaceId, title, null, TransactionType.Expense, totalAmount, paidAt, categoryId, null!, userId)
            );
        }

        [Fact]
        public void GivenInvalidTransactionType_WhenCreatingTransaction_ThenThrowsDomainException()
        {
            var workspaceId = Guid.NewGuid();
            var title = "Freelancer";
            var totalAmount = 1800M;
            var receivedAt = new DateOnly(2026, 11, 06);
            var categoryId = 2;
            var userId = Guid.NewGuid();
            var installmentInfo = InstallmentInfo.SinglePayment();

            Assert.Throws<DomainException>(() =>
                new Transaction(
                    workspaceId, title, null, (TransactionType)99, totalAmount, receivedAt, categoryId, installmentInfo, userId)
            );
        }

        [Fact]
        public void GivenSinglePayment_WhenCreatingTransaction_ThenInitializesProperties()
        {
            var lowDateTime = DateTimeOffset.UtcNow;
            var workspaceId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var title = "Presente Irmão";
            var totalAmount = 180M;
            var paidAt = new DateOnly(2026, 11, 02);
            var categoryId = 8;
            var installmentInfo = InstallmentInfo.SinglePayment();

            var transaction = new Transaction(
                workspaceId, title, null, TransactionType.Expense, 
                totalAmount, paidAt, categoryId, installmentInfo, userId);

            Assert.Equal(workspaceId, transaction.WorkspaceId);
            Assert.Equal(title, transaction.Title);
            Assert.Null(transaction.Description);
            Assert.Equal(totalAmount, transaction.Amount);
            Assert.Equal(paidAt, transaction.PaidOrReceivedAt);
            Assert.Equal(categoryId, transaction.CategoryId);
            Assert.Equal(1, transaction.InstallmentInfo.TotalInstallments);
            Assert.Equal(1, transaction.InstallmentInfo.InstallmentNumber);
            Assert.Null(transaction.InstallmentInfo.InstallmentGroupId);
            Assert.Equal(userId, transaction.CreatedByUserId);
            Assert.InRange(transaction.CreatedAt, lowDateTime, DateTimeOffset.UtcNow);
            Assert.Equal(TimeSpan.Zero, transaction.CreatedAt.Offset);
        }

        [Fact]
        public void GivenMultiplePayments_WhenCreatingTransaction_ThenInitializesProperties()
        {
            var lowDateTime = DateTimeOffset.UtcNow;
            var workspaceId = Guid.NewGuid();
            var title = "Fone Bluetooth";
            var amount = 160.99M;
            var paidAt = new DateOnly(2026, 10, 03);
            var categoryId = 5;
            var totalInstallments = 6;
            var installmentNumber = 3;
            var installmentGroupId = Guid.NewGuid();
            var installmentInfo = InstallmentInfo.Installment(totalInstallments, installmentNumber, installmentGroupId);
            var userId = Guid.NewGuid();

            var transaction = new Transaction(
                workspaceId, title, null, TransactionType.Expense, 
                amount, paidAt, categoryId, installmentInfo, userId);

            Assert.Equal(workspaceId, transaction.WorkspaceId);
            Assert.Equal(title, transaction.Title);
            Assert.Null(transaction.Description);
            Assert.Equal(amount, transaction.Amount);
            Assert.Equal(paidAt, transaction.PaidOrReceivedAt);
            Assert.Equal(categoryId, transaction.CategoryId);
            Assert.Equal(totalInstallments, transaction.InstallmentInfo.TotalInstallments);
            Assert.Equal(installmentNumber, transaction.InstallmentInfo.InstallmentNumber);
            Assert.Equal(installmentGroupId, transaction.InstallmentInfo.InstallmentGroupId);
            Assert.Equal(userId, transaction.CreatedByUserId);
            Assert.InRange(transaction.CreatedAt, lowDateTime, DateTimeOffset.UtcNow);
            Assert.Equal(TimeSpan.Zero, transaction.CreatedAt.Offset);
        }

        [Fact]
        public void GivenTitle_WhenChangingTitle_ThenTitleChanged()
        {
            var workspaceId = Guid.NewGuid();
            var title = "Fone Bluetooth";
            var amount = 160.99M;
            var paidAt = new DateOnly(2026, 10, 03);
            var categoryId = 5;
            var installmentInfo = InstallmentInfo.SinglePayment();
            var userId = Guid.NewGuid();
            var transaction = new Transaction(
                workspaceId, title, null, TransactionType.Expense,
                amount, paidAt, categoryId, installmentInfo, userId);
            var newTitle = "Caixinha de som";

            transaction.ChangeTitle(newTitle);

            Assert.Equal(newTitle, transaction.Title);
        }

        [Fact]
        public void GivenDescription_WhenChangingDescription_ThenDescriptionChanged()
        {
            var workspaceId = Guid.NewGuid();
            var title = "Fone Bluetooth";
            var amount = 160.99M;
            var paidAt = new DateOnly(2026, 10, 03);
            var categoryId = 5;
            var installmentInfo = InstallmentInfo.SinglePayment();
            var userId = Guid.NewGuid();
            var transaction = new Transaction(
                workspaceId, title, null, TransactionType.Expense,
                amount, paidAt, categoryId, installmentInfo, userId);
            var newDescription = "Fone com cancelamento de ruído comprado para reposição";

            transaction.ChangeDescription(newDescription);

            Assert.Equal(newDescription, transaction.Description);
        }

        [Fact]
        public void GivenValidAmount_WhenChangingAmount_ThenAmountChanged()
        {
            var workspaceId = Guid.NewGuid();
            var title = "Fone Bluetooth";
            var amount = 160.99M;
            var paidAt = new DateOnly(2026, 10, 03);
            var categoryId = 5;
            var installmentInfo = InstallmentInfo.SinglePayment();
            var userId = Guid.NewGuid();
            var transaction = new Transaction(
                workspaceId, title, null, TransactionType.Expense,
                amount, paidAt, categoryId, installmentInfo, userId);
            var newAmount = 280M;

            transaction.ChangeAmount(newAmount);

            Assert.Equal(newAmount, transaction.Amount);
        }

        [Fact]
        public void GivenDate_WhenReschedulingTransaction_ThenDateChanged()
        {
            var workspaceId = Guid.NewGuid();
            var title = "Fone Bluetooth";
            var amount = 160.99M;
            var paidAt = new DateOnly(2026, 10, 03);
            var categoryId = 5;
            var installmentInfo = InstallmentInfo.SinglePayment();
            var userId = Guid.NewGuid();
            var transaction = new Transaction(
                workspaceId, title, null, TransactionType.Expense,
                amount, paidAt, categoryId, installmentInfo, userId);
            var newDate = new DateOnly(2026, 11, 09);

            transaction.Reschedule(newDate);

            Assert.Equal(newDate, transaction.PaidOrReceivedAt);
        }

        [Fact]
        public void GivenCategoryId_WhenMovingToCategory_ThenCategoryIdChanged()
        {
            var workspaceId = Guid.NewGuid();
            var title = "Fone Bluetooth";
            var amount = 160.99M;
            var paidAt = new DateOnly(2026, 10, 03);
            var categoryId = 5;
            var installmentInfo = InstallmentInfo.SinglePayment();
            var userId = Guid.NewGuid();
            var transaction = new Transaction(
                workspaceId, title, null, TransactionType.Expense,
                amount, paidAt, categoryId, installmentInfo, userId);
            var newCategoryId = 36;

            transaction.MoveToCategory(newCategoryId);

            Assert.Equal(newCategoryId, transaction.CategoryId);
        }

        [Theory]
        [MemberData(nameof(BlankStrings))]
        public void GivenBlankTitle_WhenCreatingTransaction_ThenThrowsDomainException(string? invalidTitle)
        {
            var workspaceId = Guid.NewGuid();
            var amount = 160.99M;
            var paidAt = new DateOnly(2026, 10, 03);
            var categoryId = 5;
            var installmentInfo = InstallmentInfo.SinglePayment();
            var userId = Guid.NewGuid();

            Assert.Throws<DomainException>(() =>
                new Transaction(
                    workspaceId, invalidTitle!, null, TransactionType.Expense,
                    amount, paidAt, categoryId, installmentInfo, userId)
            );
        }

        [Theory]
        [MemberData(nameof(BlankStrings))]
        public void GivenBlankTitle_WhenChangingTitle_ThenThrowsDomainException(string? invalidTitle)
        {
            var workspaceId = Guid.NewGuid();
            var title = "Freelancer";
            var amount = 2600M;
            var paidAt = new DateOnly(2026, 10, 15);
            var categoryId = 7;
            var installmentInfo = InstallmentInfo.SinglePayment();
            var userId = Guid.NewGuid();
            var transaction = new Transaction(
                workspaceId, title!, null, TransactionType.Income,
                amount, paidAt, categoryId, installmentInfo, userId);

            Assert.Throws<DomainException>(() =>
                transaction.ChangeTitle(invalidTitle!));
            Assert.Equal(title, transaction.Title);
        }

        [Theory]
        [MemberData(nameof(BlankStrings))]
        public void GivenBlankDescription_WhenCreatingTransaction_ThenDescriptionIsNull(string? description)
        {
            var workspaceId = Guid.NewGuid();
            var title = "Fone Bluetooth";
            var amount = 160.99M;
            var paidAt = new DateOnly(2026, 10, 03);
            var categoryId = 5;
            var installmentInfo = InstallmentInfo.SinglePayment();
            var userId = Guid.NewGuid();
            var transaction = new Transaction(
                workspaceId, title, description, TransactionType.Expense,
                amount, paidAt, categoryId, installmentInfo, userId);

            Assert.Null(transaction.Description);
        }

        [Theory]
        [MemberData(nameof(BlankStrings))]
        public void GivenBlankDescription_WhenChangingDescription_ThenDescriptionIsNull(string? blankDescription)
        {
            var workspaceId = Guid.NewGuid();
            var title = "Fone Bluetooth";
            var description = "Fone com cancelamento de ruído comprado para reposição";
            var amount = 160.99M;
            var paidAt = new DateOnly(2026, 10, 03);
            var categoryId = 5;
            var installmentInfo = InstallmentInfo.SinglePayment();
            var userId = Guid.NewGuid();
            var transaction = new Transaction(
                workspaceId, title, description, TransactionType.Expense,
                amount, paidAt, categoryId, installmentInfo, userId);

            transaction.ChangeDescription(blankDescription);

            Assert.Null(transaction.Description);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GivenInvalidAmount_WhenCreatingTransaction_ThenThrowsDomainException(decimal amount)
        {
            var workspaceId = Guid.NewGuid();
            var title = "Fone Bluetooth";
            var description = "Fone com cancelamento de ruído comprado para reposição";
            var paidAt = new DateOnly(2026, 10, 03);
            var categoryId = 5;
            var installmentInfo = InstallmentInfo.SinglePayment();
            var userId = Guid.NewGuid();
            
            Assert.Throws<DomainException>(() =>
                new Transaction(
                    workspaceId, title, description, TransactionType.Expense,
                    amount, paidAt, categoryId, installmentInfo, userId)
            );
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GivenInvalidAmount_WhenChangingAmount_ThenThrowsDomainException(decimal invalidAmount)
        {
            var workspaceId = Guid.NewGuid();
            var title = "Fone Bluetooth";
            var description = "Fone com cancelamento de ruído comprado para reposição";
            var amount = 150.99M;
            var paidAt = new DateOnly(2026, 10, 03);
            var categoryId = 5;
            var installmentInfo = InstallmentInfo.SinglePayment();
            var userId = Guid.NewGuid();
            var transaction = new Transaction(
                workspaceId, title, description, TransactionType.Expense,
                amount, paidAt, categoryId, installmentInfo, userId);

            Assert.Throws<DomainException>(() =>
                transaction.ChangeAmount(invalidAmount));
            Assert.Equal(amount, transaction.Amount);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GivenInvalidCategoryId_WhenCreatingTransaction_ThenThrowsDomainException(long invalidCategoryId)
        {
            var workspaceId = Guid.NewGuid();
            var title = "Fone Bluetooth";
            var description = "Fone com cancelamento de ruído comprado para reposição";
            var amount = 150.99M;
            var paidAt = new DateOnly(2026, 10, 03);
            var installmentInfo = InstallmentInfo.SinglePayment();
            var userId = Guid.NewGuid();

            Assert.Throws<DomainException>(() =>
                new Transaction(
                workspaceId, title, description, TransactionType.Expense,
                amount, paidAt, invalidCategoryId, installmentInfo, userId));

        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GivenInvalidCategoryId_WhenMovingToCategory_ThenThrowsDomainException(long invalidCategoryId)
        {
            var workspaceId = Guid.NewGuid();
            var title = "Fone Bluetooth";
            var description = "Fone com cancelamento de ruído comprado para reposição";
            var amount = 150.99M;
            var paidAt = new DateOnly(2026, 10, 03);
            var categoryId = 5;
            var installmentInfo = InstallmentInfo.SinglePayment();
            var userId = Guid.NewGuid();
            var transaction = new Transaction(
                workspaceId, title, description, TransactionType.Expense,
                amount, paidAt, categoryId, installmentInfo, userId);

            Assert.Throws<DomainException>(() =>
                transaction.MoveToCategory(invalidCategoryId));
            Assert.Equal(categoryId, transaction.CategoryId);
        }

        [Theory]
        [InlineData(TransactionType.Expense)]
        [InlineData(TransactionType.Income)]
        public void GivenValidType_WhenCreatingTransaction_ThenInitializesType(TransactionType type)
        {
            var transaction = new Transaction(
                Guid.NewGuid(),
                "Teste tipo transação",
                null,
                type,
                100M,
                new DateOnly(2026, 11, 2),
                1,
                InstallmentInfo.SinglePayment(),
                Guid.NewGuid());

            Assert.Equal(type, transaction.Type);
        }
    }
}
