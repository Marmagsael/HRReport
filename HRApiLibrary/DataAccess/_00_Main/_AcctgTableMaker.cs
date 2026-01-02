
using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._00_MainPis;
using MySqlX.XDevAPI;
using System.Xml.Linq;
using HRApiLibrary.DataAccess._20_Pay.Interface;

namespace HRApiLibrary.DataAccess._00_Main;

public class _AcctgTableMaker : I_AcctgTableMaker
{
    private readonly I_90_001_MySqlDataAccess       _sql;
    private readonly I_20_002_PayTblMaker           _payTblMaker;

    public _AcctgTableMaker(I_90_001_MySqlDataAccess sql, I_20_002_PayTblMaker payTblMaker)
    {
        _sql = sql;
        _payTblMaker = payTblMaker;
    }

    public async Task AccountingTableMaker(string db, string conn)
    {
        
        await _01SchemaMaker(db, conn);
        await _payTblMaker._00_002_SystemUser(db, conn);
        await _payTblMaker._00_003_SystemUserModuleAccess(db, conn);
        await _payTblMaker._00_004_SystemUserOtherAccess(db, conn); 
        
        await _01Action(db, conn);
        await _01Bank(db, conn);
        await _01Billcostcenter(db, conn);
        await _01Billhdr(db, conn);
        await _01Billdtl(db, conn);
        await _01Billpayment(db, conn);
        await _01Billcostcenter(db, conn);
        await _01Coa(db, conn);
        await _01Coasettings(db, conn);
        await _01Collectioncategory(db, conn);
        await _01Collectiondtl(db, conn);
        await _01Collectionhdr(db, conn);
        await _01Collectiontrail(db, conn);
        await _01Collectiontype(db, conn);
        await _01Currency(db, conn);
        await _01Customer(db, conn);
        await _01Customeraddr(db, conn);
        await _01Invoicedtl(db, conn);
        await _01Invoicedtlh(db, conn);
        await _01Invoicehdr(db, conn);
        await _01Invoicehdraddress(db, conn);
        await _01Invoicetype(db, conn);
        await _01Item(db, conn);
        await _01Itemsales(db, conn);
        await _01Itemcategorysales(db, conn);
        await _01Je(db, conn);
        await _01Paymentmethod(db, conn);
        await _01Paymentreftype(db, conn);
        await _01Period(db, conn);
        await _01Vendor(db, conn);
        await _01Vendoraddress(db, conn);
        await _01Tax(db, conn);
        await _01Vendorcurrency(db, conn);


    }






    //********************************************************************
    //*** Private Functions **********************************************
    //********************************************************************


