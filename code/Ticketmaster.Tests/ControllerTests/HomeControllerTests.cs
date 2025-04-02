using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Ticketmaster.Controllers;
using Ticketmaster.Models;
using Xunit;

namespace Ticketmaster.Tests.ControllerTests
{
    public class HomeControllerTests
    {
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            var loggerMock = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(loggerMock.Object);
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            var result = _controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            var result = _controller.Privacy();
            Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public void AccessDenied_ReturnsViewResult()
        {
            var result = _controller.AccessDenied();
            Assert.IsType<ViewResult>(result);
        }
    }
}