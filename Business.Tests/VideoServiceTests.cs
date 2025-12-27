using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.DTOs;
using Business.Services;
using DAL.Models;
using Infra.DTOs;
using Infra.Repository;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Business.Tests;

public class VideoServiceTests
{
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
        var videoRepo = new Mock<IVideoRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new VideoService(videoRepo.Object, userRepo.Object);

        var dto = new VideoCreateDto
        {
            CreatorId = "c1",
            Title = "t",
            Description = "d",
            Url = "https://example.com/v.mp4",
            ThumbnailUrl = "https://example.com/t.jpg"
        };

        userRepo.Setup(r => r.GetByIdAsync(dto.CreatorId))
            .ReturnsAsync((User?)null);

        var (result, video) = await service.CreateAsync(dto);

        Assert.False(result.Succeeded);
        Assert.Null(video);
        var error = Assert.Single(result.Errors);
        Assert.Equal("CreatorNotFound", error.Code);

        userRepo.Verify(r => r.GetByIdAsync(dto.CreatorId), Times.Once);
        userRepo.VerifyNoOtherCalls();
        videoRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_Success_CreatesVideoAndReturnsDto()
    {
        var videoRepo = new Mock<IVideoRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new VideoService(videoRepo.Object, userRepo.Object);

        var dto = new VideoCreateDto
        {
            CreatorId = "c1",
            Title = "Title",
            Description = "Desc",
            Url = "https://example.com/v.mp4",
            ThumbnailUrl = "https://example.com/t.jpg"
        };

        userRepo.Setup(r => r.GetByIdAsync(dto.CreatorId))
            .ReturnsAsync(CreateUser(dto.CreatorId));

        Video? created = null;
        videoRepo.Setup(r => r.CreateAsync(It.IsAny<Video>()))
            .Callback<Video>(v => created = v)
            .Returns(Task.CompletedTask);

        var before = DateTime.UtcNow;

        var (result, video) = await service.CreateAsync(dto);

        Assert.True(result.Succeeded);
        Assert.NotNull(video);
        Assert.NotNull(created);

        Assert.Equal(dto.CreatorId, created!.CreatorId);
        Assert.Equal(dto.Title, created.Title);
        Assert.Equal(dto.Description, created.Description);
        Assert.Equal(dto.Url, created.Url);
        Assert.Equal(dto.ThumbnailUrl, created.ThumbnailUrl);

        Assert.Equal(created.CreatorId, video!.CreatorId);
        Assert.Equal(created.Title, video.Title);
        Assert.Equal(created.Description, video.Description);
        Assert.Equal(created.Url, video.Url);
        Assert.Equal(created.ThumbnailUrl, video.ThumbnailUrl);
        Assert.True(video.CreatedAt >= before);
        Assert.True(video.UpdatedAt >= video.CreatedAt);

        userRepo.Verify(r => r.GetByIdAsync(dto.CreatorId), Times.Once);
        videoRepo.Verify(r => r.CreateAsync(It.IsAny<Video>()), Times.Once);
        userRepo.VerifyNoOtherCalls();
        videoRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedList()
    {
        var videoRepo = new Mock<IVideoRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new VideoService(videoRepo.Object, userRepo.Object);

        var list = new List<Video>
        {
            CreateVideo(1, "c1", "t1", "d1", "https://e.com/1", "https://e.com/1.jpg"),
            CreateVideo(2, "c2", "t2", "d2", "https://e.com/2", "https://e.com/2.jpg")
        };
        videoRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, v => v.Id == 1 && v.Title == "t1");
        Assert.Contains(result, v => v.Id == 2 && v.Title == "t2");

        videoRepo.Verify(r => r.GetAllAsync(), Times.Once);
        videoRepo.VerifyNoOtherCalls();
        userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var videoRepo = new Mock<IVideoRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new VideoService(videoRepo.Object, userRepo.Object);

        videoRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync((Video?)null);

        var result = await service.GetByIdAsync(10);

        Assert.Null(result);
        videoRepo.Verify(r => r.GetByIdAsync(10), Times.Once);
        videoRepo.VerifyNoOtherCalls();
        userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WhenFound()
    {
        var videoRepo = new Mock<IVideoRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new VideoService(videoRepo.Object, userRepo.Object);

        var video = CreateVideo(7, "c7", "t7", "d7", "https://e.com/7", "https://e.com/7.jpg");
        videoRepo.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(video);

        var dto = await service.GetByIdAsync(7);

        Assert.NotNull(dto);
        Assert.Equal(video.Id, dto!.Id);
        Assert.Equal(video.Title, dto.Title);
        Assert.Equal(video.Url, dto.Url);

        videoRepo.Verify(r => r.GetByIdAsync(7), Times.Once);
        videoRepo.VerifyNoOtherCalls();
        userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateAsync_ReturnsError_WhenVideoNotFound()
    {
        var videoRepo = new Mock<IVideoRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new VideoService(videoRepo.Object, userRepo.Object);

        videoRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((Video?)null);

        var (result, updated) = await service.UpdateAsync(5, new VideoUpdateDto { Title = "new" });

        Assert.False(result.Succeeded);
        Assert.Null(updated);
        var error = Assert.Single(result.Errors);
        Assert.Equal("VideoNotFound", error.Code);

        videoRepo.Verify(r => r.GetByIdAsync(5), Times.Once);
        videoRepo.VerifyNoOtherCalls();
        userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateAsync_ReturnsError_WhenNewCreatorNotFound()
    {
        var videoRepo = new Mock<IVideoRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new VideoService(videoRepo.Object, userRepo.Object);

        var existing = CreateVideo(9, "c-old", "t", "d", "https://e.com/v", "https://e.com/t.jpg");
        videoRepo.Setup(r => r.GetByIdAsync(9)).ReturnsAsync(existing);

        userRepo.Setup(r => r.GetByIdAsync("c-new")).ReturnsAsync((User?)null);

        var (result, updated) = await service.UpdateAsync(9, new VideoUpdateDto { CreatorId = "c-new" });

        Assert.False(result.Succeeded);
        Assert.Null(updated);
        var error = Assert.Single(result.Errors);
        Assert.Equal("CreatorNotFound", error.Code);

        videoRepo.Verify(r => r.GetByIdAsync(9), Times.Once);
        userRepo.Verify(r => r.GetByIdAsync("c-new"), Times.Once);
        videoRepo.VerifyNoOtherCalls();
        userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateAsync_UpdatesFields_WhenProvided()
    {
        var videoRepo = new Mock<IVideoRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new VideoService(videoRepo.Object, userRepo.Object);

        var existing = CreateVideo(12, "c1", "t1", "d1", "https://e.com/1", "https://e.com/1.jpg", DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(-1));
        videoRepo.Setup(r => r.GetByIdAsync(12)).ReturnsAsync(existing);

        userRepo.Setup(r => r.GetByIdAsync("c2"))
            .ReturnsAsync(CreateUser("c2"));

        Video? saved = null;
        videoRepo.Setup(r => r.UpdateAsync(It.IsAny<Video>()))
            .Callback<Video>(v => saved = v)
            .Returns(Task.CompletedTask);

        var update = new VideoUpdateDto
        {
            CreatorId = "c2",
            Title = "t2",
            Description = "d2",
            Url = "https://e.com/2",
            ThumbnailUrl = "https://e.com/2.jpg"
        };

        var (result, dto) = await service.UpdateAsync(12, update);

        Assert.True(result.Succeeded);
        Assert.NotNull(dto);
        Assert.NotNull(saved);

        Assert.Equal("c2", saved!.CreatorId);
        Assert.Equal("t2", saved.Title);
        Assert.Equal("d2", saved.Description);
        Assert.Equal("https://e.com/2", saved.Url);
        Assert.Equal("https://e.com/2.jpg", saved.ThumbnailUrl);
        Assert.True(saved.UpdatedAt >= saved.CreatedAt);

        Assert.Equal(saved.CreatorId, dto!.CreatorId);
        Assert.Equal(saved.Title, dto.Title);
        Assert.Equal(saved.Description, dto.Description);
        Assert.Equal(saved.Url, dto.Url);
        Assert.Equal(saved.ThumbnailUrl, dto.ThumbnailUrl);

        videoRepo.Verify(r => r.GetByIdAsync(12), Times.Once);
        userRepo.Verify(r => r.GetByIdAsync("c2"), Times.Once);
        videoRepo.Verify(r => r.UpdateAsync(It.IsAny<Video>()), Times.Once);
        videoRepo.VerifyNoOtherCalls();
        userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteAsync_ReturnsError_WhenVideoNotFound()
    {
        var videoRepo = new Mock<IVideoRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new VideoService(videoRepo.Object, userRepo.Object);

        videoRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync((Video?)null);

        var result = await service.DeleteAsync(3);

        Assert.False(result.Succeeded);
        var error = Assert.Single(result.Errors);
        Assert.Equal("VideoNotFound", error.Code);

        videoRepo.Verify(r => r.GetByIdAsync(3), Times.Once);
        videoRepo.VerifyNoOtherCalls();
        userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteAsync_Success_DeletesVideo()
    {
        var videoRepo = new Mock<IVideoRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new VideoService(videoRepo.Object, userRepo.Object);

        var existing = CreateVideo(13, "c13", "t", "d", "https://e.com/v", "https://e.com/t.jpg");
        videoRepo.Setup(r => r.GetByIdAsync(13)).ReturnsAsync(existing);
        videoRepo.Setup(r => r.DeleteAsync(existing)).Returns(Task.CompletedTask);

        var result = await service.DeleteAsync(13);

        Assert.True(result.Succeeded);

        videoRepo.Verify(r => r.GetByIdAsync(13), Times.Once);
        videoRepo.Verify(r => r.DeleteAsync(existing), Times.Once);
        videoRepo.VerifyNoOtherCalls();
        userRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByFilterAsync_ReturnsMappedList()
    {
        var videoRepo = new Mock<IVideoRepository>(MockBehavior.Strict);
        var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new VideoService(videoRepo.Object, userRepo.Object);

        var filter = new VideoFilterDto { Title = "hello" };
        var videos = new List<Video>
        {
            CreateVideo(21, "c1", "hello 1", "d", "https://e.com/1", "https://e.com/1.jpg"),
            CreateVideo(22, "c2", "hello 2", "d", "https://e.com/2", "https://e.com/2.jpg")
        };
        videoRepo.Setup(r => r.GetByFilterAsync(filter)).ReturnsAsync(videos);

        var result = await service.GetByFilterAsync(filter);

        Assert.Equal(2, result.Count);
        Assert.All(result, v => Assert.Contains("hello", v.Title, StringComparison.OrdinalIgnoreCase));

        videoRepo.Verify(r => r.GetByFilterAsync(filter), Times.Once);
        videoRepo.VerifyNoOtherCalls();
        userRepo.VerifyNoOtherCalls();
    }
}

