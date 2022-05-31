﻿using BugTracker.Models;

namespace BugTracker.Services.Interfaces
{
    public interface IBTTicketService
    {
        // CRUD Methods
        Task AddNewTicketAsync(Ticket ticket);
        Task<Ticket> GetTicketByIdAsync(int ticketId);
        Task UpdateTicketAsync(Ticket ticket);
        Task ArchiveTicketAsync(Ticket ticket);

        Task AddTicketCommentAsync(TicketComment ticketComment);
        Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment);
        Task AssignTicketAsync(int ticketId, string userId);
        Task<List<Ticket>> GetArchivedTicketsAsync(int companyId);
        Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId);
        Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName);
        Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName);
        Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName);
        Task<TicketAttachment> GetTicketAttachmentByIdAsync(int ticketAttachmentId);
        Task<Ticket> GetTicketAsNoTrackingAsync(int ticketId);
        Task<BTUser> GetTicketDeveloperAsync(int ticketId, int companyId);
        Task<List<Ticket>> GetTicketsByRoleAsync(string role, string userId, int companyId);
        Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId);
        Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId);
        Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId);
        Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId);
        Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId);
        Task<List<Ticket>> GetUnassignedTicketsAsync(int companyId);

        Task<int?> LookupTicketPriorityIdAsync(string priorityName);
        Task<int?> LookupTicketStatusIdAsync(string statusName);
        Task<int?> LookupTicketTypeIdAsync(string typeName);
    }
}
