
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Dokmee.Dms.Connector.Advanced.Core.Data;
using Services.AuthService.Models;

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
              .ForMember(dest => dest.IsFolder, opt => opt.MapFrom(src => src.IsFolder));
          
      });

      return config;
    }
  }
}