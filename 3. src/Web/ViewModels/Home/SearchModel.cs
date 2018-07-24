using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.ViewModels.Home
{
  public class SearchModel
  {
    public string Key { get; set; }
    public List<DocumentIndex> TableTitles { get; set; } = new List<DocumentIndex>();

    public List<DocumentItem> DocumentItems { get; set; } = new List<DocumentItem>();
  }

  public enum IndexType
  {
    String,
    Integer,
    Decimal,
    Date
  }

  public class DocumentIndex
  {
    public string Id { get; set; } = "0";
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; } = 0;
    public string SearchKey { get; set; }

    public IndexType Type { get; set; } = IndexType.String;
  }

  public class DocumentItem
  {
    public string Id { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public bool IsFolder { get; set; } = false;
    public bool IsRoot { get; set; } = false;

    public List<DocumentIndex> Indexs { get; set; } = new List<DocumentIndex>();

  }
}