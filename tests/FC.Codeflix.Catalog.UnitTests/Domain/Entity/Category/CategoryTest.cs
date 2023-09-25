using FC.Codeflix.Catalog.Domain.Exceptions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NuGet.Frameworks;
using System.Diagnostics.SymbolStore;
using System.Linq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;

public class CategoryTest
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        //Arrange
        var validData = new
        {
            Name = "category name",
            Description = "category description"
        };
        var dateTimeBefore = DateTime.Now;

        //Act
        var category = new DomainEntity.Category(validData.Name, validData.Description);
        var dateTimeAfter = DateTime.Now;

        //Assert
        Assert.NotNull(category);
        Assert.Equal(validData.Name, category.Name);
        Assert.Equal(validData.Description, category.Description);
        Assert.NotEqual(default(Guid), category.Id);
        Assert.NotEqual(default(DateTime), category.CreatedAt);
        Assert.True(category.CreatedAt > dateTimeBefore);
        Assert.True(category.CreatedAt < dateTimeAfter);
        Assert.True(category.IsActive);
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [InlineData(true)]
    [InlineData(false)]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateWithIsActive(bool isActive)
    {
        //Arrange
        var validData = new
        {
            Name = "category name",
            Description = "category description"
        };
        var dateTimeBefore = DateTime.Now;

        //Act
        var category = new DomainEntity.Category(validData.Name, validData.Description, isActive);
        var dateTimeAfter = DateTime.Now;

        //Assert
        Assert.NotNull(category);
        Assert.Equal(validData.Name, category.Name);
        Assert.Equal(validData.Description, category.Description);
        Assert.NotEqual(default(Guid), category.Id);
        Assert.NotEqual(default(DateTime), category.CreatedAt);
        Assert.True(category.CreatedAt > dateTimeBefore);
        Assert.True(category.CreatedAt < dateTimeAfter);
        Assert.Equal(isActive, category.IsActive);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void InstantiateErrorWhenNameIsEmpty(string? name)
    {
        Action action =
            () => new DomainEntity.Category(name!, "Category Description");

        var exception = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Name should not be empty or null", exception.Message);
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        Action action =
            () => new DomainEntity.Category("Category Name", null!);

        var exception = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Description should not be empty or null", exception.Message);
    }

    [Theory(DisplayName = nameof(InstantieteErrorWhenNameIsLessThan3Characteres))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("a")]
    [InlineData("ab")]
    [InlineData("1")]
    [InlineData("12")]
    public void InstantieteErrorWhenNameIsLessThan3Characteres(string name)
    {
        var obj = new
        {
            Name = name,
            Description = "category description"
        };

        Action action = () => new DomainEntity.Category(obj.Name, obj.Description);

        var exption = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Name should be at least 3 characteres long", exption.Message);
    }

    [Fact(DisplayName = nameof(InstantieteErrorWhenNameIsGreaterThan255characteres))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantieteErrorWhenNameIsGreaterThan255characteres()
    {
        var invalidName = String.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());

        var obj = new
        {
            Name = invalidName,
            Description = "category description"
        };

        Action action = () => new DomainEntity.Category(obj.Name, obj.Description);

        var exption = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Name should be less or equal 255 characteres long", exption.Message);
    }

    [Fact(DisplayName = nameof(InstantieteErrorWhenDescriptionIsGreaterThan10_000characteres))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantieteErrorWhenDescriptionIsGreaterThan10_000characteres()
    {
        var invalidDescription = String.Join(null, Enumerable.Range(1, 10_001).Select(_ => "a").ToArray());

        var obj = new
        {
            Name = "Name",
            Description = invalidDescription
        };

        Action action = () => new DomainEntity.Category(obj.Name, obj.Description);

        var exption = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Description should be less or equal than 10.000 characteres long", exption.Message);
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        //Arrange
        var validData = new
        {
            Name = "category name",
            Description = "category description"
        };
        var dateTimeBefore = DateTime.Now;

        //Act
        var category = new DomainEntity.Category(validData.Name, validData.Description, false);
        category.Activate();

        //Assert
        Assert.True(category.IsActive);
    }


    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Deactivate()
    {
        //Arrange
        var validData = new
        {
            Name = "category name",
            Description = "category description"
        };
        var dateTimeBefore = DateTime.Now;

        //Act
        var category = new DomainEntity.Category(validData.Name, validData.Description, true);
        category.Deactivate();

        //Assert
        Assert.False(category.IsActive);
    }


    [Fact(DisplayName = nameof(update))]
    [Trait("Domain", "Category - Aggregates")]
    public void update()
    {
        var validEntity = new DomainEntity.Category("Category name", "Category description");
        var newEntity = new { Name = "New Name", Description = "New Description" };
        validEntity.Update(newEntity.Name, newEntity.Description);

        Assert.Equal(validEntity.Name, "New Name");
        Assert.Equal(validEntity.Description, "New Description");
    }

    [Fact(DisplayName = nameof(updateOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void updateOnlyName()
    {
        var validEntity = new DomainEntity.Category("Category name", "Category description");
        var newEntity = new { Name = "New Name", Description = "New Description" };
        validEntity.Update(newEntity.Name);

        Assert.Equal(validEntity.Name, "New Name");
        Assert.Equal(validEntity.Description, "Category description");
    }    

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateErrorWhenNameIsEmpty(string? name)
    {
        var categoryEntity = new DomainEntity.Category("Category Name", "Category Description");
        Action action =
            () => categoryEntity.Update(name!);

        var exception = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Name should not be empty or null", exception.Message);
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsLessThan3Characteres))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("a")]
    [InlineData("ab")]
    [InlineData("1")]
    [InlineData("12")]
    public void UpdateErrorWhenNameIsLessThan3Characteres(string invalidName)
    {

        var categoryEntity = new DomainEntity.Category("Category Name", "Category Description");
        Action action = () => categoryEntity.Update(invalidName);

        var exption = Assert.Throws<EntityValidationException>(action);
        Assert.Equal("Name should be at least 3 characteres long", exption.Message);
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameIsGreaterThan255characteres))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameIsGreaterThan255characteres()
    {
        var invalidName = String.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());
        var categoryEntity = new DomainEntity.Category("Category Name", "Category Description");

        Action action = () => categoryEntity.Update(invalidName);
        var exption = Assert.Throws<EntityValidationException>(action);

        Assert.Equal("Name should be less or equal 255 characteres long", exption.Message);
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionIsGreaterThan10_000characteres))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenDescriptionIsGreaterThan10_000characteres()
    {
        var invalidDescription = String.Join(null, Enumerable.Range(1, 10_001).Select(_ => "a").ToArray());
        var categoryEntity = new DomainEntity.Category("Category Name", "Category Description");

        Action action = () => categoryEntity.Update(categoryEntity.Name, invalidDescription);
        var exption = Assert.Throws<EntityValidationException>(action);

        Assert.Equal("Description should be less or equal than 10.000 characteres long", exption.Message);
    }
}
