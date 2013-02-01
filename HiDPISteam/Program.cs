using System;
using System.CodeDom.Compiler;
using System.IO;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace ConsoleApplication1
{
    internal class Program
    {
        private const decimal ScaleFactor = 2m;

        private static readonly DirectoryInfo[] OutputDirs = new[]
        {
            new DirectoryInfo(@"C:\Program Files (x86)\Steam\skins\192dpi\resource\"),
            new DirectoryInfo(@"C:\Program Files (x86)\Steam\skins\192dpi\Public\"),
        };

        private static readonly DirectoryInfo[] InputDirs = new[]
        {
            new DirectoryInfo(@"C:\Program Files (x86)\Steam\resource\"),
            new DirectoryInfo(@"C:\Program Files (x86)\Steam\Public\"),
        };

        private static void Main(string[] args)
        {
            for (int i = 0; i < InputDirs.Length;i++ )
            {
                var InputDir = InputDirs[i];
                var OutputDir = OutputDirs[i];
                foreach (FileInfo file in InputDir.EnumerateFiles("*.*", SearchOption.AllDirectories))
                {
                    FileInfo outputFile = DetermineOutputFilePath(file, InputDir, OutputDir);
                    if (!outputFile.Directory.Exists)
                        outputFile.Directory.Create();
                    switch (file.Extension.ToUpperInvariant())
                    {
                        case ".RES":
                        case ".STYLES":
                        case ".LAYOUT":
                            ProcessResourceFile(file, outputFile);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static FileInfo DetermineOutputFilePath(FileInfo inputFile, DirectoryInfo indir, DirectoryInfo outdir)
        {
            // See: http://stackoverflow.com/a/9045399
            var fileUri = new Uri(inputFile.FullName, UriKind.Absolute);
            var dirUri = new Uri(indir.FullName, UriKind.Absolute);
            string relativePath = dirUri.MakeRelativeUri(fileUri).ToString();

            return new FileInfo(Path.Combine(outdir.FullName, relativePath));
        }

        private static void ProcessResourceFile(FileInfo file, FileInfo outputFile)
        {
            CommonTree ast = GetAst(file);

            new ScalerTreeWalker(ScaleFactor).Walk(ast);

            using (var sw = new StreamWriter(outputFile.FullName))
            using (var tw = new IndentedTextWriter(sw))
            {
                new PrinterTreeWalker(tw).Walk(ast);
            }
        }

        private static CommonTree GetAst(FileInfo file)
        {
            var v = new ANTLRFileStream(file.FullName);
            var lex = new vguiLexer(v);
            var cts = new CommonTokenStream(lex);
            var prs = new vguiParser(cts) {TreeAdaptor = new CommonTreeAdaptor()};
            return (CommonTree) prs.start().Tree;
        }
    }
}