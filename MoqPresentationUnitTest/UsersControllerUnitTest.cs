using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MoqPresentation.Controllers;
using MoqPresentation.DataContracts;
using MoqPresentation.Model;
using MoqPresentation.Repositories;
using Nest;

namespace MoqPresentationUnitTest
{
    [TestClass]
    public class UsersControllerUnitTest
    {
        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly ElasticsearchConfiguration Configuration = new ElasticsearchConfiguration { Address = "http://localhost:9200", IndexName = "users", TimeOutInSeconds = 60 };

        /// <summary>
        /// Creates the repository.
        /// </summary>
        /// <returns>The repository.</returns>
        /// <param name="client">Client.</param>
        private UserElasticsearchRepository CreateRepository(IElasticClient client)
        {
            UserElasticsearchRepository repository = new UserElasticsearchRepository(client);
            repository.Configuration = Configuration;
            return repository;
        }

        #region Get

        /// <summary>
        /// Must returns http code 200, failed false, error null and the object in Data property.
        /// </summary>
        [TestMethod]
        public void Get_when_user_exists()
        {
            // Setup mock response.
            User expectedResponseUser = new User { Id = 1, Name = "User" };
            Mock<IGetResponse<User>> mockResponse = new Mock<IGetResponse<User>>();

            // Setup readonly properties.
            mockResponse.SetupGet(r => r.Found).Returns(true);
            mockResponse.SetupGet(r => r.Source).Returns(expectedResponseUser);

            // Setup mock get behavior.
            Mock<IElasticClient> mockClient = new Mock<IElasticClient>();
            mockClient.Setup(client => client.Get<User>(It.IsAny<GetRequest>())).Returns(mockResponse.Object);

            // Initializing repository.
            UserElasticsearchRepository repository = CreateRepository(mockClient.Object);

            // Setup Logger Mock
            Mock<ILogger<UsersController>> mockLogger = new Mock<ILogger<UsersController>>();

            // Initializing the controller
            UsersController controller = new UsersController(null, mockLogger.Object);
            controller.Repository = repository;

            // Calling get.
            ObjectResult result = controller.Get(1) as ObjectResult;
            ServiceResponse response = result.Value as ServiceResponse;

            // Assertions.
            mockClient.Verify(c => c.Get<User>(It.IsAny<GetRequest>()), Times.Once());
            mockLogger.Verify(l => l.Log(It.IsAny<Microsoft.Extensions.Logging.LogLevel>(), It.IsAny<EventId>(), It.IsAny<string>(), It.IsAny<Exception>(), It.IsNotNull<Func<string, Exception, string>>()), Times.Never());
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsFalse(response.Failed);
            Assert.IsNull(response.Error);
            Assert.AreEqual(1, (response.Data as User).Id);
            Assert.AreEqual("User", (response.Data as User).Name);
        }

        /// <summary>
        /// Must returns http code 404, failed true, error null and the object in Data null.
        /// </summary>
        [TestMethod]
        public void Get_when_user_not_exists()
        {
            // Setup mock response.
            Mock<IGetResponse<User>> mockResponse = new Mock<IGetResponse<User>>();

            // Setup readonly properties.
            mockResponse.SetupGet(r => r.Found).Returns(false);

            // Setup mock get behavior.
            Mock<IElasticClient> mockClient = new Mock<IElasticClient>();
            mockClient.Setup(client => client.Get<User>(It.IsAny<GetRequest>())).Returns(mockResponse.Object);

            // Initializing repository.
            UserElasticsearchRepository repository = CreateRepository(mockClient.Object);

            // Setup Logger Mock
            Mock<ILogger<UsersController>> mockLogger = new Mock<ILogger<UsersController>>();

            // Initializing controller.
            UsersController controller = new UsersController(null, mockLogger.Object);
            controller.Repository = repository;

            // Calling get.
            ObjectResult result = controller.Get(1) as ObjectResult;
            ServiceResponse response = result.Value as ServiceResponse;

            // Assertions.
            mockClient.Verify(c => c.Get<User>(It.IsAny<GetRequest>()), Times.Once());
            mockLogger.Verify(l => l.Log(Microsoft.Extensions.Logging.LogLevel.Information, It.IsAny<EventId>(), "Not found.", null, It.IsNotNull<Func<string, Exception, string>>()));
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsTrue(response.Failed);
            Assert.IsNull(response.Error);
            Assert.IsNull(response.Data);
        }

        /// <summary>
        /// Must returns http code 500, failed true, error wich raised exception and the object in Data null.
        /// </summary>
        [TestMethod]
        public void Get_when_repository_threw_a_timeout_exception()
        {
            // Setup mock response.
            Mock<IGetResponse<User>> mockResponse = new Mock<IGetResponse<User>>();

            // Setup readonly properties.
            mockResponse.SetupGet(r => r.Found).Returns(false);

            // Setup mock get behavior.
            Mock<IElasticClient> mockClient = new Mock<IElasticClient>();
            mockClient.Setup(client => client.Get<User>(It.IsAny<GetRequest>())).Throws(new TimeoutException());

            // Initializing repository.
            UserElasticsearchRepository repository = CreateRepository(mockClient.Object);

            // Setup Logger Mock
            Mock<ILogger<UsersController>> mockLogger = new Mock<ILogger<UsersController>>();

            // Initilizing controller.
            UsersController controller = new UsersController(null,mockLogger.Object);
            controller.Repository = repository;

            // Calling get.
            ObjectResult result = controller.Get(1) as ObjectResult;
            ServiceResponse response = result.Value as ServiceResponse;

            // Assertions.
            mockClient.Verify(c => c.Get<User>(It.IsAny<GetRequest>()), Times.Once());
            mockLogger.Verify(l => l.Log(Microsoft.Extensions.Logging.LogLevel.Error, It.IsAny<EventId>(), "Server error.", It.IsAny<Exception>(), It.IsNotNull<Func<string,Exception,string>>()));
            Assert.AreEqual(500, result.StatusCode);
            Assert.IsTrue(response.Failed);
            Assert.IsNotNull(response.Error);
            Assert.IsNull(response.Data);
            Assert.IsInstanceOfType(response.Error, typeof(TimeoutException));
        }

        #endregion

    }
}
