using System.Threading.Tasks;

namespace TudaSuda
{
    public interface IAppCommandProcessor
    {
        Task Process(AppCommandArgs message);
    }
}