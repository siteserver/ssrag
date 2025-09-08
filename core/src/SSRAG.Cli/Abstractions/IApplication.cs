using System.Threading.Tasks;
using Quartz;

namespace SSRAG.Cli.Abstractions
{
    public interface IApplication
    {
        Task RunAsync(string[] args);

        Task RunExecuteAsync(string commandName, string[] commandArgs, string[] commandExtras,
            IJobExecutionContext jobContext);
    }
}
