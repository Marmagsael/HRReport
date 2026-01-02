using HRApiLibrary.Models._90_Utils;

namespace HRApiLibrary.DataAccess._90_Utils.Interface
{
    public interface IMsdsDataAccess
    {
        public Task SendEmail(EmailModel email);
        object ObjectMapper(object src, object des);
    }
}