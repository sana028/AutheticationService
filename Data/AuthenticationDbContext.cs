using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using NetCoreIntermediate.Models;

namespace NetCoreIntermediate.DbContextService
{
    public class AuthenticationDbContext: DbContext
    {
        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> contextOptions) : base(contextOptions) { }

        public DbSet<SignUpUser> SignUpUsers { get; set; }

        public DbSet<AdminInfo> AdminInfo { get; set; }
        public DbSet<OtpInfos> OtpInfos { get; set; }

        public DbSet<TemporaryUser> TemporaryUser { get; set; }

    }
}
