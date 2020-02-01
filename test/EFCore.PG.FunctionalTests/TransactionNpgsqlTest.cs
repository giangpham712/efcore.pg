using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.TestUtilities;
using Xunit;

namespace Npgsql.EntityFrameworkCore.PostgreSQL
{
    public class TransactionNpgsqlTest : TransactionTestBase<TransactionNpgsqlTest.TransactionNpgsqlFixture>, IDisposable
    {
        public TransactionNpgsqlTest(TransactionNpgsqlFixture fixture)
            : base(fixture)
        {
        }

        [Fact(Skip= "Npgsql batches the inserts, creating an implicit transaction which fails the test (see https://github.com/npgsql/npgsql/issues/1307)")]
        public override void SaveChanges_can_be_used_with_no_transaction() {}

        [Fact(Skip = "Npgsql batches the inserts, creating an implicit transaction which fails the test (see https://github.com/npgsql/npgsql/issues/1307)")]
        public override Task SaveChangesAsync_can_be_used_with_no_transaction() => null;

        public void Dispose() => TestNpgsqlRetryingExecutionStrategy.Suspended = true;

        protected override DbContext CreateContextWithConnectionString()
        {
            var options = Fixture.AddOptions(
                    new DbContextOptionsBuilder()
                        .UseNpgsql(
                            TestStore.ConnectionString,
                            b => b.ApplyConfiguration().ExecutionStrategy(c => new NpgsqlExecutionStrategy(c))))
                .UseInternalServiceProvider(Fixture.ServiceProvider);

            return new DbContext(options.Options);
        }

        protected override bool AmbientTransactionsSupported => true;

        protected override bool SnapshotSupported => true;

        protected override bool DirtyReadsOccur => false;

        public class TransactionNpgsqlFixture : TransactionFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => NpgsqlTestStoreFactory.Instance;

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                new NpgsqlDbContextOptionsBuilder(
                        base.AddOptions(builder))
                    .ExecutionStrategy(c => new NpgsqlExecutionStrategy(c));
                return builder;
            }
        }
    }
}
