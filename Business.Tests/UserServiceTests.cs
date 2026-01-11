using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.DTOs;
using Business.Services;
using Business.Validators;
using DAL.Data;
using DAL.Models;
using ErrorOr;
using FluentValidation;
using Infra.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Business.Tests;

public class UserServiceTests
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<UserCreateDto> _createValidator;
    private readonly IValidator<UserUpdateDto> _updateValidator;
    private readonly Mock<UserManager<User>> _userManager;
    private readonly Mock<IUserRepository> _repo;
    private readonly UserService _service;

    public UserServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new AppDbContext(options);
        _createValidator = new UserCreateDtoValidator();
        _updateValidator = new UserUpdateDtoValidator();
        var store = new Mock<IUserStore<User>>();
        _userManager = new Mock<UserManager<User>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        _repo = new Mock<IUserRepository>(MockBehavior.Strict);
        _service = new UserService(_repo.Object, _userManager.Object, _dbContext, _createValidator, _updateValidator);
    }

    private static User CreateUser(string id, string userName, string? email = null) => new User
    {
        Id = id,
        UserName = userName,
        Email = email
    };

    [Fact]
    public async Task GetAllAsync_ReturnsMappedList()
    {

        var users = new List<User>
        {
            CreateUser("u1", "alice", "a@example.com"),
            CreateUser("u2", "bob", null)
        };
        _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, u => u.Id == "u1" && u.UserName == "alice" && u.Email == "a@example.com");
        Assert.Contains(result, u => u.Id == "u2" && u.UserName == "bob" && u.Email == null);

        _repo.Verify(r => r.GetAllAsync(), Times.Once);
        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {

        _repo.Setup(r => r.GetByIdAsync("x")).ReturnsAsync((User?)null);

        var result = await _service.GetByIdAsync("x");

        Assert.Null(result);
        _repo.Verify(r => r.GetByIdAsync("x"), Times.Once);
        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenFound()
    {

        var user = CreateUser("id-7", "sue", "sue@example.com");
        _repo.Setup(r => r.GetByIdAsync("id-7")).ReturnsAsync(user);

        var dto = await _service.GetByIdAsync("id-7");

        Assert.NotNull(dto);
        Assert.Equal(user.Id, dto!.Id);
        Assert.Equal(user.UserName, dto.UserName);
        Assert.Equal(user.Email, dto.Email);

        _repo.Verify(r => r.GetByIdAsync("id-7"), Times.Once);
        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_Success_CreatesUserAndReturnsDto()
    {

        var dto = new UserCreateDto
        {
            UserName = "john",
            Email = "john@example.com",
            Password = "P@ssw0rd!"
        };

        User? created = null;
        string? passedPassword = null;
        _repo.Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .Callback<User, string>((u, p) => { created = u; passedPassword = p; })
            .ReturnsAsync(IdentityResult.Success);

        var result = await _service.CreateAsync(dto);

        Assert.False(result.IsError);
        var userDto = result.Value;
        Assert.NotNull(userDto);
        Assert.NotNull(created);
        Assert.Equal(dto.UserName, created!.UserName);
        Assert.Equal(dto.Email, created.Email);
        Assert.Equal(dto.Password, passedPassword);
        Assert.Equal(created.Id, userDto.Id); // Id may be empty default, mapping should reflect entity
        Assert.Equal(created.UserName, userDto.UserName);
        Assert.Equal(created.Email, userDto.Email);

        _repo.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_Failure_ReturnsFailureAndNoDto()
    {

        var dto = new UserCreateDto
        {
            UserName = "john",
            Email = "john@example.com",
            Password = "pw"
        };

        var failure = IdentityResult.Failed(new IdentityError { Code = "DuplicateEmail", Description = "Email already exists" });
        _repo.Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(failure);

        var result = await _service.CreateAsync(dto);

        Assert.True(result.IsError);
        Assert.Equal("DuplicateEmail", result.FirstError.Code);

        _repo.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateAsync_ReturnsError_WhenUserNotFound()
    {

        var updateDto = new UserUpdateDto { UserName = "x" };
        _repo.Setup(r => r.GetByIdAsync("missing")).ReturnsAsync((User?)null);

        var result = await _service.UpdateAsync("missing", updateDto);

        Assert.True(result.IsError);
        Assert.Equal(ErrorType.NotFound, result.FirstError.Type);

        _repo.Verify(r => r.GetByIdAsync("missing"), Times.Once);
        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateAsync_FailureFromRepository_ReturnsFailureAndNoDto()
    {

        var existing = CreateUser("id-1", "old", "old@example.com");
        _repo.Setup(r => r.GetByIdAsync("id-1")).ReturnsAsync(existing);

        var failure = IdentityResult.Failed(new IdentityError { Code = "UpdateFailed", Description = "Update failed" });
        _repo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(failure);

        var updateDto = new UserUpdateDto
        {
            UserName = "new",
            Email = "new@example.com"
        };
        var result = await _service.UpdateAsync("id-1", updateDto);

        Assert.True(result.IsError);
        Assert.Equal("UpdateFailed", result.FirstError.Code);

        _repo.Verify(r => r.GetByIdAsync("id-1"), Times.Once);
        _repo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateAsync_Success_UpdatesFieldsAndReturnsDto()
    {

        var existing = CreateUser("id-2", "old-name", "old@example.com");
        _repo.Setup(r => r.GetByIdAsync("id-2")).ReturnsAsync(existing);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success);

        var updateDto = new UserUpdateDto
        {
            UserName = "new-name",
            Email = "new@example.com"
        };
        var result = await _service.UpdateAsync("id-2", updateDto);

        Assert.False(result.IsError);
        var dto = result.Value;
        Assert.NotNull(dto);
        Assert.Equal("id-2", dto.Id);
        Assert.Equal("new-name", dto.UserName);
        Assert.Equal("new@example.com", dto.Email);

        _repo.Verify(r => r.GetByIdAsync("id-2"), Times.Once);
        _repo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteAsync_ReturnsError_WhenUserNotFound()
    {

        _repo.Setup(r => r.GetByIdAsync("gone")).ReturnsAsync((User?)null);

        var result = await _service.DeleteAsync("gone");

        Assert.True(result.IsError);
        Assert.Equal(ErrorType.NotFound, result.FirstError.Type);

        _repo.Verify(r => r.GetByIdAsync("gone"), Times.Once);
        _repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteAsync_ReturnsRepositoryResult()
    {

        var user = CreateUser("id-x", "x");
        _repo.Setup(r => r.GetByIdAsync("id-x")).ReturnsAsync(user);
        _repo.Setup(r => r.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

        var resultOk = await _service.DeleteAsync("id-x");
        Assert.False(resultOk.IsError);

        _repo.Verify(r => r.GetByIdAsync("id-x"), Times.Once);
        _repo.Verify(r => r.DeleteAsync(user), Times.Once);
        _repo.VerifyNoOtherCalls();
    }
}
