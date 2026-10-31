using Saldoa.Domain.Entities;
using Saldoa.Domain.Exceptions;
using System.Drawing;

namespace Saldoa.UnitTests.Domain.Entities
{
    public class CategoryTests
    {
        public static TheoryData<string?> BlankStrings => new()
        {
            "",
            null,
            " ",
            "\t"
        };

        [Fact]
        public void GivenEmptyWorkspaceId_WhenCreatingCategory_ThenThrowsDomainException()
        {
            var workspaceId = Guid.Empty;
            var name = "Transporte";

            Assert.Throws<DomainException>(() =>
                new Category(workspaceId, name, null, null));
        }

        [Theory]
        [MemberData(nameof(BlankStrings))]
        public void GivenBlankName_WhenCreatingCategory_ThenThrowsDomainException(string? name)
        {
            Assert.Throws<DomainException>(() =>
                new Category(Guid.NewGuid(), name!, null, null));
        }

        [Theory]
        [MemberData(nameof(BlankStrings))]
        public void GivenBlankDescription_WhenCreatingCategory_ThenDescriptionIsNull(string? description)
        {
            var workspaceId = Guid.NewGuid();
            var name = "Mercado";

            var category = new Category(workspaceId, name, description!, null);

            Assert.Null(category.Description);
        }

        [Theory]
        [MemberData(nameof(BlankStrings))]
        public void GivenBlankColor_WhenCreatingCategory_ThenColorIsNull(string? color)
        {
            var workspaceId = Guid.NewGuid();
            var name = "Mercado";

            var category = new Category(workspaceId, name, null, color!);

            Assert.Null(category.Color);
        }

        [Theory]
        [MemberData(nameof(BlankStrings))]
        public void GivenBlankDescription_WhenChangingDescription_ThenDescriptionIsNull(string? description)
        {
            var workspaceId = Guid.NewGuid();
            var name = "Mercado";

            var category = new Category(workspaceId, name, "Gastos com mercado", null);

            category.ChangeDescription(description);

            Assert.Null(category.Description);
        }

        [Theory]
        [MemberData(nameof(BlankStrings))]
        public void GivenBlankColor_WhenChangingColor_ThenColorIsNull(string? color)
        {
            var workspaceId = Guid.NewGuid();
            var name = "Mercado";

            var category = new Category(workspaceId, name, null, "#FFA500");

            category.ChangeColor(color);

            Assert.Null(category.Color);
        }

        [Theory]
        [InlineData("Mercado")]
        [InlineData("mercado")]
        [InlineData("MERCADO")]
        [InlineData("mercAdo")]
        public void GivenName_WhenRenamingCategory_ThenNormalizedNameIsUppercase(string name)
        {
            var workspaceId = Guid.NewGuid();

            var category = new Category(workspaceId, "transporte", null, null);

            category.Rename(name);

            Assert.Equal("MERCADO", category.NormalizedName);
            Assert.Equal(name, category.Name);
        }

        [Theory]
        [MemberData(nameof(BlankStrings))]
        public void GivenBlankName_WhenRenamingCategory_ThenThrowsDomainException(string? name)
        {
            var workspaceId = Guid.NewGuid();

            var category = new Category(workspaceId, "transporte", null, null);

            Assert.Throws<DomainException>(() => category.Rename(name!));
            Assert.Equal("transporte", category.Name);
            Assert.Equal("TRANSPORTE", category.NormalizedName);
        }

        [Fact]
        public void GivenValidData_WhenCreatingCategory_ThenSuccess()
        {
            var workspaceId = Guid.NewGuid();
            var name = " Comida ";
            var description = " Gastos com mercado, padaria e restaurantes ";
            var color = " #FFA500 ";

            var category = new Category(workspaceId, name, description, color);

            Assert.Equal(workspaceId, category.WorkspaceId);
            Assert.Equal("Comida", category.Name);
            Assert.Equal("COMIDA", category.NormalizedName);
            Assert.Equal("Gastos com mercado, padaria e restaurantes", category.Description);
            Assert.Equal("#FFA500", category.Color);
        }

        [Fact]
        public void GivenName_WhenRenamingCategory_ThenNameChanged()
        {
            var workspaceId = Guid.NewGuid();
            var name = "Comida";
            var newName = " Lazer ";
            var category = new Category(workspaceId, name, null, null);

            category.Rename(newName);

            Assert.Equal("Lazer", category.Name);
            Assert.Equal("LAZER", category.NormalizedName);
        }

        [Fact]
        public void GivenDescription_WhenChangingDescription_ThenDescriptionChanged()
        {
            var workspaceId = Guid.NewGuid();
            var name = "Comida";
            var newDescription = " Gastos com mercado, padaria e restaurantes ";
            var category = new Category(workspaceId, name, null, null);

            category.ChangeDescription(newDescription);

            Assert.Equal("Gastos com mercado, padaria e restaurantes", category.Description);
        }

        [Fact]
        public void GivenColor_WhenChangingColor_ThenColorChanged()
        {
            var workspaceId = Guid.NewGuid();
            var name = "Comida";
            var newColor = " #FFA500 ";
            var category = new Category(workspaceId, name, null, null);

            category.ChangeColor(newColor);

            Assert.Equal("#FFA500", category.Color);
        }
    }
}
