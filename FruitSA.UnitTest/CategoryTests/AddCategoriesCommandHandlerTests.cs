﻿using FruitSA.Application.Categories.Commands.AddCategory;
using FruitSA.Application.Interfaces;
using FruitSA.Application.Shared.Category;
using FruitSA.Domain.Entities;
using FruitSA.Domain.Helper;
using Moq;

namespace FruitSA.UnitTest.CategoryTests
{
    public class AddCategoriesCommandHandlerTests
    {
        private readonly Mock<ICategoryRepository> _mockRepository;

        public AddCategoriesCommandHandlerTests()
        {
            _mockRepository = new Mock<ICategoryRepository>();
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenCategoryIsAdded()
        {
            // Arrange
            var handler = new AddCategoryHandler(_mockRepository.Object);

            var command = new AddCategoryCommand
            {
                Name = "Fruits",
                Description = "Fresh fruits",
                CategoryCode = "FRU001",
                CreatedBy = "System"
            };

            var expectedCategory = new CategoryViewModel
            {
                Name = command.Name,
                Description = command.Description,
                CategoryCode = command.CategoryCode,
                CreatedBy = command.CreatedBy,
                IsActive = true,
            };

            _mockRepository
                .Setup(repo => repo.AddCategoryAsync(It.IsAny<Category>(), CancellationToken.None))
                .ReturnsAsync(Result<CategoryViewModel>.Ok(expectedCategory, "Category added successfully."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Fruits", result.Data.Name);
            Assert.Equal("FRU001", result.Data.CategoryCode);
            _mockRepository.Verify(repo => repo.AddCategoryAsync(It.IsAny<Category>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenRepositoryFails()
        {
            // Arrange
            var handler = new AddCategoryHandler(_mockRepository.Object);

            var command = new AddCategoryCommand
            {
                Name = "Fruits",
                Description = "Fresh fruits",
                CategoryCode = "FRU001",
                CreatedBy = "System"
            };

            _mockRepository.Setup(repo => repo.AddCategoryAsync(It.IsAny<Category>(), CancellationToken.None))
                .ReturnsAsync(Result<CategoryViewModel>.Fail("A database error occurred while adding the category."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("A database error occurred while adding the category.", result.Message);
            _mockRepository.Verify(repo => repo.AddCategoryAsync(It.IsAny<Category>(), CancellationToken.None), Times.Once);
        }
    }
}
