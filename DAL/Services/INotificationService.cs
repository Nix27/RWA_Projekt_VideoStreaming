﻿using DAL.DTO;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Services
{
    public interface INotificationService
    {
        ICollection<NotificationDto> GetAll();
        NotificationDto? Get(int id);
        NotificationDto Create(NotificationDto notification);
        NotificationDto? Update(int id, NotificationDto notification);
        NotificationDto? Delete(int id);
        void Send();
        int GetNumberOfUnsent();
    }
}
