using app_reclamos_seguros.Controllers;
using app_reclamos_seguros.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;

public class ClaimReportControllerTest
{
    private readonly Mock<IClaimsRepository> mockRepo;
    private readonly ClaimReportController controller;

    public ClaimReportControllerTest()
    {
        mockRepo = new Mock<IClaimsRepository>();
        var mockLogger = new Mock<ILogger<ClaimController>>();

        controller = new ClaimReportController(mockLogger.Object, mockRepo.Object);
    }

    private ClaimReportEntryDTO CreateDTO(int claimNumber = 1234)
    {
        return new ClaimReportEntryDTO(
            Comment: "Test comment",
            ClaimNumber: claimNumber,
            DateAndTime: DateTime.Now
        );
    }

    private List<ClaimReportEntry> CreateEntries(int count, int claimNumber = 1234)
    {
        var list = new List<ClaimReportEntry>();

        for (int i = 0; i < count; i++)
        {
            list.Add(new ClaimReportEntry(
                comment: $"Comment {i}",
                claimNumber: claimNumber
            ));
        }

        return list;
    }

    #region AddNewEntryToClaim

    [Fact]
    public void AddNewEntryToClaimReturnsOkWhenSuccessful()
    {
        // Arrange
        var dto = CreateDTO();

        mockRepo
            .Setup(r => r.SetNewReport(It.IsAny<ClaimReportEntry>()));

        // Act
        var result = controller.AddNewEntryToClaim(dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);

        mockRepo.Verify(
            r => r.SetNewReport(It.IsAny<ClaimReportEntry>()),
            Times.Once
        );
    }

    [Fact]
    public void AddNewEntryToClaimReturnsBadRequestWhenClaimDoesNotExist()
    {
        var dto = CreateDTO();
        mockRepo
            .Setup(repo => repo.SetNewReport(It.IsAny<ClaimReportEntry>()))
            .Throws(new DatabaseException("Claim not found", new InvalidOperationException()));

        var result = controller.AddNewEntryToClaim(dto);

        Assert.IsType<BadRequestObjectResult>(result);
        mockRepo.Verify(repo => repo.SetNewReport(It.IsAny<ClaimReportEntry>()), Times.Once);
    }

    [Fact]
    public void AddNewEntryToClaimReturnsBadRequestOnDatabaseException()
    {
        var dto = CreateDTO();
        mockRepo
            .Setup(r => r.SetNewReport(It.IsAny<ClaimReportEntry>()))
            .Throws(new DatabaseException("DB error", new Exception()));

        var result = controller.AddNewEntryToClaim(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    #endregion

    #region GetAllClaimEntries

    [Fact]
    public void GetAllClaimEntriesReturnsOkWithEntries()
    {
        int claimNumber = 1234;
        mockRepo
            .Setup(repo => repo.GetAllReportsByID(claimNumber))
            .Returns(CreateEntries(2, claimNumber));

        var result = controller.GetAllClaimEntries(claimNumber);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsType<List<ClaimReportEntryDTO>>(ok.Value);

        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void GetAllClaimEntries_ReturnsOk_WithEmptyList()
    {
        int claimNumber = 1234;
        mockRepo
            .Setup(r => r.GetAllReportsByID(claimNumber))
            .Returns(new List<ClaimReportEntry>());

        var result = controller.GetAllClaimEntries(claimNumber);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsType<List<ClaimReportEntryDTO>>(ok.Value);

        Assert.Empty(list);
    }

    [Fact]
    public void GetAllClaimEntries_ReturnsBadRequest_OnDatabaseException()
    {
        int claimNumber = 1234;
        mockRepo
            .Setup(r => r.GetAllReportsByID(claimNumber))
            .Throws(new DatabaseException("DB error", new Exception()));

        var result = controller.GetAllClaimEntries(claimNumber);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    #endregion
}