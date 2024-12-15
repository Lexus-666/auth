using System;
using System.Collections.Generic;

namespace kursah_5semestr;

public partial class Session
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public string Token { get; set; } = null!;
}
