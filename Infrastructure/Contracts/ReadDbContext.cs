using Domain.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Contracts
{
    public class ReadDbContext : DbContext
    {
        private readonly IMongoDatabase _database;

        public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options) { }

        public IMongoCollection<UserReadDto> User => _database.GetCollection<UserReadDto>("User");
    }

}
