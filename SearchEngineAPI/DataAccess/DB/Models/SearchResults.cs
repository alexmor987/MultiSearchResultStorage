using System;
using System.Collections.Generic;

namespace DataAccess.DB.Models;

public partial class SearchResults
{
    public int Id { get; set; }

    public string SearchEngine { get; set; } = null!;

    public string Title { get; set; } = null!;

    public DateTime Entereddate { get; set; }
}
