using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.Models._00_MainPis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HRApiLibrary.Controllers._00_Main
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class HR_00_003EmpmasController : ControllerBase
    {

        private readonly I_10_EmpmasDataAccess _empmas;

        public HR_00_003EmpmasController(I_10_EmpmasDataAccess empmas)
        {
            _empmas = empmas;
        }

        // *********************************************************************************************
        // --- Empmas **********************************************************************************
        //**********************************************************************************************
        [HttpPut("01Empmas/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasModel?>> _01Empmas([FromBody] EmpmasModel empmas, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._01Empmas(empmas, schema, conn);
            return Ok(res);

        }


        [HttpGet("02Empmas/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasModel?>> _02Empmas(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02Empmas(id, schema, conn);
            return Ok(res);
        }

        [HttpPost("03Empmas/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasModel?>> _03Empmas([FromBody] EmpmasModel empmas, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._03Empmas(id, empmas, schema, conn);
            return Ok(res);
        }

        [HttpDelete("04Empmas/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasModel?>> _04Empmas(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._04Empmas(id, schema, conn);
            return Ok(res);
        }



        // *********************************************************************************************
        // --- EmpmasAddress ***************************************************************************
        //**********************************************************************************************
        [HttpPut("01EmpmasAddress/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasAddressModel?>> _01EmpmasAddress([FromBody] EmpmasAddressModel empmasAddress, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._01EmpmasAddress(id, empmasAddress, schema, conn);
            return Ok(res);

        }

        [HttpGet("02EmpmasAddress/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasAddressModel?>> _02EmpmasAddress(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasAddress(id, schema, conn);
            return Ok(res);

        }

        [HttpPost("03EmpmasAddress/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasAddressModel?>> _03EmpmasAddress([FromBody] EmpmasAddressModel empmasAddress, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._03EmpmasAddress(id, empmasAddress, schema, conn);
            return Ok(res);

        }

        [HttpDelete("04EmpmasAddress/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasAddressModel?>> _04EmpmasAddress(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._04EmpmasAddress(id, schema, conn);
            return Ok(res);

        }

        // *********************************************************************************************
        // --- EmpmasCharRef ***************************************************************************
        //**********************************************************************************************
        [HttpPost("01EmpmasCharRef/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasCharRefModel?>> _01EmpmasCharRef([FromBody] EmpmasCharRefModel empmasCharRef, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._01EmpmasCharRef(empmasCharRef, schema, conn);
            return Ok(res);

        }

        [HttpGet("02EmpmasCharRef/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasCharRefModel?>> _02EmpmasCharRef(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasCharRef(id, schema, conn);
            return Ok(res);
        }

        [HttpGet("02EmpmasCharRefList/{empmasId}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasCharRefModel?>> _02EmpmasCharRefList(int empmasId, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasCharRefList(empmasId, schema, conn);
            return Ok(res);
        }

        [HttpPut("03EmpmasCharRef/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasCharRefModel?>> _03EmpmasCharRef([FromBody] EmpmasCharRefModel empmasCharRef, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._03EmpmasCharRef(id, empmasCharRef, schema, conn);
            return Ok(res);

        }

        [HttpDelete("04EmpmasCharRef/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasCharRefModel?>> _04EmpmasCharRef(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._04EmpmasCharRef(id, schema, conn);
            return Ok(res);

        }

        // *********************************************************************************************
        // --- EmpmasClearancePh ***************************************************************************
        //**********************************************************************************************
        [HttpPut("01EmpmasClearancePh/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasClearancePhModel?>> _01EmpmasClearancePh([FromBody] EmpmasClearancePhModel empmasClearancePh, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._01EmpmasClearancePh(id, empmasClearancePh, schema, conn);
            return Ok(res);
        }

        [HttpGet("02EmpmasClearancePh/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasClearancePhModel?>> _02EmpmasClearancePh(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasClearancePh(id, schema, conn);
            return Ok(res);
        }

        [HttpPost("03EmpmasClearancePh/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasClearancePhModel?>> _03EmpmasClearancePh([FromBody] EmpmasClearancePhModel empmasClearancePh, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._03EmpmasClearancePh(id, empmasClearancePh, schema, conn);
            return Ok(res);
        }

        [HttpDelete("04EmpmasClearancePh/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasClearancePhModel?>> _04EmpmasClearancePh(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._04EmpmasClearancePh(id, schema, conn);
            return Ok(res);
        }


        // *********************************************************************************************
        // --- EmpmasEducate ***************************************************************************
        //**********************************************************************************************
        [HttpPost("01EmpmasEducate/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasEducateModel?>> _01EmpmasEducate([FromBody] EmpmasEducateModel empmasEducate, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._01EmpmasEducate(empmasEducate, schema, conn);
            return Ok(res);
        }

        [HttpGet("02EmpmasEducate/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasEducateModel?>> _02EmpmasEducate(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasEducate(id, schema, conn);
            return Ok(res);
        }
        [HttpGet("02EmpmasEducateList/{empmasId}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasEducateModel?>> _02EmpmasEducateList(int empmasId, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasEducateList(empmasId, schema, conn);
            return Ok(res);
        }

        [HttpPut("03EmpmasEducate/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasEducateModel?>> _03EmpmasEducate([FromBody] EmpmasEducateModel empmasEducate, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._03EmpmasEducate(id, empmasEducate, schema, conn);
            return Ok(res);
        }

        [HttpDelete("04EmpmasEducate/{id}/{schema}/{conn}")]
        public async Task<ActionResult> _04EmpmasEducate(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
             await _empmas._04EmpmasEducate(id, schema, conn);
            return Ok();
        }


        // *********************************************************************************************
        // --- EmpmasEducateRef ***************************************************************************
        //**********************************************************************************************
        [HttpPut("01EmpmasEducateRef/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasEducateRefModel?>> _01EmpmasEducateRef([FromBody] EmpmasEducateRefModel empmasEducateRef, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._01EmpmasEducateRef(id, empmasEducateRef, schema, conn);
            return Ok(res);

        }

        [HttpGet("02EmpmasEducateRef/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasEducateRefModel?>> _02EmpmasEducateRef(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasEducateRef(id, schema, conn);
            return Ok(res);
        }

        [HttpPost("03EmpmasEducateRef/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasEducateRefModel?>> _03EmpmasEducateRef([FromBody] EmpmasEducateRefModel empmasEducateRef, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._03EmpmasEducateRef(id, empmasEducateRef, schema, conn);
            return Ok(res);

        }

        [HttpDelete("04EmpmasEducateRef/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasEducateRefModel?>> _04EmpmasEducateRef(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._04EmpmasEducateRef(id, schema, conn);
            return Ok(res);

        }

        // *********************************************************************************************
        // --- EmpmasEmployment ***************************************************************************
        //**********************************************************************************************
        [HttpPut("01EmpmasEmployment/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasEmploymentModel?>> _01EmpmasEmployment([FromBody] EmpmasEmploymentModel empmasEmployment, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._01EmpmasEmployment(empmasEmployment, schema, conn);
            return Ok(res);
        }

        [HttpGet("02EmpmasEmployment/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasEmploymentModel?>> _02EmpmasEmployment(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasEmployment(id, schema, conn);
            return Ok(res);
        }
        [HttpGet("02EmpmasEmploymentList/{empmasId}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasEmploymentModel?>> _02EmpmasEmploymentList(int empmasId, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasEmploymentList(empmasId, schema, conn);
            return Ok(res);
        }

        [HttpPost("03EmpmasEmployment/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasEmploymentModel?>> _03EmpmasEmployment([FromBody] EmpmasEmploymentModel empmasEmployment, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._03EmpmasEmployment(id, empmasEmployment, schema, conn);
            return Ok(res);
        }

        [HttpDelete("04EmpmasEmployment/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasEmploymentModel?>> _04EmpmasEmployment(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._04EmpmasEmployment(id, schema, conn);
            return Ok(res);
        }


        // *********************************************************************************************
        // --- EmpmasFamily ***************************************************************************
        //**********************************************************************************************
        [HttpPut("01EmpmasFamily/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasFamilyModel?>> _01EmpmasFamily([FromBody] EmpmasFamilyModel empmasFamily, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._01EmpmasFamily(empmasFamily, schema, conn);
            return Ok(res);
        }

        [HttpGet("02EmpmasFamily/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasFamilyModel?>> _02EmpmasFamily(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasFamily(id, schema, conn);
            return Ok(res);
        }
        [HttpGet("02EmpmasFamilyList/{empmasId}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasFamilyModel?>> _02EmpmasFamilyList(int empmasId, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasFamilyList(empmasId, schema, conn);
            return Ok(res);
        }

        [HttpPost("03EmpmasFamily/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasFamilyModel?>> _03EmpmasFamily([FromBody] EmpmasFamilyModel empmasFamily, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._03EmpmasFamily(id, empmasFamily, schema, conn);
            return Ok(res);
        }

        [HttpDelete("04EmpmasFamily/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasFamilyModel?>> _04EmpmasFamily(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._04EmpmasFamily(id, schema, conn);
            return Ok(res);
        }

        // *********************************************************************************************
        // --- EmpmasFamilyRef ***************************************************************************
        //**********************************************************************************************
        [HttpPut("01EmpmasFamilyRef/{code}/{name}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasFamilyRefModel?>> _01EmpmasFamilyRef(string code, string name, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._01EmpmasFamilyRef(code, name, schema, conn);
            return Ok(res);

        }

        [HttpGet("02EmpmasFamilyRef/{code}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasFamilyRefModel?>> _02EmpmasFamilyRef(string code, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasFamilyRef(code, schema, conn);
            return Ok(res);
        }

        [HttpPost("03EmpmasFamilyRef/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasFamilyRefModel?>> _03EmpmasFamilyRef(string code, string name, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._03EmpmasFamilyRef(code, name, schema, conn);
            return Ok(res);

        }

        [HttpDelete("04EmpmasFamilyRef/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasFamilyRefModel?>> _04EmpmasFamilyRef(string code, string name, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._04EmpmasFamilyRef(code, schema, conn);
            return Ok(res);

        }


        // *********************************************************************************************
        // --- EmpmasGovPh ***************************************************************************
        //**********************************************************************************************
        [HttpPut("01EmpmasGovPh/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasGovPhModel?>> _01EmpmasGovPh([FromBody] EmpmasGovPhModel empmasGovPh, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._01EmpmasGovPh(id, empmasGovPh, schema, conn);
            return Ok(res);

        }

        [HttpGet("02EmpmasGovPh/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasGovPhModel?>> _02EmpmasGovPh(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasGovPh(id, schema, conn);
            return Ok(res);

        }

        [HttpPost("03EmpmasGovPh/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasGovPhModel?>> _03EmpmasGovPh([FromBody] EmpmasGovPhModel empmasGovPh, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._03EmpmasGovPh(id, empmasGovPh, schema, conn);
            return Ok(res);

        }

        [HttpDelete("04EmpmasGovPh/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasGovPhModel?>> _04EmpmasGovPh(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._04EmpmasGovPh(id, schema, conn);
            return Ok(res);

        }


        // *********************************************************************************************
        // --- EmpmasInsurance ***************************************************************************
        //**********************************************************************************************
        [HttpPut("01EmpmasInsurance/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasInsuranceModel?>> _01EmpmasInsurance([FromBody] EmpmasInsuranceModel empmasInsurance, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._01EmpmasInsurance(id, empmasInsurance, schema, conn);
            return Ok(res);

        }

        [HttpGet("02EmpmasInsurance/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasInsuranceModel?>> _02EmpmasInsurance(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasInsurance(id, schema, conn);
            return Ok(res);

        }

        [HttpPost("03EmpmasInsurance/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasInsuranceModel?>> _03EmpmasInsurance([FromBody] EmpmasInsuranceModel empmasInsurance, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._03EmpmasInsurance(id, empmasInsurance, schema, conn);
            return Ok(res);

        }

        [HttpDelete("04EmpmasInsurance/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasInsuranceModel?>> _04EmpmasInsurance(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._04EmpmasInsurance(id, schema, conn);
            return Ok(res);

        }

        // *********************************************************************************************
        // --- EmpmasPI ***************************************************************************
        //**********************************************************************************************
        [HttpPut("01EmpmasPI/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasPIModel?>> _01EmpmasPI([FromBody] EmpmasPIModel empmasPI, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._01EmpmasPI(id, empmasPI, schema, conn);
            return Ok(res);

        }

        [HttpGet("02EmpmasPI/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasPIModel?>> _02EmpmasPI(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasPI(id, schema, conn);
            return Ok(res);

        }

        [HttpPost("03EmpmasPI/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasPIModel?>> _03EmpmasPI([FromBody] EmpmasPIModel empmasPI, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._03EmpmasPI(id, empmasPI, schema, conn);
            return Ok(res);

        }

        [HttpDelete("04EmpmasPI/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasPIModel?>> _04EmpmasPI(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._04EmpmasPI(id, schema, conn);
            return Ok(res);

        }


        // *********************************************************************************************
        // --- EmpmasRelatives ***************************************************************************
        //**********************************************************************************************
        [HttpPut("01EmpmasRelatives/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasRelativesModel?>> _01EmpmasRelatives([FromBody] EmpmasRelativesModel empmasRelatives, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._01EmpmasRelatives(empmasRelatives, schema, conn);
            return Ok(res);
        }

        [HttpGet("02EmpmasRelatives/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasRelativesModel?>> _02EmpmasRelatives(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasRelatives(id, schema, conn);
            return Ok(res);
        }
        [HttpGet("02EmpmasRelativesList/{empmasId}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasRelativesModel?>> _02EmpmasRelativesList(int empmasId, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasRelativesList(empmasId, schema, conn);
            return Ok(res);
        }

        [HttpPost("03EmpmasRelatives/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasRelativesModel?>> _03EmpmasRelatives([FromBody] EmpmasRelativesModel empmasRelatives, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._03EmpmasRelatives(id, empmasRelatives, schema, conn);
            return Ok(res);
        }

        [HttpDelete("04EmpmasRelatives/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasRelativesModel?>> _04EmpmasRelatives(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._04EmpmasRelatives(id, schema, conn);
            return Ok(res);
        }


        // *********************************************************************************************
        // --- EmpmasRelativesRef ***************************************************************************
        //**********************************************************************************************
        [HttpPut("01EmpmasRelativesRef/{code}/{name}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasRelativesRefModel?>> _01EmpmasRelativesRef(string code, string name, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._01EmpmasRelativesRef(code, name, schema, conn);
            return Ok(res);

        }

        [HttpGet("02EmpmasRelativesRef/{code}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasRelativesRefModel?>> _02EmpmasRelativesRef(string code, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasRelativesRef(code, schema, conn);
            return Ok(res);
        }

        [HttpPost("03EmpmasRelativesRef/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasRelativesRefModel?>> _03EmpmasRelativesRef(string code, string name, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._03EmpmasRelativesRef(code, name, schema, conn);
            return Ok(res);

        }

        [HttpDelete("04EmpmasRelativesRef/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasRelativesRefModel?>> _04EmpmasRelativesRef(string code, string name, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._04EmpmasRelativesRef(code, schema, conn);
            return Ok(res);

        }


        // *********************************************************************************************
        // --- EmpmasSecLic ***************************************************************************
        //**********************************************************************************************
        [HttpPut("01EmpmasSecLic/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasSecLicModel?>> _01EmpmasSecLic([FromBody] EmpmasSecLicModel empmasSecLic, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._01EmpmasSecLic(id, empmasSecLic, schema, conn);
            return Ok(res);

        }

        [HttpGet("02EmpmasSecLic/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasSecLicModel?>> _02EmpmasSecLic(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasSecLic(id, schema, conn);
            return Ok(res);

        }

        [HttpPost("03EmpmasSecLic/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasSecLicModel?>> _03EmpmasSecLic([FromBody] EmpmasSecLicModel empmasSecLic, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._03EmpmasSecLic(id, empmasSecLic, schema, conn);
            return Ok(res);

        }

        [HttpDelete("04EmpmasSecLic/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasSecLicModel?>> _04EmpmasSecLic(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._04EmpmasSecLic(id, schema, conn);
            return Ok(res);

        }


        // *********************************************************************************************
        // --- EmpmasTraining ***************************************************************************
        //**********************************************************************************************
        [HttpPut("01EmpmasTraining/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasTrainingModel?>> _01EmpmasTraining([FromBody] EmpmasTrainingModel empmasTraining, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._01EmpmasTraining(empmasTraining, schema, conn);
            return Ok(res);
        }

        [HttpGet("02EmpmasTraining/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasTrainingModel?>> _02EmpmasTraining(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasTraining(id, schema, conn);
            return Ok(res);
        }
        [HttpGet("02EmpmasTrainingList/{empmasId}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasTrainingModel?>> _02EmpmasTrainingList(int empmasId, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._02EmpmasTrainingList(empmasId, schema, conn);
            return Ok(res);
        }

        [HttpPost("03EmpmasTraining/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasTrainingModel?>> _03EmpmasTraining([FromBody] EmpmasTrainingModel empmasTraining, int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._03EmpmasTraining(id, empmasTraining, schema, conn);
            return Ok(res);
        }

        [HttpDelete("04EmpmasTraining/{id}/{schema}/{conn}")]
        public async Task<ActionResult<EmpmasTrainingModel?>> _04EmpmasTraining(int id, string schema = "MainPis", string conn = "MySqlConn")
        {
            var res = await _empmas._04EmpmasTraining(id, schema, conn);
            return Ok(res);
        }












    }
}
