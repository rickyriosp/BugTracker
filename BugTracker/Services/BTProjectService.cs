using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;

        public BTProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        // CRUD - Create
        public async Task AddNewProjectAsync(Project project)
        {
            _context.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            BTUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                if (!await IsUserOnProjectAsync(userId, projectId))
                {
                    try
                    {
                        project.Members.Add(user);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            return false;
        }

        // CRUD - Archive (Delete)
        public async Task ArchiveProjectAsync(Project project)
        {
            project.Archived = true;

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task<List<BTUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Project>> GetAllProjectsByCompanyAsync(int companyId)
        {
            List<Project> projects = await _context.Projects
                .Where(p => p.CompanyId == companyId && p.Archived == false)
                .Include(p => p.ProjectPriority)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.TicketType)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.TicketPriority)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.TicketStatus)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.OwnerUser)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.DeveloperUser)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Comments)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Attachments)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.History)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Notifications)
                .Include(p => p.Members)
                .AsSplitQuery()
                .ToListAsync();

            return projects;
        }

        public async Task<List<Project>> GetAllProjectsByPriorityAsync(int companyId, string priorityName)
        {
            List<Project> projects = await GetAllProjectsByCompanyAsync(companyId);
            int priorityId = await LookupProjectPriorityIdAsync(priorityName);

            return projects.Where(p => p.ProjectPriority.Id == priorityId).ToList();
        }

        public async Task<List<Project>> GetArchivedProjectsByCompanyAsync(int companyId)
        {
            List<Project> projects = await GetAllProjectsByCompanyAsync(companyId);

            return projects.Where(p => p.Archived == true).ToList();
        }

        public async Task<List<BTUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        // CRUD - Read
        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            Project? project = await _context.Projects
                .Include(p => p.Tickets)
                .Include(p => p.Members)
                .Include(p => p.ProjectPriority)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);

            return project!;
        }

        public async Task<BTUser> GetProjectManagerAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BTUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {
                List<Project> userProjects = (await _context.Users
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Company)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Members)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.DeveloperUser)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.OwnerUser)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.TicketPriority)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.TicketStatus)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.TicketType)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(u => u.Id == userId))!
                    .Projects.ToList()!;

                return userProjects;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Getting User Projects. ---> {ex.Message}");
                throw;
            }
        }

        public async Task<List<BTUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            Project? project = await _context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            bool result = false;

            if (project != null)
            {
                result = project.Members.Any(m => m.Id == userId);
            }
            
            return result;
        }

        public async Task<int> LookupProjectPriorityIdAsync(string priorityName)
        {
            int priorityId = (await _context.ProjectPriorities.FirstOrDefaultAsync(p => p.Name == priorityName)).Id;

            return priorityId;
        }

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            try
            {
                BTUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                try
                {
                    if (await IsUserOnProjectAsync(userId, projectId))
                    {
                        project.Members.Remove(user);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Removing User from project. ---> {ex.Message}");
            }
        }

        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            throw new NotImplementedException();
        }

        // CRUD - Update
        public async Task UpdateProjectAsync(Project project)
        {
            _context.Update(project);
            await _context.SaveChangesAsync();
        }
    }
}
