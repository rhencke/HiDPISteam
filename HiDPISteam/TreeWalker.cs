using Antlr.Runtime.Tree;

namespace ConsoleApplication1
{
    abstract class TreeWalker
    {
        protected void WalkChildren(CommonTree ast)
        {
            if (ast == null || ast.Children == null) 
                return;

            foreach (CommonTree child in ast.Children)
                Walk(child);
        }

        public abstract void Walk(CommonTree ast);
    }
}