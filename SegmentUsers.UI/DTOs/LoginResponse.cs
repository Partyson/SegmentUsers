﻿namespace SegmentUsers.UI.DTOs;

public class LoginResponse 
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}