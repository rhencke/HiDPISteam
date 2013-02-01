using System.Globalization;
using System.Text.RegularExpressions;
using Antlr.Runtime.Tree;

namespace ConsoleApplication1
{
    internal class ScalerTreeWalker : TreeWalker
    {
        private readonly decimal _scaleFactor;

        public ScalerTreeWalker(decimal scaleFactor)
        {
            _scaleFactor = scaleFactor;
        }

        public override void Walk(CommonTree ast)
        {
            switch (ast.Type)
            {
                case vguiLexer.ASSIGNMENT:
                    var propName = (CommonTree) ast.Children[0];
                    var propValue = (CommonTree) ast.Children[1];

                    // Multiply any sequence of digits found in properties like these.
                    // If it's a group, skip it.
                    if (propValue.Type != vguiLexer.STRING)
                        break;
                    switch (Utils.Unquote(propName.Text))
                    {
                        case "width":
                        case "height":
                        case "inset":
                        case "x":
                        case "xpos":
                        case "y":
                        case "ypos":
                        case "wide":
                        case "tall":
                        case "font-size":
                        case "margin":
                        case "margin-top":
                        case "margin-bottom":
                        case "margin-right":
                        case "margin-left":
                        case "padding":
                        case "padding-top":
                        case "padding-bottom":
                        case "padding-right":
                        case "padding-left":
                        case "spacing":
                        case "minimum-width":
                        case "Frame.ClientInsetX":
                        case "Frame.ClientInsetY":
                        case "Frame.AutoSnapRange":
                        case "ListPanel.PerPixelScrolling":
                        case "Menu.TextInset":
                        case "RichText.InsetX":
                        case "RichText.InsetY":
                        case "ScrollBar.Wide":
                        case "SectionedListPanel.CollapserWidth":
                        case "HTML.SearchInsetY":
                        case "HTML.SearchInsetX":
                        case "HTML.SearchWide":
                        case "HTML.SearchTall":
                        case "MessageBox.ButtonHeight":
                            string value = Utils.Unquote(propValue.Text);
                            string[] digits = Regex.Split(value, "([0-9]+)");
                            for (int i = 0; i < digits.Length; i++)
                            {
                                digits[i] = TryScale(digits[i]);
                            }
                            propValue.Token.Text = Utils.Quote(string.Concat(digits));
                            break;
                        case "0":
                        case "1":
                        case "2":
                        case "3":
                        case "4":
                        case "5":
                        case "6":
                        case "7":
                        case "8":
                        case "9":
                        case "10":
                        case "11":
                        case "12":
                        case "13":
                        case "14":
                        case "15":
                        case "16":
                        case "17":
                        case "18":
                        case "19":
                            string funcvalue = Utils.Unquote(propValue.Text);
                            string func = Regex.Replace(funcvalue, @"([+-]\s*)([0-9]+)", m => m.Groups[1].Value + TryScale(m.Groups[2].Value));
                            propValue.Token.Text = Utils.Quote(func);
                            break;
                    }
                    break;
            }

            WalkChildren(ast);
        }

        private string TryScale(string digit)
        {
            long num;
            if (long.TryParse(digit, NumberStyles.Any, CultureInfo.InvariantCulture, out num))
            {
                num = (long) (num*_scaleFactor);
                return num.ToString(CultureInfo.InvariantCulture);
            }
            return digit;
        }
    }
}