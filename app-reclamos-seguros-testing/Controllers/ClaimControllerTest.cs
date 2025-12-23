namespace app_reclamos_seguros_testing.Controllers
{
    using app_reclamos_seguros.Controllers;
    using app_reclamos_seguros.Model;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Newtonsoft.Json.Bson;
    using Xunit;

    public class ClaimControllerTest
    {
        private readonly ClaimController controller;

        private readonly VehicleClaim mockClaim = new VehicleClaim(
            claimNumber: 4153,
            description:  "some description",
            direction:  "myplace",
            city:  "monte",
            dateAndHour:  DateTime.Parse("2025-11-04T18:32:00-03:00"),
            clientDNI:  43394080,
            clientName:  "tomas",
            clientSurname:  "pedro",
            phoneNumber:  227141234,
            email:  "me@email.com",
            policyNumber:  1234,
            companyName:  "digna",
            coverage:  "TD3",
            vehicleBrand:  "VW",
            vehicleModel:  "Corsa",
            licensePlate:  "aaa111",
            registeredOwner:  "me",
            archived: false
        );
        private readonly VehicleClaimDTO mockDTO = new VehicleClaimDTO
        {
            ClaimNumber = 4153,
            Description = "some description",
            Direction = "myplace",
            City = "monte",
            DateAndHour = DateTime.Parse("2025-11-04T18:32:00-03:00"),
            ClientDNI = 43394080,
            ClientName = "tomas",
            ClientSurname = "pedro",
            PhoneNumber = 227141234,
            Email = "me@email.com",
            PolicyNumber = 1234,
            CompanyName = "digna",
            Coverage = "TD3",
            VehicleBrand = "VW",
            VehicleModel = "Corsa",
            LicensePlate = "aaa111",
            RegisteredOwner = "me",
            Archived = false
        };

        Mock<ILogger<ClaimController>> mockLogger = new Mock<ILogger<ClaimController>>();
        Mock<IClaimsRepository> mockRepo = new Mock<IClaimsRepository>();

        public ClaimControllerTest()
        {
            controller = new ClaimController(mockLogger.Object, mockRepo.Object);
        }

        #region AddNewCarClaim
        [Fact]
        public void ValidVehicleClaimDTOReturnsOk()
        {
            IActionResult result = controller.AddNewCarClaim(mockDTO);

            mockRepo.Verify(
                repo => repo.SetNewClaim(It.IsAny<VehicleClaim>()),
                Times.Once
            );

            OkObjectResult okObject = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<string>(okObject.Value);
        }

        [Fact]
        public void InvalidVehicleClaimDTOReturnsBadRequest()
        {
            controller.ModelState.AddModelError("key", "error message");
            IActionResult result = controller.AddNewCarClaim(mockDTO);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ClaimAlreadyExistsCausesDatabaseExceptionReturnsBadRequest()
        {
            const int mockClaimNum = 122300;
            mockRepo
                .Setup(service => service.SetNewClaim(It.Is<VehicleClaim>(claim => claim.ClaimNumber == mockClaimNum)))
                .Throws<DatabaseException>();

            mockDTO.ClaimNumber = mockClaimNum;
            IActionResult result = controller.AddNewCarClaim(mockDTO);
            Assert.IsType<BadRequestObjectResult>(result);
        }
        #endregion

        #region GetClaimByID
        [Fact]
        public void GetClaimByIDExistingReturnsOk()
        {
            const int claimNumber = 4153;
            mockRepo
                .Setup(service => service.GetByID(It.Is<int>(claimNum => claimNum == claimNumber)))
                .Returns(mockClaim);

            var result = controller.GetClaimByID(claimNumber);

            OkObjectResult okObject = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<VehicleClaimDTO>(okObject.Value);
        }

        [Fact]
        public void GetClaimByIDNonexistentReturnsBadRequest()
        {

            var result = controller.GetClaimByID(1111);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void GetClaimByIDDatabaseErrorReturnsBadRequest()
        {
            mockRepo
                .Setup(service => service.GetByID(It.Is<int>(claimNum => claimNum == 9999)))
                .Throws<DatabaseException>();

            var result = controller.GetClaimByID(9999);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
        #endregion

        #region GetAllClaims - All - Active - Archived
        #region HelperSearchResult
        private ClaimSearchResult SearchResultWithCount(int count)
        {
            var list = new List<ClaimSearchItem>();

            for (int i = 0; i < count; i++)
            {
                list.Add(new ClaimSearchItem(
                    claimNumber: 1000 + i,
                    name: "Test",
                    surname: "User",
                    dateAndHour: DateTime.Now,
                    archived: false
                ));
            }

            return new ClaimSearchResult(list);
        }
        #endregion

        #region GetAllActiveClaims
        [Fact]
        public void GetAllClaimsActiveReturnsOkWithClaims()
        {
            mockRepo
                .Setup(repo => repo.GetClaimsList(false))
                .Returns(SearchResultWithCount(2));

            var result = controller.GetAllClaimsActive();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<ClaimSearchResultDTO>(ok.Value);

            Assert.Equal(2, dto.ResultsList.Count);
        }

        [Fact]
        public void GetAllClaimsActiveReturnsOkWithEmptyList()
        {
            mockRepo
                .Setup(repo => repo.GetClaimsList(false))
                .Returns(SearchResultWithCount(0));

            var result = controller.GetAllClaimsActive();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<ClaimSearchResultDTO>(ok.Value);

            Assert.Empty(dto.ResultsList);
        }

        [Fact]
        public void GetAllClaimsActiveReturnsBadRequestOnException()
        {
            mockRepo
                .Setup(repo => repo.GetClaimsList(false))
                .Throws(new DatabaseException("DB error", new Exception()));

            var result = controller.GetAllClaimsActive();

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
        #endregion

        #region GetAllArchivedClaims
        [Fact]
        public void GetAllClaimsArchivedReturnsOkWithClaims()
        {
            mockRepo
                .Setup(repo => repo.GetClaimsList(true))
                .Returns(SearchResultWithCount(1));

            var result = controller.GetAllClaimsArchived();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<ClaimSearchResultDTO>(ok.Value);

            Assert.Single(dto.ResultsList);
        }

        [Fact]
        public void GetAllClaimsArchivedReturnsBadRequestOnException()
        {
            mockRepo
                .Setup(repo => repo.GetClaimsList(true))
                .Throws(new DatabaseException("DB error", new Exception()));

            var result = controller.GetAllClaimsArchived();

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
        #endregion

        #region GetAllClaims
        [Fact]
        public void GetAllClaimsReturnsOkWithCombinedClaims()
        {
            mockRepo
                .Setup(repo => repo.GetClaimsList(false))
                .Returns(SearchResultWithCount(2));

            mockRepo
                .Setup(repo => repo.GetClaimsList(true))
                .Returns(SearchResultWithCount(3));

            var result = controller.GetAllClaims();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<ClaimSearchResultDTO>(ok.Value);

            Assert.Equal(5, dto.ResultsList.Count);
        }

        [Fact]
        public void GetAllClaimsReturnsBadRequestOnException()
        {
            mockRepo
                .Setup(repo => repo.GetClaimsList(It.IsAny<bool>()))
                .Throws(new DatabaseException("DB error", new Exception()));

            var result = controller.GetAllClaims();

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
        #endregion
        #endregion

        #region ArchiveClaim

        [Fact]
        public void ArchiveClaimReturnsOkAndCallsSetArchivedOnce()
        {
            int claimNum = 4153;
            mockRepo.Setup(repo => repo.SetArchived(claimNum, true));

            var result = controller.ArchiveClaim(claimNum);
            var ok = Assert.IsType<OkObjectResult>(result);

            mockRepo.Verify(repo => repo.SetArchived(claimNum, true), Times.Once);
        }

        [Fact]
        public void ArchiveClaimReturnsBadRequestOnDatabaseException()
        {
            int claimNum = 4153;
            mockRepo
                .Setup(repo => repo.SetArchived(claimNum, true))
                .Throws(new DatabaseException("DB error", new Exception()));

            var result = controller.ArchiveClaim(claimNum);

            Assert.IsType<BadRequestObjectResult>(result);
            mockRepo.Verify(repo => repo.SetArchived(claimNum, true), Times.Once);
        }

        #endregion
    }
}
