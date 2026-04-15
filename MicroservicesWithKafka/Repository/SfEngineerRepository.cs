using Dapper;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System.Data;

namespace MicroservicesWithKafka.Repository
{
    public class SfEngineerRepository : ISfEngineerRepository
    {
        private readonly SQLConnectionService _sql;

        public SfEngineerRepository(SQLConnectionService sql)
        {
            _sql = sql;
        }

        // MAIN CRUD OPERATION
        public async Task<IEnumerable<dynamic>> ManageEngineersAsync(SfEngineerRequest request)
        {
            using var con = _sql.GetConnection();

            var param = new DynamicParameters();
            param.Add("@Action", request.Action);
            param.Add("@Eng_ID", request.Eng_ID);
            param.Add("@Eng_RegionID", request.Eng_RegionID);
            param.Add("@Eng_BranchID", request.Eng_BranchID);
            param.Add("@Eng_SFId", request.Eng_SFId);
            param.Add("@Eng_Name", request.Eng_Name);
            param.Add("@Eng_Catid", request.Eng_Catid);
            param.Add("@Eng_MobNo", request.Eng_MobNo);
            param.Add("@Eng_Status", request.Eng_Status);
            param.Add("@Eng_CityId", request.Eng_CityId);
            param.Add("@Eng_LocalityID", request.Eng_LocalityID);
            param.Add("@Eng_TotExpY", request.Eng_TotExpY);
            param.Add("@Eng_TotExpM", request.Eng_TotExpM);
            param.Add("@Eng_DOJ", request.Eng_DOJ);
            param.Add("@Eng_GradeID", request.Eng_GradeID);
            param.Add("@Eng_ICard", request.Eng_ICard);
            param.Add("@IMEINo", request.IMEINo);
            param.Add("@SWcode", request.SWcode);
            param.Add("@HighestQualification", request.HighestQualification);
            param.Add("@TechnicalQualification", request.TechnicalQualification);

            return await con.QueryAsync<dynamic>(
                "SP_CRUD_Tbl_MstSfEngg",
                param,
                commandType: CommandType.StoredProcedure
            );
        }

        // NEW METHOD: GET ACTIVE ENGINEERS
        public async Task<IEnumerable<dynamic>> GetActiveEngineersBySFIDAsync(long sfid)
        {
            using var con = _sql.GetConnection();

            var param = new DynamicParameters();
            param.Add("@SFID", sfid);
            param.Add("@Action", "GetActiveEngineers");

            return await con.QueryAsync<dynamic>(
                "SP_CRUD_Tbl_MstSfEngg",
                param,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
