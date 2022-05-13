using BugTracker.Data;
using BugTracker.Models;

namespace BugTracker.Services.Interfaces
{
    public interface IBTProjectService
    {
        // CRUD Methods
        Task AddNewProjectAsync(Project project);
        Task<Project> GetProjectByIdAsync(int projectId, int companyId);
        Task UpdateProjectAsync(Project project);
        Task ArchiveProjectAsync(Project project);
        Task RestoreProjectAsync(Project project);

        Task<bool> AddProjectManagerAsync(string userId, int projectId);
        Task<bool> AddUserToProjectAsync(string userId, int projectId);
        Task<List<BTUser>> GetAllProjectMembersExceptPMAsync(int projectId);
        Task<List<Project>> GetAllProjectsByCompanyAsync(int companyId);
        Task<List<Project>> GetAllProjectsByPriorityAsync(int companyId, string priorityName);
        Task<List<Project>> GetArchivedProjectsByCompanyAsync(int companyId);
        Task<List<BTUser>> GetDevelopersOnProjectAsync(int projectId);
        Task<BTUser> GetProjectManagerAsync(int projectId);
        Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role);
        Task<List<BTUser>> GetSubmittersOnProjectAsync(int projectId);
        Task<List<Project>> GetUserProjectsAsync(string userId);
        Task<List<BTUser>> GetUsersNotOnProjectAsync(int projectId, int companyId);
        Task RemoveProjectManagerAsync(int projectId);
        Task RemoveUserFromProjectAsync(string userId, int projectId);
        Task RemoveUsersFromProjectByRoleAsync(string role, int projectId);

        Task<bool> IsUserOnProjectAsync(string userId, int projectId);
        Task<int> LookupProjectPriorityIdAsync(string priorityName);
    }
}
