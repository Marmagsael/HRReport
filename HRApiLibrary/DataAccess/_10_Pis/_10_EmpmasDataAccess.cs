using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_MainPis;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class _10_EmpmasDataAccess : I_10_EmpmasDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public _10_EmpmasDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    // **************************************************************************************
    // --- Empmas ***************************************************************************
    //***************************************************************************************
    public async Task<EmpmasModel?> _01Empmas(EmpmasModel empmas, string schema, string conn)
    {
        int id = empmas.Id;
        string  sql = $@"SELECT * FROM {schema}.Empmas WHERE Id = @Id";
        var     res = await _sql.FetchData<EmpmasModel?, dynamic>(sql, new { Id = id }, conn);
        if (res.Count > 0) return res.FirstOrDefault();

        sql = $@"Insert into {schema}.Empmas 
                    (Id,  EmpLastNm,  EmpFirstNm,  EmpMidNm,  Suffix,  EmpAlias) values 
                    (@Id, @EmpLastNm, @EmpFirstNm, @EmpMidNm, @Suffix, @EmpAlias); 
                 SELECT * FROM {schema}.Empmas WHERE Id = @Id; ";
        res = await _sql.FetchData<EmpmasModel?, dynamic>(sql, empmas, conn);
        return res.FirstOrDefault();
    }
    
    public async Task<EmpmasModel?> _01EmpmasV1(EmpmasModel empmas, string schema, string conn)
    {
        int id = empmas.Id;
        if (id != 0) return empmas; 

        string sql = $@"Insert into {schema}.Empmas 
                    (Id,  EmpNumber,  EmpLastNm,  EmpFirstNm,  EmpMidNm,  Suffix,  EmpAlias) values 
                    (@Id, @EmpNumber, @EmpLastNm, @EmpFirstNm, @EmpMidNm, @Suffix, @EmpAlias); 
                 SELECT * FROM {schema}.Empmas  WHERE ID = (SELECT @@IDENTITY); ";
        var res = await _sql.FetchData<EmpmasModel?, dynamic>(sql, empmas, conn);
        return res.FirstOrDefault();
    }


    public async Task<EmpmasModel?> _02Empmas(int id, string schema, string conn)
    {

        string  sql     = $@"select  Id, EmpLastNm, EmpFirstNm, EmpMidNm, Suffix, EmpAlias from {schema}.Empmas where Id = @Id";
        var     data    = await _sql.FetchData<EmpmasModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<List<EmpmasModel?>?> _02Empmas_EmpnumberOwnedByOthers(int id, string empnumber, string schema, string conn)
    {

        string  sql     = $@"select  * from {schema}.Empmas where Empnumber = @Empnumber and  Id != @Id";
        var     data    = await _sql.FetchData<EmpmasModel?, dynamic>(sql, new { Empnumber=empnumber, Id = id }, conn);
        return data;
    }
    
    public async Task<EmpmasModel?> _02ByUserId(int id, string schema="MainPis", string conn="MySqlConn")
    {

        string  sql     = $@"select  Id, EmpLastNm, EmpFirstNm, EmpMidNm, Suffix, EmpAlias 
                                from {schema}.Empmas where Id = @Id";
        var     data    = await _sql.FetchData<EmpmasModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<List<EmpmasModel?>?> _02By1stLetterRange(string firstLetter, string secondLetter, string schema = "MainPis", string conn = "MySqlConn")
    {

        string  sql = $@"select  e.*, concat(trim(e.EmpLastNm),', ' , trim(e.EmpFirstNm),' ', trim(e.EmpMidNm)) FullName 
                        from {schema}.Empmas e 
                        where left(trim(e.EmpLastNm),1) between @FirstLetter and @SecondLetter
                        order by e.EmplastNm, e.EmpFirstNm";
        var     data = await _sql.FetchData<EmpmasModel?, dynamic>(sql, new { FirstLetter=firstLetter, SecondLetter=secondLetter }, conn);
        return data;
    }
    
    public async Task<List<EmpmasModel?>?> _02SearchName(string skey, string schema = "MainPis", string conn = "MySqlConn")
    {
        string searchKey = $"{skey}%"; 
        string sql = $@"select  e.*, concat(trim(e.EmpLastNm),', ' , trim(e.EmpFirstNm),' ', trim(e.EmpMidNm)) FullName 
                        from {schema}.Empmas e 
                        where e.EmpLastNm like @SearchKey or e.EmpFirstNm like @SearchKey
                        order by e.EmplastNm, e.EmpFirstNm";
        
        var data = await _sql.FetchData<EmpmasModel?, dynamic>(sql, new { SearchKey = searchKey}, conn);
        return data;
    }


    public async Task<EmpmasModel?> _03Empmas(int id, EmpmasModel empmas, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmas set 
                            EmpNumber   = @EmpNumber, 
                            EmpLastNm   = @EmpLastNm,  
                            EmpFirstNm  = @EmpFirstNm,  
                            EmpMidNm    = @EmpMidNm,  
                            Suffix      = @Suffix,  
                            EmpAlias    = @EmpAlias where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, empmas, conn);
        sql = $@" select  * from {schema}.Empmas x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasModel?> _04Empmas(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmas where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Empmas x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // **************************************************************************************
    // --- EmpmasInternal *******************************************************************
    //***************************************************************************************
    public async Task<EmpmasInternalModel?> _01EmpmasInternal(EmpmasInternalModel empmas, string schema, string conn)
    {
        Console.WriteLine($"schema : {schema} * SystemId : {empmas.SystemId}");
        string sql = $@"Insert into {schema}.Empmas 
                                (SystemId,  EmpNumber,  EmpLastNm,  EmpFirstNm,  EmpMidNm,  Suffix,  EmpAlias) values 
                                (@SystemId, @EmpNumber, @EmpLastNm, @EmpFirstNm, @EmpMidNm, @Suffix, @EmpAlias)";
        await _sql.ExecuteCmd<dynamic>(sql, empmas, conn);

        sql = $@"SELECT * FROM {schema}.Empmas WHERE Id = @Id";
        var res = await _sql.FetchData<EmpmasInternalModel?, dynamic>(sql, new { Id = empmas.Id}, conn);

        return res.FirstOrDefault();
    }


    public async Task<EmpmasInternalModel?> _02EmpmasInternal(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Empmas where Id = @Id";
        var data = await _sql.FetchData<EmpmasInternalModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<EmpmasInternalModel?> _03EmpmasInternal(int id, EmpmasInternalModel empmas, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmas set 
                            SystemId    = @SystemId, 
                            EmpNumber   = @EmpNumber, 
                            EmpLastNm   = @EmpLastNm, 
                            EmpFirstNm  = @EmpFirstNm, 
                            EmpMidNm    = @EmpMidNm, 
                            Suffix      = @Suffix, 
                            EmpAlias    = @EmpAlias where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, empmas, conn);

        sql = $@" select  * from {schema}.Empmas x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasInternalModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasInternalModel?> _04EmpmasInternal(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmas where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql      = $@" select  * from {schema}.Empmas x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasInternalModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    // *********************************************************************************************
    // --- EmpmasAddress ***************************************************************************
    //**********************************************************************************************
    public async Task<EmpmasAddressModel?> _01EmpmasAddress(int id, EmpmasAddressModel empmasaddress, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmasaddress (Id, 
                                                            PresAddStreet, 
                                                            PresAddVillage, 
                                                            PresAddBrgy,  
                                                            PresAddCityId,  
                                                            PresAddCity,  
                                                            PresAddProvId,  
                                                            PresAddProv,  
                                                            PresAddStateId,  
                                                            PresAddState,  
                                                            PresAddCountryId,  
                                                            PresAddCountry,  
                                                            PresAddZipCode,  
                                                            PresAdd,  
                                                            PresAddTelNo,  
                                                            ProvAddStreet,  
                                                            ProvAddVillage,  
                                                            ProvAddBrgy,  
                                                            ProvAddCityId,  
                                                            ProvAddCity,  
                                                            ProvAddProvId,  
                                                            ProvAddProv,  
                                                            ProvAddStateId,  
                                                            ProvAddState,  
                                                            ProvAddCountryId,  
                                                            ProvAddCountry,  
                                                            ProvAddZipCode,  
                                                            ProvAdd,  
                                                            ProvAddTelNo,  
                                                            Countrycode,  
                                                            EmailAdd,  
                                                            EmailAdd1,  
                                                            CellNo,  
                                                            CellNo1) values  
                                                           (@Id, 
                                                            @PresAddStreet,  
                                                            @PresAddVillage,  
                                                            @PresAddBrgy,  
                                                            @PresAddCityId,  
                                                            @PresAddCity,  
                                                            @PresAddProvId,  
                                                            @PresAddProv,  
                                                            @PresAddStateId,  
                                                            @PresAddState,  
                                                            @PresAddCountryId,  
                                                            @PresAddCountry,  
                                                            @PresAddZipCode,  
                                                            @PresAdd,  
                                                            @PresAddTelNo,  
                                                            @ProvAddStreet,  
                                                            @ProvAddVillage,  
                                                            @ProvAddBrgy,  
                                                            @ProvAddCityId,  
                                                            @ProvAddCity,  
                                                            @ProvAddProvId,  
                                                            @ProvAddProv,  
                                                            @ProvAddStateId,  
                                                            @ProvAddState,  
                                                            @ProvAddCountryId,  
                                                            @ProvAddCountry,  
                                                            @ProvAddZipCode,  
                                                            @ProvAdd,  
                                                            @ProvAddTelNo,  
                                                            @Countrycode,  
                                                            @EmailAdd,  
                                                            @EmailAdd1,  
                                                            @CellNo,  
                                                            @CellNo1)";
        await _sql.ExecuteCmd<dynamic>(sql, empmasaddress, conn);

        sql = $@"SELECT * FROM {schema}.EmpmasAddress WHERE Id = @Id";

        var res = await _sql.FetchData<EmpmasAddressModel?, dynamic>(sql, new { Id = id }, conn);

        return res.FirstOrDefault();
    }

        public async Task<EmpmasAddressModel?> _01EmpmasAddress_EmailOnly(int id, string  emailAdd, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmasaddress (Id, EmailAdd ) VALUES  (@Id,@EmailAdd) on duplicate key  update EmailAdd = @EmailAdd; ";

        var eaddress = new EmpmasAddressModel()
        {
            Id = id,
            EmailAdd = emailAdd
        };
        
        await _sql.ExecuteCmd<dynamic>(sql, eaddress, conn);
        sql = $@"SELECT * FROM {schema}.EmpmasAddress WHERE Id = @Id";

        var res = await _sql.FetchData<EmpmasAddressModel?, dynamic>(sql, new { Id = id }, conn);

        return res.FirstOrDefault();
    }

    public async Task<EmpmasAddressModel?> _02EmpmasAddress(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Empmasaddress where Id = @Id";
        var data = await _sql.FetchData<EmpmasAddressModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<List<EmpmasAddressModel?>?> _02EmpmasAddresss(int id, string schema, string conn)
    {
        string sql  = $@"select  * from {schema}.Empmasaddress where Id = @Id";
        var data    = await _sql.FetchData<EmpmasAddressModel?, dynamic>(sql, new { Id = id }, conn);
        return data;
    }

    
    public async Task<EmpmasAddressModel?> _02EmpmasAddress_ByIdAndEmail(int id, string email, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Empmasaddress where Id = @Id and EmailAdd = @EmailAdd";
        var data = await _sql.FetchData<EmpmasAddressModel?, dynamic>(sql, new { Id = id, EmailAdd =email }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<List<EmpmasAddressModel?>?> _02EmpmasAddress_ByEmailNotTheOwner(int id, string email, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Empmasaddress where EmailAdd = @EmailAdd  and Id != @Id";
        var data = await _sql.FetchData<EmpmasAddressModel?, dynamic>(sql, new { Id = id, EmailAdd = email }, conn);
        return data;
    }


    public async Task<EmpmasAddressModel?> _03EmpmasAddress(int id, EmpmasAddressModel empmasaddress, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmasaddress set   PresAddStreet       = @PresAddStreet, 
                                                            PresAddVillage      = @PresAddVillage, 
                                                            PresAddBrgy         = @PresAddBrgy, 
                                                            PresAddCityId       = @PresAddCityId, 
                                                            PresAddCity         = @PresAddCity, 
                                                            PresAddProvId       = @PresAddProvId, 
                                                            PresAddProv         = @PresAddProv, 
                                                            PresAddStateId      = @PresAddStateId, 
                                                            PresAddState        = @PresAddState, 
                                                            PresAddCountryId    = @PresAddCountryId, 
                                                            PresAddCountry      = @PresAddCountry, 
                                                            PresAddZipCode      = @PresAddZipCode, 
                                                            PresAdd             = @PresAdd, 
                                                            PresAddTelNo        = @PresAddTelNo, 
                                                            ProvAddStreet       = @ProvAddStreet, 
                                                            ProvAddVillage      = @ProvAddVillage, 
                                                            ProvAddBrgy         = @ProvAddBrgy, 
                                                            ProvAddCityId       = @ProvAddCityId, 
                                                            ProvAddCity         = @ProvAddCity, 
                                                            ProvAddProvId       = @ProvAddProvId, 
                                                            ProvAddProv         = @ProvAddProv, 
                                                            ProvAddStateId      = @ProvAddStateId, 
                                                            ProvAddState        = @ProvAddState, 
                                                            ProvAddCountryId    = @ProvAddCountryId, 
                                                            ProvAddCountry      = @ProvAddCountry, 
                                                            ProvAddZipCode      = @ProvAddZipCode, 
                                                            ProvAdd             = @ProvAdd, 
                                                            ProvAddTelNo        = @ProvAddTelNo, 
                                                            Countrycode         = @Countrycode, 
                                                            EmailAdd            = @EmailAdd, 
                                                            EmailAdd1           = @EmailAdd1, 
                                                            CellNo              = @CellNo, 
                                                            CellNo1             = @CellNo1 where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, empmasaddress, conn);

        sql = $@" select  * from {schema}.EmpmasAddress x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasAddressModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasAddressModel?> _04EmpmasAddress(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmasaddress where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Empmasaddress x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasAddressModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    // *********************************************************************************************
    // --- EmpmasCharRef ***************************************************************************
    //**********************************************************************************************


    public async Task<EmpmasCharRefModel?> _01EmpmasCharRef(EmpmasCharRefModel empmascharref, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmascharref (EmpmasId,  Name,  Addr,  Tel,  Position) values 
                                                           (@EmpmasId, @Name, @Addr, @Tel, @Position)";
        await _sql.ExecuteCmd<dynamic>(sql, empmascharref, conn);

        sql = $@"SELECT * FROM {schema}.Empmascharref WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<EmpmasCharRefModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();
    }


    public async Task<EmpmasCharRefModel?> _02EmpmasCharRef(int id, string schema, string conn)
    {
        string sql = $@"select  Id, EmpmasId, Name, Addr, Tel, Position from {schema}.Empmascharref where Id = @Id";
        var data = await _sql.FetchData<EmpmasCharRefModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<EmpmasCharRefModel?>> _02EmpmasCharRefList(int empmasId, string schema, string conn)
    {
        string sql = $@"select  Id, EmpmasId, Name, Addr, Tel, Position from {schema}.Empmascharref where EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<EmpmasCharRefModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data;
    }

    public async Task<EmpmasCharRefModel?> _03EmpmasCharRef(int id, EmpmasCharRefModel empmascharref, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmascharref set 
                                    EmpmasId    = @EmpmasId, 
                                    Name        = @Name, 
                                    Addr        = @Addr, 
                                    Tel         = @Tel, 
                                    Position    = @Position where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, empmascharref, conn);

        sql = $@" select  * from {schema}.Empmascharref x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasCharRefModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasCharRefModel?> _04EmpmasCharRef(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmascharref where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Empmascharref x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasCharRefModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    // *********************************************************************************************
    // --- EmpmasClearancePh ***********************************************************************
    //**********************************************************************************************

    public async Task<EmpmasClearancePhModel?> _01EmpmasClearancePh(int id, EmpmasClearancePhModel empmasclearanceph, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.EmpmasClearancePh 
                            (Id, Nbi_Taken, Nbi_Exp, Nbi_Remarks,Nbi_Link,  
                             Police_Taken, Police_Exp, Police_Remarks, Police_Link,  
                             Pnp_Taken, Pnp_Exp, Pnp_Remarks, Pnp_Link,  
                             Brgy_Taken, Brgy_Exp, Brgy_Remarks, Brgy_Link,  
                             Court_Taken, Court_Exp, Court_Remarks, Court_Link,  
                             Neuro_Taken, Neuro_Exp, Neuro_Remarks, Neuro_Link,  
                             Drug_Taken, Drug_Exp, Drug_Remarks, Drug_Link) values 
                            (@Nbi_Taken, @Nbi_Exp, @Nbi_Remarks, @Nbi_Link,  
                             @Police_Taken, @Police_Exp, @Police_Remarks, @Police_Link,  
                             @Pnp_Taken, @Pnp_Exp, @Pnp_Remarks, @Pnp_Link,  
                             @Brgy_Taken, @Brgy_Exp, @Brgy_Remarks, @Brgy_Link,  
                             @Court_Taken, @Court_Exp, @Court_Remarks, @Court_Link,  
                             @Neuro_Taken, @Neuro_Exp, @Neuro_Remarks, @Neuro_Link,  
                             @Drug_Taken, @Drug_Exp, @Drug_Remarks, @Drug_Link)";
        await _sql.ExecuteCmd<dynamic>(sql, empmasclearanceph, conn);

        sql = $@"SELECT * FROM {schema}.Empmasclearanceph WHERE Id = @Id";

        var res = await _sql.FetchData<EmpmasClearancePhModel?, dynamic>(sql, new { Id = id }, conn);

        return res.FirstOrDefault();
    }


    public async Task<EmpmasClearancePhModel?> _02EmpmasClearancePh(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Nbi_Taken, Nbi_Exp, Nbi_Remarks, Nbi_Link, 
                                Police_Taken, Police_Exp, Police_Remarks, Police_Link, 
                                Pnp_Taken, Pnp_Exp, Pnp_Remarks, Pnp_Link, 
                                Brgy_Taken, Brgy_Exp, Brgy_Remarks, Brgy_Link, 
                                Court_Taken, Court_Exp, Court_Remarks, Court_Link, 
                                Neuro_Taken, Neuro_Exp, Neuro_Remarks, Neuro_Link, 
                                Drug_Taken, Drug_Exp, Drug_Remarks, Drug_Link from {schema}.EmpmasClearancePh where Id = @Id";
        var data = await _sql.FetchData<EmpmasClearancePhModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<EmpmasClearancePhModel?> _03EmpmasClearancePh(int id, EmpmasClearancePhModel empmasclearanceph, string schema, string conn)
    {
        string sql = $@"Update {schema}.EmpmasClearancePh set 
                                  Nbi_Taken = @Nbi_Taken, Nbi_Exp = @Nbi_Exp, Nbi_Remarks = @Nbi_Remarks, Nbi_Link = @Nbi_Link, 
                                  Police_Taken = @Police_Taken, Police_Exp = @Police_Exp, Police_Remarks = @Police_Remarks, Police_Link = @Police_Link,  
                                  Pnp_Taken = @Pnp_Taken, Pnp_Exp = @Pnp_Exp, Pnp_Remarks = @Pnp_Remarks, Pnp_Link = @Pnp_Link,  
                                  Brgy_Taken = @Brgy_Taken, Brgy_Exp = @Brgy_Exp, Brgy_Remarks = @Brgy_Remarks, Brgy_Link = @Brgy_Link,  
                                  Court_Taken = @Court_Taken, Court_Exp = @Court_Exp, Court_Remarks = @Court_Remarks, Court_Link = @Court_Link,  
                                  Neuro_Taken = @Neuro_Taken, Neuro_Exp = @Neuro_Exp, Neuro_Remarks = @Neuro_Remarks, Neuro_Link = @Neuro_Link,  
                                  Drug_Taken = @Drug_Taken, Drug_Exp = @Drug_Exp, Drug_Remarks = @Drug_Remarks, Drug_Link = @Drug_Link where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, empmasclearanceph, conn);

        sql = $@" select  * from {schema}.Empmasclearanceph x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasClearancePhModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasClearancePhModel?> _04EmpmasClearancePh(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmasclearanceph where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Empmasclearanceph x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasClearancePhModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // *********************************************************************************************
    // --- EmpmasEducate **************************************************************************
    //**********************************************************************************************

    public async Task<EmpmasEducateModel?> _01EmpmasEducate(EmpmasEducateModel empmaseducate, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmaseducate (EmpmasId,  Code,  School,  FROM_,  TO_,  COURSE,  LEVEL) values 
                                                           (@EmpmasId, @Code, @School, @FROM_, @TO_, @COURSE, @LEVEL)";
        await _sql.ExecuteCmd<dynamic>(sql, empmaseducate, conn);

        sql = $@"SELECT * FROM {schema}.Empmaseducate WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<EmpmasEducateModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();
    }


    public async Task<EmpmasEducateModel?> _02EmpmasEducate(int id, string schema, string conn)
    {
        string sql = $@"select  Id, EmpmasId, Code, School, FROM_, TO_, COURSE, LEVEL from {schema}.Empmaseducate where Id = @Id";
        var data = await _sql.FetchData<EmpmasEducateModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<EmpmasEducateModel?>> _02EmpmasEducateList(int empmasId, string schema, string conn)
    {
        string sql = $@"select  e.*, er.Name LEVELNAME from {schema}.Empmaseducate e
                        LEFT JOIN {schema}.empmaseducateref er on er.Code = level where EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<EmpmasEducateModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data;
    }


    public async Task<EmpmasEducateModel?> _03EmpmasEducate(int id, EmpmasEducateModel empmaseducate, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmaseducate set 
                            EmpmasId    = @EmpmasId, 
                            Code        = @Code, 
                            School      = @School, 
                            FROM_       = @FROM_, 
                            TO_         = @TO_, 
                            COURSE      = @COURSE, 
                            LEVEL       = @LEVEL where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, empmaseducate, conn);

        sql = $@" select  * from {schema}.Empmaseducate x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasEducateModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasEducateModel?> _04EmpmasEducate(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmaseducate where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Empmaseducate x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasEducateModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    // *********************************************************************************************
    // --- EmpmasEducateRef **************************************************************************
    //**********************************************************************************************

    public async Task<EmpmasEducateRefModel?> _01EmpmasEducateRef(int id, EmpmasEducateRefModel empmaseducateref, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.EmpmasEducateRef (Id, Code, Name) values (@Id, @Code, @Name)";
        await _sql.ExecuteCmd<dynamic>(sql, empmaseducateref, conn);

        sql = $@"SELECT * FROM {schema}.Empmaseducateref WHERE Id = @Id";

        var res = await _sql.FetchData<EmpmasEducateRefModel?, dynamic>(sql, new { Id = id }, conn);

        return res.FirstOrDefault();
    }


    public async Task<EmpmasEducateRefModel?> _02EmpmasEducateRef(int id, string schema, string conn)
    {
        string sql = $@"select  Code, Name from {schema}.EmpmasEducateRef where Id = @Id";
        var data = await _sql.FetchData<EmpmasEducateRefModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<EmpmasEducateRefModel?>?> _02EmpmasEducateRef( string schema, string conn)
    {
        string sql = $@"select  Code, Name from {schema}.EmpmasEducateRef";
        var data = await _sql.FetchData<EmpmasEducateRefModel?, dynamic>(sql, new {  }, conn);
        return data;
    }


    public async Task<EmpmasEducateRefModel?> _03EmpmasEducateRef(int id, EmpmasEducateRefModel empmaseducateref, string schema, string conn)
    {
        string sql = $@"Update {schema}.EmpmasEducateRef set Code = @Code, Name = @Name where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, empmaseducateref, conn);

        sql = $@" select  * from {schema}.EmpmasEducateRef x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasEducateRefModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasEducateRefModel?> _04EmpmasEducateRef(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.EmpmasEducateRef where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.EmpmasEducateRef x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasEducateRefModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // *********************************************************************************************
    // --- EmpmasEmployment ************************************************************************
    //**********************************************************************************************

    public async Task<EmpmasEmploymentModel?> _01EmpmasEmployment(EmpmasEmploymentModel empmasemployment, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmasemployment 
                            (EmpmasId,  CompName,  Address,  Tel,  Pos,  From_,  To_,  Sal,  Rem) values 
                            (@EmpmasId, @CompName, @Address, @Tel, @Pos, @From_, @To_, @Sal, @Rem)";
        await _sql.ExecuteCmd<dynamic>(sql, empmasemployment, conn);

        sql = $@"SELECT * FROM {schema}.Empmasemployment WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<EmpmasEmploymentModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<EmpmasEmploymentModel?> _02EmpmasEmployment(int id, string schema, string conn)
    {
        string sql = $@"select  Id, EmpmasId, CompName, Address, Tel, Pos, From_, To_, Sal, Rem from {schema}.EmpmasEmployment where Id = @Id";
        var data = await _sql.FetchData<EmpmasEmploymentModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<EmpmasEmploymentModel?>> _02EmpmasEmploymentList(int empmasId, string schema, string conn)
    {
        string sql = $@"select  Id, EmpmasId, CompName, Address, Tel, Pos, From_, To_, Sal, Rem from {schema}.EmpmasEmployment where EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<EmpmasEmploymentModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data;
    }

    public async Task<EmpmasEmploymentModel?> _03EmpmasEmployment(int id, EmpmasEmploymentModel empmasemployment, string schema, string conn)
    {
        string sql = $@"Update {schema}.EmpmasEmployment set 
                                        CompName    = @CompName, 
                                        Address     = @Address, 
                                        Tel         = @Tel, 
                                        Pos         = @Pos, 
                                        From_       = @From_, 
                                        To_         = @To_, 
                                        Sal         = @Sal, 
                                        Rem         = @Rem where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, empmasemployment, conn);

        sql = $@" select  * from {schema}.EmpmasEmployment x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasEmploymentModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasEmploymentModel?> _04EmpmasEmployment(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.EmpmasEmployment where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.EmpmasEmployment x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasEmploymentModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
   
    
    // *********************************************************************************************
    // --- EmpmasFamily ****************************************************************************
    //**********************************************************************************************
    public async Task<EmpmasFamilyModel?> _01EmpmasFamily(EmpmasFamilyModel empmasfamily, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmasfamily (EmpmasId, Name, Birth, Sex, RelationCode) values (@EmpmasId, @Name, @Birth, @Sex, @RelationCode)";
        await _sql.ExecuteCmd<dynamic>(sql, empmasfamily, conn);

        sql = $@"SELECT * FROM {schema}.Empmasfamily WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<EmpmasFamilyModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();
    }


    public async Task<EmpmasFamilyModel?> _02EmpmasFamily(int id, string schema, string conn)
    {
        string sql = $@"select  Id, EmpmasId, Name, Birth, Sex, RelationCode from {schema}.Empmasfamily where Id = @Id";
        var data = await _sql.FetchData<EmpmasFamilyModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<EmpmasFamilyModel?>> _02EmpmasFamilyList(int empmasId, string schema, string conn)
    {
        string sql = $@"select ef.*, er.name relationshipname from {schema}.Empmasfamily ef
                      LEFT JOIN {schema}.EmpmasFamilyRef er on er.code =  ef.RelationCode
                     where EmpmasId = @empmasId";
        var data = await _sql.FetchData<EmpmasFamilyModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data;
    }

    public async Task<EmpmasFamilyModel?> _03EmpmasFamily(int id, EmpmasFamilyModel empmasfamily, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmasfamily set EmpmasId = @EmpmasId, Name = @Name, Birth = @Birth, Sex = @Sex, RelationCode = @RelationCode where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, empmasfamily, conn);

        sql = $@" select  * from {schema}.Empmasfamily x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasFamilyModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasFamilyModel?> _04EmpmasFamily(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmasfamily where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Empmasfamily x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasFamilyModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    // *********************************************************************************************
    // --- EmpmasFamilyRef ****************************************************************************
    //**********************************************************************************************
    public async Task<EmpmasFamilyRefModel?> _01EmpmasFamilyRef(string code, string name, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.EmpmasFamilyRef (Code, Name) values (@Code, @Name)";
        await _sql.ExecuteCmd<dynamic>(sql, new { Code = code, Name = name }, conn);

        sql = $@"SELECT * FROM {schema}.EmpmasFamilyRef WHERE Code = @Code";
        var res = await _sql.FetchData<EmpmasFamilyRefModel?, dynamic>(sql, new { Code = code}, conn);
        return res.FirstOrDefault();
    }

    public async Task<EmpmasFamilyRefModel?> _02EmpmasFamilyRef(string code, string schema, string conn)
    {
        string sql = $@"select  Code, Name from {schema}.Empmasfamilyref where Code = @Code";
        var data = await _sql.FetchData<EmpmasFamilyRefModel?, dynamic>(sql, new { Code = code }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<List<EmpmasFamilyRefModel?>?> _02EmpmasFamilyRefList(string schema, string conn)
    {
        string sql = $@"select  Code, Name from {schema}.Empmasfamilyref ";
        var data = await _sql.FetchData<EmpmasFamilyRefModel?, dynamic>(sql, new {  }, conn);
        return data;
    }


    public async Task<EmpmasFamilyRefModel?> _03EmpmasFamilyRef(string code, string name, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmasfamilyref set Code = @Code, Name = @Name where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Code = code, Name = name }, conn);

        sql = $@" select  * from {schema}.Empmasfamilyref where Code = @Code ;";
        var data = await _sql.FetchData<EmpmasFamilyRefModel?, dynamic>(sql, new { Code = code }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasFamilyRefModel?> _04EmpmasFamilyRef(string code, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmasfamilyref where Code = @Code; ";
        await _sql.ExecuteCmd<dynamic>(sql, new { Code = code }, conn);

        sql = $@" select  * from {schema}.Empmasfamilyref x where x.Code = @Code ;";
        var data = await _sql.FetchData<EmpmasFamilyRefModel?, dynamic>(sql, new { Code = code }, conn);
        return data?.FirstOrDefault();
    }

    // *********************************************************************************************
    // --- EmpmasGovPh *****************************************************************************
    //**********************************************************************************************
    public async Task<EmpmasGovPhModel?> _01EmpmasGovPh(int id, EmpmasGovPhModel empmasgovph, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmasgovph (Id,  Sss,  Tin,  PagibigNo,  Phic,  Hdmf,  BankNo,  BankName,  Drv_License,  Drv_Exp,  dpadate,  TaxCode) values 
                                                         (@Id, @Sss, @Tin, @PagibigNo, @Phic, @Hdmf, @BankNo, @BankName, @Drv_License, @Drv_Exp, @dpadate, @TaxCode)";
        await _sql.ExecuteCmd<dynamic>(sql, empmasgovph, conn);

        sql = $@"SELECT * FROM {schema}.Empmasgovph WHERE Id = @Id";
        var res = await _sql.FetchData<EmpmasGovPhModel?, dynamic>(sql, new { Id = id }, conn);

        return res.FirstOrDefault();
    }


    public async Task<EmpmasGovPhModel?> _02EmpmasGovPh(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Sss, Tin, PagibigNo, Phic, Hdmf, BankNo, BankName, Drv_License, Drv_Exp, dpadate, TaxCode from {schema}.Empmasgovph where Id = @Id";
        var data = await _sql.FetchData<EmpmasGovPhModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<EmpmasGovPhModel?> _03EmpmasGovPh(int id, EmpmasGovPhModel empmasgovph, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmasgovph set 
                            Sss = @Sss, 
                            Tin = @Tin,  
                            PagibigNo = @PagibigNo,  
                            Phic = @Phic,  
                            Hdmf = @Hdmf,  
                            BankNo = @BankNo,  
                            BankName = @BankName,  
                            Drv_License = @Drv_License,  
                            Drv_Exp = @Drv_Exp,  
                            dpadate = @dpadate,  
                            TaxCode = @TaxCode where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, empmasgovph, conn);

        sql = $@" select  * from {schema}.Empmasgovph x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasGovPhModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasGovPhModel?> _04EmpmasGovPh(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmasgovph where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Empmasgovph x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasGovPhModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // *********************************************************************************************
    // --- EmpmasInsurance *************************************************************************
    //**********************************************************************************************
    public async Task<EmpmasInsuranceModel?> _01EmpmasInsurance(int id, EmpmasInsuranceModel empmasinsurance, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmasinsurance (Id,  INSURANCE,  PolicyNo,  FaceValue,  Premium,  InsExpire) values 
                                                             (@Id, @INSURANCE, @PolicyNo, @FaceValue, @Premium, @InsExpire)";
        await _sql.ExecuteCmd<dynamic>(sql, empmasinsurance, conn);

        sql = $@"SELECT * FROM {schema}.Empmasinsurance WHERE Id = @Id";
        var res = await _sql.FetchData<EmpmasInsuranceModel?, dynamic>(sql, new { Id = id }, conn);
        return res.FirstOrDefault();
    }


    public async Task<EmpmasInsuranceModel?> _02EmpmasInsurance(int id, string schema, string conn)
    {
        string sql = $@"select  Id, INSURANCE, PolicyNo, FaceValue, Premium, InsExpire from {schema}.Empmasinsurance where Id = @Id";
        var data = await _sql.FetchData<EmpmasInsuranceModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<EmpmasInsuranceModel?> _03EmpmasInsurance(int id, EmpmasInsuranceModel empmasinsurance, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmasinsurance set 
                            INSURANCE   = @INSURANCE, 
                            PolicyNo    = @PolicyNo,  
                            FaceValue   = @FaceValue,  
                            Premium     = @Premium,  
                            InsExpire   = @InsExpire where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, empmasinsurance, conn);

        sql = $@" select  * from {schema}.Empmasinsurance x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasInsuranceModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasInsuranceModel?> _04EmpmasInsurance(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmasinsurance where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Empmasinsurance x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasInsuranceModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // *********************************************************************************************
    // --- EmpmasPI ********************************************************************************
    //**********************************************************************************************
    public async Task<EmpmasPIModel?> _01EmpmasPI(int id, EmpmasPIModel empmaspi, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmaspi (Id,    EmpBirth,  BirthPlace,  Sex_,   CivStat_,   Citizen,  Religion,    Height,  Weight, 
                                                       Hair,  Eyes,      Complexion,  Marks,  BloodType,  Spouse,   Occupation,  NoChildren) values 
                                                      (@Id,   @EmpBirth, @BirthPlace, @Sex_,  @CivStat_,  @Citizen, @Religion,   @Height, @Weight, 
                                                       @Hair, @Eyes,     @Complexion, @Marks, @BloodType, @Spouse,  @Occupation, @NoChildren) ; 
                        SELECT * FROM {schema}.Empmaspi WHERE Id = @Id; ";
        var res = await _sql.FetchData<EmpmasPIModel?, dynamic>(sql, empmaspi, conn);
        return res.FirstOrDefault();
    }


    public async Task<EmpmasPIModel?> _02EmpmasPI(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Empmaspi where Id = @Id";
        var data = await _sql.FetchData<EmpmasPIModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<EmpmasPIModel?>?> _02EmpmasPIs(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Empmaspi where Id = @Id";
        var data = await _sql.FetchData<EmpmasPIModel?, dynamic>(sql, new { Id = id }, conn);
        return data;
    }


    public async Task<EmpmasPIModel?> _03EmpmasPI(int id, EmpmasPIModel empmaspi, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmaspi set 
                            EmpBirth    = @EmpBirth, 
                            BirthPlace  = @BirthPlace,  
                            Sex_        = @Sex_,  
                            CivStat_    = @CivStat_,  
                            Citizen     = @Citizen,  
                            Religion    = @Religion,  
                            Height      = @Height,  
                            HeightInch  = @HeightInch,  
                            Weight      = @Weight,  
                            Hair        = @Hair,  
                            Eyes        = @Eyes,  
                            Complexion  = @Complexion,  
                            Marks       = @Marks,  
                            BloodType   = @BloodType,  
                            Spouse      = @Spouse,  
                            Occupation  = @Occupation,  
                            NoChildren  = @NoChildren where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, empmaspi, conn);

        sql = $@" select  * from {schema}.Empmaspi x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasPIModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasPIModel?> _04EmpmasPI(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmaspi where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Empmaspi x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasPIModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    // *********************************************************************************************
    // --- EmpmasRelatives *************************************************************************
    //**********************************************************************************************
    public async Task<EmpmasRelativesModel?> _01EmpmasRelatives(EmpmasRelativesModel empmasrelatives, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmasrelatives (EmpmasId,  Name,  Birth,  Sex,  RelativesRefCode) values 
                                                             (@EmpmasId, @Name, @Birth, @Sex, @RelativesRefCode)";
        await _sql.ExecuteCmd<dynamic>(sql, empmasrelatives, conn);

        sql = $@"SELECT * FROM {schema}.Empmasrelatives WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<EmpmasRelativesModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();
    }


    public async Task<EmpmasRelativesModel?> _02EmpmasRelatives(int id, string schema, string conn)
    {
        string sql = $@"select  Id, EmpmasId, Name, Birth, Sex, RelativesRefCode from {schema}.Empmasrelatives where Id = @Id";
        var data = await _sql.FetchData<EmpmasRelativesModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<EmpmasRelativesModel?>> _02EmpmasRelativesList(int empmasId, string schema, string conn)
    {
        string sql = $@"select  Id, EmpmasId, Name, Birth, Sex, RelativesRefCode from {schema}.Empmasrelatives where EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<EmpmasRelativesModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data;
    }


    public async Task<EmpmasRelativesModel?> _03EmpmasRelatives(int id, EmpmasRelativesModel empmasrelatives, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmasrelatives set 
                            EmpmasId = @EmpmasId, 
                            Name    = @Name,  
                            Birth   = @Birth,  
                            Sex     = @Sex,  
                            RelativesRefCode = @RelativesRefCode where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, empmasrelatives, conn);

        sql = $@" select  * from {schema}.Empmasrelatives x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasRelativesModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasRelativesModel?> _04EmpmasRelatives(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmasrelatives where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Empmasrelatives x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasRelativesModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // *********************************************************************************************
    // --- EmpmasRelativesRef **********************************************************************
    //**********************************************************************************************
    public async Task<EmpmasRelativesRefModel?> _01EmpmasRelativesRef(string code, string name, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmasrelativesref (Code, Name) values (@Code, @Name)";
        await _sql.ExecuteCmd<dynamic>(sql, new { Code = code, Name = name }, conn);

        sql = $@"SELECT * FROM {schema}.Empmasrelativesref WHERE Code = @Code";
        var res = await _sql.FetchData<EmpmasRelativesRefModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();
    }


    public async Task<EmpmasRelativesRefModel?> _02EmpmasRelativesRef(string code, string schema, string conn)
    {
        string sql = $@"select  Code, Name from {schema}.Empmasrelativesref where Code = @Code";
        var data = await _sql.FetchData<EmpmasRelativesRefModel?, dynamic>(sql, new { Code = code }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<EmpmasRelativesRefModel?> _03EmpmasRelativesRef(string code, string name, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmasrelativesref set Name = @Name where Code = @Code;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Code = code, Name = name }, conn);

        sql = $@" select  * from {schema}.Empmasrelativesref where Code = @Code;";
        var data = await _sql.FetchData<EmpmasRelativesRefModel?, dynamic>(sql, new { Code = code }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasRelativesRefModel?> _04EmpmasRelativesRef(string code, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmasrelativesref where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Code = code }, conn);

        sql = $@" select  * from {schema}.Empmasrelativesref x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasRelativesRefModel?, dynamic>(sql, new { Code = code }, conn);
        return data?.FirstOrDefault();
    }

    // *********************************************************************************************
    // --- EmpmasSecLic *****************************************************************************
    //**********************************************************************************************
    public async Task<EmpmasSecLicModel?> _01EmpmasSecLic(int id, EmpmasSecLicModel empmasseclic, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmasseclic (Id,  SecLicense,  LicExpire,  BadgeNo,  SbrNo,  OpNo,  Validated,  VFee,  Revalidated,  ValStatus) values 
                                                          (@Id, @SecLicense, @LicExpire, @BadgeNo, @SbrNo, @OpNo, @Validated, @VFee, @Revalidated, @ValStatus)";
        await _sql.ExecuteCmd<dynamic>(sql, empmasseclic, conn);

        sql = $@"SELECT * FROM {schema}.Empmasseclic WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<EmpmasSecLicModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();
    }


    public async Task<EmpmasSecLicModel?> _02EmpmasSecLic(int id, string schema, string conn)
    {
        string sql = $@"select  Id, SecLicense, LicExpire, BadgeNo, SbrNo, OpNo, Validated, VFee, Revalidated, ValStatus from {schema}.Empmasseclic where Id = @Id";
        var data = await _sql.FetchData<EmpmasSecLicModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<EmpmasSecLicModel?> _03EmpmasSecLic(int id, EmpmasSecLicModel empmasseclic, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmasseclic set 
                            SecLicense  = @SecLicense,  LicExpire   = @LicExpire, 
                            BadgeNo     = @BadgeNo,     SbrNo       = @SbrNo,  
                            OpNo        = @OpNo,        Validated   = @Validated,  
                            VFee        = @VFee,        Revalidated = @Revalidated,  
                            ValStatus = @ValStatus where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, empmasseclic, conn);

        sql = $@" select  * from {schema}.Empmasseclic x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasSecLicModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasSecLicModel?> _04EmpmasSecLic(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmasseclic where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Empmasseclic x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasSecLicModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // *********************************************************************************************
    // --- EmpmasTraining **************************************************************************
    //**********************************************************************************************
    public async Task<EmpmasTrainingModel?> _01EmpmasTraining(EmpmasTrainingModel empmastraining, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmastraining (EmpmasId,  ProgramName,  DateTaken,  DateStart,  DateEnd,  TotalHrs,  TotalDays,  School,  Trainor) values 
                                                            (@EmpmasId, @ProgramName, @DateTaken, @DateStart, @DateEnd, @TotalHrs, @TotalDays, @School, @Trainor)";
        await _sql.ExecuteCmd<dynamic>(sql, empmastraining, conn);

        sql = $@"SELECT * FROM {schema}.Empmastraining WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<EmpmasTrainingModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();
    }


    public async Task<EmpmasTrainingModel?> _02EmpmasTraining(int id, string schema, string conn)
    {

        string sql = $@"select  Id, EmpmasId, ProgramName, DateTaken, DateStart, DateEnd, TotalHrs, TotalDays, School, Trainor from {schema}.Empmastraining where Id = @Id";
        var data = await _sql.FetchData<EmpmasTrainingModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();

    }
    public async Task<List<EmpmasTrainingModel?>> _02EmpmasTrainingList(int empmasId, string schema, string conn)
    {

        string sql = $@"select  Id, EmpmasId, ProgramName, DateTaken, DateStart, DateEnd, TotalHrs, TotalDays, School, Trainor from {schema}.Empmastraining where EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<EmpmasTrainingModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data;

    }


    public async Task<EmpmasTrainingModel?> _03EmpmasTraining(int id, EmpmasTrainingModel empmastraining, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmastraining set 
                            EmpmasId    = @EmpmasId, 
                            ProgramName = @ProgramName,  
                            DateTaken   = @DateTaken,  
                            DateStart   = @DateStart,  
                            DateEnd     = @DateEnd,  
                            TotalHrs    = @TotalHrs,  
                            TotalDays   = @TotalDays,  
                            School      = @School,  
                            Trainor     = @Trainor where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, empmastraining, conn);

        sql = $@" select  * from {schema}.Empmastraining x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasTrainingModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasTrainingModel?> _04EmpmasTraining(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmastraining where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Empmastraining x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasTrainingModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // *********************************************************************************************
    // --- Deprec **********************************************************************************
    //**********************************************************************************************
    public async Task<DeprecModel?> _01Deprec(DeprecModel deprec, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Deprec 
                        (EmpmasId,  DivId,  DepId,  SecId,  PositionId,  LeavegrpId,  PayrollgrpId,  EmploymentTypeId,  EmpStatusId,  DHired,  DRegularization,  DTraineeStart,  DTraineeEnd,  DContractualStart,  DContractualEnd,  DProbationaryStart,  DProbationaryEnd,  DRegularizationStart,  DRegularizationEnd,  DPermanentStart,  DResigned,  DTerminated,  DSeparated,  Remarks) values 
                        (@EmpmasId, @DivId, @DepId, @SecId, @PositionId, @LeavegrpId, @PayrollgrpId, @EmploymentTypeId, @EmpStatusId, @DHired, @DRegularization, @DTraineeStart, @DTraineeEnd, @DContractualStart, @DContractualEnd, @DProbationaryStart, @DProbationaryEnd, @DRegularizationStart, @DRegularizationEnd, @DPermanentStart, @DResigned, @DTerminated, @DSeparated, @Remarks); 
                        
                        select  d.*, p.Name Positionname, s.Name Empstatusname  
                        from {schema}.Deprec d 
                        left join {schema}.Position p on p.Id = d.Positionid 
                        left join {schema}.REmpstat s on s.Id = d.Empstatusid 
                        where d.EmpmasId = @EmpmasId;";
        var res = await _sql.FetchData<DeprecModel?, dynamic>(sql, deprec, conn);
        return res.FirstOrDefault();
    }


    public async Task<DeprecModel?> _02Deprec(int id, string schema, string conn)
    {
        string sql = $@"select  d.*, p.Name Positionname, s.Name Empstatusname  
                        from {schema}.Deprec d 
                        left join {schema}.Position p on p.Id = d.Positionid 
                        left join {schema}.REmpstat s on s.Id = d.Empstatusid 
                        where d.EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<DeprecModel?, dynamic>(sql, new { EmpmasId = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<List<DeprecModel?>?> _02Deprecs(int id, string schema, string conn)
    {
        string sql = $@"select  d.*, p.Name Positionname, s.Name Empstatusname  
                        from {schema}.Deprec d 
                        left join {schema}.Position p on p.Id = d.Positionid 
                        left join {schema}.REmpstat s on s.Id = d.Empstatusid 
                        where d.EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<DeprecModel?, dynamic>(sql, new { EmpmasId = id }, conn);
        return data;
    }


    public async Task<DeprecModel?> _03Deprec(DeprecModel deprec, string schema, string conn)
    {
        string sql = $@"Update {schema}.Deprec set 
                            DivId           = @DivId, 
                            DepId           = @DepId, 
                            SecId           = @SecId, 
                            PositionId          = @PositionId, 
                            LeavegrpId          = @LeavegrpId, 
                            PayrollgrpId        = @PayrollgrpId, 
                            EmploymentTypeId    = @EmploymentTypeId, 
                            EmpStatusId         = @EmpStatusId, 
                            DHired              = @DHired, 
                            DRegularization     = @DRegularization, 
                            DTraineeStart       = @DTraineeStart, 
                            DTraineeEnd         = @DTraineeEnd, 
                            DContractualStart   = @DContractualStart, 
                            DContractualEnd     = @DContractualEnd, 
                            DProbationaryStart  = @DProbationaryStart, 
                            DProbationaryEnd    = @DProbationaryEnd, 
                            DRegularizationStart = @DRegularizationStart, 
                            DRegularizationEnd  = @DRegularizationEnd, 
                            DPermanentStart     = @DPermanentStart, 
                            DResigned           = @DResigned, 
                            DTerminated         = @DTerminated, 
                            DSeparated          = @DSeparated, 
                            Remarks             = @Remarks 
                        where EmpmasId          = @EmpmasId;
                        
                        select  d.*, p.Name Positionname, s.Name Empstatusname  
                        from {schema}.Deprec d 
                        left join {schema}.Position p on p.Id = d.Positionid 
                        left join {schema}.REmpstat s on s.Id = d.Empstatusid 
                        where d.EmpmasId = @EmpmasId;";
        var data = await _sql.FetchData<DeprecModel?, dynamic>(sql, deprec, conn);
        return data?.FirstOrDefault();
    }

    public async Task<DeprecModel?> _04Deprec(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Deprec where EmpmasId = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { EmpmasId = id }, conn);

        sql = $@" select  * from {schema}.Deprec x where x.Id = @Id ;";
        var data = await _sql.FetchData<DeprecModel?, dynamic>(sql, new { EmpmasId = id }, conn);
        return data?.FirstOrDefault();
    }
}
