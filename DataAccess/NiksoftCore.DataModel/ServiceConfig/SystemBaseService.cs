using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace NiksoftCore.DataModel
{
    public interface ISystemBaseService
    {
        ISystemSettingService iSystemSettingServ { get; set; }
        IPanelMenuService iPanelMenuService { get; set; }
        IUserProfileService iUserProfileServ { get; set; }
        ICountryService iCountryServ { get; set; }
        IProvinceService iProvinceServ { get; set; }
        ICityService iCityServ { get; set; }
        ISystemFileService iSystemFileServ { get; set; }
        IContentCategoryService iContentCategoryServ { get; set; }
        IGeneralContentService iGeneralContentServ { get; set; }
        IContentFileService iContentFileServ { get; set; }
        IMenuCategoryService iMenuCategoryServ { get; set; }
        IMenuService iMenuServ { get; set; }
        INikUserService iNikUserServ { get; set; }
        INikRoleService iNikRoleServ { get; set; }
        IFormService iFormServ { get; set; }
        IFormControlService iFormControlServ { get; set; }
        IControlItemService iControlItemServ { get; set; }
        IFormDataService iFormDataServ { get; set; }
        ISiscoRecordService iSiscoRecordServ { get; set; }
        IBaseInfoService iBaseInfoServ { get; set; }
    }

    public class SystemBaseService : ISystemBaseService
    {
        public ISystemSettingService iSystemSettingServ { get; set; }
        public IPanelMenuService iPanelMenuService { get; set; }
        public IUserProfileService iUserProfileServ { get; set; }
        public ICountryService iCountryServ { get; set; }
        public IProvinceService iProvinceServ { get; set; }
        public ICityService iCityServ { get; set; }
        public ISystemFileService iSystemFileServ { get; set; }
        public IContentCategoryService iContentCategoryServ { get; set; }
        public IGeneralContentService iGeneralContentServ { get; set; }
        public IContentFileService iContentFileServ { get; set; }
        public IMenuCategoryService iMenuCategoryServ { get; set; }
        public IMenuService iMenuServ { get; set; }
        public INikUserService iNikUserServ { get; set; }
        public INikRoleService iNikRoleServ { get; set; }
        public IFormService iFormServ { get; set; }
        public IFormControlService iFormControlServ { get; set; }
        public IControlItemService iControlItemServ { get; set; }
        public IFormDataService iFormDataServ { get; set; }

        public ISiscoRecordService iSiscoRecordServ { get; set; }
        public IBaseInfoService iBaseInfoServ { get; set; }


        public SystemBaseService(IConfiguration config)
        {
            ISysUnitOfWork uow = new SysDbContext(config.GetConnectionString("SystemBase"));
            iSystemSettingServ = new SystemSettingService(uow);
            iPanelMenuService = new PanelMenuService(uow);
            iUserProfileServ = new UserProfileService(uow);
            iCountryServ = new CountryService(uow);
            iProvinceServ = new ProvinceService(uow);
            iCityServ = new CityService(uow);
            iSystemFileServ = new SystemFileService(uow);
            iContentCategoryServ = new ContentCategoryService(uow);
            iGeneralContentServ = new GeneralContentService(uow);
            iContentFileServ = new ContentFileService(uow);
            iMenuCategoryServ = new MenuCategoryService(uow);
            iMenuServ = new MenuService(uow);
            iNikUserServ = new NikUserService(uow);
            iNikRoleServ = new NikRoleService(uow);
            iFormServ = new FormService(uow);
            iFormControlServ = new FormControlService(uow);
            iControlItemServ = new ControlItemService(uow);
            iFormDataServ = new FormDataService(uow);
            iSiscoRecordServ = new SiscoRecordService(uow);
            iBaseInfoServ = new BaseInfoService(uow);
        }


    }
}
