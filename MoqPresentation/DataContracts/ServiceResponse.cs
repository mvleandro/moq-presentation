using System;
namespace MoqPresentation.DataContracts
{
    public class ServiceResponse
    {
        public bool Failed { get; set; }
        public Exception Error { get; set; }
        public object Data { get; set; }
    }
}
