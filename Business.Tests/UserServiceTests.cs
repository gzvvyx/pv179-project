using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.DTOs;
using Business.Services;
using DAL.Models;
using Infra.Repository;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Business.Tests;

public class UserServiceTests
{
    private static User CreateUser(string id, string userName, string? email = null) => new User
    {
        Id = id,
        UserName = userName,
        Email = email
    };

    [Fact]
    public async Task GetAllAsync_ReturnsMappedList()
    {
        var repo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new UserService(repo.Object);

        var users = new List<User>
        {
            CreateUser("u1", "alice", "a@example.com"),
            CreateUser("u2", "bob", null)
        };
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, u => u.Id == "u1" && u.UserName == "alice" && u.Email == "a@example.com");
        Assert.Contains(result, u => u.Id == "u2" && u.UserName == "bob" && u.Email == null);

        repo.Verify(r => r.GetAllAsync(), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var repo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new UserService(repo.Object);

        repo.Setup(r => r.GetByIdAsync("x")).ReturnsAsync((User?)null);

        var result = await service.GetByIdAsync("x");

        Assert.Null(result);
        repo.Verify(r => r.GetByIdAsync("x"), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenFound()
    {
        var repo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new UserService(repo.Object);

        var user = CreateUser("id-7", "sue", "sue@example.com");
        repo.Setup(r => r.GetByIdAsync("id-7")).ReturnsAsync(user);

        var dto = await service.GetByIdAsync("id-7");

        Assert.NotNull(dto);
        Assert.Equal(user.Id, dto!.Id);
        Assert.Equal(user.UserName, dto.UserName);
        Assert.Equal(user.Email, dto.Email);

        repo.Verify(r => r.GetByIdAsync("id-7"), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_Success_CreatesUserAndReturnsDto()
    {
        var repo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new UserService(repo.Object);

        var dto = new UserCreateDto
        {
            UserName = "john",
            Email = "john@example.com",
            Password = "P@ssw0rd!"
        };

        User? created = null;
        string? passedPassword = null;
        repo.Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .Callback<User, string>((u, p) => { created = u; passedPassword = p; })
            .ReturnsAsync(IdentityResult.Success);

        var (result, userDto) = await service.CreateAsync(dto);

        Assert.True(result.Succeeded);
        Assert.NotNull(userDto);
        Assert.NotNull(created);
        Assert.Equal(dto.UserName, created!.UserName);
        Assert.Equal(dto.Email, created.Email);
        Assert.Equal(dto.Password, passedPassword);
        Assert.Equal(created.Id, userDto!.Id); // Id may be empty default, mapping should reflect entity
        Assert.Equal(created.UserName, userDto.UserName);
        Assert.Equal(created.Email, userDto.Email);

        repo.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_Failure_ReturnsFailureAndNoDto()
    {
        var repo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new UserService(repo.Object);

        var dto = new UserCreateDto
        {
            UserName = "john",
            Email = "john@example.com",
            Password = "pw"
        };

        var failure = IdentityResult.Failed(new IdentityError { Code = "DuplicateEmail" });
        repo.Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(failure);

        var (result, user) = await service.CreateAsync(dto);

        Assert.False(result.Succeeded);
        Assert.Null(user);
        var error = Assert.Single(result.Errors);
        Assert.Equal("DuplicateEmail", error.Code);

        repo.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateAsync_ReturnsError_WhenUserNotFound()
    {
        var repo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new UserService(repo.Object);

        repo.Setup(r => r.GetByIdAsync("missing")).ReturnsAsync((User?)null);

        var (result, user) = await service.UpdateAsync("missing", new UserUpdateDto { UserName = "x" });

        Assert.False(result.Succeeded);
        Assert.Null(user);
        var error = Assert.Single(result.Errors);
        Assert.Equal("UserNotFound", error.Code);

        repo.Verify(r => r.GetByIdAsync("missing"), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateAsync_FailureFromRepository_ReturnsFailureAndNoDto()
    {
        var repo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new UserService(repo.Object);

        var existing = CreateUser("id-1", "old", "old@example.com");
        repo.Setup(r => r.GetByIdAsync("id-1")).ReturnsAsync(existing);

        var failure = IdentityResult.Failed(new IdentityError { Code = "UpdateFailed" });
        repo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(failure);

        var (result, dto) = await service.UpdateAsync("id-1", new UserUpdateDto
        {
            UserName = "new",
            Email = "new@example.com"
        });

        Assert.False(result.Succeeded);
        Assert.Null(dto);
        var error = Assert.Single(result.Errors);
        Assert.Equal("UpdateFailed", error.Code);

        repo.Verify(r => r.GetByIdAsync("id-1"), Times.Once);
        repo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateAsync_Success_UpdatesFieldsAndReturnsDto()
    {
        var repo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new UserService(repo.Object);

        var existing = CreateUser("id-2", "old-name", "old@example.com");
        repo.Setup(r => r.GetByIdAsync("id-2")).ReturnsAsync(existing);
        repo.Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success);

        var (result, dto) = await service.UpdateAsync("id-2", new UserUpdateDto
        {
            UserName = "new-name",
            Email = "new@example.com"
        });

        Assert.True(result.Succeeded);
        Assert.NotNull(dto);
        Assert.Equal("id-2", dto!.Id);
        Assert.Equal("new-name", dto.UserName);
        Assert.Equal("new@example.com", dto.Email);

        repo.Verify(r => r.GetByIdAsync("id-2"), Times.Once);
        repo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteAsync_ReturnsError_WhenUserNotFound()
    {
        var repo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new UserService(repo.Object);

        repo.Setup(r => r.GetByIdAsync("gone")).ReturnsAsync((User?)null);

        var result = await service.DeleteAsync("gone");

        Assert.False(result.Succeeded);
        var error = Assert.Single(result.Errors);
        Assert.Equal("UserNotFound", error.Code);

        repo.Verify(r => r.GetByIdAsync("gone"), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteAsync_ReturnsRepositoryResult()
    {
        var repo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new UserService(repo.Object);

        var user = CreateUser("id-x", "x");
        repo.Setup(r => r.GetByIdAsync("id-x")).ReturnsAsync(user);
        repo.Setup(r => r.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

        var resultOk = await service.DeleteAsync("id-x");
        Assert.True(resultOk.Succeeded);

        repo.Verify(r => r.GetByIdAsync("id-x"), Times.Once);
        repo.Verify(r => r.DeleteAsync(user), Times.Once);
        repo.VerifyNoOtherCalls();
    }
}

