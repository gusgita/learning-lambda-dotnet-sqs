using System.Collections.Generic;
using System.Threading.Tasks;

namespace SQSHelper.Abstraction
{
    public interface ISQSHelper
    {
        Task SendMessage(string messageBody);
        ValueTask<IEnumerable<string>> ReceiveMessages(int maxNumOfMessages);
    }
}
