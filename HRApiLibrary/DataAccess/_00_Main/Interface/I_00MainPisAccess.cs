using HRApiLibrary.Models._00_MainPis;

namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface I_00MainPisAccess
    {
        Task<EmpmasModel?> _01Empmas(EmpmasModel empmas, string schema, string conn);
        Task<EmpmasAddressModel?> _01EmpmasAddress(EmpmasAddressModel empmasaddress, string schema, string conn);
        Task<EmpmasCharRefModel?> _01EmpmasCharacterReference(EmpmasCharRefModel empmasrelatives, string schema, string conn);
        Task<EmpmasEducateModel?> _01EmpmasEducate(EmpmasEducateModel empmaseducate, string schema, string conn);
        Task<EmpmasEmergencyContactModel?> _01EmpmasEmergencyContact(EmpmasEmergencyContactModel empmasemergencycontact, string schema, string conn);
        Task<EmpmasEmploymentModel?> _01EmpmasEmployment(EmpmasEmploymentModel employment, string schema, string conn);
        Task<EmpmasFamilyModel?> _01EmpmasFamily(EmpmasFamilyModel empmasfamily, string schema, string conn);
        Task<EmpmasPIModel?> _01EmpmasPI(EmpmasPIModel empmaspi, string schema, string conn);
        Task<EmpmasRelativesModel?> _01EmpmasRelatives(EmpmasRelativesModel empmasrelatives, string schema, string conn);
        Task<EmpmasTrainingModel?> _01EmpmasTrainings(EmpmasTrainingModel empmastraining, string schema, string conn);
        Task<EmpmasModel?> _02Empmas(int id, string schema, string conn);
        Task<EmpmasAddressModel?> _02EmpmasAddress(int id, string schema, string conn);
        Task<EmpmasCharRefModel?> _02EmpmasCharacterReference(int id, string schema, string conn);
        Task<List<EmpmasCharRefModel?>?> _02EmpmasCharacterReferenceList(int empmasId, string schema, string conn);
        Task<EmpmasEducateModel?> _02EmpmasEducate(int enpmasId, string schema, string conn);
        Task<List<EmpmasEducateModel?>?> _02EmpmasEducateList(int id, string schema, string conn);
        Task<List<EmpmasEducateRefModel?>> _02EmpmasEducateRefList(string schema, string conn);
        Task<EmpmasEmergencyContactModel?> _02EmpmasEmergencyContact(int empmasId, string schema, string conn);
        Task<List<EmpmasEmergencyContactModel?>?> _02EmpmasEmergencyContacts(int empmasId, string schema, string conn);
        Task<EmpmasEmploymentModel?> _02EmpmasEmployment(int id, string schema, string conn);
        Task<List<EmpmasEmploymentModel?>?> _02EmpmasEmploymentList(int empmasId, string schema, string conn);
        Task<EmpmasFamilyModel?> _02EmpmasFamily(int id, string schema, string conn);
        Task<List<EmpmasFamilyModel?>> _02EmpmasFamilyList(int empmasId, string schema, string conn);
        Task<List<EmpmasFamilyRefModel?>?> _02EmpmasFamilyRefList(string schema, string conn);
        Task<EmpmasPIModel?> _02EmpmasPI(int id, string schema, string conn);
        Task<EmpmasRecordModel?> _02EmpmasRecord(int id, string schema, string conn);
        Task<List<EmpmasRelativesRefModel?>?> _02EmpmasRelativeReferenceList(string schema, string conn);
        Task<EmpmasRelativesModel?> _02EmpmasRelatives(int id, string schema, string conn);
        Task<List<EmpmasRelativesModel?>?> _02EmpmasRelativesList(int empmasId, string schema, string conn);
        Task<EmpmasTrainingModel?> _02EmpmasTrainings(int id, string schema, string conn);
        Task<List<EmpmasTrainingModel?>?> _02EmpmasTrainingsList(int empmasId, string schema, string conn);
        Task<EmpmasModel?> _03Empmas(int id, EmpmasModel empmas, string schema, string conn);
        Task<EmpmasAddressModel?> _03EmpmasAddress(int id, EmpmasAddressModel empmasaddress, string schema, string conn);
        Task<EmpmasCharRefModel?> _03EmpmasCharacterReference(int id, EmpmasCharRefModel empmasrelatives, string schema, string conn);
        Task<EmpmasEducateModel?> _03EmpmasEducate(int id, EmpmasEducateModel empmaseducate, string schema, string conn);
        Task<EmpmasEmergencyContactModel?> _03EmpmasEmergencyContact(int empmasId, EmpmasEmergencyContactModel empmasemergencycontact, string schema, string conn);
        Task<EmpmasEmploymentModel?> _03EmpmasEmployment(int id, EmpmasEmploymentModel employment, string schema, string conn);
        Task<EmpmasFamilyModel?> _03EmpmasFamily(int id, EmpmasFamilyModel empmasfamily, string schema, string conn);
        Task<EmpmasPIModel?> _03EmpmasPI(int id, EmpmasPIModel empmaspi, string schema, string conn);
        Task<EmpmasRelativesModel?> _03EmpmasRelatives(int id, EmpmasRelativesModel empmasrelatives, string schema, string conn);
        Task<EmpmasTrainingModel?> _03EmpmasTrainings(int id, EmpmasTrainingModel empmastraining, string schema, string conn);
        Task<EmpmasModel?> _04Empmas(int id, string schema, string conn);
        Task<EmpmasAddressModel?> _04EmpmasAddress(int id, string schema, string conn);
        Task<EmpmasCharRefModel?> _04EmpmasCharacterReference(int id, string schema, string conn);
        Task<EmpmasEducateModel?> _04EmpmasEducate(int id, string schema, string conn);
        Task<EmpmasFamilyModel?> _04EmpmasEmergencyContact(int id, string schema, string conn);
        Task<EmpmasEmploymentModel?> _04EmpmasEmployment(int id, string schema, string conn);
        Task<EmpmasFamilyModel?> _04EmpmasFamily(int id, string schema, string conn);
        Task<EmpmasPIModel?> _04EmpmasPI(int id, string schema, string conn);
        Task<EmpmasRelativesModel?> _04EmpmasRelatives(int id, string schema, string conn);
        Task<EmpmasTrainingModel?> _04EmpmasTrainings(int id, string schema, string conn);
    }
}