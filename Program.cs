using DotMake.CommandLine;
using SimpleMSI;

return await Cli.RunAsync<SimpleMsi>(args, new()
{
    EnableDefaultExceptionHandler = true,
    Theme = CliTheme.NoColor,
    EnablePosixBundling = true,
    Output = Console.Out,
    Error = Console.Error
});

namespace SimpleMSI
{
    [CliCommand]
    internal class SimpleMsi
    {
        public int Run(CliContext ctx)
        {
            ctx.ShowLogo();
            ctx.WriteLine("\n");
            ctx.ShowHelp();
            return ExitCodes.InvalidArguments;
        }
    }
}