    //--- Schema Main ------------------------------------------------------
    private async Task _01SchemaMaker(string schema, string conn)
    {
        string sql = $"CREATE DATABASE IF NOT EXISTS {schema}";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    //--- Action ------------------------------------------------------
    private async Task _01Action(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Action` (
                          `Module`      char(15)    DEFAULT '-',
                          `Trans_id`    int(11)     DEFAULT 0,
                          `Action`      char(15)    DEFAULT '-',
                          `Completed`   int(11)     DEFAULT 0,
                          `Lvl`         int(11)     DEFAULT 0,
                          `Attempt`     int(11)     DEFAULT 0,
                          `Creation`    datetime    DEFAULT NULL,
                          `Userid` bigint(20) DEFAULT 0 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    //--- Action ------------------------------------------------------
    private async Task _01Bank(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Bank` (
                              `Id`            int(10)        unsigned NOT NULL AUTO_INCREMENT,
                              `Bankname`      varchar(80)               DEFAULT '',
                              `Acctno`        char(25)                  DEFAULT '',
                              `Bankbranch`    varchar(60)               DEFAULT ' ',
                              `Currencyid`    int(10)        unsigned   DEFAULT 0,
                              `Chequeno`      int(10)        unsigned   DEFAULT 0,
                              `Reportformat`  varchar(45)               DEFAULT ' ',
                              `Coaid`         varchar(15)               DEFAULT ' ',
                              `Description`   varchar(150)              DEFAULT ' ',
                          PRIMARY KEY (`id`)) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

    }

    //--- Billhdr ------------------------------------------------------
    private async Task _01Billhdr(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Billhdr` (
                              `Id`                  int(10)         unsigned NOT NULL AUTO_INCREMENT,
                              `Pono`                varchar(25)                         DEFAULT ' ',
                              `Vendorid`            int(10)         unsigned NOT NULL   DEFAULT 0,
                              `Currencyiddef`       smallint(5)     unsigned            DEFAULT 0,
                              `Transactiondate`     datetime                            DEFAULT NULL,
                              `Billingdate`         datetime                            DEFAULT NULL,
                              `Duedate`             datetime                            DEFAULT NULL,
                              `Amount`              double(25,4)                        DEFAULT 0.0000,
                              `Amountdue`           double(25,4)                        DEFAULT 0.0000,
                              `Status`              varchar(10)                         DEFAULT ' ',
                              `Notes`               varchar(120),
                              `Discounttype`        int(10)         unsigned            DEFAULT 0,
                              `Discountperc`        double(10,4)                        DEFAULT 0.0000,
                              `Discountamt`         double(25,4)                        DEFAULT 0.0000,
                              `Billcostcenterid`    int(10)         unsigned            DEFAULT 0,
                              `Period`              varchar(10)                         DEFAULT ' ',
                        PRIMARY KEY (`id`)) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    //--- Billdtl ------------------------------------------------------
    private async Task _01Billdtl(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Billdtl` (
                          `Id`                int(10)           unsigned NOT NULL AUTO_INCREMENT,
                          `Billhdrid`         int(10)           unsigned NOT NULL   DEFAULT 0,
                          `Itemid`            int(10)           unsigned NOT NULL   DEFAULT 0,
                          `Coaid`             int(10)           unsigned NOT NULL   DEFAULT 0,
                          `Qty`               double(25,4)                          DEFAULT 0.0000,
                          `Price`             double(25,4)                          DEFAULT 0.0000,
                          `Amount`            double(25,4)                          DEFAULT 0.0000,
                          `Taxpercent`        double(10,4)                          DEFAULT 0.0000,
                          `Taxamt`            double(25,4)                          DEFAULT 0.0000,
                          `Netamt`            double(25,4)                          DEFAULT 0.0000,
                          `Discounttype`      int(10) unsigned                      DEFAULT 0,
                          `Discountperc`      double(10,4)                          DEFAULT 0.0000,
                          `Discountamt`       double(25,4)                          DEFAULT 0.0000,
                          `Billcostcenterid`  int(10)           unsigned            DEFAULT 0,
                          PRIMARY KEY (`id`)) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    //--- Billpayment ------------------------------------------------------
    private async Task _01Billpayment(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Billpayment` (
                          `Id`               int(10)            unsigned NOT NULL AUTO_INCREMENT,
                          `Billhdrid`        int(10)            unsigned        DEFAULT 0,
                          `Amount`           double(25, 4)                      DEFAULT 0.0000,
                          `Paymentdate`      datetime                           DEFAULT NULL,
                          `Paymentmethodid`  tinyint(3)         unsigned        DEFAULT 0,
                          `Coaid`            int(10)            unsigned        DEFAULT 0,
                          `Remarks`          varchar(120)                       DEFAULT ' ',
                        PRIMARY KEY (`Id`)) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    //--- Billhdr ------------------------------------------------------
    private async Task _01Billcostcenter(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Billcostcenter` (
                            `Id`            int(10)         unsigned NOT NULL AUTO_INCREMENT,
                            `Name`          varchar(45)     DEFAULT ' ',
                            `Targetamt`     double(25,4)    DEFAULT 0.0000,
                            `Maxamt`        double(25,4)    DEFAULT 0.0000,
                            `Minamt`        double(25,4)    DEFAULT 0.0000,
                        PRIMARY KEY (`Id`)) ENGINE=InnoDB   DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    //--- Coa ------------------------------------------------------
    private async Task _01Coa(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Coa` (
                          `Id`              int(10)             unsigned NOT NULL AUTO_INCREMENT,
                          `Account_code`    char(12)                            DEFAULT '',
                          `Account_name`    varchar(60)         NOT NULL        DEFAULT ' ',
                          `Contraacctto`    varchar(15)         NOT NULL        DEFAULT ' ',
                          `Account_type`    varchar(10)         NOT NULL        DEFAULT 'Dtl',
                          `Inactive`        tinyint(1)          NOT NULL        DEFAULT 0,
                          `Db`              decimal(18, 4)      NOT NULL        DEFAULT 0.0000,
                          `Cr`              decimal(18, 4)      NOT NULL        DEFAULT 0.0000,
                          `Parentid`        int(10)             unsigned        DEFAULT 0,
                          `WithChild`       int(10)             unsigned        DEFAULT 0,
                          `AllowEntry`      char(1)             NOT NULL        DEFAULT 'N',
                          `CoaType_id`      int(10)             unsigned        DEFAULT 0,
                          `Description`     longtext,          
                          `Editable`        tinyint(1)          NOT NULL        DEFAULT 1,
                          PRIMARY KEY (`Id`)) ENGINE=InnoDB AUTO_INCREMENT=85 DEFAULT CHARSET=utf8mb4; ";
        await _sql.ExecuteCmd(sql, new { }, conn);

        sql = @$"select Id from {db}.Coa limit 1 ";
        var res = await _sql.FetchData<CountryModel, dynamic>(sql, new { }, conn);
        if (res == null || res.Count == 0)
        {
            sql = $@"INSERT INTO {db}.`coa` 
                        (`id`,`account_code`,`account_name`,`contraAcctTo`,`account_type`,
                         `inactive`,`db`,`cr`,`parentid`,`withChild`,`allowEntry`,`coaType_id`,`description`,`editable`) VALUES 
                         (1,'10000000','Assets','','Hdr',0,'0.0000','0.0000',0,0,'N',0,NULL,1),
                         (2,'20000000','Liabilities & Credit Cards','','Hdr',0,'0.0000','0.0000',0,0,'N',0,NULL,1),
                         (3,'30000000','Equity','','Hdr',0,'0.0000','0.0000',0,0,'N',0,NULL,1),
                         (4,'40000000','Expenses','','Hdr',0,'0.0000','0.0000',0,0,'N',0,NULL,1),
                         (5,'50000000','Income','','Hdr',0,'0.0000','0.0000',0,0,'N',0,NULL,1),
                         (6,'10100000','Cash and Bank','','SubHdr',0,'0.0000','0.0000',1,0,'N',0,NULL,1),
                         (7,'10200000','Money in Transit','','SubHdr',0,'0.0000','0.0000',1,0,'N',0,NULL,1),
                         (8,'10300000','Expected Payments from Customers','','SubHdr',0,'0.0000','0.0000',1,0,'N',0,NULL,1),
                         (9,'10400000','Inventory','','SubHdr',0,'0.0000','0.0000',1,0,'N',0,NULL,1),
                         (10,'10500000','Property, Plant, Equipment','','SubHdr',0,'0.0000','0.0000',1,0,'N',0,NULL,1),
                         (11,'10600000','Depreciation and Amortization','','SubHdr',0,'0.0000','0.0000',1,0,'N',0,NULL,1);";
            await _sql.ExecuteCmd(sql, new { }, conn);

            sql = $@"INSERT INTO {db}.`coa` 
                        (`id`,`account_code`,`account_name`,`contraAcctTo`,`account_type`,
                         `inactive`,`db`,`cr`,`parentid`,`withChild`,`allowEntry`,`coaType_id`,`description`,`editable`) VALUES 
                         (12,'10700000','Vendor Prepayments and Vendor Credits','','SubHdr',0,'0.0000','0.0000',1,0,'N',0,NULL,1),
                         (13,'10800000','Other Short-Term Asset','','SubHdr',0,'0.0000','0.0000',1,0,'N',0,NULL,1),
                         (14,'10900000','Other Long-Term Asset','','SubHdr',0,'0.0000','0.0000',1,0,'N',0,NULL,1),
                         (15,'20100000','Credit Card','','SubHdr',0,'0.0000','0.0000',2,0,'N',0,NULL,1),
                         (16,'20200000','Loan and Line of Credit','','SubHdr',0,'0.0000','0.0000',2,0,'N',0,NULL,1),
                         (17,'20300000','Expected Payments to Vendors','','SubHdr',0,'0.0000','0.0000',2,0,'N',0,NULL,1),
                         (18,'20400000','Sales Taxes','','SubHdr',0,'0.0000','0.0000',2,0,'N',0,NULL,1),
                         (19,'20500000','Due For Payroll','','SubHdr',0,'0.0000','0.0000',2,0,'N',0,NULL,1),
                         (20,'20600000','Due to You and Other Business Owners','','SubHdr',0,'0.0000','0.0000',2,0,'N',0,NULL,1),
                         (21,'20700000','Customer Prepayments and Customer Credits','','SubHdr',0,'0.0000','0.0000',2,0,'N',0,NULL,1);";
            await _sql.ExecuteCmd(sql, new { }, conn);

            sql = $@"INSERT INTO {db}.`coa` 
                        (`id`,`account_code`,`account_name`,`contraAcctTo`,`account_type`,
                         `inactive`,`db`,`cr`,`parentid`,`withChild`,`allowEntry`,`coaType_id`,`description`,`editable`) VALUES 
                         (22,'20800000','Other Short-Term Liability','','SubHdr',0,'0.0000','0.0000',2,0,'N',0,NULL,1),
                         (23,'20900000','Other Long-Term Liability','','SubHdr',0,'0.0000','0.0000',2,0,'N',0,NULL,1),
                         (24,'30100000','Business Owner Contribution and Drawing','','SubHdr',0,'0.0000','0.0000',3,0,'N',0,NULL,1),
                         (25,'30200000','Retained Earnings: Profit','','SubHdr',0,'0.0000','0.0000',3,0,'N',0,NULL,1),
                         (26,'40100000','Operating Expense','','SubHdr',0,'0.0000','0.0000',4,0,'N',0,NULL,1),
                         (27,'40200000','Cost of Goods Sold','','SubHdr',0,'0.0000','0.0000',4,0,'N',0,NULL,1),
                         (28,'40300000','Payment Processing Fee','','SubHdr',0,'0.0000','0.0000',4,0,'N',0,NULL,1),
                         (29,'40400000','Payroll Expense','','SubHdr',0,'0.0000','0.0000',4,0,'N',0,NULL,1),
                         (30,'40500000','Uncategorized Expense','','SubHdr',0,'0.0000','0.0000',4,0,'N',0,NULL,1),
                         (31,'40600000','Loss On Foreign Exchange','','SubHdr',0,'0.0000','0.0000',4,0,'N',0,NULL,1);";
            await _sql.ExecuteCmd(sql, new { }, conn);

            sql = $@"INSERT INTO {db}.`coa` 
                        (`id`,`account_code`,`account_name`,`contraAcctTo`,`account_type`,
                         `inactive`,`db`,`cr`,`parentid`,`withChild`,`allowEntry`,`coaType_id`,`description`,`editable`) VALUES 
                         (32,'50100000','Income','','SubHdr',0,'0.0000','0.0000',5,0,'N',0,NULL,1),
                         (33,'50200000','Discount','','SubHdr',0,'0.0000','0.0000',5,0,'N',0,NULL,1),
                         (34,'50300000','Other Income','','SubHdr',0,'0.0000','0.0000',5,0,'N',0,NULL,1),
                         (35,'50400000','Uncategorized Income','','SubHdr',0,'0.0000','0.0000',5,0,'N',0,NULL,1),
                         (36,'50500000','Gain On Foreign Exchange','','SubHdr',0,'0.0000','0.0000',5,0,'N',0,NULL,1),
                         (37,'10100001','Cash on Hand','','Dtl',0,'0.0000','0.0000',6,0,'Y',0,NULL,1),
                         (38,'10300001','Accounts Receivable','','Dtl',0,'0.0000','0.0000',8,0,'Y',0,NULL,0),
                         (39,'20300001','Accounts Payable','','Dtl',0,'0.0000','0.0000',17,0,'Y',0,NULL,1),
                         (40,'30100001','Owner Investment / Drawings','','Dtl',0,'0.0000','0.0000',24,0,'Y',0,NULL,1),
                         (41,'30200001','Owner\'s Equity','','Dtl',0,'0.0000','0.0000',25,0,'Y',0,NULL,1),
                         (42,'40100001','Accounting Fees','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1);";
            await _sql.ExecuteCmd(sql, new { }, conn);

            sql = $@"INSERT INTO {db}.`coa` 
                        (`id`,`account_code`,`account_name`,`contraAcctTo`,`account_type`,
                         `inactive`,`db`,`cr`,`parentid`,`withChild`,`allowEntry`,`coaType_id`,`description`,`editable`) VALUES 
                         (43,'40100002','Advertising & Promotion','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1),
                         (44,'40100003','Bank Service Charges','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1),
                         (45,'40100004','Computer - Hardware','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1),
                         (46,'40100005','Computer - Hosting','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1),
                         (47,'40100006','Computer - Internet','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1),
                         (48,'40100007','Computer - Software','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1),
                         (49,'40100008','Depreciation Expense','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1),
                         (50,'40100009','Dues & Subscriptions','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1),
                         (51,'40100010','Insurance - Vehicles','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1),
                         (52,'40100011','Interest Expense','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1),
                         (53,'40100012','Meals and Entertainment','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1);";
            await _sql.ExecuteCmd(sql, new { }, conn);

            sql = $@"INSERT INTO {db}.`coa` 
                        (`id`,`account_code`,`account_name`,`contraAcctTo`,`account_type`,
                         `inactive`,`db`,`cr`,`parentid`,`withChild`,`allowEntry`,`coaType_id`,`description`,`editable`) VALUES 
                         (54,'40100013','Office Supplies','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1),
                         (55,'40100014','Postage & Delivery','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1),
                         (56,'40100015','Professional Fees','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1),
                         (57,'40100016','Rent Expense','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1),
                         (58,'40100017','Repairs & Maintenance','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1),
                         (59,'40100018','Telephone - Land Line','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1),
                         (60,'40100019','Telephone - Wireless','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1),
                         (61,'40100020','Travel Expense','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1),
                         (62,'40100021','Utilities','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1),
                         (63,'40100022','Vehicle - Fuel','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1),
                         (64,'40100023','Vehicle - Repairs & Maintenance','','Dtl',0,'0.0000','0.0000',26,0,'Y',0,NULL,1);";
            await _sql.ExecuteCmd(sql, new { }, conn);

            sql = $@"INSERT INTO {db}.`coa` 
                        (`id`,`account_code`,`account_name`,`contraAcctTo`,`account_type`,
                         `inactive`,`db`,`cr`,`parentid`,`withChild`,`allowEntry`,`coaType_id`,`description`,`editable`) VALUES 
                         (65,'40200001','Freight & Shipping Costs','','Dtl',0,'0.0000','0.0000',27,0,'Y',0,NULL,1),
                         (66,'40200002','Product Samples','','Dtl',0,'0.0000','0.0000',27,0,'Y',0,NULL,1),
                         (67,'40200003','Purchases - Parts & Materials','','Dtl',0,'0.0000','0.0000',27,0,'Y',0,NULL,1),
                         (68,'40200004','Purchases - Resale Items','','Dtl',0,'0.0000','0.0000',27,0,'Y',0,NULL,1),
                         (69,'40300001','Merchant Account Fees','','Dtl',0,'0.0000','0.0000',28,0,'Y',0,NULL,1),
                         (70,'40400001','Payroll - Employee Benefits','','Dtl',0,'0.0000','0.0000',29,0,'Y',0,NULL,1),
                         (71,'40400002','Payroll - Employer\'s Share of Benefits','','Dtl',0,'0.0000','0.0000',29,0,'Y',0,NULL,1),
                         (72,'40400003','Payroll - Salary & Wages','','Dtl',0,'0.0000','0.0000',29,0,'Y',0,NULL,1),
                         (73,'40500001','Uncategorized Expense','','Dtl',0,'0.0000','0.0000',30,0,'Y',0,NULL,1),
                         (74,'40600001','Loss on Foreign Exchange','','Dtl',0,'0.0000','0.0000',31,0,'Y',0,NULL,1);";
            await _sql.ExecuteCmd(sql, new { }, conn);

            sql = $@"INSERT INTO {db}.`coa` 
                        (`id`,`account_code`,`account_name`,`contraAcctTo`,`account_type`,
                         `inactive`,`db`,`cr`,`parentid`,`withChild`,`allowEntry`,`coaType_id`,`description`,`editable`) VALUES 
                         (75,'50100001','Sales','','Dtl',0,'0.0000','0.0000',32,0,'Y',0,NULL,1),
                         (76,'50400001','Uncategorized Income','','Dtl',0,'0.0000','0.0000',35,0,'Y',0,NULL,1),
                         (77,'50500001','Gain on Foreign Exchange','','Dtl',0,'0.0000','0.0000',36,0,'Y',0,NULL,1),
                         (78,'20400001','Goods and Services Tax/Harmonized Sales Tax - 5%','','Dtl',0,'0.0000','0.0000',18,0,'N',0,NULL,0),
                         (79,'20400002','Goods and Services Tax/Harmonized Sales Tax - 13%','','Dtl',0,'0.0000','0.0000',18,0,'N',0,NULL,0),
                         (80,'20400003','Goods and Services Tax/Harmonized Sales Tax - 15%','','Dtl',0,'0.0000','0.0000',18,0,'N',0,NULL,0),
                         (81,'10100002','China Bank',' ','Dtl',0,'0.0000','0.0000',6,0,'Y',0,'Bank Account',0),
                         (82,'50100002','Phone repairs','0','Dtl',0,'0.0000','0.0000',32,1,'0',0,' ',1),
                         (83,'50100002','Phone repairs','0','Dtl',0,'0.0000','0.0000',32,1,'0',0,' ',1),
                         (84,'50100002','Phone repair','0','Dtl',0,'0.0000','0.0000',32,1,'0',0,' ',1);";
            await _sql.ExecuteCmd(sql, new { }, conn);


        }

    }

    //--- Coasettings ------------------------------------------------------
    private async Task _01Coasettings(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Coasettings` (
                          `Id`                  int(10)             unsigned NOT NULL AUTO_INCREMENT,
                          `Showcoa`             smallint(5)         unsigned DEFAULT 1,
                          `Acctlength`          smallint(5)         unsigned DEFAULT 8,
                          `Assetname`           varchar(60)         DEFAULT 'Assets',
                          `Liabilityname`       varchar(60)         DEFAULT 'Liabilities',
                          `Equityname`          varchar(60)         DEFAULT 'Equity',
                          `Expensename`         varchar(60)         DEFAULT 'Expenses',
                          `Incomename`          varchar(60)         DEFAULT 'Income',
                          `Assetcode`           varchar(20)         DEFAULT '10000000',
                          `Liabilitycode`       varchar(20)         DEFAULT '20000000',
                          `Equitycode`          varchar(20)         DEFAULT '30000000',
                          `Expensescode`        varchar(20)         DEFAULT '40000000',
                          `Incomecode`          varchar(20)         DEFAULT '50000000',
                          PRIMARY KEY (`Id`)) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        sql = @$"select * from {db}.Coasettings limit 1 ";
        var res = await _sql.FetchData<CountryModel, dynamic>(sql, new { }, conn);
        if (res == null || res.Count == 0)
        {
            sql = $@"INSERT INTO {db}.`coasettings` 
                        (`id`,`showcoa`,`acctlength`,`Assetname`,`Liabilityname`,`EquityName`,`Expensename`,`Incomename`,
                         `assetcode`,`liabilitycode`,`equitycode`,`expensescode`,`incomecode`) VALUES
                        (1,1,8,'Assets','Liabilities','Equity','Expenses','Income','10000000','20000000','30000000','40000000','50000000');";
            await _sql.ExecuteCmd(sql, new { }, conn);
        }





    }

    //--- Collectioncategory ------------------------------------------------------
    private async Task _01Collectioncategory(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Collectioncategory` (
                          `Id`                  int(10)         unsigned NOT NULL AUTO_INCREMENT,
                          `Collectioncategory`  varchar(45)     DEFAULT ' ',
                          PRIMARY KEY (`id`)) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        sql = @$"select * from {db}.Collectioncategory limit 1 ";
        var res = await _sql.FetchData<CountryModel, dynamic>(sql, new { }, conn);
        if (res == null || res.Count == 0)
        {
            sql = $@"INSERT INTO {db}.`Collectioncategory` (`Id`,`Collectioncategory`) VALUES 
                         (1,'Customer Payment'),
                         (2,'Batch Payment'),
                         (3,'Single Invoice Payment');";
            await _sql.ExecuteCmd(sql, new { }, conn);
        }
    }


    //--- Collectiondtl ------------------------------------------------------
    private async Task _01Collectiondtl(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Collectiondtl` (
                          `Id`                  int(10) unsigned NOT NULL AUTO_INCREMENT,
                          `Collectionhdrid`     int(10) unsigned DEFAULT 0,
                          `Invoicehdrid`        int(10) unsigned DEFAULT 0,
                          `Amount`              double(25,4) DEFAULT 0.0000,
                          `Forexpayamt`         double(12,4) DEFAULT 0.0000,
                          PRIMARY KEY (`id`)) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    //--- Collectionhdr ------------------------------------------------------
    private async Task _01Collectionhdr(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Collectionhdr` (
                          `Id`                          int(10)             unsigned NOT NULL AUTO_INCREMENT,
                          `Status`                      varchar(10)                     DEFAULT 'A',
                          `Collectioincategoryid`       int(10)             unsigned    DEFAULT 0,
                          `Orno`                        varchar(15)                     DEFAULT '',
                          `Collectiointypeid`           int(10)             unsigned    DEFAULT 0,
                          `Collectionamt`               double(25, 4)                   DEFAULT 0.0000,
                          `Overpayment`                 double(16, 4)                   DEFAULT 0.0000,
                          `Paymentdate`                 datetime                        DEFAULT NULL,                            
                          `Paymentmethodid`             tinyint(3)          unsigned    DEFAULT 0,
                          `Paymentaccountid`            int(10)             unsigned    DEFAULT 0,
                          `Customerid`                  int(10)             unsigned    DEFAULT 0,
                          `Customername`                varchar(80)                     DEFAULT ' ',
                          `Currencyid`                  int(10)             unsigned    DEFAULT 0,
                          `Checkbankid`                 int(10)             unsigned    DEFAULT 0,
                          `Checkno`                     varchar(45)                     DEFAULT ' ',
                          `Checkdate`                   datetime                        DEFAULT NULL,                            
                          `Bankbranch`                  varchar(45)                     DEFAULT ' ',
                          `Depositdate`                 datetime                        DEFAULT NULL,                            
                          `Depositbankid`               int(10)             unsigned    DEFAULT 0,
                          `Period`                      varchar(10)                     DEFAULT ' ',
                          `Inputtaxcoaid`               int(10)             unsigned    DEFAULT 0,
                          `Inputtaxpercent`             double(6,2)                     DEFAULT 0.00,
                          `Inputtaxamount`              double(18, 4)                   DEFAULT 0.0000,
                          `Conversionamount`            double(16, 4)                   DEFAULT 0.0000,
                          `Remarks`                     varchar(120)                    DEFAULT ' ',
                          PRIMARY KEY (`id`)) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    //--- Collectiontrail ------------------------------------------------------
    private async Task _01Collectiontrail(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Collectiontrail` (
                          `Id`                  int(10)         unsigned NOT NULL AUTO_INCREMENT,
                          `Collectionid`        int(10)         unsigned    DEFAULT 0,
                          `Created`             datetime                    DEFAULT NULL,      
                          `Userid`              int(10)         unsigned    DEFAULT 0,
                          `Remarks`             varchar(45)                 DEFAULT ' ',
                          PRIMARY KEY (`id`)) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }


    //--- Collectiontype ------------------------------------------------------
    private async Task _01Collectiontype(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Collectiontype` (
                          `Id`                  int(10)         unsigned NOT NULL AUTO_INCREMENT,
                          `Collectiontype`      varchar(45)                     DEFAULT ' ',
                          PRIMARY KEY (`id`)) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        sql = @$"select * from {db}.Collectiontype limit 1 ";
        var res = await _sql.FetchData<CountryModel, dynamic>(sql, new { }, conn);
        if (res == null || res.Count == 0)
        {
            sql = $@"INSERT INTO {db}.`Collectiontype` 
                        (`Id`, `Collectiontype`) VALUES
                        (1,'Cash Payment'),
                        (2,'Invoice Collection');";
            await _sql.ExecuteCmd(sql, new { }, conn);
        }
    }

    // --- Currency ------------------------------------------------------
    private async Task _01Currency(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Currency` (
                          `Id`              int(10)             unsigned NOT NULL AUTO_INCREMENT,
                          `Code`            varchar(5)          DEFAULT ' ',
                          `Name`            varchar(45)         DEFAULT ' ',
                          `Symbol`          varchar(5)          DEFAULT ' ',
                          PRIMARY KEY (`id`)) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        sql = @$"select * from {db}.Currency limit 1 ";
        var res = await _sql.FetchData<CountryModel, dynamic>(sql, new { }, conn);
        if (res == null || res.Count == 0)
        {
            sql = $@"INSERT INTO {db}.`Currency` (`id`,`code`,`name`,`symbol`) VALUES 
                         (1,'PHP','Peso','PHP'),
                         (2,'USD','US Dollar','$'),
                         (3,'CAD','Canadian Dollar','C$');";
            await _sql.ExecuteCmd(sql, new { }, conn);
        }
    }


    // --- Customer ------------------------------------------------------
    private async Task _01Customer(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Customer` (
                          `Id`                      int(10)         unsigned NOT NULL AUTO_INCREMENT,
                          `Name`                    varchar(80)                 DEFAULT ' ',
                          `Taxidno`                 varchar(25)                 DEFAULT ' ',
                          `Email`                   varchar(80)                 DEFAULT ' ',
                          `Phone`                   varchar(45)                 DEFAULT ' ',
                          `Contact`                 varchar(80)                 DEFAULT ' ',
                          `Currencyiddef`           smallint(5)     unsigned    DEFAULT 0,
                          `Paymenttermiddef`        int(10)         unsigned    DEFAULT 0,
                          `Acctno`                  varchar(45)                 DEFAULT ' ',
                          `Bankname`                varchar(60)                 DEFAULT ' ',
                          `Faxno`                   varchar(25)                 DEFAULT ' ',
                          `Mobileno`                varchar(25)                 DEFAULT ' ',
                          `Website`                 varchar(45)                 DEFAULT ' ',
                          `Remarks`                 varchar(120)                DEFAULT ' ',
                          `Status`                  varchar(1)                  DEFAULT 'A',
                          `Multiname`               smallint(5)     unsigned    DEFAULT 0,
                          `Shortname`               varchar(15)                 DEFAULT ' ',
                          `Creditlimit`             decimal(18,2)               DEFAULT 0.00,
                          `Taxrate`                 decimal(5, 2)               DEFAULT 0.00,
                          PRIMARY KEY (`id`)) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);

    }

    // --- Customeraddr ------------------------------------------------------
    private async Task _01Customeraddr(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Customeraddr` (
                          `Id`                      int(10)        unsigned NOT NULL,
                          `Billaddr1`               varchar(120)            DEFAULT ' ',
                          `Billaddr2`               varchar(120)            DEFAULT ' ',
                          `Billcountrycode`         char(3)                 DEFAULT '',
                          `Billprovince`            varchar(80)             DEFAULT ' ',
                          `Billcityid`              int(10)        unsigned DEFAULT 0,
                          `Issamebillship`          tinyint(3)     unsigned DEFAULT 0,
                          `Billzipcode`             varchar(15)             DEFAULT ' ',
                          `Shipaddr1`               varchar(120)            DEFAULT ' ',
                          `Shipaddr2`               varchar(120)            DEFAULT ' ',
                          `Shipcountrycode`         char(3)                 DEFAULT '',
                          `Shipprovince`            varchar(80)             DEFAULT ' ',
                          `Shipcityid`              int(10)        unsigned DEFAULT 0,
                          `Shipzipcode`             varchar(15)             DEFAULT ' ',
                          PRIMARY KEY (`id`)) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);

    }

    // --- Invoicedtl ------------------------------------------------------
    private async Task _01Invoicedtl(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Invoicedtl` (
                          `Id`                      int(10)                 unsigned NOT NULL AUTO_INCREMENT,
                          `Invoicehdrid`            int(10)                 unsigned    DEFAULT 0,
                          `Itemid`                  int(10)                 unsigned    DEFAULT 0,
                          `Coaid`                   int(10)                 unsigned    DEFAULT 0,
                          `Qty`                     double(12, 4)                       DEFAULT 0.0000,
                          `Price`                   double(12, 4)                       DEFAULT 0.0000,
                          `Amount`                  double(12, 4)                       DEFAULT 0.0000,
                          `Taxpercent`              double(5, 2)                        DEFAULT 0.00,
                          `Taxamount`               double(12, 2)                       DEFAULT 0.00,
                          `Netamt`                  double(16, 4)                       DEFAULT 0.0000,
                          `Taxid`                   int(10)                 unsigned    DEFAULT 0,
                          PRIMARY KEY (`id`)
                        ) ENGINE=InnoDB AUTO_INCREMENT=184 DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);

    }

    // --- Invoicedtlh ------------------------------------------------------
    private async Task _01Invoicedtlh(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Invoicedtlh` (
                          `id`                  int(10)             unsigned NOT NULL AUTO_INCREMENT,
                          `invoicehdrid`        int(10)             unsigned    DEFAULT 0,
                          `itemid`              int(10)             unsigned    DEFAULT 0,
                          `coaid`               int(10)             unsigned    DEFAULT 0,
                          `qty`                 double(12, 4)                   DEFAULT 0.0000,
                          `price`               double(12, 4)                   DEFAULT 0.0000,
                          `amount`              double(12, 4)                   DEFAULT 0.0000,
                          `taxpercent`          double(5, 2)                    DEFAULT 0.00,
                          `taxamount`           double(12, 2)                   DEFAULT 0.00,
                          `netamt`              double(16, 4)                   DEFAULT 0.0000,
                          `taxid`               int(10)             unsigned    DEFAULT 0,
                          `userid`              int(10)             unsigned    DEFAULT 0,
                          `remarks`             varchar(15)                     DEFAULT '',
                          `created`             datetime DEFAULT NULL,          
                          PRIMARY KEY (`id`)) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);

    }

    // --- Invoicehdr ------------------------------------------------------
    private async Task _01Invoicehdr(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Invoicehdr` (
                          `Id`                      int(10)         unsigned NOT NULL AUTO_INCREMENT,
                          `Invoicetypeid`           int(10)         unsigned        DEFAULT 1,
                          `Invoiceno`               int(10)         unsigned        DEFAULT 0,
                          `Customerid`              int(10)         unsigned        DEFAULT 0,
                          `Currencyid`              int(10)         unsigned        DEFAULT 0,
                          `Currencycode`            char(5)                         DEFAULT '',
                          `Transactiondate`         datetime                        DEFAULT NULL,
                          `Paymentdue`              datetime                        DEFAULT NULL,
                          `Amountpayment`           double(16,4)                    DEFAULT 0.0000,
                          `invoiceamount`           double(16,4)                    DEFAULT 0.0000,
                          `Prdcvrdfrom`             datetime                        DEFAULT NULL,
                          `Prdcvrdto`               datetime                        DEFAULT NULL,
                          `Status`                  varchar(10)                     DEFAULT 'A',
                          `Notes`                   varchar(120)                    DEFAULT ' ',
                          `Estimateid`              int(10) unsigned                DEFAULT 0,
                          `Period`                  varchar(10)                     DEFAULT ' ',
                          `Sonum`                   char(10)                        DEFAULT '',
                          `Taxtotal`                double(16,4)                    DEFAULT 0.0000,
                          `Overpayment`             double(16,4)                    DEFAULT 0.0000,
                          `Created`                 datetime                        DEFAULT NULL,
                          `Datesent`                datetime                        DEFAULT NULL,
                          `Conversionamount`        double(16,4)                    DEFAULT 0.0000,
                          PRIMARY KEY (`id`)) ENGINE=InnoDB AUTO_INCREMENT=91 DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);

    }

    // --- Invoicehdraddress ------------------------------------------------------
    private async Task _01Invoicehdraddress(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Invoicehdraddress` (
                          `Invoicehdrid`            int(10)         unsigned NOT NULL DEFAULT 0,
                          `Address1`                char(120)       DEFAULT '',
                          `Address2`                char(120)       DEFAULT '',
                          `City`                    char(60)        DEFAULT '',
                          `Country`                 char(60)        DEFAULT '',
                          `Invoicetitle`            char(60)        DEFAULT '',
                          PRIMARY KEY (`invoicehdrid`)) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

    }

    // --- Invoicetype ------------------------------------------------------
    private async Task _01Invoicetype(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Invoicetype` (
                          `Id`              int(10)             unsigned NOT NULL AUTO_INCREMENT,
                          `Shortname`       varchar(10)         DEFAULT '',
                          `Name`            varchar(80)         NOT NULL,
                          PRIMARY KEY (`id`)) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        sql = @$"select * from {db}.Invoicetype limit 1 ";
        var res = await _sql.FetchData<CountryModel, dynamic>(sql, new { }, conn);
        if (res == null || res.Count == 0)
        {
            sql = $@"INSERT INTO {db}.`Invoicetype` 
                         (`id`,`shortname`,`name`) VALUES 
                         (1,'INV','INVOICE'),
                         (2,'SI','Sales Invoice'),
                         (3,'CI','Cash Invoice'),
                         (4,'SrvI','Service Invoice');";
            await _sql.ExecuteCmd(sql, new { }, conn);
        }

    }

    // --- Item ------------------------------------------------------
    private async Task _01Item(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Item` (
                          `Id`                  int(10)         unsigned NOT NULL AUTO_INCREMENT,
                          `Code`                varchar(25)                         DEFAULT ' ',
                          `Name`                varchar(45)                         DEFAULT ' ',
                          `Description`         varchar(120)                        DEFAULT ' ',
                          `Itemcategoryid`      int(10)         unsigned NOT NULL   DEFAULT 0,
                          `Itemgrpid`           int(10)         unsigned NOT NULL   DEFAULT 0,
                          `Uom`                 varchar(15)                         DEFAULT ' ',
                          `Sp`                  int(10)         unsigned            DEFAULT 0,
                          `Cost`                double(25, 4)                       DEFAULT 0.0000,
                          `Barcode`             varchar(45)                         DEFAULT ' ',
                          `Sbarcode`            varchar(45)                         DEFAULT ' ',
                          `Issalesacct`         tinyint(3)      unsigned            DEFAULT 0,
                          `Isbuyacct`           tinyint(3)      unsigned            DEFAULT 0,
                          `Idsalesacct`         tinyint(3)      unsigned NOT NULL   DEFAULT 0,
                          `Idbuyacct`           int(10)         unsigned NOT NULL   DEFAULT 0,
                          `Idtax`               int(10)         unsigned NOT NULL   DEFAULT 0,
                          `Reorder`             int(10)         unsigned            DEFAULT 0,
                          `Status`              char(1)                             DEFAULT 'A',
                          PRIMARY KEY (`id`)) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);


    }

    // --- Itemsales ------------------------------------------------------
    private async Task _01Itemsales(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Itemsales` (
                          `Id`                  int(10)            unsigned NOT NULL AUTO_INCREMENT,
                          `Code`                varchar(25)                             DEFAULT ' ',
                          `Name`                varchar(45)                             DEFAULT ' ',
                          `Description`         varchar(120)                            DEFAULT NULL,
                          `Status`              varchar(1)                              DEFAULT 'A',
                          `Itemcategoryid`      int(10)            unsigned NOT NULL    DEFAULT 0,
                          `Itemgrpid`           int(10)            unsigned NOT NULL    DEFAULT 0,
                          `Uom`                 varchar(15)                             DEFAULT ' ',
                          `Sp`                  double(12, 4)       unsigned            DEFAULT 0.0000,
                          `Cost`                double(12, 4)       unsigned            DEFAULT 0.0000,
                          `Barcode`             varchar(45)                             DEFAULT ' ',
                          `Sbarcode`            varchar(45)                             DEFAULT ' ',
                          `Issalesacct`         tinyint(3)          unsigned            DEFAULT 0,
                          `Isbuyacct`           tinyint(3)          unsigned            DEFAULT 0,
                          `Idsalesacct`         tinyint(3)          unsigned NOT NULL   DEFAULT 0,
                          `Idbuyacct`           int(10)             unsigned NOT NULL   DEFAULT 0,
                          `Idtax`               int(10)             unsigned NOT NULL   DEFAULT 0,
                          `Reorder`             double(12, 4)       unsigned            DEFAULT 0.0000,
                          PRIMARY KEY (`Id`)) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    // --- Itemcategorysales ------------------------------------------------------
    private async Task _01Itemcategorysales(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Itemcategorysales` (
                          `Id`              int(10)         unsigned NOT NULL AUTO_INCREMENT,
                          `Name`            varchar(45)     DEFAULT ' ',
                          PRIMARY KEY (`id`)) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    // --- Je ------------------------------------------------------
    private async Task _01Je(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Je` (
                          `Id`              int(10)         unsigned NOT NULL AUTO_INCREMENT,
                          `Source`          varchar(10)                 DEFAULT ' ',
                          `Transno`         varchar(15)                 DEFAULT ' ',
                          `Coaid`           int(10)         unsigned,
                          `Db`              double(18, 4)               DEFAULT 0.0000,
                          `Cr`              double(18, 4)               DEFAULT 0.0000,
                          `Created`         datetime                    DEFAULT NULL,          
                          `Userid`          int(10)         unsigned    DEFAULT 0,
                          PRIMARY KEY (`id`)) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    // --- Jeh ------------------------------------------------------
    private async Task _01Jeh(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Jeh` (
                          `Id`              int(10)         unsigned NOT NULL DEFAULT 0,
                          `Source`          varchar(10)                 DEFAULT ' ',
                          `Transno`         varchar(15)                 DEFAULT NULL,
                          `Coaid`           int(10)         unsigned,
                          `Db`              double(18, 4)               DEFAULT 0.0000,
                          `Cr`              double(18, 4)               DEFAULT 0.0000,
                          `Created`         datetime                    DEFAULT NULL,          
                          `Userid`          int(10)         unsigned    DEFAULT 0,
                          `Useridh`         int(10)         unsigned    DEFAULT 0,
                          `Remarks`         varchar(15)                 DEFAULT ' ',
                          PRIMARY KEY (`Id`)) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    // --- Paymentmethod ------------------------------------------------------
    private async Task _01Paymentmethod(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Paymentmethod` (
                          `Id`          int(10)         unsigned NOT NULL AUTO_INCREMENT,
                          `Name`        varchar(45)     DEFAULT ' ',
                          PRIMARY KEY (`id`)) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        sql = @$"select * from {db}.Paymentmethod limit 1 ";
        var res = await _sql.FetchData<CountryModel, dynamic>(sql, new { }, conn);
        if (res == null || res.Count == 0)
        {
            sql = $@"INSERT INTO {db}.`paymentmethod` (`id`,`name`) VALUES 
                         (1,'Bank Payment'),
                         (2,'Cash'),
                         (3,'Cheque'),
                         (4,'Credit Card'),
                         (5,'Paypal'),
                         (6,'Other');";
            await _sql.ExecuteCmd(sql, new { }, conn);
        }

    }

    // --- Paymentreftype ------------------------------------------------------
    private async Task _01Paymentreftype(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Paymentreftype` (
                          `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
                          `name` varchar(45) DEFAULT ' ',
                        PRIMARY KEY (`id`)) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    // --- Period ------------------------------------------------------
    private async Task _01Period(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Period` (
                          `Id`              int(10)         unsigned NOT NULL AUTO_INCREMENT,
                          `Name`            varchar(45)                 DEFAULT ' ',
                          `Period`          varchar(10)                 DEFAULT ' ',
                          `Createdby`       int(10)                     DEFAULT 0,
                          `Datecreated`     datetime                    DEFAULT NULL,      
                          `Islocked`        tinyint(3)      unsigned    DEFAULT 0,
                          PRIMARY KEY (`Id`)) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    // --- Vendor ------------------------------------------------------
    private async Task _01Vendor(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Vendor` (
                          `Id`                      int(10)       unsigned NOT NULL AUTO_INCREMENT,
                          `Vendorname`              char(100)     NOT NULL,
                          `Tin`                     varchar(20)   DEFAULT ' ',
                          `Email`                   char(45)      DEFAULT '',
                          `Currencycode`            char(5)       DEFAULT '',
                          `BankbatchpaymentId`      int(10)       unsigned DEFAULT 0,
                          `Checkname`               varchar(60)   DEFAULT ' ',
                          `Status`                  char(1)       DEFAULT 'A',
                          PRIMARY KEY (`id`)) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    // --- Vendoraddress ------------------------------------------------------
    private async Task _01Vendoraddress(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Vendoraddress` (
                          `Id`              int(10)         unsigned NOT NULL,
                          `Address`         longtext        DEFAULT NULL,      
                          `Town`            char(35)        DEFAULT '',
                          `State`           char(30)        DEFAULT '',
                          `Country`         char(60)        DEFAULT '',
                          `Postalcode`      char(10)        DEFAULT '',
                          `Phone`           char(45)        DEFAULT '',
                          `Fax`             char(25)        DEFAULT '',
                          `Mobile`          char(25)        DEFAULT '',
                          `Ddial`           char(25)        DEFAULT '',
                          `Website`         char(45)        DEFAULT '',
                          PRIMARY KEY (`id`)) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    // --- Tax ------------------------------------------------------
    private async Task _01Tax(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Tax` (
                          `Id`                  int(10)             unsigned NOT NULL AUTO_INCREMENT,
                          `Account_code`        char(12)            DEFAULT '',
                          `Taxname`             varchar(60)         NOT NULL DEFAULT '',
                          `Abbreviation`        varchar(25)         DEFAULT '',
                          `Description`         varchar(150)        DEFAULT ' ',
                          `Rate`                int(10)             NOT NULL DEFAULT 0,
                          `Conv`                double(18, 4)       NOT NULL DEFAULT 0.0000,
                          `Coaid`               int(10)             unsigned DEFAULT 0,
                          PRIMARY KEY (`id`)
                        ) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        sql = @$"select * from {db}.Tax limit 1 ";
        var res = await _sql.FetchData<CountryModel, dynamic>(sql, new { }, conn);
        if (res == null || res.Count == 0)
        {
            sql = $@"INSERT INTO {db}.`tax` 
                        (`id`,`account_code`,`taxname`,`abbreviation`,`description`,`rate`,`conv`,`coaid`) VALUES
                        (1, '20400001', 'Goods and Services Tax/Harmonized Sales Tax - 5%', 'GST/HST5%', '', 5, 0.0500, 78),
                        (2, '20400002', 'Goods and Services Tax/Harmonized Sales Tax - 13%', 'GST/HST13%', '', 13, 0.1300, 79),
                        (3, '20400003', 'Goods and Services Tax/Harmonized Sales Tax - 15%', 'GST/HST15%', '', 15, 0.1500, 80);";
            await _sql.ExecuteCmd(sql, new { }, conn);
        }
    }

    // --- Vendorcurrency ------------------------------------------------------
    private async Task _01Vendorcurrency(string db, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {db}.`Vendorcurrency` (
                          `Vendorid`        int(10) unsigned NOT NULL,
                          `Currencycode`    char(5)             DEFAULT '',
                          PRIMARY KEY (`vendorid`)) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }
    






}