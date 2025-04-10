﻿using System;

namespace AMS.Models
{
    public class Comment : EntityBase
    {
        public Int64 Id { get; set; }
        public Int64 AssetId { get; set; }
        public string Message { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAdmin { get; set; }
    }
}
