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
                User.Create("deneme@deneme.com", "Deneme", "Deneme", "passwordhash")
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

                org.SetEmail(user.Email);
                org.SetFirstName(user.FirstName);
                org.SetLastName(user.LastName);
            });

            return mockRepo;
        }
    }
}
