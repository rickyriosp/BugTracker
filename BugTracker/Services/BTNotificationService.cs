﻿using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTNotificationService : IBTNotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IBTRolesService _rolesService;

        public BTNotificationService(ApplicationDbContext contex, IEmailSender emailSender, IBTRolesService rolesService)
        {
            _context = contex;
            _emailSender = emailSender;
            _rolesService = rolesService;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            try
            {
                await _context.AddAsync(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Adding Notification. ---> {ex.Message}");
                throw;
            }
        }

        public async Task<List<Notification>> GetReceivedNotificationsAsync(string userId)
        {
            try
            {
                List<Notification> notifications = await _context.Notifications
                    .Include(n => n.Recipient)
                    .Include(n => n.Sender)
                    .Include(n => n.Ticket)
                        .ThenInclude(t => t.Project)
                    .AsSplitQuery()
                    .Where(n => n.RecipientId == userId).ToListAsync();

                return notifications;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Retrieving Received Notifications. ---> {ex.Message}");
                throw;
            }
        }

        public async Task<List<Notification>> GetSentNotificationsAsync(string userId)
        {
            try
            {
                List<Notification> notifications = await _context.Notifications
                    .Include(n => n.Recipient)
                    .Include(n => n.Sender)
                    .Include(n => n.Ticket)
                        .ThenInclude(t => t.Project)
                    .AsSplitQuery()
                    .Where(n => n.SenderId == userId).ToListAsync();

                return notifications;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Retrieving Sent Notifications. ---> {ex.Message}");
                throw;
            }
        }

        public async Task<bool> SendEmailNotificationAsync(Notification notification, string emailSubject)
        {
            BTUser btUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == notification.RecipientId);

            if (btUser == null)
            {
                return false;
            }

            string btUserEmail = btUser.Email;
            string message = notification.Message;

            // Send Email
            try
            {
                await _emailSender.SendEmailAsync(btUserEmail, emailSubject, message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Sending Email Notification. ---> {ex.Message}");
                throw;
            }
        }

        public async Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role)
        {
            try
            {
                List<BTUser> members = await _rolesService.GetUsersInRoleAsync(role, companyId);

                foreach (BTUser btUser in members)
                {
                    notification.RecipientId = btUser.Id;
                    await SendEmailNotificationAsync(notification, notification.Title);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Sending Email Notification By Role. ---> {ex.Message}");
                throw;
            }
        }

        public async Task SendMembersEmailNotificationsAsync(Notification notification, List<BTUser> members)
        {
            try
            {
                foreach (BTUser btUser in members)
                {
                    notification.RecipientId = btUser.Id;
                    await SendEmailNotificationAsync(notification, notification.Title);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Sending Members Email Notification. ---> {ex.Message}");
                throw;
            }
        }
    }
}
