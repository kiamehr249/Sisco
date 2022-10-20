using Microsoft.EntityFrameworkCore;
using NiksoftCore.DataAccess;

namespace NiksoftCore.DataModel
{
    public class SysDbContext : NikDbContext, ISysUnitOfWork
    {

        public SysDbContext(string connectionString) : base(connectionString)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<PanelMenu> PanelMenus { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<SystemFile> SystemFiles { get; set; }
        public DbSet<ContentCategory> ContentCategories { get; set; }
        public DbSet<GeneralContent> GeneralContents { get; set; }
        public DbSet<ContentFile> ContentFiles { get; set; }
        public DbSet<MenuCategory> MenuCategories { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormControl> FormControls { get; set; }
        public DbSet<ControlItem> ControlItems { get; set; }
        public DbSet<FormData> FormDatas { get; set; }
        public DbSet<SiscoRecord> SiscoRecords { get; set; }
        public DbSet<BaseInfo> BaseInfos { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new SystemSettingMap());
            builder.ApplyConfiguration(new PanelMenuMap());
            builder.ApplyConfiguration(new UserProfileMap());
            builder.ApplyConfiguration(new CountryMap());
            builder.ApplyConfiguration(new ProvinceMap());
            builder.ApplyConfiguration(new CityMap());
            builder.ApplyConfiguration(new SystemFileMap());
            builder.ApplyConfiguration(new ContentCategoryMap());
            builder.ApplyConfiguration(new GeneralContentMap());
            builder.ApplyConfiguration(new ContentFileMap());
            builder.ApplyConfiguration(new MenuCategoryMap());
            builder.ApplyConfiguration(new MenuMap());
            builder.ApplyConfiguration(new UserMap());
            builder.ApplyConfiguration(new RoleMap());
            builder.ApplyConfiguration(new UserLoginMap());
            builder.ApplyConfiguration(new UserRoleMap());
            builder.ApplyConfiguration(new UserTokenMap());
            builder.ApplyConfiguration(new FormMap());
            builder.ApplyConfiguration(new FormControlMap());
            builder.ApplyConfiguration(new ControlItemMap());
            builder.ApplyConfiguration(new FormDataMap());
            builder.ApplyConfiguration(new FormDataMap());
            builder.ApplyConfiguration(new SiscoRecordMap());
            builder.ApplyConfiguration(new BaseInfoMap());
        }
    }
}
