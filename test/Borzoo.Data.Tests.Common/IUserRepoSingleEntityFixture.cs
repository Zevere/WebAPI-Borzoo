using Borzoo.Data.Abstractions.Entities;

namespace Borzoo.Data.Tests.Common
{
    public interface IUserRepoSingleEntityFixture
    {
        User NewUser { get; set; }
    }
}