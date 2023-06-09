﻿using BL.DTO;
using BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public IActionResult GetAllNotifications()
        {
            try
            {
                return Ok(_notificationService.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetNotification(int id)
        {
            try
            {
                var foundNotification = _notificationService.Get(id);

                if (foundNotification == null) return NotFound();

                return Ok(foundNotification);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("[action]")]
        public IActionResult GetNumberOfUnsentNotifications()
        {
            try
            {
                return Ok(_notificationService.GetNumberOfUnsent());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CreateNotification([FromBody] NotificationDto notification)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                return Ok(_notificationService.Create(notification));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateNotification(int id, [FromBody] NotificationDto notification)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updatedNotification = _notificationService.Update(id, notification);

                if (updatedNotification == null) return NotFound();

                return Ok(updatedNotification);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            } 
        }

        [HttpPost("[action]")]
        public IActionResult SendAllNotifications()
        {
            try
            {
                _notificationService.Send();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteNotification(int id)
        {
            try
            {
                var deletedNotification = _notificationService.Delete(id);

                if (deletedNotification == null) return NotFound();

                return Ok(deletedNotification);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
