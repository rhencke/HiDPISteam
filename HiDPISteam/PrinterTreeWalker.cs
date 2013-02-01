using System.CodeDom.Compiler;
using Antlr.Runtime.Tree;

namespace ConsoleApplication1
{
    internal class PrinterTreeWalker : TreeWalker
    {
        private readonly IndentedTextWriter _writer;

        public PrinterTreeWalker(IndentedTextWriter writer)
        {
            _writer = writer;
        }

        public override void Walk(CommonTree ast)
        {
            switch (ast.Type)
            {
                case vguiLexer.ASSIGNMENT:
                    Walk((CommonTree) ast.Children[0]); // key

                    if (ast.Children.Count == 3)
                    {
                        _writer.Write(" ");
                        Walk((CommonTree) ast.Children[2]); // OS tag
                    } 
                
                    // Groups don't seem to work with '='.  Both strings and groups seem to work with ' '.
                    _writer.Write(" ");

                    Walk((CommonTree) ast.Children[1]); // value
                    _writer.WriteLine();
                    break;
                case vguiLexer.GROUP:
                    if (ast.Children == null || ast.Children.Count == 0)
                    {
                        _writer.WriteLine("{}");
                        break;
                    }
                    _writer.WriteLine("{");
                    _writer.Indent++;
                    WalkChildren(ast);
                    _writer.Indent--;
                    _writer.WriteLine("}");
                    break;
                case vguiLexer.STRING:
                case vguiLexer.TAG:
                    _writer.Write(ast.Text);
                    break;
                default:
                    WalkChildren(ast);
                    break;
            }
        }
    }
}