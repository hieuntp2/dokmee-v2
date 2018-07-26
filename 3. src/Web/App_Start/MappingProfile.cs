
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Dms.Contracts;
using Dokmee.Dms.Connector.Advanced.Core.Data;
using Services.AuthService.Models;
using Web.ViewModels.Home;

namespace Web.App_Start
{
  public static class MappingProfile
  {
    public static MapperConfiguration InitializeAutoMapper()
    {
      MapperConfiguration config = new MapperConfiguration(cfg =>
      {
        //cfg.CreateMap<Question, QuestionModel>();
        //cfg.CreateMap<QuestionModel, Question>();
        /*etc...*/

        cfg.CreateMap<DokmeeCabinet, Cabinet>()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.CabinetID))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CabinetName));

        cfg.CreateMap<DmsNode, Node>()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.IsFolder, opt => opt.MapFrom(src => src.IsFolder))
            .ForMember(dest => dest.IsRoot, opt => opt.MapFrom(src => src.IsRoot))
          .ForMember(dest => dest.FileType, opt => opt.MapFrom(src => src.FileType));

        cfg.CreateMap<DokmeeIndex, DocumentIndex>()
          .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DokmeeIndexID.ToString()))
          .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name))
          .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.SortOrder))
          .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.ValueType));


          cfg.CreateMap<NodeIndexInfo, DocumentIndex>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IndexFieldGuid.ToString()))
              .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.IndexName))
              .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.SortOrder))
              .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.IndexValueType))
              .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.IndexValue));

          cfg.CreateMap<DokmeeFilesystem, DocumentItem>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.FsGuid.ToString()))
              .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.FsName))
              .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.FileType))
              .ForMember(dest => dest.IsFolder, opt => opt.MapFrom(src => !src.IsDocument))
              .ForMember(dest => dest.DisplayFileSize, opt => opt.MapFrom(src => src.DisplayFileSize))
              .ForMember(dest => dest.FullPath, opt => opt.MapFrom(src => src.FullPath))
              .ForMember(dest => dest.ParentFsGuid, opt => opt.MapFrom(src => src.ParentFsGuid.ToString()))
              .ForMember(dest => dest.Indexs, opt => opt.MapFrom(src => src.IndexFieldPairCollection))
              .ForMember(dest => dest.IsInRecycleBin, opt => opt.MapFrom(src => src.IsInRecycleBin))
              .ForMember(dest => dest.IsRecycleBin, opt => opt.MapFrom(src => src.IsRecycleBin));
      });

      return config;
    }
  }
}