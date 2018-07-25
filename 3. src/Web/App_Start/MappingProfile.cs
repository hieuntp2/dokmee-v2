
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
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
      });

      return config;
    }
  }
}