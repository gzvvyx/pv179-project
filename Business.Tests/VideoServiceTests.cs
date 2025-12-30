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
using Infra.DTOs;
using Infra.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Business.Tests;

public class VideoServiceTests
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<VideoCreateDto> _createValidator;
    private readonly IValidator<VideoUpdateDto> _updateValidator;
    private readonly Mock<IVideoRepository> _videoRepo;
    private readonly Mock<IUserRepository> _userRepo;
    private readonly VideoService _service;

    public VideoServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new AppDbContext(options);
        _createValidator = new VideoCreateDtoValidator();
        _updateValidator = new VideoUpdateDtoValidator();
        _videoRepo = new Mock<IVideoRepository>(MockBehavior.Strict);
        _userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        _service = new VideoService(_videoRepo.Object, _userRepo.Object, _dbContext, _createValidator, _updateValidator);
    }

    private static User CreateUser(string id) => new User
    {
        Id = id,
        UserName = $"user-{id}"
    };

    private static Video CreateVideo(
        int id,
        string creatorId,
        string title,
        string description,
        string url,
        string thumbnailUrl,
        DateTime? createdAt = null,
        DateTime? updatedAt = null)
    {
        var now = DateTime.UtcNow;
        var creator = CreateUser(creatorId);
        return new Video
        {
            Id = id,
            CreatorId = creatorId,
            Creator = creator,
            Title = title,
            Description = description,
            Url = url,
            ThumbnailUrl = thumbnailUrl,
            CreatedAt = createdAt ?? now,
            UpdatedAt = updatedAt ?? now
        };
    }

    [Fact]
    public async Task CreateAsync_ReturnsError_WhenCreatorNotFound()
    {

        var dto = new VideoCreateDto
        {
            CreatorId = "c1",
            Title = "t",
            Description = "d",
            Url = "https://example.com/v.mp4",
            ThumbnailUrl = "https://example.com/t.jpg"
        };

        _userRepo.Setup(r => r.GetByIdAsync(dto.CreatorId))
            .ReturnsAsync((User?)null);

        var result = await _service.CreateAsync(dto);

        Assert.True(result.IsError);
        Assert.Equal(ErrorType.NotFound, result.FirstError.Type);

        _userRepo.Verify(r => r.GetByIdAsync(dto.CreatorId), Times.Once);
        _userRepo.VerifyNoOtherCalls();
        _videoRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_Success_CreatesVideoAndReturnsDto()
    {

        var dto = new VideoCreateDto
        {
            CreatorId = "c1",
            Title = "Title",
            Description = "Desc",
            Url = "https://example.com/v.mp4",
            ThumbnailUrl = "https://example.com/t.jpg"
        };

        _userRepo.Setup(r => r.GetByIdAsync(dto.CreatorId))
            .ReturnsAsync(CreateUser(dto.CreatorId));

        Video? created = null;
        _videoRepo.Setup(r => r.CreateAsync(It.IsAny<Video>()))
            .Callback<Video>(v => 
            {
                created = v;
                var now = DateTime.UtcNow;
                v.CreatedAt = now;
                v.UpdatedAt = now;
            })
            .Returns(Task.CompletedTask);

        var before = DateTime.UtcNow;

        var result = await _service.CreateAsync(dto);

        Assert.False(result.IsError);
        var video = result.Value;
        Assert.NotNull(video);
        Assert.NotNull(created);

        Assert.Equal(dto.CreatorId, created!.CreatorId);
        Assert.Equal(dto.Title, created.Title);
        Assert.Equal(dto.Description, created.Description);
        Assert.Equal(dto.Url, created.Url);
        Assert.Equal(dto.ThumbnailUrl, created.ThumbnailUrl);

        Assert.Equal(created.CreatorId, video!.Creator.Id);
        Assert.Equal(created.Title, video.Title);
        Assert.Equal(created.Description, video.Description);
        Assert.Equal(created.Url, video.Url);
        Assert.Equal(created.ThumbnailUrl, video.ThumbnailUrl);
        Assert.True(video.CreatedAt >= before);
        Assert.True(video.UpdatedAt >= video.CreatedAt);

        _userRepo.Verify(r => r.GetByIdAsync(dto.CreatorId), Times.Once);
        _videoRepo.Verify(r => r.CreateAsync(It.IsAny<Video>()), Times.Once);
        _userRepo.VerifyNoOtherCalls();
        _videoRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedList()
    {

        var list = new List<Video>
        {
            CreateVideo(1, "c1", "t1", "d1", "https://e.com/1", "https://e.com/1.jpg"),
            CreateVideo(2, "c2", "t2", "d2", "https://e.com/2", "https://e.com/2.jpg")
        };
        _videoRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, v => v.Id == 1 && v.Title == "t1");
        Assert.Contains(result, v => v.Id == 2 && v.Title == "t2");

        _videoRepo.Verify(r => r.GetAllAsync(), Times.Once);
        _videoRepo.VerifyNoOtherCalls();
        _userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {

        _videoRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync((Video?)null);

        var result = await _service.GetByIdAsync(10);

        Assert.Null(result);
        _videoRepo.Verify(r => r.GetByIdAsync(10), Times.Once);
        _videoRepo.VerifyNoOtherCalls();
        _userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenFound()
    {

        var video = CreateVideo(7, "c7", "t7", "d7", "https://e.com/7", "https://e.com/7.jpg");
        _videoRepo.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(video);

        var dto = await _service.GetByIdAsync(7);

        Assert.NotNull(dto);
        Assert.Equal(video.Id, dto!.Id);
        Assert.Equal(video.Title, dto.Title);
        Assert.Equal(video.Url, dto.Url);

        _videoRepo.Verify(r => r.GetByIdAsync(7), Times.Once);
        _videoRepo.VerifyNoOtherCalls();
        _userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateAsync_ReturnsError_WhenVideoNotFound()
    {

        var updateDto = new VideoUpdateDto { Id = 5, Title = "new" };
        _videoRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((Video?)null);

        var result = await _service.UpdateAsync(updateDto);

        Assert.True(result.IsError);
        Assert.Equal(ErrorType.NotFound, result.FirstError.Type);

        _videoRepo.Verify(r => r.GetByIdAsync(5), Times.Once);
        _videoRepo.VerifyNoOtherCalls();
        _userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateAsync_ReturnsError_WhenNewCreatorNotFound()
    {

        var existing = CreateVideo(9, "c-old", "t", "d", "https://e.com/v", "https://e.com/t.jpg");
        _videoRepo.Setup(r => r.GetByIdAsync(9)).ReturnsAsync(existing);

        _userRepo.Setup(r => r.GetByIdAsync("c-new")).ReturnsAsync((User?)null);

        var updateDto = new VideoUpdateDto { Id = 9, CreatorId = "c-new" };
        var result = await _service.UpdateAsync(updateDto);

        Assert.True(result.IsError);
        Assert.Equal(ErrorType.NotFound, result.FirstError.Type);

        _videoRepo.Verify(r => r.GetByIdAsync(9), Times.Once);
        _userRepo.Verify(r => r.GetByIdAsync("c-new"), Times.Once);
        _videoRepo.VerifyNoOtherCalls();
        _userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateAsync_UpdatesFields_WhenProvided()
    {

        var existing = CreateVideo(12, "c1", "t1", "d1", "https://e.com/1", "https://e.com/1.jpg", DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(-1));
        _videoRepo.Setup(r => r.GetByIdAsync(12)).ReturnsAsync(existing);

        _userRepo.Setup(r => r.GetByIdAsync("c2"))
            .ReturnsAsync(CreateUser("c2"));

        Video? saved = null;
        _videoRepo.Setup(r => r.UpdateAsync(It.IsAny<Video>()))
            .Callback<Video>(v => 
            {
                saved = v;
                v.UpdatedAt = DateTime.UtcNow;
            })
            .Returns(Task.CompletedTask);

        var update = new VideoUpdateDto
        {
            Id = 12,
            CreatorId = "c2",
            Title = "t2",
            Description = "d2",
            Url = "https://e.com/2",
            ThumbnailUrl = "https://e.com/2.jpg"
        };

        var result = await _service.UpdateAsync(update);

        Assert.False(result.IsError);
        var dto = result.Value;
        Assert.NotNull(dto);
        Assert.NotNull(saved);

        Assert.Equal("c2", saved!.CreatorId);
        Assert.Equal("t2", saved.Title);
        Assert.Equal("d2", saved.Description);
        Assert.Equal("https://e.com/2", saved.Url);
        Assert.Equal("https://e.com/2.jpg", saved.ThumbnailUrl);
        Assert.True(saved.UpdatedAt >= saved.CreatedAt);

        Assert.Equal(saved.CreatorId, dto!.Creator.Id);
        Assert.Equal(saved.Title, dto.Title);
        Assert.Equal(saved.Description, dto.Description);
        Assert.Equal(saved.Url, dto.Url);
        Assert.Equal(saved.ThumbnailUrl, dto.ThumbnailUrl);

        _videoRepo.Verify(r => r.GetByIdAsync(12), Times.Once);
        _userRepo.Verify(r => r.GetByIdAsync("c2"), Times.Once);
        _videoRepo.Verify(r => r.UpdateAsync(It.IsAny<Video>()), Times.Once);
        _videoRepo.VerifyNoOtherCalls();
        _userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteAsync_ReturnsError_WhenVideoNotFound()
    {

        _videoRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync((Video?)null);

        var result = await _service.DeleteAsync(3);

        Assert.True(result.IsError);
        Assert.Equal(ErrorType.NotFound, result.FirstError.Type);

        _videoRepo.Verify(r => r.GetByIdAsync(3), Times.Once);
        _videoRepo.VerifyNoOtherCalls();
        _userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteAsync_Success_DeletesVideo()
    {

        var existing = CreateVideo(13, "c13", "t", "d", "https://e.com/v", "https://e.com/t.jpg");
        _videoRepo.Setup(r => r.GetByIdAsync(13)).ReturnsAsync(existing);
        _videoRepo.Setup(r => r.DeleteAsync(existing)).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(13);

        Assert.False(result.IsError);

        _videoRepo.Verify(r => r.GetByIdAsync(13), Times.Once);
        _videoRepo.Verify(r => r.DeleteAsync(existing), Times.Once);
        _videoRepo.VerifyNoOtherCalls();
        _userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByFilterAsync_ReturnsMappedList()
    {

        var filter = new VideoFilterDto { Title = "hello" };
        var videos = new List<Video>
        {
            CreateVideo(21, "c1", "hello 1", "d", "https://e.com/1", "https://e.com/1.jpg"),
            CreateVideo(22, "c2", "hello 2", "d", "https://e.com/2", "https://e.com/2.jpg")
        };
        _videoRepo.Setup(r => r.GetByFilterAsync(filter)).ReturnsAsync(videos);

        var result = await _service.GetByFilterAsync(filter);

        Assert.Equal(2, result.Count);
        Assert.All(result, v => Assert.Contains("hello", v.Title, StringComparison.OrdinalIgnoreCase));

        _videoRepo.Verify(r => r.GetByFilterAsync(filter), Times.Once);
        _videoRepo.VerifyNoOtherCalls();
        _userRepo.VerifyNoOtherCalls();
    }
}

