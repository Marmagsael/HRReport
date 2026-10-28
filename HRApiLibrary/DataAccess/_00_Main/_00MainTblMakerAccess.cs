using Dapper;
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

    public async Task _01MainTable(string? schema = "Main", string? connName = "MySqlConn")
    {
        await _01MainSchema(schema, connName);
        await _01MainSchema("MainPay", connName);
        await _01MainSchema("MainPis", connName);

        await _01Users(schema, connName);
        await _01Menus("Menus10User", schema, connName);
        await _01Menus("Menus20Employment", schema, connName);
        await _01Menus("Menus30AMS", schema, connName);
        await _01Menus("Menus40Pis", schema, connName);
        await _01Menus("Menus50Pay", schema, connName);
        await _01OtherAccess(schema, connName);
        await _01Country(schema, connName);
        await _01ProvinceState(schema, connName);
        await _01City(schema, connName);
        await _01Psgc(schema, connName);
        await _01Currency(schema, connName);
        await _01UsersCompany(schema, connName);
        await _01UC_AccessReq(schema, connName);
        await _01UsersCompanyAddress(schema, connName);
        await _01CompanyUsers(schema, connName);
        await _01CompanyInvite(schema, connName);
        await _01CompanyUserType(schema, connName);
        await _01EmailOtp(schema, connName);
        await _01MenuSystemApplication(schema, connName);


    }

    //********************************************************************
    //*** Private Functions **********************************************
    //********************************************************************


    //--- Schema Main ------------------------------------------------------
    private async Task _01MainSchema(string? schema, string? connName)
    {
        string? sql = $"CREATE DATABASE IF NOT EXISTS {schema} DEFAULT CHARACTER SET latin1";
        await _sql.ExecuteCmd(sql, new { }, connName);

    
    }

    private async Task _01UsersSchema(string? userId, string? companyId, string? schemaPrefix)
    {
        string? schema = $"u{userId}c{companyId}_{schemaPrefix}";
        string? sql = $"CREATE DATABASE IF NOT EXISTS {schema} DEFAULT CHARACTER SET latin1";
        await _sql.ExecuteCmd(sql, new { });
    }

    //--- Tables ------------------------------------------------------

    private async Task _01Users(string? schema, string? connName)
    {
        // UserType => 0 = Ordinary Users, 1 = System Users 
        string? sql = @$"CREATE TABLE if not exists {schema}.Users (
                          Id            INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                          LoginName     VARCHAR(45),
                          Password      VARCHAR(150),
                          Email         VARCHAR(60),
                          Domain        VARCHAR(25),
                          UserType      INTEGER UNSIGNED NOT NULL DEFAULT 0,
                          Status        Char(1)         DEFAULT 'A',
                          DefaultCoId   INTEGER UNSIGNED NOT NULL DEFAULT 1,
                          PRIMARY KEY(`Id`))ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01Menus(string? menuName, string? schema, string? connName)
    {
        string? sql = @$"CREATE TABLE if not exists {schema}.{menuName}
        (
            Id INTEGER UNSIGNED NOT NULL, 
                            IdParent            int             NULL, 
                            Indent              int             NULL, 
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
    

    private async Task _01OtherAccess(string? schema, string? connName)
    {
        string? sql = @$"CREATE TABLE if not exists {schema}.OtherAccess
        (
            Id              INTEGER UNSIGNED NOT NULL, 
            Name            VARCHAR(80)     NULL, 
            Module          VARCHAR(50)     NULL, 
            Action          VARCHAR(50)     NULL, 
            PRIMARY KEY (`Id`)) Engine = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }
    

    private async Task _01Country(string? schema, string? connName)
    {
        string? sql = @$"CREATE TABLE if not exists {schema}.Country ( 
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

    private async Task _01Currency(string? schema, string? connName)
    {
        string? sql = @$"CREATE TABLE if not exists  {schema}.Currency
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

    private async Task _01ProvinceState(string? schema, string? connName)
    {
        string? sql = $@"CREATE TABLE if not exists  {schema}.ProvinceState
            (
                Id INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                Code      CHAR (10) NOT NULL,
                Name      CHAR (60) NOT NULL,
                CountryId int       NOT NULL DEFAULT 0,
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

    // -- PSGC (PSA Q4 2024) : regions, provinces, cities / municipalities, barangays; seeded once from the embedded TSV --
    private const string PsgcVersion  = "Q4_2024";
    private const string PsgcResource = "HRApiLibrary.Data.psgc-q4-2024.tsv";

    private async Task _01Psgc(string? schema, string? connName)
    {
        string? sql = @$"CREATE TABLE if not exists {schema}.Psgc (
                            PsgcCode        VARCHAR(9)      NOT NULL,
                            Name            VARCHAR(100)    NOT NULL DEFAULT '',
                            Level           VARCHAR(4)      NOT NULL DEFAULT '',
                            RegCode         CHAR(2)         NOT NULL DEFAULT '',
                            ProvCode        VARCHAR(4)      NOT NULL DEFAULT '',
                            CityMunCode     VARCHAR(6)      NOT NULL DEFAULT '',
                            Version         VARCHAR(10)     NOT NULL DEFAULT '',
                        PRIMARY KEY (PsgcCode),
                        KEY IxLevelReg (Level, RegCode),
                        KEY IxCityMun (CityMunCode)) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        var count = await _sql.FetchData<long, dynamic>($"select count(*) from {schema}.Psgc", new { }, connName);
        if (count.FirstOrDefault() > 0) return;

        using var stream = typeof(_00MainTblMakerAccess).Assembly.GetManifestResourceStream(PsgcResource)
                           ?? throw new InvalidOperationException($"Embedded resource {PsgcResource} is missing.");
        using var reader = new StreamReader(stream, System.Text.Encoding.UTF8);

        var batch = new List<string[]>();
        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (line.Length == 0 || line.StartsWith("#")) continue;
            batch.Add(line.Split('\t'));
            if (batch.Count == 500) { await InsertPsgc(schema, batch, connName); batch.Clear(); }
        }
        if (batch.Count > 0) await InsertPsgc(schema, batch, connName);
    }

    // -- One multi-row insert per batch (Level, PsgcCode, Name, RegCode, ProvCode, CityMunCode) --
    private async Task InsertPsgc(string? schema, List<string[]> rows, string? connName)
    {
        var p      = new DynamicParameters();
        var values = new List<string>();
        for (int i = 0; i < rows.Count; i++)
        {
            var r = rows[i];
            values.Add($"(@c{i}, @n{i}, @l{i}, @r{i}, @p{i}, @m{i}, @v)");
            p.Add($"l{i}", r[0]); p.Add($"c{i}", r[1]); p.Add($"n{i}", r[2]);
            p.Add($"r{i}", r[3]); p.Add($"p{i}", r[4]); p.Add($"m{i}", r[5]);
        }
        p.Add("v", PsgcVersion);

        string sql = $@"INSERT INTO {schema}.Psgc (PsgcCode, Name, Level, RegCode, ProvCode, CityMunCode, Version)
                        VALUES {string.Join(", ", values)}";
        await _sql.ExecuteCmd(sql, p, connName);
    }

    private async Task _01City(string? schema, string? connName)
    {
        string? sql = @$"CREATE TABLE if not exists  {schema}.City
                (
                    Id INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,         
                    CountryId       int           NULL,
                    CountryCode     CHAR(10)      NULL,
                    RegionId        int           NULL,
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

    private async Task _01UsersCompany(string? schema, string? connName)
    {
        string? sql = @$"CREATE TABLE if not exists {schema}.UsersCompany
                (
                    Id INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                    OwnerId         int             NULL,
                    CompanySName    VARCHAR (15)    NULL,
                    CompanyName     VARCHAR (120)   NULL,
                    CountryId       int             NULL, 
                    RegionId        int             NULL,
                    CityId          int             NULL,
                    Zipcode         char(10)        NULL, 
                    CurrencyId      int             NULL,
                    StorageId       VARCHAR (120)   NULL,
                    AMSSchema       VARCHAR (60)    NULL,
                    ApplicantSchema VARCHAR (60)    NULL,
                    PISSchema       VARCHAR (60)    NULL,
                    PaySchema       VARCHAR (60)    NULL,
                    AcctgSchema     VARCHAR (60)    NULL,
                    OldPay          VARCHAR (60)    NULL,
                    OldPis          VARCHAR (60)    NULL,
                    CompanyCode     VARCHAR (10)    NULL,
                    PRIMARY KEY (`Id`),
                    UNIQUE KEY uq_userscompany_companycode (CompanyCode)) Engine = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        // -- MySQL 5.5 : no CHECK constraints; CompanyCode (letters and numbers only) is checked by the forms --

        // -- Columns added after the table was created --
        var columns = await _sql.FetchData<dynamic, dynamic>($@"DESC {schema}.UsersCompany", new { }, connName);
        if (!columns.Any(c => c.Field == "CompanyCode"))
        {
            sql = $@"ALTER TABLE {schema}.UsersCompany
                        ADD COLUMN CompanyCode VARCHAR (10) NULL,
                        ADD UNIQUE KEY uq_userscompany_companycode (CompanyCode)";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
        if (!columns.Any(c => c.Field == "AcctgSchema"))
        {
            sql = $@"ALTER TABLE {schema}.UsersCompany ADD COLUMN AcctgSchema VARCHAR (60) NULL AFTER PaySchema";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
        if (!columns.Any(c => c.Field == "OldPay"))
        {
            sql = $@"ALTER TABLE {schema}.UsersCompany ADD COLUMN OldPay VARCHAR (60) NULL";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
        if (!columns.Any(c => c.Field == "OldPis"))
        {
            sql = $@"ALTER TABLE {schema}.UsersCompany ADD COLUMN OldPis VARCHAR (60) NULL";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
    }
    
    private async Task _01UC_AccessReq(string? schema, string? connName)
    {
        string? sql = @$"CREATE TABLE if not exists {schema}.`UC_AccessReq` (
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

    private async Task _01UsersCompanyAddress(string? schema, string? connName)
    {
        string? sql = @$"CREATE TABLE if not exists  {schema}.UserCompanyAdd (
                            Id      INTEGER         UNSIGNED NOT NULL,
                            Address VARCHAR(200)             NOT NULL,
                            TelNos  VARCHAR(80)              NOT NULL,
                            website VARCHAR(60)              NOT NULL,
                        PRIMARY KEY(`Id`) ) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01CompanyUsers(string? schema, string? connName)
    {
        string? sql = @$"CREATE TABLE if not exists  {schema}.CompanyUsers (
                            UserId              int unsigned    NOT NULL,
                            CompanyId           int unsigned    NOT NULL,
                            Status              char(2)         NOT NULL     DEFAULT 'FA',
                            DateInvited         datetime                     DEFAULT NULL,
                            DateAccepted        datetime                     DEFAULT NULL,
                            CompanyUserTypeId   int unsigned                 DEFAULT 0    COMMENT '1 - Owner, 2 - User, 3 - Employee, 5 - System Admin',
                            InvitedById         int unsigned                 Default 0 ,
                            WithPISAccess        tinyint(1)     NOT NULL     DEFAULT 0,
                            WithPayAccess        tinyint(1)     NOT NULL     DEFAULT 0,
                            WithAMSAccess        tinyint(1)     NOT NULL     DEFAULT 0,
                            WithAccountingAccess tinyint(1)     NOT NULL     DEFAULT 0,
                            WithApplicantAccess  tinyint(1)     NOT NULL     DEFAULT 0,
                            EmpmasId            int unsigned                 DEFAULT 0    COMMENT '{{PISSchema}}.Empmas.Id of the user in this company',
                        PRIMARY KEY (UserId,CompanyId)) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        // -- Columns added after the table was created --
        var columns     = await _sql.FetchData<dynamic, dynamic>($@"DESC {schema}.CompanyUsers", new { }, connName);
        var columnNames = new List<string> { "WithPISAccess", "WithPayAccess", "WithAMSAccess", "WithAccountingAccess", "WithApplicantAccess" };

        foreach (var columnName in columnNames)
        {
            if (columns.Any(c => c.Field == columnName)) continue;

            sql = $@"ALTER TABLE {schema}.CompanyUsers ADD COLUMN `{columnName}` tinyint(1) NOT NULL DEFAULT 0";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }

        if (!columns.Any(c => c.Field == "EmpmasId"))
        {
            sql = $@"ALTER TABLE {schema}.CompanyUsers ADD COLUMN EmpmasId int unsigned DEFAULT 0 COMMENT '{{PISSchema}}.Empmas.Id of the user in this company'";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }

        // -- User types used : 1 Owner, 2 User, 3 Employee (portal only), 5 System Admin (invites and approves like the owner) --
        sql = $@"ALTER TABLE {schema}.CompanyUsers MODIFY COLUMN CompanyUserTypeId int unsigned DEFAULT 0 COMMENT '1 - Owner, 2 - User, 3 - Employee, 5 - System Admin'";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    // -- Invites sent by a company to an existing user : same columns as CompanyUsers -----------
    // -- Status I invited, A accepted (row copied to CompanyUsers), D declined -----------------
    // -- P pending : the employee registered (InvitedById 0) and waits for the company ----------
    private async Task _01CompanyInvite(string? schema, string? connName)
    {
        string? sql = @$"CREATE TABLE if not exists  {schema}.CompanyInvite (
                            UserId              int unsigned    NOT NULL,
                            CompanyId           int unsigned    NOT NULL,
                            Status              char(2)         NOT NULL     DEFAULT 'I'  COMMENT 'I - Invited, P - Pending (registered), A - Accepted, D - Declined',
                            DateInvited         datetime                     DEFAULT NULL,
                            DateAccepted        datetime                     DEFAULT NULL COMMENT 'accepted or declined',
                            CompanyUserTypeId   int unsigned                 DEFAULT 2    COMMENT '1 - Owner, 2 - User, 3 - Employee, 5 - System Admin',
                            InvitedById         int unsigned                 Default 0 ,
                            WithPISAccess        tinyint(1)     NOT NULL     DEFAULT 0,
                            WithPayAccess        tinyint(1)     NOT NULL     DEFAULT 0,
                            WithAMSAccess        tinyint(1)     NOT NULL     DEFAULT 0,
                            WithAccountingAccess tinyint(1)     NOT NULL     DEFAULT 0,
                            WithApplicantAccess  tinyint(1)     NOT NULL     DEFAULT 0,
                            EmpmasId            int unsigned                 DEFAULT 0    COMMENT '{{PISSchema}}.Empmas.Id of the invited employee',
                        PRIMARY KEY (UserId,CompanyId)) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        // -- Status P and roles 3 / 5 were added after the table was created --
        sql = $@"ALTER TABLE {schema}.CompanyInvite
                     MODIFY COLUMN Status            char(2)      NOT NULL DEFAULT 'I' COMMENT 'I - Invited, P - Pending (registered), A - Accepted, D - Declined',
                     MODIFY COLUMN CompanyUserTypeId int unsigned          DEFAULT 2   COMMENT '1 - Owner, 2 - User, 3 - Employee, 5 - System Admin'";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01EmailOtp(string? schema, string? connName)
    {
        string? sql = @$"CREATE TABLE if not exists  {schema}.EmailOtp (
                            Id          int unsigned    NOT NULL     AUTO_INCREMENT,
                            Email       varchar(60)     NOT NULL,
                            Code        varchar(150)    NOT NULL     COMMENT 'sha2(otp,512)',
                            Purpose     varchar(20)     NOT NULL     COMMENT 'REG-EMP, REG-OWNER',
                            ExpiresAt   datetime        NOT NULL,
                            Attempts    int unsigned    NOT NULL     DEFAULT 0,
                            UsedAt      datetime                     DEFAULT NULL,
                            CreatedAt   timestamp       NOT NULL     DEFAULT CURRENT_TIMESTAMP     COMMENT 'timestamp : MySQL 5.5 defaults only TIMESTAMP to now',
                        PRIMARY KEY (Id),
                        KEY ix_emailotp_email (Email, Purpose)) ENGINE=InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01CompanyUserType(string? schema, string? connName)
    {
        string? sql = @$"CREATE TABLE if not exists  {schema}.CompanyUserType (
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
                    (3,     'Employee',     1),
                    (4,     'Developer',    0),
                    (5,     'SystemAdmin',  1)";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }

        // -- Types added after the table was filled; Employee (3) is now used --
        sql = @$"insert into {schema}.CompanyUserType (Id, Name, IsVisible)
                 select 5, 'SystemAdmin', 1 from dual
                 where not exists (select 1 from {schema}.CompanyUserType where Id = 5)";
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = @$"update {schema}.CompanyUserType set IsVisible = 1 where Id = 3 and IsVisible <> 1";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }



    // -- System Application (15/502) in the left menu; main.Menu itself is not made here --
    private async Task _01MenuSystemApplication(string? schema, string? connName)
    {
        var exists = await _sql.FetchData<long, dynamic>(
            "select count(*) from information_schema.tables where lower(table_schema) = lower(@Schema) and lower(table_name) = 'menu'", new { Schema = schema }, connName);
        if (exists.FirstOrDefault() == 0) return;

        string? sql = $@"INSERT IGNORE INTO {schema}.Menu (Id, Type, IdParent, Indent, Icon, DispText, Action, Odr) VALUES
                            (1501, 'SHdr-EPortal', 1000, 1, 'fa-solid fa-serve',  'Applications',       '12/501', NULL),
                            (1502, 'Dtl-EPortal',  1501, 2, 'fa-solid fa-server', 'System Application', '15/502', NULL);";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    // -- 03 Patch : every table maker now makes latin1 tables (works on MySQL 5.5 and 8) -----------------------
    // -- Tables of this schema still in utf8mb4 (table or any column) are converted to latin1. A table whose ----
    // -- text does not fit latin1 (emoji, CJK ...) is left as it is and returned, so nothing is lost silently --
    public async Task<List<string>> _03CharsetLatin1(string schema, string? connName)
    {
        var skipped = new List<string>();

        // -- The schema's own default too, so tables made without a CHARSET clause are latin1 --
        var charset = await _sql.FetchData<string, dynamic>(
            "select DEFAULT_CHARACTER_SET_NAME from information_schema.SCHEMATA where SCHEMA_NAME = @Schema", new { Schema = schema }, connName);
        if (charset.Count == 0) return skipped;
        if (!string.Equals(charset.First(), "latin1", StringComparison.OrdinalIgnoreCase))
            await _sql.ExecuteCmd($"ALTER DATABASE `{schema}` DEFAULT CHARACTER SET latin1", new { }, connName);

        string sql = @"select distinct t.TABLE_NAME
                       from   information_schema.TABLES t
                              left join information_schema.COLUMNS c on c.TABLE_SCHEMA = t.TABLE_SCHEMA and c.TABLE_NAME = t.TABLE_NAME
                       where  t.TABLE_SCHEMA = @Schema and t.TABLE_TYPE = 'BASE TABLE'
                              and (t.TABLE_COLLATION like 'utf8mb4%' or c.CHARACTER_SET_NAME = 'utf8mb4')";
        var tables = await _sql.FetchData<string, dynamic>(sql, new { Schema = schema }, connName);

        foreach (var table in tables)
        {
            // -- Text columns whose values change when put in latin1 --
            var columns = await _sql.FetchData<string, dynamic>(
                @"select COLUMN_NAME from information_schema.COLUMNS
                  where  TABLE_SCHEMA = @Schema and TABLE_NAME = @Table and CHARACTER_SET_NAME = 'utf8mb4'",
                new { Schema = schema, Table = table }, connName);

            if (columns.Count > 0)
            {
                string lossy = string.Join(" or ", columns.Select(c => $"binary `{c}` <> binary convert(convert(`{c}` using latin1) using utf8mb4)"));
                var bad = await _sql.FetchData<long, dynamic>($"select count(*) from `{schema}`.`{table}` where {lossy}", new { }, connName);
                if (bad.FirstOrDefault() > 0)
                {
                    skipped.Add(table);
                    continue;
                }
            }

            await _sql.ExecuteCmd($"ALTER TABLE `{schema}`.`{table}` CONVERT TO CHARACTER SET latin1", new { }, connName);
        }

        return skipped;
    }

    // -- Patch : '0000-00-00' defaults are invalid in strict mode (NO_ZERO_DATE) and block every ALTER on the table --
    // -- Key columns stay NOT NULL without a default; the others default to NULL --
    public async Task _03ZeroDateDefaults(string schema, string? connName)
    {
        string sql = @"select TABLE_NAME, COLUMN_NAME, COLUMN_TYPE, IS_NULLABLE, COLUMN_KEY
                       from   information_schema.COLUMNS
                       where  TABLE_SCHEMA = @Schema and DATA_TYPE in ('date', 'datetime', 'timestamp')
                              and COLUMN_DEFAULT like '0000-00-00%'";
        var columns = await _sql.FetchData<dynamic, dynamic>(sql, new { Schema = schema }, connName);
        if (columns.Count == 0) return;

        // -- Own connection : zero dates already in the rows must not stop the table copy, and the pooled session gets its mode back --
        using var cn = _sql.GetConnString(connName);
        cn.Open();
        string oldMode = await cn.ExecuteScalarAsync<string>("select @@SESSION.sql_mode") ?? "";
        string relaxed = string.Join(",", oldMode.Split(',').Where(m => m != "NO_ZERO_DATE" && m != "NO_ZERO_IN_DATE"));

        try
        {
            await cn.ExecuteAsync("SET SESSION sql_mode = @Mode", new { Mode = relaxed });

            foreach (var table in columns.GroupBy(c => (string)c.TABLE_NAME))
            {
                var fixes = table.Select(c => (string)c.COLUMN_KEY == "PRI" || (string)c.IS_NULLABLE == "NO"
                                                ? $"MODIFY COLUMN `{c.COLUMN_NAME}` {c.COLUMN_TYPE} NOT NULL"
                                                : $"MODIFY COLUMN `{c.COLUMN_NAME}` {c.COLUMN_TYPE} NULL DEFAULT NULL");

                await cn.ExecuteAsync($"ALTER TABLE `{schema}`.`{table.Key}` {string.Join(", ", fixes)}");
            }
        }
        finally
        {
            await cn.ExecuteAsync("SET SESSION sql_mode = @Mode", new { Mode = oldMode });
        }
    }

    public async Task<SchemaStructureModel?> _02_TableExists(string? schema, string? tableName, string? connName)
    {
        string? sql = @"SELECT TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE FROM information_schema.TABLES 
                        WHERE TABLE_SCHEMA = @Schema and TABLE_NAME = @TableName;";
        var data = await _sql.FetchData<SchemaStructureModel, dynamic>(sql, new { Schema = schema, TableName = tableName }, connName);
        return data.FirstOrDefault();


    }
}
