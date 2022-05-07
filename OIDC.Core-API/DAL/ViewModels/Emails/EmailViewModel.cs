using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace OAuthServer.DAL.ViewModels.Emails;

public abstract class EmailViewModel
{
    [Required] 
    public string Slug { get; set; }

    [Required]
    public string Subject { get; set; }

    [Required]
    public string ToName { get; set; }

    [Required]
    [EmailAddress]
    public string ToAddress { get; set; }
    
    public string FromName { get; set; }
    
    public string FromAddress { get; set; }
    
    public Dictionary<string, string> Data { get; set; }
}