using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _rolesService;

        public BTProjectService(ApplicationDbContext context, IBTRolesService rolesService)
        {
            _context = context;
            _rolesService = rolesService;
        }

        // CRUD Methods
        public async Task AddNewProjectAsync(Project project)
        {
            try
            {
                await _context.AddAsync(project);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        
        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            try
            {
                Project project = await _context.Projects
                        .Include(p => p.Tickets)
                            .ThenInclude(t => t.TicketPriority)
                        .Include(p => p.Tickets)
                            .ThenInclude(t => t.TicketStatus)
                        .Include(p => p.Tickets)
                            .ThenInclude(t => t.TicketType)
                        .Include(p => p.Tickets)
                            .ThenInclude(t => t.DeveloperUser)
                        .Include(p => p.Tickets)
                            .ThenInclude(t => t.OwnerUser)
                        .Include(p => p.Members)
                        .Include(p => p.ProjectPriority)
                        .AsSplitQuery()
                        .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);

                return project;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Retrieving Project by Id. ---> {ex.Message}");
                throw;
            }
        }
        
        public async Task UpdateProjectAsync(Project project)
        {
            try
            {
                _context.Update(project);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        
        public async Task ArchiveProjectAsync(Project project)
        {
            try
            {
                project.Archived = true;
                await UpdateProjectAsync(project);

                // Archive the Tickets for the Project
                foreach (Ticket ticket in project.Tickets)
                {
                    ticket.ArchivedByProject = true;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RestoreProjectAsync(Project project)
        {
            try
            {
                project.Archived = false;
                await UpdateProjectAsync(project);

                // Archive the Tickets for the Project
                foreach (Ticket ticket in project.Tickets)
                {
                    ticket.ArchivedByProject = false;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            BTUser currentPM = await GetProjectManagerAsync(projectId);
            
            // Remove the current PM if necessary
            if (currentPM != null)
            {
                try
                {
                    await RemoveProjectManagerAsync(projectId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"***** ERROR ***** - Error Removing current PM. ---> {ex.Message}");
                    throw;
                }
            }

            // Add the new PM
            try
            {
                await AddUserToProjectAsync(userId, projectId);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Adding the new PM. ---> {ex.Message}");
                return false;
            }
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

        public async Task<List<BTUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            List<BTUser> admins = await GetProjectMembersByRoleAsync(projectId, Roles.Admin.ToString());
            List<BTUser> developers = await GetProjectMembersByRoleAsync(projectId, Roles.Developer.ToString());
            List<BTUser> submitters = await GetProjectMembersByRoleAsync(projectId, Roles.Submitter.ToString());

            List<BTUser> teamMembers = admins.Concat(developers).Concat(submitters).ToList();
            
            return teamMembers;
        }

        public async Task<List<Project>> GetAllProjectsByCompanyAsync(int companyId)
        {
            try
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
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Project>> GetAllProjectsByPriorityAsync(int companyId, string priorityName)
        {
            List<Project> projects = await GetAllProjectsByCompanyAsync(companyId);
            int priorityId = await LookupProjectPriorityIdAsync(priorityName);

            return projects.Where(p => p.ProjectPriority.Id == priorityId).ToList();
        }

        public async Task<List<Project>> GetArchivedProjectsByCompanyAsync(int companyId)
        {
            List<Project> projects = await _context.Projects
                        .Where(p => p.CompanyId == companyId && p.Archived == true)
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

        public async Task<List<BTUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            List<BTUser> developers = await GetProjectMembersByRoleAsync(projectId, Roles.Developer.ToString());

            return developers;
        }

        public async Task<BTUser> GetProjectManagerAsync(int projectId)
        {
            try
            {
                Project? project = await _context.Projects
                        .Include(p => p.Members)
                        .FirstOrDefaultAsync(p => p.Id == projectId);

                foreach (var user in project.Members)
                {
                    if (await _rolesService.IsUserInRoleAsync(user, Roles.ProjectManager.ToString()))
                    {
                        return user;
                    }
                }

                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            try
            {
                Project? project = await _context.Projects
                        .Include(p => p.Members)
                        .FirstOrDefaultAsync(p => p.Id == projectId);

                List<BTUser> members = new();

                foreach (var user in project.Members)
                {
                    if (await _rolesService.IsUserInRoleAsync(user, role))
                    {
                        members.Add(user);
                    }
                }

                return members;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<BTUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            List<BTUser> submitters = await GetProjectMembersByRoleAsync(projectId, Roles.Submitter.ToString());

            return submitters;
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
                    .FirstOrDefaultAsync(u => u.Id == userId))
                    .Projects.ToList();

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
            try
            {
                List<BTUser> users = await _context.Users
                       .Where(u => u.Projects.All(p => p.Id != projectId)).ToListAsync();

                return users.Where(u => u.CompanyId == companyId).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            try
            {
                Project? project = await _context.Projects
                    .Include(p => p.Members)
                    .FirstOrDefaultAsync(p => p.Id == projectId);

                foreach (var user in project.Members)
                {
                    if (await _rolesService.IsUserInRoleAsync(user, Roles.ProjectManager.ToString()))
                    {
                        await RemoveUserFromProjectAsync(user.Id, projectId);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Removing project manager from project. ---> {ex.Message}");
                throw;
            }
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
                Console.WriteLine($"***** ERROR ***** - Error Removing user from project. ---> {ex.Message}");
                throw;
            }
        }

        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            try
            {
                List<BTUser> members = await GetProjectMembersByRoleAsync(projectId, role);
                Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                foreach (var user in members)
                {
                    try
                    {
                        project.Members.Remove(user);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Removing users from project. ---> {ex.Message}");
                throw;
            }
        }
        
        
        public async Task<bool> IsAssignedProjectManagerAsync(string userId, int projectId)
        {
            try
            {
                string projectManagerId = (await GetProjectManagerAsync(projectId))?.Id;
                return projectManagerId == userId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Checking Assigned Project Manager. ---> {ex.Message}");
                throw;
            }
        }

        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            try
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
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<int> LookupProjectPriorityIdAsync(string priorityName)
        {
            try
            {
                int priorityId = (await _context.ProjectPriorities.FirstOrDefaultAsync(p => p.Name == priorityName)).Id;

                return priorityId;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
