﻿namespace SegmentUsers.UI.DTOs;

public class ValidationProblemDetailsResponse
{
    public string Type { get; set; }
    public string Title { get; set; }
    public int Status { get; set; }
    public Dictionary<string, string[]> Errors { get; set; }
    public string TraceId { get; set; }
}