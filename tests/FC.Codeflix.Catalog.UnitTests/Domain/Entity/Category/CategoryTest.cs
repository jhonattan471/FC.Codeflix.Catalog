using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NuGet.Frameworks;
using System.Diagnostics.SymbolStore;
using System.Linq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;

[Collection(nameof(CategoryTestFixture))]

public class CategoryTest
{
    private readonly CategoryTestFixture _categoryTestFixture;

    public CategoryTest(CategoryTestFixture categoryTestFixture)
    {
        _categoryTestFixture = categoryTestFixture;
    }

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        //Arrange
        var validCategory = _categoryTestFixture.getValidCategory();
        var dateTimeBefore = DateTime.Now;

        //Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description);
        var dateTimeAfter = DateTime.Now.AddSeconds(1);

        //Assert
        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);

        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));

        (category.CreatedAt >= dateTimeBefore).Should().BeTrue();
        (category.CreatedAt <= dateTimeAfter).Should().BeTrue();
        (category.IsActive).Should().BeTrue();
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [InlineData(true)]
    [InlineData(false)]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateWithIsActive(bool isActive)
    {
        //Arrange
        var validCategory = _categoryTestFixture.getValidCategory();
        var dateTimeBefore = DateTime.Now;

        //Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, isActive);
        var dateTimeAfter = DateTime.Now.AddSeconds(1);

        //Assert
        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);

        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));

        (category.CreatedAt >= dateTimeBefore).Should().BeTrue();
        (category.CreatedAt <= dateTimeAfter).Should().BeTrue();

        category.IsActive.Should().Be(isActive);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void InstantiateErrorWhenNameIsEmpty(string? name)
    {
        var validCategory = _categoryTestFixture.getValidCategory();

        Action action =
            () => new DomainEntity.Category(name!, validCategory.Description);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        var validCategory = _categoryTestFixture.getValidCategory();

        Action action =
            () => new DomainEntity.Category(validCategory.Name, null!);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should not be empty or null");
    }

    [Theory(DisplayName = nameof(InstantieteErrorWhenNameIsLessThan3Characteres))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("a")]
    [InlineData("ab")]
    [InlineData("1")]
    [InlineData("12")]
    public void InstantieteErrorWhenNameIsLessThan3Characteres(string name)
    {
        var validCategory = _categoryTestFixture.getValidCategory();

        Action action = () => new DomainEntity.Category(name, validCategory.Description);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be at least 3 characteres long");
    }

    [Fact(DisplayName = nameof(InstantieteErrorWhenNameIsGreaterThan255characteres))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantieteErrorWhenNameIsGreaterThan255characteres()
    {
        var invalidName = String.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());
        var validCategory = _categoryTestFixture.getValidCategory();

        Action action = () => new DomainEntity.Category(invalidName, validCategory.Description);
        action.Should()
           .Throw<EntityValidationException>()
           .WithMessage("Name should be less or equal 255 characteres long");
    }

    [Fact(DisplayName = nameof(InstantieteErrorWhenDescriptionIsGreaterThan10_000characteres))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantieteErrorWhenDescriptionIsGreaterThan10_000characteres()
    {
        var invalidDescription = String.Join(null, Enumerable.Range(1, 10_001).Select(_ => "a").ToArray());
        var validCategory = _categoryTestFixture.getValidCategory();

        Action action = () => new DomainEntity.Category(validCategory.Name, invalidDescription);

        action.Should()
           .Throw<EntityValidationException>()
           .WithMessage("Description should be less or equal than 10.000 characteres long");
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        //Arrange
        var validCategory = _categoryTestFixture.getValidCategory();
        var dateTimeBefore = DateTime.Now;

        //Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, false);
        category.Activate();

        //Assert
        category.IsActive.Should().BeTrue();
    }


    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Deactivate()
    {
        //Arrange
        var validCategory = _categoryTestFixture.getValidCategory();

        //Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, true);
        category.Deactivate();

        //Assert
        category.IsActive.Should().BeFalse();
    }


    [Fact(DisplayName = nameof(update))]
    [Trait("Domain", "Category - Aggregates")]
    public void update()
    {
        var validEntity = _categoryTestFixture.getValidCategory();
        var newEntity = new { Name = "New Name", Description = "New Description" };
        validEntity.Update(newEntity.Name, newEntity.Description);

        validEntity.Name.Should().Be("New Name");
        validEntity.Description.Should().Be("New Description");
    }

    [Fact(DisplayName = nameof(updateOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void updateOnlyName()
    {
        var validEntity = _categoryTestFixture.getValidCategory();
        var newEntity = new { Name = "New Name" };
        validEntity.Update(newEntity.Name);

        validEntity.Name.Should().Be(newEntity.Name);
        validEntity.Description.Should().Be(validEntity.Description);
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateErrorWhenNameIsEmpty(string? name)
    {
        var categoryEntity = _categoryTestFixture.getValidCategory();
        Action action =
            () => categoryEntity.Update(name!);

        action.Should()
         .Throw<EntityValidationException>()
         .WithMessage("Name should not be empty or null");
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsLessThan3Characteres))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("a")]
    [InlineData("ab")]
    [InlineData("1")]
    [InlineData("12")]
    public void UpdateErrorWhenNameIsLessThan3Characteres(string invalidName)
    {

        var categoryEntity = _categoryTestFixture.getValidCategory();
        Action action = () => categoryEntity.Update(invalidName);

        action.Should()
         .Throw<EntityValidationException>()
         .WithMessage("Name should be at least 3 characteres long");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameIsGreaterThan255characteres))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameIsGreaterThan255characteres()
    {
        var invalidName = String.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());
        var categoryEntity = _categoryTestFixture.getValidCategory();

        Action action = () => categoryEntity.Update(invalidName);

        action.Should()
         .Throw<EntityValidationException>()
         .WithMessage("Name should be less or equal 255 characteres long");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionIsGreaterThan10_000characteres))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenDescriptionIsGreaterThan10_000characteres()
    {
        var invalidDescription = String.Join(null, Enumerable.Range(1, 10_001).Select(_ => "a").ToArray());
        var categoryEntity = _categoryTestFixture.getValidCategory();

        Action action = () => categoryEntity.Update(categoryEntity.Name, invalidDescription);

        action.Should()
        .Throw<EntityValidationException>()
        .WithMessage("Description should be less or equal than 10.000 characteres long");
    }
}
