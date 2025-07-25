﻿using System.ComponentModel.DataAnnotations;

namespace SegmentUsers.Application.DTOs;

public class CreateSegmentDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public List<Guid>? VkUserIds { get; set; }
}