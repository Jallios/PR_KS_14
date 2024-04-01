using System;
using System.Collections.Generic;

namespace api.Models;

public partial class ProjectUserRole
{
    public long? Id { get; set; }

    public long? UserId { get; set; }

    public sbyte? RoleId { get; set; }

    public long? ProjectId { get; set; }

    public Project? Project { get; set; } = null!;

    public Role? Role { get; set; } = null!;

    public User? User { get; set; }
}
