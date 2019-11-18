using System.Collections.Generic;

namespace LT.HttpClientHelper.TestConsole.Models.Response
{
    public class RichiestaPagamentoResponse
    {
        public List<MessaggiNode> MEX_OK { get; set; }

        public List<MessaggiNode> MEX_ERR { get; set; }

        public List<MessaggiNode> MEX_WARNING { get; set; }

        public List<MessaggiNode> MEX_INFO { get; set; }
    }
}
