using System.Security.Claims;
using Filharmonia.Controllers;
using Filharmonia.Data;
using Filharmonia.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Filharmonia.Tests
{
    public class AdminControllerTests
    {
        [Fact]
        public void TicketsReport_ShouldReturnViewWithCorrectModel()
        {
          
            var mockEventService = new Mock<IEventService>();
            mockEventService.Setup(service => service.GetEventReport())
                .Returns(new List<EventReport>
                {
                    new EventReport { EventName = "Test Event", TicketsSold = 10 }
                });

            var controller = new AdminController(null, null, mockEventService.Object);

            
            var result = controller.TicketsReport() as ViewResult;

            Assert.NotNull(result);
            Assert.IsType<List<EventReport>>(result.Model);
        }

        [Fact]
        public async Task Index_ShouldRedirectToLoginWhenUserIsNotAuthenticated()
        {
            
            var mockUserStore = new Mock<IUserStore<IdentityUser>>();
            var mockUserManager = new Mock<UserManager<IdentityUser>>(
                mockUserStore.Object,
                null, null, null, null, null, null, null, null);

            var mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            var mockEventService = new Mock<IEventService>();

            var controller = new AdminController(mockContext.Object, mockUserManager.Object, mockEventService.Object);

            
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                           .ReturnsAsync((IdentityUser)null);

           
            var result = await controller.Index() as RedirectToActionResult;

            
            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
            Assert.Equal("Account", result.ControllerName);
        }

        [Fact]
        public void TicketsReport_ShouldReturnViewWithEmptyModel_WhenNoEvents()
        {
            
            var mockEventService = new Mock<IEventService>();
            mockEventService.Setup(service => service.GetEventReport()).Returns(new List<EventReport>());

            var controller = new AdminController(null, null, mockEventService.Object);

           
            var result = controller.TicketsReport() as ViewResult;

           
            Assert.NotNull(result);
            var model = result.Model as List<EventReport>;
            Assert.NotNull(model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task Index_ShouldLogRolesForAuthenticatedUser()
        {
            
            var mockUserManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(),
                null, null, null, null, null, null, null, null);

            var mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            var mockEventService = new Mock<IEventService>();

            var mockUser = new IdentityUser { UserName = "admin@example.com" };
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(mockUser);
            mockUserManager.Setup(um => um.GetRolesAsync(It.IsAny<IdentityUser>())).ReturnsAsync(new List<string> { "Administrator" });

            var controller = new AdminController(mockContext.Object, mockUserManager.Object, mockEventService.Object);

          
            var result = await controller.Index();

            Assert.NotNull(result);
            mockUserManager.Verify(um => um.GetRolesAsync(mockUser), Times.Once);
        }

        [Fact]
        public void TicketsReport_ShouldCallGetEventReportOnce()
        {
            
            var mockEventService = new Mock<IEventService>();
            mockEventService.Setup(service => service.GetEventReport()).Returns(new List<EventReport>());

            var controller = new AdminController(null, null, mockEventService.Object);

           
            controller.TicketsReport();

            
            mockEventService.Verify(service => service.GetEventReport(), Times.Once);
        }

        [Fact]
        public void UsersReport_ShouldReturnNonEmptyModel_WhenUsersExist()
        {
            
            var mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            var fakeUsers = new List<IdentityUser>
    {
        new IdentityUser { UserName = "user1@example.com" },
        new IdentityUser { UserName = "user2@example.com" }
    }.AsQueryable();

            var mockDbSet = new Mock<DbSet<IdentityUser>>();
            mockDbSet.As<IQueryable<IdentityUser>>().Setup(m => m.Provider).Returns(fakeUsers.Provider);
            mockDbSet.As<IQueryable<IdentityUser>>().Setup(m => m.Expression).Returns(fakeUsers.Expression);
            mockDbSet.As<IQueryable<IdentityUser>>().Setup(m => m.ElementType).Returns(fakeUsers.ElementType);
            mockDbSet.As<IQueryable<IdentityUser>>().Setup(m => m.GetEnumerator()).Returns(fakeUsers.GetEnumerator());

            mockContext.Setup(ctx => ctx.Users).Returns(mockDbSet.Object);

            var controller = new AdminController(mockContext.Object, null, null);

            
            var result = controller.UsersReport() as ViewResult;

            
            Assert.NotNull(result);
            var model = result.Model as List<IdentityUser>;
            Assert.NotNull(model);
            Assert.Equal(2, model.Count); 
        }

        [Fact]
        public void TicketsReport_ShouldReturnCorrectTicketCounts()
        {
            
            var mockEventService = new Mock<IEventService>();
            mockEventService.Setup(service => service.GetEventReport()).Returns(new List<EventReport>
    {
        new EventReport { EventName = "Event 1", TicketsSold = 100 },
        new EventReport { EventName = "Event 2", TicketsSold = 50 }
    });

            var controller = new AdminController(null, null, mockEventService.Object);

            
            var result = controller.TicketsReport() as ViewResult;

            Assert.NotNull(result);
            var model = result.Model as List<EventReport>;
            Assert.NotNull(model);
            Assert.Equal(2, model.Count); 
            Assert.Equal(100, model.First().TicketsSold); 
        }

    }
}
