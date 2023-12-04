using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.CreateCategory;

public class CreateCategoryTest
{
    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Application","CreateCategory - Use Cases")]
    public void CreateCategory()
    {
        var respositoryMock = new Mock<ICategoryRepository>();
        var unitOfWorkMock = new Mock<IunitOfWork>();
        var useCase = new CreateCategory(
            respositoryMock.Object,
            unitOfWorkMock.Object);
    }
}
