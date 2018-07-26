using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dokmee.Dms.Advanced.WebAccess.Data;

namespace Web.ViewModels.Home
{
  public class SearchModel
  {
    public string Key { get; set; }
    public List<DocumentIndex> TableTitles { get; set; } = new List<DocumentIndex>();

    public List<DocumentItem> DocumentItems { get; set; } = new List<DocumentItem>();
    public string CabinetId { get; set; }
  }

  public class DocumentIndex
  {
    public string Id { get; set; } = "0";
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; } = 0;


    private object _value = null;
    public object Value
    {
      get => _value;
      set
      {
        _value = value;

        ValueString = convertToStringValue(_value, _type);
      }
    }

    private IndexValueType _type = IndexValueType.String;
    public IndexValueType Type
    {
      get => _type;
      set
      {
        _type = value;
        ValueString = convertToStringValue(_value, _type);
      }
    }

    public string ValueString { get; set; }



    private string convertToStringValue(object input, IndexValueType type)
    {
      if (input != null)
      {
        switch (type)
        {
          case IndexValueType.String:
            // return ((String[])input)[0];
            String[] checkConvert = input as String[];
            if (checkConvert == null)
            {
              return (string)input;
            }
            else
            {
              return checkConvert[0];
            }
          case IndexValueType.DateTime:
            string getConvert = string.Empty;
            String[] checkConvertdate = input as String[];
            if (checkConvertdate == null)
            {
              getConvert = (string)input;
            }
            else
            {
              getConvert = checkConvertdate[0];
            }

            if (!string.IsNullOrWhiteSpace(getConvert))
            {
              return getConvert.ToString();
            }

            return string.Empty;

          case IndexValueType.Float:
          case IndexValueType.Integer:
            return input.ToString();
        }

      }
      return string.Empty;
    }

  }

  public class DocumentItem
  {
    public string Id { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    public bool IsFolder { get; set; } = false;
    public bool IsRoot { get; set; } = false;

    public List<DocumentIndex> Indexs { get; set; } = new List<DocumentIndex>();
    public string DisplayFileSize { get; set; }
    public string FullPath { get; set; }
    public string ParentFsGuid { get; set; }
  }
}