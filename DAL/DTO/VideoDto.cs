﻿using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class VideoDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int TotalSeconds { get; set; }
        public string? StreamingURL { get; set; }
        [DisplayName("Genre")]
        public int GenreId { get; set; }
        [DisplayName("Image")]
        public int? ImageId { get; set; }
        public List<string> Tags { get; set; }
    }
}
