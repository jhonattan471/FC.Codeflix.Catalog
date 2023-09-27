using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;



public class CategoryTestFixture
{
    public DomainEntity.Category getValidCategory()
    {
        return new DomainEntity.Category("Category Name", "Category Description");
    }
}

[CollectionDefinition(nameof(CategoryTestFixture))]
public class CategoryTestFixtureCollection
    : ICollectionFixture<CategoryTestFixture>
{
}
