using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Task
{
    public long? Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? Publication { get; set; }

    public DateTime? Deadline { get; set; }

    public long? CreatorId { get; set; }

    public long? ExecutorId { get; set; }

    public long? ProjectId { get; set; }

    public sbyte? PriorityId { get; set; }

    public sbyte? StatusId { get; set; }

    public User? Creator { get; set; }

    public User? Executor { get; set; }

    public Priority? Priority { get; set; } = null!;

    public Project? Project { get; set; } = null!;

    public Status? Status { get; set; } = null!;
}
