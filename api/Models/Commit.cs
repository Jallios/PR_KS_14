using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Commit
{
    public long? Id { get; set; }

    public string? Description { get; set; } = null!;

    public long? UserId { get; set; }

    public long? TaskId { get; set; }

    public Task? Task { get; set; } = null!;

    public User? User { get; set; }

}
