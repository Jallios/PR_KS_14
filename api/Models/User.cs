using System;
using System.Collections.Generic;

namespace api.Models;

public partial class User
{
    public long? Id { get; set; }

    public string? Email { get; set; } = null!;

    public string? Password { get; set; } = null!;

    public string? Salt { get; set; } = null!;

    public bool? IsConfrim { get; set; }

}
