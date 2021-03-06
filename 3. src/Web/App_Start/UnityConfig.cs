using DokCapture.ServicenNetFramework.Auth;
using Services.AuthService;
using System;
using System.Web.Mvc;
using System.Web.SessionState;
using Services.SessionHelperService;
using Unity;
using Unity.AspNet.Mvc;
using Unity.Injection;
using Web.Controllers;
using System.Web;
using AutoMapper;
using Web.App_Start;
using Services.TempDbService;
using Services.UserSerivce;
using System.Security.Principal;
using Services.ConfiguraionService;

namespace Web
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion


        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {

            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<HttpSessionState>(
                new InjectionFactory(c => { return HttpContext.Current.Session; }));
            container.RegisterType<IPrincipal>(
                new InjectionFactory(c => { return HttpContext.Current.User; }));

            container.RegisterType<IUserService, UserService>();

            container.RegisterType<IUserService, UserService>();

            container.RegisterType<IDokmeeService, DokmeeService>();
            container.RegisterType<ITempDbService, TempDbService>();
            container.RegisterType<IConfigurationService, ConfigurationService>();
            
            container.RegisterType<ISessionHelperService, SessionHelperService>();
            container.RegisterInstance<IMapper>(MappingProfile.InitializeAutoMapper().CreateMapper());
            
            container.RegisterType<AccountController>(new InjectionConstructor());
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}