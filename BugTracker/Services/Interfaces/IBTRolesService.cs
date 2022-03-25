using BugTracker.Models;

namespace BugTracker.Services.Interfaces
{
    public interface IBTRolesService
    {
        Task<bool> AddUserToRoleAsync(BTUser user, string roleName);
        
        Task<string> GetRoleNameByIdAsync(string roleId);

        Task<IEnumerable<string>> GetUserRolesAsync(BTUser user);
        
        Task<List<BTUser>> GetUsersInRoleAsync(string roleName, int companyId);
        
        Task<List<BTUser>> GetUsersNotInRoleAsync(string roleName, int companyId);

        Task<bool> RemoveUserFromRoleAsync(BTUser user, string roleName);

        Task<bool> RemoveUserFromRolesAsync(BTUser user, IEnumerable<string> roles);

        Task<bool> IsUserInRoleAsync(BTUser user, string roleName);
    }
}
