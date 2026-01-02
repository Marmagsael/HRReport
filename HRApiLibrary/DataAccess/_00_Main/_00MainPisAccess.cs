using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_MainPis;

namespace HRApiLibrary.DataAccess._00_Main;

public class _00MainPisAccess : I_00MainPisAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public _00MainPisAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    #region EmpmasRecord
    //*****************************************************************************************
    //--- EmpmasRecord ------------------------------------------------------------------------

    public async Task<EmpmasRecordModel?> _02EmpmasRecord(int id, string schema, string conn)
    {
        EmpmasRecordModel empmasRecord = new EmpmasRecordModel();

        string sql = $@"select  Id, EmpLastNm, EmpFirstNm, EmpMidNm, Suffix, EmpAlias from {schema}.Empmas where Id = @Id";
        var data = await _sql.FetchData<EmpmasModel?, dynamic>(sql, new { Id = id }, conn);

        return empmasRecord;
    }
    //--- EmpmasRecord ------------------------------------------------------------------------
    //*****************************************************************************************
    #endregion

    #region Empmas
    //*****************************************************************************************
    //--- Empmas ------------------------------------------------------------------------------

    public async Task<EmpmasModel?> _01Empmas(EmpmasModel empmas, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmas 
                    (Id,  EmpLastNm,  EmpFirstNm,  EmpMidNm,  Suffix,  EmpAlias) values 
                    (@Id, @EmpLastNm, @EmpFirstNm, @EmpMidNm, @Suffix, @EmpAlias)";
        await _sql.ExecuteCmd<dynamic>(sql, empmas, conn);

        sql = $@"SELECT * FROM {schema}.Empmas WHERE Id = @Id";
        var res = await _sql.FetchData<EmpmasModel?, dynamic>(sql, new { Id = empmas.Id }, conn);
        return res.FirstOrDefault();
    }


    public async Task<EmpmasModel?> _02Empmas(int id, string schema, string conn)
    {
        string sql = $@"select  Id, EmpLastNm, EmpFirstNm, EmpMidNm, Suffix, EmpAlias from {schema}.Empmas where Id = @Id";
        var data = await _sql.FetchData<EmpmasModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<EmpmasModel?> _03Empmas(int id, EmpmasModel empmas, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmas set 
                                EmpLastNm   = @EmpLastNm, 
                                EmpFirstNm  = @EmpFirstNm,  
                                EmpMidNm    = @EmpMidNm,  
                                Suffix      = @Suffix,  
                                EmpAlias    = @EmpAlias where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, empmas, conn);

        sql = $@"Select  * from {schema}.Empmas x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasModel?> _04Empmas(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmas where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@"Select  * from {schema}.Empmas x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();

    }

    //--- Empmas ------------------------------------------------------------------------------
    //*****************************************************************************************



    //*****************************************************************************************
    //--- EmpmasPi ------------------------------------------------------------------------------
    #endregion

    #region EmpmasPI
    public async Task<EmpmasPIModel?> _01EmpmasPI(EmpmasPIModel empmaspi, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmaspi 
                            (Id, EmpBirth, BirthPlace, Sex_, CivStat_, Citizen, Religion, Height, HeightInch, Weight, 
                             Hair, Eyes, Complexion, Marks, BloodType, Spouse, Occupation, NoChildren) values 
                        (@Id, @EmpBirth, @BirthPlace, @Sex_, @CivStat_, @Citizen, @Religion, @Height, @HeightInch, @Weight, 
                         @Hair, @Eyes, @Complexion, @Marks, @BloodType, @Spouse, @Occupation, @NoChildren)";
        await _sql.ExecuteCmd<dynamic>(sql, empmaspi, conn);

        sql = $@"SELECT * FROM {schema}.Empmaspi WHERE Id = @Id";
        //sql = $@"SELECT * FROM {schema}.Empmaspi WHERE Id = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<EmpmasPIModel?, dynamic>(sql, new { empmaspi.Id }, conn);
        return res.FirstOrDefault();
    }


    public async Task<EmpmasPIModel?> _02EmpmasPI(int id, string schema, string conn)
    {
        string sql = $@"select  Id, EmpBirth, BirthPlace, Sex_, CivStat_, Citizen, Religion, Height, HeightInch,  Weight, 
                        Hair, Eyes, Complexion, Marks, BloodType, Spouse, Occupation, NoChildren from {schema}.Empmaspi where Id = @Id";
        var data = await _sql.FetchData<EmpmasPIModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
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
    //--- EmpmasPi ------------------------------------------------------------------------------
    //*****************************************************************************************



    //*****************************************************************************************
    //--- EmpmasAddress------------------------------------------------------------------------------
    #endregion

    #region EmpmasAddress
    public async Task<EmpmasAddressModel?> _01EmpmasAddress(EmpmasAddressModel empmasaddress, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmasaddress 
                    (Id, PresAddStreet, PresAddVillage, PresAddBrgy, PresAddCityId, PresAddCity, PresAddProvId, PresAddProv, PresAddStateId,  PresAddState, PresAddCountryId, PresAddCountry, PresAddZipCode, PresAdd, PresAddTelNo, ProvAddStreet, ProvAddVillage, ProvAddBrgy, ProvAddCityId, ProvAddCity, ProvAddProvId, ProvAddProv, ProvAddStateId, ProvAddState, ProvAddCountryId, ProvAddCountry, ProvAddZipCode, ProvAdd, ProvAddTelNo, Countrycode, EmailAdd, EmailAdd1, CellNo, CellNo1) values
                    (@Id, @PresAddStreet, @PresAddVillage, @PresAddBrgy, @PresAddCityId, @PresAddCity, @PresAddProvId, @PresAddProv, @PresAddStateId, @PresAddState, @PresAddCountryId, @PresAddCountry, @PresAddZipCode, @PresAdd, @PresAddTelNo, @ProvAddStreet, @ProvAddVillage, @ProvAddBrgy, @ProvAddCityId, @ProvAddCity, @ProvAddProvId, @ProvAddProv, @ProvAddStateId, @ProvAddState, @ProvAddCountryId, @ProvAddCountry, @ProvAddZipCode, @ProvAdd, @ProvAddTelNo, @Countrycode, @EmailAdd, @EmailAdd1, @CellNo, @CellNo1)";
        await _sql.ExecuteCmd<dynamic>(sql, empmasaddress, conn);

        sql = $@"SELECT * FROM {schema}.Empmasaddress WHERE ID = @Id";

        var res = await _sql.FetchData<EmpmasAddressModel?, dynamic>(sql, new { Id = empmasaddress.Id }, conn);

        return res.FirstOrDefault();
    }


    public async Task<EmpmasAddressModel?> _02EmpmasAddress(int id, string schema, string conn)
    {
        string sql = $@"select  Id, PresAddStreet, PresAddVillage, PresAddBrgy, PresAddCityId, PresAddCity, PresAddProvId, PresAddProv, PresAddStateId, PresAddState, PresAddCountryId, PresAddCountry, PresAddZipCode, PresAdd, PresAddTelNo, ProvAddStreet, ProvAddVillage, ProvAddBrgy, ProvAddCityId, ProvAddCity, ProvAddProvId, ProvAddProv, ProvAddStateId, ProvAddState, ProvAddCountryId, ProvAddCountry, ProvAddZipCode, ProvAdd, ProvAddTelNo, Countrycode, EmailAdd, EmailAdd1, CellNo, CellNo1 from {schema}.Empmasaddress where Id = @Id";
        var data = await _sql.FetchData<EmpmasAddressModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<EmpmasAddressModel?> _03EmpmasAddress(int id, EmpmasAddressModel empmasaddress, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmasaddress set 
                                PresAddStreet       = @PresAddStreet, 
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

        sql = $@" select  * from {schema}.Empmasaddress x where x.Id = @Id ;";
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
    //--- EmpmasAddress ------------------------------------------------------------------------------
    //*****************************************************************************************



    //*****************************************************************************************
    //--- EmpmasEducate------------------------------------------------------------------------------
    #endregion

    #region EmpasEducate
    public async Task<EmpmasEducateModel?> _01EmpmasEducate(EmpmasEducateModel empmaseducate, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmaseducate (EmpmasId, Code, School, FROM_, TO_, COURSE, LEVEL) values ( @EmpmasId, @Code, @School, @FROM_, @TO_, @COURSE, @LEVEL)";
        await _sql.ExecuteCmd<dynamic>(sql, empmaseducate, conn);

        string getId = $@"SELECT * FROM {schema}.Empmaseducate WHERE ID = (SELECT MAX(id) FROM {schema}.Empmaseducate)";
        var lastId = await _sql.FetchData<EmpmasEducateModel?, dynamic>(getId, new { }, conn);

        sql = $@"SELECT * FROM {schema}.Empmaseducate WHERE ID = @Id";

        var res = await _sql.FetchData<EmpmasEducateModel?, dynamic>(sql, new { Id = lastId.FirstOrDefault()?.Id }, conn);

        return res.FirstOrDefault();

    }
    public async Task<EmpmasEducateModel?> _02EmpmasEducate(int enpmasId, string schema, string conn)
    {
        string sql = $@"select  Id, EmpmasId, Code, School, FROM_, TO_, COURSE, LEVEL from {schema}.Empmaseducate where EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<EmpmasEducateModel?, dynamic>(sql, new { EmpmasId = enpmasId }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<EmpmasEducateModel?>?> _02EmpmasEducateList(int id, string schema, string conn)
    {
        string sql = $@"select  Id, EmpmasId, Code, School, FROM_, TO_, COURSE, LEVEL from {schema}.Empmaseducate where EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<EmpmasEducateModel?, dynamic>(sql, new { EmpmasId = id }, conn);
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

    //--- EmpmasEducate------------------------------------------------------------------------
    //*****************************************************************************************


    //*****************************************************************************************
    //--- EmpmasRef----------------------------------------------------------------------------

    #endregion

    #region EmpmasEdudateRef
    public async Task<List<EmpmasEducateRefModel?>> _02EmpmasEducateRefList(string schema, string conn)
    {
        string sql = $@"Select * from {schema}.EmpmasEducateRef";
        var data = await _sql.FetchData<EmpmasEducateRefModel?, dynamic>(sql, new { }, conn);
        return data;
    }
    //--- EmpmasRef----------------------------------------------------------------------------
    //*****************************************************************************************




    //*****************************************************************************************
    //--- EmpmasFamily-------------------------------------------------------------------------

    #endregion

    #region EmpmasFamily
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
        string sql = $@"select  Id, EmpmasId, Name, Birth, Sex, RelationCode from {schema}.Empmasfamily where EmpmasId= @EmpmasId";
        var data = await _sql.FetchData<EmpmasFamilyModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data.ToList();
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
    //--- EmpmasFamily-------------------------------------------------------------------------
    //*****************************************************************************************



    //*****************************************************************************************
    //--- EmpmasFamilyRef----------------------------------------------------------------------

    public async Task<List<EmpmasFamilyRefModel?>?> _02EmpmasFamilyRefList(string schema, string conn)
    {
        string sql = $@"select * from {schema}.EmpmasFamilyRef";
        var data = await _sql.FetchData<EmpmasFamilyRefModel?, dynamic>(sql, new { }, conn);
        return data.ToList();
    }

    //--- EmpmasFamilyRef----------------------------------------------------------------------
    //*****************************************************************************************



    //*****************************************************************************************
    //--- EmpmasEmergencyContact---------------------------------------------------------------
    #endregion

    #region EmpmasEmergencyContact
    public async Task<EmpmasEmergencyContactModel?> _01EmpmasEmergencyContact(EmpmasEmergencyContactModel empmasemergencycontact, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmasemergencycontact (EmpmasId, Name, Addr, Relationship, TelNo) values (@EmpmasId, @Name, @Addr, @Relationship, @TelNo)";
        await _sql.ExecuteCmd<dynamic>(sql, empmasemergencycontact, conn);

        sql = $@"SELECT * FROM {schema}.Empmasemergencycontact WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<EmpmasEmergencyContactModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }

    public async Task<EmpmasEmergencyContactModel?> _02EmpmasEmergencyContact(int empmasId, string schema, string conn)
    {
        string sql = $@"select  Id, EmpmasId, Name, Addr, Relationship, TelNo from {schema}.Empmasemergencycontact where EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<EmpmasEmergencyContactModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<EmpmasEmergencyContactModel?>?> _02EmpmasEmergencyContacts(int empmasId, string schema, string conn)
    {
        string sql = $@"select  Id, EmpmasId, Name, Addr, Relationship, TelNo from {schema}.Empmasemergencycontact where EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<EmpmasEmergencyContactModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data;
    }


    public async Task<EmpmasEmergencyContactModel?> _03EmpmasEmergencyContact(int empmasId, EmpmasEmergencyContactModel empmasemergencycontact, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmasemergencycontact set 
                            EmpmasId        = @EmpmasId,  
                            Name            = @Name,  
                            Addr            = @Addr,  
                            Relationship    = @Relationship,  
                            TelNo           = @TelNo where EmpmasId = @EmpmasId;";
        await _sql.ExecuteCmd<dynamic>(sql, empmasemergencycontact, conn);

        sql = $@" select  * from {schema}.Empmasemergencycontact x where x.EmpmasId = @EmpmasId ;";
        var data = await _sql.FetchData<EmpmasEmergencyContactModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasFamilyModel?> _04EmpmasEmergencyContact(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmasemergencycontact where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Empmasfamily x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasFamilyModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    //--- EmpmasEmergencyContact---------------------------------------------------------------
    //*****************************************************************************************


    //*****************************************************************************************
    //--- EmpmasRelative----------------------------------------------------------------------
    #endregion

    #region EmpmasRelatives
    public async Task<EmpmasRelativesModel?> _01EmpmasRelatives(EmpmasRelativesModel empmasrelatives, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmasrelatives (EmpmasId, Name, Birth, Sex, RelativesRefCode) values (@EmpmasId, @Name, @Birth, @Sex, @RelativesRefCode)";
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

    public async Task<List<EmpmasRelativesModel?>?> _02EmpmasRelativesList(int empmasId, string schema, string conn)
    {
        string sql = $@"select  Id, EmpmasId, Name, Birth, Sex, RelativesRefCode from {schema}.Empmasrelatives where EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<EmpmasRelativesModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data?.ToList();
    }

    public async Task<EmpmasRelativesModel?> _03EmpmasRelatives(int id, EmpmasRelativesModel empmasrelatives, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmasrelatives set EmpmasId = @EmpmasId, Name = @Name, Birth = @Birth, Sex = @Sex, RelativesRefCode = @RelativesRefCode where Id = @Id;";
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
    //--- EmpmasRelative---------------------------------------------------------------
    //*****************************************************************************************

    //*****************************************************************************************
    //--- EmpmasRelativeReference-------------------------------------------------------------

    public async Task<List<EmpmasRelativesRefModel?>?> _02EmpmasRelativeReferenceList(string schema, string conn)
    {
        string sql = $@"select * from {schema}.empmasrelativesref";
        var data = await _sql.FetchData<EmpmasRelativesRefModel?, dynamic>(sql, new { }, conn);
        return data.ToList();
    }

    //--- EmpmasRelativeReference---------------------------------------------------------------
    //*****************************************************************************************

    //*****************************************************************************************
    //--- EmpmasCharacterReference-------------------------------------------------------------
    #endregion

    #region EmpmasCharRef
    public async Task<EmpmasCharRefModel?> _01EmpmasCharacterReference(EmpmasCharRefModel empmasrelatives, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmascharref 
                            (EmpmasId,  Name,  Addr,  Tel,  Position) values 
                            (@EmpmasId, @Name, @Addr, @Tel, @Position)";
        await _sql.ExecuteCmd<dynamic>(sql, empmasrelatives, conn);

        sql = $@"SELECT * FROM {schema}.Empmascharref WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<EmpmasCharRefModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }

    public async Task<EmpmasCharRefModel?> _02EmpmasCharacterReference(int id, string schema, string conn)
    {
        string sql = $@"select  Id, EmpmasId, Name, Addr, Tel, Position from {schema}.Empmascharref where Id = @Id";
        var data = await _sql.FetchData<EmpmasCharRefModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();

    }

    public async Task<List<EmpmasCharRefModel?>?> _02EmpmasCharacterReferenceList(int empmasId, string schema, string conn)
    {
        string sql = $@"select  Id, EmpmasId, Name, Addr, Tel, Position from {schema}.EmpmasCharRef where EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<EmpmasCharRefModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data?.ToList();
    }

    public async Task<EmpmasCharRefModel?> _03EmpmasCharacterReference(int id, EmpmasCharRefModel empmasrelatives, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmascharref set 
                                Name       = @Name, 
                                Addr       = @Addr,  
                                Tel        = @Tel,  
                                Position   = @Position where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, empmasrelatives, conn);

        sql = $@" select  * from {schema}.Empmascharref x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasCharRefModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasCharRefModel?> _04EmpmasCharacterReference(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmascharref where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Empmascharref x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasCharRefModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    //*****************************************************************************************
    //--- EmpmasCharacterReference-------------------------------------------------------------


    //*****************************************************************************************
    //--- EmpmasTrainings-------------------------------------------------------------
    #endregion

    #region EmpmasTraining
    public async Task<EmpmasTrainingModel?> _01EmpmasTrainings(EmpmasTrainingModel empmastraining, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmastraining (EmpmasId, ProgramName, DateTaken, DateStart, DateEnd, TotalHrs, TotalDays, School, Trainor) values (@EmpmasId, @ProgramName, @DateTaken, @DateStart, @DateEnd, @TotalHrs, @TotalDays, @School, @Trainor)";
        await _sql.ExecuteCmd<dynamic>(sql, empmastraining, conn);

        sql = $@"SELECT * FROM {schema}.Empmastraining WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<EmpmasTrainingModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }
    public async Task<EmpmasTrainingModel?> _02EmpmasTrainings(int id, string schema, string conn)
    {
        string sql = $@"select  Id, EmpmasId, ProgramName, DateTaken, DateStart, DateEnd, TotalHrs, TotalDays, School, Trainor from {schema}.Empmastraining where Id = @Id";
        var data = await _sql.FetchData<EmpmasTrainingModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<EmpmasTrainingModel?>?> _02EmpmasTrainingsList(int empmasId, string schema, string conn)
    {
        string sql = $@"select  Id, EmpmasId, ProgramName, DateTaken, DateStart, DateEnd, TotalHrs, TotalDays, School, Trainor from {schema}.Empmastraining where EmpmasId = @Id";
        var data = await _sql.FetchData<EmpmasTrainingModel?, dynamic>(sql, new { Id = empmasId }, conn);
        return data;
    }

    public async Task<EmpmasTrainingModel?> _03EmpmasTrainings(int id, EmpmasTrainingModel empmastraining, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmastraining set EmpmasId = @EmpmasId, ProgramName = @ProgramName, DateTaken = @DateTaken, DateStart = @DateStart, DateEnd = @DateEnd, TotalHrs = @TotalHrs, TotalDays = @TotalDays, School = @School, Trainor = @Trainor where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, empmastraining, conn);

        sql = $@" select  * from {schema}.Empmastraining x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasTrainingModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasTrainingModel?> _04EmpmasTrainings(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmastraining where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Empmastraining x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasTrainingModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    //--- EmpmasTrainings---------------------------------------------------------------------
    //*****************************************************************************************

    #endregion

    #region EmpmasEmployment 
    public async Task<EmpmasEmploymentModel?> _01EmpmasEmployment(EmpmasEmploymentModel employment, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.EmpmasEmployment (EmpmasId, CompName, Address, Tel, Pos, From_, To_, Sal, Rem) 
                                                       Values (@EmpmasId, @CompName, @Address, @Tel, @Pos, @From_, @To_, @Sal, @Rem)";
        await _sql.ExecuteCmd<dynamic>(sql, employment, conn);

        sql = $@"SELECT * FROM {schema}.EmpmasEmployment WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<EmpmasEmploymentModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }

    public async Task<EmpmasEmploymentModel?> _02EmpmasEmployment(int id, string schema, string conn)
    {
        string sql = $@"SELECT * FROM {schema}.EmpmasEmployment WHERE Id = @Id";
        var data = await _sql.FetchData<EmpmasEmploymentModel?, dynamic> (sql, new { Id = id }, conn);
        return data.FirstOrDefault();
    }

    public async Task<List<EmpmasEmploymentModel?>?> _02EmpmasEmploymentList(int empmasId, string schema, string conn)
    {
        string sql = $@"SELECT * FROM {schema}.EmpmasEmployment WHERE EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<EmpmasEmploymentModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data;
    }

    public async Task<EmpmasEmploymentModel?> _03EmpmasEmployment(int id, EmpmasEmploymentModel employment, string schema, string conn)
    {
        string sql = $@"UPDATE {schema}.EmpmasEmployment SET  CompName = @CompName, Address = @Address, Tel = @Tel, Pos = @Pos, From_ = @From_, To_ = @To_, Sal = @Sal, Rem = @Rem 
                    WHERE Id =@Id";
        await _sql.ExecuteCmd<dynamic>(sql, employment, conn);

        sql = $@" SELECT  * FROM {schema}.EmpmasEmployment x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasEmploymentModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasEmploymentModel?> _04EmpmasEmployment(int id, string schema, string conn)
    {
        // Construct the SQL DELETE command
        string sql = $@"DELETE FROM {schema}.EmpmasEmployment WHERE Id = @Id";
        await _sql.ExecuteCmd<dynamic>(sql, new {Id = id}, conn);

        sql = $@" SELECT  * FROM {schema}.EmpmasEmployment x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasEmploymentModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    #endregion
}
