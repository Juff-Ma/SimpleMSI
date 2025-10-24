using DotMake.CommandLine;

namespace SimpleMSI;

internal static class CliContextExtensions
{
    public static readonly string AsciiLogo =
        $"""
         =========================================================
              __ _                 _              __   _____ 
             / _(_)_ __ ___  _ __ | | ___  /\/\  / _\  \_   \
             \ \| | '_ ` _ \| '_ \| |/ _ \/    \ \ \    / /\/
             _\ \ | | | | | | |_) | |  __/ /\/\ \_\ \/\/ /_
             \__/_|_| |_| |_| .__/|_|\___\/    \/\__/\____/
                            |_|                              
         (C) {DateTime.Now.Year} SimpleMSI ======================================
         """;

    public static void ShowLogo(this CliContext ctx)
    {
        ctx.WriteLine(AsciiLogo);
    }

    public static void Write(this CliContext ctx, ReadOnlySpan<char> str)
        => ctx.Output.Write(str);

    public static void WriteLine(this CliContext ctx, ReadOnlySpan<char> str)
        => ctx.Output.WriteLine(str);

    public static Task WriteAsync(this CliContext ctx, ReadOnlyMemory<char> str, CancellationToken token = default)
        => ctx.Output.WriteAsync(str, token);

    public static Task WriteLineAsync(this CliContext ctx, ReadOnlyMemory<char> str, CancellationToken token = default)
        => ctx.Output.WriteLineAsync(str, token);
}
