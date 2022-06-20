using System.Threading.Tasks;

namespace SQSHelper.Abstraction
{
    public interface ISQSHelper
    {
        Task SendMessage(string messageBody);
    }
}