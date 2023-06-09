﻿using BL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Models;
using NuGet.ProjectModel;
using System.Security.Claims;
using System.Transactions;

namespace MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class VideoController : Controller
    {
        private readonly IVideoService _videoService;
        private readonly IGenreService _genreService;
        private readonly IImageService _imageService;
        private readonly ITagService _tagService;
        private readonly ILogger<VideoController> _logger;

        public VideoController(
            IVideoService videoService, 
            IGenreService genreService, 
            IImageService imageService, 
            ITagService tagService,
            ILogger<VideoController> logger)
        {
            _videoService = videoService;
            _genreService = genreService;
            _imageService = imageService;
            _tagService = tagService;
            _logger = logger;
        }

        public IActionResult AllVideos(int page, int size, string? filterBy, string? filter)
        {
            try
            {
                if (size == 0)
                    size = 5;

                var pagedVideos = _videoService.GetPagedVideos(page, size);
                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["pages"] = (int)Math.Ceiling((double)_videoService.GetNumberOfVideos() / size);

                if(filter != null && filterBy != "none")
                {
                    pagedVideos = _videoService.GetFilteredVideos(pagedVideos, filterBy, filter);
                }

                var videosVm = pagedVideos.Select(v => new VideoVM
                {
                    Video = v,
                    ImageURL = Url.Action("GetImage", new { id = v.ImageId }),
                    Genre = _genreService.Get(v.GenreId).Name,
                    TagsOfVideo = _tagService.GetAll()
                                             .Where(t => v.Tags.Contains(t.Name))
                                             .Select(t => t.Name)
                });

                return View(videosVm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to get videos");
                return RedirectToAction("Error", "Home", new { area = "Public" });
            }
        }

        public IActionResult VideoTableBodyPartial(int page, int size, string? filterBy, string? filter)
        {
            try
            {
                if (size == 0)
                    size = 5;

                var pagedVideos = _videoService.GetPagedVideos(page, size);
                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["pages"] = (int)Math.Ceiling((double)_videoService.GetNumberOfVideos() / size);

                if (filter != null && filterBy != "none")
                {
                    pagedVideos = _videoService.GetFilteredVideos(pagedVideos, filterBy, filter);
                }

                var videosVm = pagedVideos.Select(v => new VideoVM
                {
                    Video = v,
                    ImageURL = Url.Action("GetImage", new { id = v.ImageId }),
                    Genre = _genreService.Get(v.GenreId).Name,
                    TagsOfVideo = _tagService.GetAll()
                                             .Where(t => v.Tags.Contains(t.Name))
                                             .Select(t => t.Name)
                });

                return PartialView("_VideoTableBodyPartial", videosVm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to get videos");
                return RedirectToAction("Error", "Home", new { area = "Public" });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public FileContentResult GetImage(int id)
        {
            var imageContent = _imageService.Get(id).Content;
            var imageBytes = Convert.FromBase64String(imageContent);

            return new FileContentResult(imageBytes, "application/octet-stream");
        }

        public IActionResult CreateVideo()
        {
            try
            {
                VideoVM videoVM = new()
                {
                    Video = new(),
                    Genres = _genreService.GetAll().Select(g => new SelectListItem
                    {
                        Text = g.Name,
                        Value = g.Id.ToString()
                    }),
                    Tags = _tagService.GetAll().Select(t => new SelectListItem
                    {
                        Text = t.Name,
                        Value = t.Name
                    })
                };

                return View(videoVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to send data to view for create video");
                return RedirectToAction("Error", "Home", new { area = "Public" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateVideo(VideoVM newVideo)
        {
            if(!ModelState.IsValid) return View(newVideo);

            try
            {
                using(var scope = new TransactionScope())
                {
                    var createdImage = _imageService.Create(newVideo.Image);
                    newVideo.Video.ImageId = createdImage.Id;

                    _videoService.Create(newVideo.Video);

                    scope.Complete();
                }
                
                
                return RedirectToAction("AllVideos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to create video or save image");
                return RedirectToAction("Error", "Home", new { area = "Public" });
            }
        }

        public IActionResult EditVideo(int id)
        {
            if(id == 0) return NotFound();

            try
            {
                var videoForEdit = _videoService.Get(id);

                if (videoForEdit == null) return NotFound();

                VideoVM videoVM = new()
                {
                    Video = videoForEdit,
                    Genres = _genreService.GetAll().Select(g => new SelectListItem
                    {
                        Text = g.Name,
                        Value = g.Id.ToString()
                    }),
                    ImageURL = Url.Action("GetImage", new { id = videoForEdit.ImageId }),
                    Tags = _tagService.GetAll().Select(t => new SelectListItem
                    {
                        Text = t.Name,
                        Value = t.Name
                    })
                };

                return View(videoVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to send data to view for edit video");
                return RedirectToAction("Error", "Home", new { area = "Public" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditVideo(VideoVM videoForEdit)
        {
            if(!ModelState.IsValid) return View(videoForEdit);

            try
            {
                if(videoForEdit.Image != null)
                {
                    var updatedImage = _imageService.Update(videoForEdit.Video.ImageId ?? 0, videoForEdit.Image);
                }

                int id = videoForEdit.Video.Id ?? 0;

                _videoService.Update(id, videoForEdit.Video);

                return RedirectToAction("AllVideos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to edit video");
                return RedirectToAction("Error", "Home", new { area = "Public" });
            }
        }

        [HttpDelete]
        public IActionResult DeleteVideo(int id)
        {
            if(id == 0) return NotFound();

            try
            {
                var deletedVideo = _videoService.Delete(id);

                if (deletedVideo == null)
                    return Json(new { success = false, message = "Unable to delete video" });

                return Json(new { success = true, message = "Video deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to delete video");
                return RedirectToAction("Error", "Home", new { area = "Public" });
            }
        }
    }
}
