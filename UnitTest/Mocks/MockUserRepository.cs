using ApplicationService.Repositories;
using Domain.Entities;
using Moq;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Mocks
{
    public static class MockUserRepository
    {
        public static Mock<IUserRepository> GetUserRepository()
        {
            var userList = new List<User>()
            {
                new User() {Id=1, Email="deneme@deneme.com", FirstName="Deneme", LastName="Deneme", IsBlocked=false, Status=StatusType.Available},
                new User() {Id=2, Email="test@test.com", FirstName="Test", LastName="Test", IsBlocked=true, Status=StatusType.NotAvailable}
            };

            var mockRepo = new Mock<IUserRepository>();

            mockRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(userList);

            mockRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int i) => userList.SingleOrDefault(a => a.Id == i));

            mockRepo.Setup(x => x.AddAsync(It.IsAny<User>())).Callback((User user) =>
            {
                userList.Add(user);
            });

            mockRepo.Setup(x => x.UpdateAsync(It.IsAny<User>())).Callback((User user) =>
            {
                var org = userList.Where(a => a.Id == user.Id).Single();

                if (org == null)
                    throw new InvalidOperationException();

                org.Email = user.Email;
                org.FirstName = user.FirstName;
                org.LastName = user.LastName;
                org.IsBlocked = user.IsBlocked;
                org.Status = user.Status;
            });

            return mockRepo;
        }
    }
}
