using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._90_Utils;

namespace HRApiLibrary.DataAccess._00_Main;

public class _00MainTblMakerAccess : I_00MainTblMakerAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public _00MainTblMakerAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task _01MainTable(string schema = "Main", string connName = "MySqlConn")
    {
        await _01MainSchema(schema, connName);
        await _01MainSchema("MainPay", connName);
        await _01MainSchema("MainPis", connName);

        _01Users(schema, connName);
        _01Menus("Menus10User", schema, connName);
        _01Menus("Menus20Employment", schema, connName);
        _01Menus("Menus30AMS", schema, connName);
        _01Menus("Menus40Pis", schema, connName);
        _01Menus("Menus50Pay", schema, connName);
        _01OtherAccess(schema, connName);
        _01Country(schema, connName);
        _01ProvinceState(schema, connName);
        _01City(schema, connName);
        _01Currency(schema, connName);
        _01UsersCompany(schema, connName);
        _01UC_AccessReq(schema, connName);
        _01UsersCompanyAddress(schema, connName);
        _01CompanyUsers(schema, connName);
        _01CompanyUserType(schema, connName);


    }

    //********************************************************************
    //*** Private Functions **********************************************
    //********************************************************************


    //--- Schema Main ------------------------------------------------------
    private async Task _01MainSchema(string schema, string connName)
    {
        string sql = $"CREATE DATABASE IF NOT EXISTS {schema}";
        await _sql.ExecuteCmd(sql, new { }, connName);

    
    }

    private async void _01UsersSchema(string userId, string companyId, string schemaPrefix)
    {
        string schema = $"u{userId}c{companyId}_{schemaPrefix}";
        string sql = $"CREATE DATABASE IF NOT EXISTS {schema}";
        await _sql.ExecuteCmd(sql, new { });
    }

    //--- Tables ------------------------------------------------------

    private void _01Users(string schema, string connName)
    {
        // UserType => 0 = Ordinary Users, 1 = System Users 
        string sql = @$"CREATE TABLE if not exists {schema}.Users (
                          Id            INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                          LoginName     VARCHAR(45),
                          Password      VARCHAR(150),
                          Email         VARCHAR(60),
                          Domain        VARCHAR(25),
                          UserType      INTEGER UNSIGNED NOT NULL DEFAULT 0,
                          Status        Char(1)         DEFAULT 'A',
                          DefaultCoId   INTEGER UNSIGNED NOT NULL DEFAULT 1,
                          PRIMARY KEY(`Id`))ENGINE = InnoDB;";
        _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async void _01Menus(string menuName, string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.{menuName}
        (
            Id INTEGER UNSIGNED NOT NULL, 
                            IdParent            INT             NULL, 
                            Indent              INT             NULL, 
                            Type                NCHAR(10)       NULL, 
                            Code                NCHAR(10)       NULL, 
                            Icon1               VARCHAR(80)     NULL, 
                            Icon2               VARCHAR(80)     NULL, 
                            DispText            VARCHAR(80)     NULL, 
                            IsWithChild         SMALLINT        NULL, 
                            IsWithDivider       SMALLINT        NULL, 
                            IsSelected          SMALLINT        NULL, 
                            Controller          VARCHAR(50)     NULL, 
                            Action              VARCHAR(50)     NULL, 
                            Odr                 double(12,2)            DEFAULT     999,
                            IsPaid              SmallInt                Default     0, 
                            PaidType            SmallInt                Default     0, 
                            PRIMARY KEY (`Id`)) Engine = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }
    

    private async void _01OtherAccess(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.OtherAccess
        (
            Id              INTEGER UNSIGNED NOT NULL, 
            Name            VARCHAR(80)     NULL, 
            Module          VARCHAR(50)     NULL, 
            Action          VARCHAR(50)     NULL, 
            PRIMARY KEY (`Id`)) Engine = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }
    

    private async void _01Country(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.Country ( 
                            Id INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                            Code        CHAR(10) NULL,
                            Name        CHAR(60) NULL,
                        PRIMARY KEY (`Id`)) Engine = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = @$"select * from {schema}.country limit 1";
        var res = await _sql.FetchData<CountryModel, dynamic>(sql, new { }, connName);
        if (res.Count == 0 || res == null)
        {
            sql = @$"Insert into {schema}.Country 
                        (Code, Name) values 
                        ('CAN', 'Canada'), 
                        ('PHL', 'Philippines')
                    ";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
    }

    private async void _01Currency(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists  {schema}.Currency
                        (
                            Id      INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                            Code    NVARCHAR(5)          NULL,
                            Name    NVARCHAR(45)         NULL,
                            Symbol  NVARCHAR(5)          NULL,
                            PRIMARY KEY (`Id`)) Engine = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = $"select * from {schema}.Currency limit 1";
        var res = await _sql.FetchData<CurrencyModel, dynamic>(sql, new { }, connName);
        if (res.Count == 0)
        {
            sql = @$"insert into {schema}.Currency 
                  (Code, Name, Symbol) values 
                  ('CAD', 'Canada', 'C$'), 
		          ('PHL', 'Philippines','Php'); ";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
    }

    private async void _01ProvinceState(string schema, string connName)
    {
        string sql = $@"CREATE TABLE if not exists  {schema}.ProvinceState
            (
                Id INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                Code      CHAR (10) NOT NULL,
                Name      CHAR (60) NOT NULL,
                CountryId INT       NOT NULL DEFAULT 0,
                PRIMARY KEY (`Id`)) Engine = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        // --- Canada -----------------------------------------------
        sql = @$"select * from {schema}.ProvinceState where CountryId = 1 limit 1";
        var res = await _sql.FetchData<ProvinceStateModel, dynamic>(sql, new { }, connName);
        if (res == null || res.Count == 0)
        {
            sql = @$"insert into {schema}.ProvinceState 
                    (Code, Name,                CountryId) values 
                    (' ', 'Alberta',            1), 
                    (' ', 'British Colombia',   1), 
                    (' ', 'Manitoba',           1), 
                    (' ', 'Newfoundland',       1), 
                    (' ', 'Nova Scotia',        1), 
                    (' ', 'Ontario',            1), 
                    (' ', 'Québec',             1), 
                    (' ', 'Saskatchewan',       1)";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }

        // --- Philippines -------------------------------------------------
        sql = @$"select * from {schema}.ProvinceState where CountryId = 2 limit 1";
        res = await _sql.FetchData<ProvinceStateModel, dynamic>(sql, new { }, connName);
        if (res == null || res.Count == 0)
        {
            sql = @$"insert into {schema}.ProvinceState 
                    (Code,    Name,                             CountryId) values 
                    ('I',   'Ilocos',                           2),
                    ('II',  'Cagayan Valley',                   2),
                    ('III', 'Central Luzon',                    2),
                    ('IV-A','Calabarson',                       2),
                    ('V',   'Bicol',                            2),
                    ('VI',  'Western Visayas',                  2), 
                    ('VII', 'Central Visayas',                  2),
                    ('VIII','Eastern Visayas',                  2),
                    ('IX',  'Zamboanga Peninsula',              2), 
                    ('X',   'Northern Mindanao',                2),
                    ('XI',  'Davao Region',                     2), 
                    ('XII',  'Soccsksargen',                    2),  
                    ('NCR', 'National Capital Region',          2),
                    ('CAR', 'Cordillera Administrative Region', 2),
                    ('XIII', 'Caraga',                          2),
                    ('MIMAROPA', 'Southwestern Tagalog Region', 2),
                    ('BARMM', 'Bangsamoro',                     2)
                    ";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }


    }

    private async void _01City(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists  {schema}.City
                (
                    Id INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,         
                    CountryId       INT           NULL,
                    CountryCode     CHAR(10)      NULL,
                    RegionId        INT           NULL,
                    CityName        VARCHAR (100) NULL,
                    PRIMARY KEY (`Id`)) Engine = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = @$"select * from {schema}.City where CountryId in (1, 2) limit 1";
        var res = await _sql.FetchData<CityModel, dynamic>(sql, new { }, connName);
        if (res == null || res.Count == 0)
        {
            sql = @$"insert into {schema}.City 
                    (CountryId,    CountryCode,     RegionId,   CityName) values 
                    (1,             'CAN',          2,          'Abbotsford'),
                    (1,             'CAN',          6,          'Barrie'),
                    (1,             'CAN',          6,          'Brampton'),
                    (1,             'CAN',          6,          'Burlington'),
                    (1,             'CAN',          2,          'Burnaby'),
                    (1,             'CAN',          1,          'Calgary'),
                    (1,             'CAN',          6,          'Cambridge'),
                    (1,             'CAN',          5,          'Cape Breton'),
                    (1,             'CAN',          2,          'Coquitlam'),
                    (1,             'CAN',          2,          'Delta'),
                    (1,             'CAN',          6,          'East York'),
                    (1,             'CAN',          1,          'Edmonton'),
                    (1,             'CAN',          6,          'Etobicoke'),
                    (1,             'CAN',          7,          'Gatineau'),
                    (1,             'CAN',          6,          'Gloucester'),
                    (1,             'CAN',          6,          'Guelph'),
                    (1,             'CAN',          5,          'Halifax'),
                    (1,             'CAN',          6,          'Hamilton'),
                    (1,             'CAN',          2,          'Kelowna'),
                    (1,             'CAN',          6,          'Kitchener'),
                    (1,             'CAN',          7,          'Laval'),
                    (1,             'CAN',          6,          'London'),
                    (1,             'CAN',          7,          'Longueuil'),
                    (1,             'CAN',          6,          'Markham'),
                    (1,             'CAN',          6,          'Mississauga'),
                    (1,             'CAN',          7,          'Montréal'),
                    (1,             'CAN',          6,          'Nepean'),
                    (1,             'CAN',          6,          'North York'),
                    (1,             'CAN',          6,          'Oakville'),
                    (1,             'CAN',          6,          'Oshawa'),
                    (1,             'CAN',          6,          'Ottawa'),
                    (1,             'CAN',          7,          'Québec'),
                    (1,             'CAN',          8,          'Regina'),
                    (1,             'CAN',          2,          'Richmond'),
                    (1,             'CAN',          6,          'Richmond Hill'),
                    (1,             'CAN',          2,          'Saanich'),
                    (1,             'CAN',          6,          'Saint Catharines'),
                    (1,             'CAN',          4,          'Saint John´s'),
                    (1,             'CAN',          8,          'Saskatoon'),
                    (1,             'CAN',          6,          'Scarborough'),
                    (1,             'CAN',          6,          'Sudbury'),
                    (1,             'CAN',          2,          'Surrey'),
                    (1,             'CAN',          6,          'Thunder Bay'),
                    (1,             'CAN',          6,          'Toronto'),
                    (1,             'CAN',          2,          'Vancouver'),
                    (1,             'CAN',          6,          'Vaughan'),
                    (1,             'CAN',          6,          'Windsor'),
                    (1,             'CAN',          3,          'Winnipeg'),
                    (1,             'CAN',          6,          'York'),
                    (2,             'PHL',          11,         'Angeles'),
                    (2,             'PHL',          12,         'Antipolo'),
                    (2,             'PHL',          11,         'Arayat'),
                    (2,             'PHL',          14,         'Bacolod'),
                    (2,             'PHL',          12,         'Bacoor'),
                    (2,             'PHL',          14,         'Bago'),
                    (2,             'PHL',          22,         'Baguio'),
                    (2,             'PHL',          11,         'Baliuag'),
                    (2,             'PHL',          12,         'Batangas'),
                    (2,             'PHL',          9,          'Bayambang'),
                    (2,             'PHL',          15,         'Bayawan (Tulong)'),
                    (2,             'PHL',          16,         'Baybay'),
                    (2,             'PHL',          23,         'Bayugan'),
                    (2,             'PHL',          12,         'Biñan'),
                    (2,             'PHL',          12,         'Binangonan'),
                    (2,             'PHL',          23,         'Bislig'),
                    (2,             'PHL',          23,         'Butuan'),
                    (2,             'PHL',          11,         'Cabanatuan'),
                    (2,             'PHL',          12,         'Cabuyao'),
                    (2,             'PHL',          14,         'Cadiz'),
                    (2,             'PHL',          18,         'Cagayan de Oro'),
                    (2,             'PHL',          12,         'Cainta'),
                    (2,             'PHL',          12,         'Calamba'),
                    (2,             'PHL',          24,         'Calapan'),
                    (2,             'PHL',          16,         'Calbayog'),
                    (2,             'PHL',          12,         'Candelaria'),
                    (2,             'PHL',          11,         'Capas'),
                    (2,             'PHL',          10,         'Cauayan'),
                    (2,             'PHL',          12,         'Cavite'),
                    (2,             'PHL',          15,         'Cebu'),
                    (2,             'PHL',          11,         'Concepcion'),
                    (2,             'PHL',          25,         'Cotabato'),
                    (2,             'PHL',          9,          'Dagupan'),
                    (2,             'PHL',          15,         'Danao'),
                    (2,             'PHL',          13,         'Daraga (Locsin)'),
                    (2,             'PHL',          12,         'Dasmariñas'),
                    (2,             'PHL',          19,         'Davao'),
                    (2,             'PHL',          19,         'Digos'),
                    (2,             'PHL',          17,         'Dipolog'),
                    (2,             'PHL',          15,         'Dumaguete'),
                    (2,             'PHL',          12,         'General Mariano Alvarez'),
                    (2,             'PHL',          20,         'General Santos'),
                    (2,             'PHL',          12,         'General Trias'),
                    (2,             'PHL',          18,         'Gingoog'),
                    (2,             'PHL',          11,         'Guagua'),
                    (2,             'PHL',          11,         'Hagonoy'),
                    (2,             'PHL',          10,         'Ilagan'),
                    (2,             'PHL',          18,         'Iligan'),
                    (2,             'PHL',          14,         'Iloilo'),
                    (2,             'PHL',          12,         'Imus'),
                    (2,             'PHL',          14,         'Kabankalan'),
                    (2,             'PHL',          21,         'Kalookan'),
                    (2,             'PHL',          20,         'Kidapawan'),
                    (2,             'PHL',          20,         'Koronadal'),
                    (2,             'PHL',          9,          'Laoag'),
                    (2,             'PHL',          15,         'Lapu-Lapu'),
                    (2,             'PHL',          21,         'Las Piñas'),
                    (2,             'PHL',          13,         'Legazpi'),
                    (2,             'PHL',          13,         'Ligao'),
                    (2,             'PHL',          12,         'Lipa'),
                    (2,             'PHL',          11,         'Lubao'),
                    (2,             'PHL',          12,         'Lucena'),
                    (2,             'PHL',          11,         'Mabalacat'),
                    (2,             'PHL',          21,         'Makati'),
                    (2,             'PHL',          21,         'Malabon'),
                    (2,             'PHL',          9,          'Malasiqui'),
                    (2,             'PHL',          18,         'Malaybalay'),
                    (2,             'PHL',          19,         'Malita'),
                    (2,             'PHL',          11,         'Malolos'),
                    (2,             'PHL',          20,         'Malungon'),
                    (2,             'PHL',          21,         'Mandaluyong'),
                    (2,             'PHL',          15,         'Mandaue'),
                    (2,             'PHL',          21,         'Manila'),
                    (2,             'PHL',          25,         'Marawi'),
                    (2,             'PHL',          21,         'Marikina'),
                    (2,             'PHL',          11,         'Marilao'),
                    (2,             'PHL',          19,         'Mati'),
                    (2,             'PHL',          11,         'Mexico'),
                    (2,             'PHL',          11,         'Meycauayan'),
                    (2,             'PHL',          20,         'Midsayap'),
                    (2,             'PHL',          21,         'Muntinlupa'),
                    (2,             'PHL',          13,         'Naga'),
                    (2,             'PHL',          12,         'Nasugbu'),
                    (2,             'PHL',          12,         'Navotas'),
                    (2,             'PHL',          11,         'Olongapo'),
                    (2,             'PHL',          16,         'Ormoc'),
                    (2,             'PHL',          18,         'Ozamis'),
                    (2,             'PHL',          17,         'Pagadian'),
                    (2,             'PHL',          19,         'Panabo'),
                    (2,             'PHL',          21,         'Parañaque'),
                    (2,             'PHL',          21,         'Pasay'),
                    (2,             'PHL',          21,         'Pasig'),
                    (2,             'PHL',          20,         'Polomolok'),
                    (2,             'PHL',          24,         'Puerto Princesa'),
                    (2,             'PHL',          12,         'Quezon'),
                    (2,             'PHL',          12,         'Rodriguez (Montalban)'),
                    (2,             'PHL',          14,         'Roxas'),
                    (2,             'PHL',          14,         'Sagay'),
                    (2,             'PHL',          9,          'San Carlos'),
                    (2,             'PHL',          14,         'San Carlos'),
                    (2,             'PHL',          9,          'San Fernando'),
                    (2,             'PHL',          11,         'San Fernando'),
                    (2,             'PHL',          11,         'San Jose'),
                    (2,             'PHL',          null,       'San Jose'),
                    (2,             'PHL',          11,         'San José del Monte'),
                    (2,             'PHL',          null,       'San Juan del Monte'),
                    (2,             'PHL',          12,         'San Mateo'),
                    (2,             'PHL',          11,         'San Miguel'),
                    (2,             'PHL',          12,         'San Pablo'),
                    (2,             'PHL',          12,         'San Pedro'),
                    (2,             'PHL',          21,         'Santa Cruz'),
                    (2,             'PHL',          11,         'Santa Maria'),
                    (2,             'PHL',          12,         'Santa Rosa'),
                    (2,             'PHL',          10,         'Santiago'),
                    (2,             'PHL',          12,         'Sariaya'),
                    (2,             'PHL',          12,         'Silang'),
                    (2,             'PHL',          14,         'Silay'),
                    (2,             'PHL',          13,         'Sorsogon'),
                    (2,             'PHL',          20,         'Sultan Kudarat'),
                    (2,             'PHL',          23,         'Surigao'),
                    (2,             'PHL',          13,         'Tabaco'),
                    (2,             'PHL',          16,         'Tacloban'),
                    (2,             'PHL',          21,         'Taguig'),
                    (2,             'PHL',          19,         'Tagum'),
                    (2,             'PHL',          11,         'Talavera'),
                    (2,             'PHL',          15,         'Talisay'),
                    (2,             'PHL',          12,         'Tanauan'),
                    (2,             'PHL',          12,         'Tanza'),
                    (2,             'PHL',          11,         'Tarlac'),
                    (2,             'PHL',          12,         'Taytay'),
                    (2,             'PHL',          15,         'Toledo'),
                    (2,             'PHL',          10,         'Tuguegarao'),
                    (2,             'PHL',          9,          'Urdaneta'),
                    (2,             'PHL',          18,         'Valencia'),
                    (2,             'PHL',          21,         'Valenzuela'),
                    (2,             'PHL',          17,         'Zamboanga')
                    ";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }

    }

    private async void _01UsersCompany(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.UsersCompany
                (
                    Id INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                    OwnerId         INT             NULL,
                    CompanySName    VARCHAR (15)    NULL,
                    CompanyName     VARCHAR (120)   NULL,
                    CountryId       int             NULL, 
                    RegionId        int             NULL,
                    CityId          int             NULL,
                    Zipcode         char(10)        NULL, 
                    CurrencyId      INT             NULL,
                    StorageId       VARCHAR (120)   NULL,
                    AMSSchema       VARCHAR (60)    NULL,
                    ApplicantSchema VARCHAR (60)    NULL,
                    PISSchema       VARCHAR (60)    NULL,
                    PaySchema       VARCHAR (60)    NULL,
                    PRIMARY KEY (`Id`)) Engine = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }
    
    private async void _01UC_AccessReq(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.`UC_AccessReq` (
                            `Id`                BIGINT      UNSIGNED NOT NULL AUTO_INCREMENT,
                            `EmpmasId`          INTEGER     UNSIGNED,
                            `RequestedById`     INTEGER     UNSIGNED DEFAULT 0,
                            `UCRequestingId`    INTEGER     UNSIGNED,
                            `DateRequested`     DATETIME,
                            `DateApproved`      DATETIME,
                            `Allowed`           SMALLINT    UNSIGNED NOT NULL DEFAULT 0,
                            `AInfo`             SMALLINT    UNSIGNED NOT NULL DEFAULT 0,
                            `APersonalData`     SMALLINT    UNSIGNED NOT NULL DEFAULT 0,
                            `AAddress`          SMALLINT    UNSIGNED NOT NULL DEFAULT 0,
                            `AEducaion`         SMALLINT    UNSIGNED NOT NULL DEFAULT 0,
                            `AFamily`           SMALLINT    UNSIGNED NOT NULL DEFAULT 0,
                            `AReferences`       SMALLINT    UNSIGNED NOT NULL DEFAULT 0,
                            `AEmployment`       SMALLINT    UNSIGNED NOT NULL DEFAULT 0,
                            `ATrainings`        SMALLINT    UNSIGNED NOT NULL DEFAULT 0,
                            `Isaddressed`       SMALLINT    UNSIGNED NOT NULL DEFAULT 0,
                            PRIMARY KEY(`Id`)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);
        
        
    }

    private async void _01UsersCompanyAddress(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists  {schema}.UserCompanyAdd (
                            Id      INTEGER         UNSIGNED NOT NULL,
                            Address VARCHAR(200)             NOT NULL,
                            TelNos  VARCHAR(80)              NOT NULL,
                            website VARCHAR(60)              NOT NULL,
                        PRIMARY KEY(`Id`) ) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async void _01CompanyUsers(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists  {schema}.CompanyUsers (
                            UserId              int unsigned    NOT NULL,
                            CompanyId           int unsigned    NOT NULL,
                            Status              char(2)         NOT NULL     DEFAULT 'FA',
                            DateInvited         datetime                     DEFAULT NULL,
                            DateAccepted        datetime                     DEFAULT NULL,
                            CompanyUserTypeId   int unsigned                 DEFAULT 0    COMMENT '1 - Owner, 2 - System User, 3 - Employee, 4 - Developer',
                            InvitedById         int unsigned                 Default 0 ,
                        PRIMARY KEY (UserId,CompanyId)) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async void _01CompanyUserType(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists  {schema}.CompanyUserType (
                          Id        INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                          Name      VARCHAR(45),
                          IsVisible INTEGER UNSIGNED    DEFAULT 1 COMMENT '0 - false, 1- yes',
                        PRIMARY KEY(`Id`)) ENGINE = InnoDB DEFAULT CHARSET=latin1;";         
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = @$"select * from {schema}.CompanyUserType limit 1";
        var res = await _sql.FetchData<CompanyUserTypeModel, dynamic>(sql, new { }, connName);
        if (res == null || res.Count == 0)
        {
            sql = @$"insert into {schema}.CompanyUserType 
                    (Id,    Name,           IsVisible) values 
                    (1,     'Owner',        1),
                    (2,     'SystemUser',   1),
                    (3,     'Employee',     0),
                    (4,     'Developer',    0)";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
    }



    public async Task<SchemaStructureModel?> _02_TableExists(string schema, string tableName, string connName)
    {
        string sql = @"SELECT TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE FROM information_schema.TABLES 
                        WHERE TABLE_SCHEMA = @Schema and TABLE_NAME = @TableName;";
        var data = await _sql.FetchData<SchemaStructureModel, dynamic>(sql, new { Schema = schema, TableName = tableName }, connName);
        return data.FirstOrDefault();


    }
}
