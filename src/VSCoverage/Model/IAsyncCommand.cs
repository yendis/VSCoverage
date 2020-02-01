using System.Threading.Tasks;
using System.Windows.Input;

namespace VSCoverage.Model
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
