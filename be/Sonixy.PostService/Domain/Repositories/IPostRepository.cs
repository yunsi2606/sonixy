using Sonixy.Shared.Common;
using Sonixy.PostService.Domain.Entities;

namespace Sonixy.PostService.Domain.Repositories;

public interface IPostRepository : IRepository<Post>
{
    // Custom post queries if needed
}
