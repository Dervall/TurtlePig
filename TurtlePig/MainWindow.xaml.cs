using System;
using System.Collections.Generic;
using System.Windows;
using Piglet.Lexer;
using Piglet.Parser;

namespace TurtlePig
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Run(object sender, RoutedEventArgs e)
        {
            try
            {
                canvas1.Children.Clear();

                // Runtime stuff
                var turtle = new Turtle(canvas1);
                var variables = new Dictionary<string, int>();

                // Parser configurator
                var configurator = ParserFactory.Fluent();
                var statementList = configurator.Rule();
                var statement = configurator.Rule();
                var variableDeclaration = configurator.Rule();
                var expression = configurator.Rule();
                var addSub = configurator.Rule();
                var mulDiv = configurator.Rule();
                var factor = configurator.Rule();

                var variableIdentifier = configurator.Expression();
                variableIdentifier.ThatMatches("$[A-Za-z0-9]+").AndReturns(f => f.Substring(1));

                expression.IsMadeUp.By(addSub);

                addSub.IsMadeUp.By(addSub).As("First").Followed.By("+").Followed.By(mulDiv).As("Second").WhenFound(f => f.First + f.Second)
                    .Or.By(addSub).As("First").Followed.By("-").Followed.By(mulDiv).As("Second").WhenFound(f => f.First - f.Second)
                    .Or.By(mulDiv);

                mulDiv.IsMadeUp.By(mulDiv).As("First").Followed.By("*").Followed.By(factor).As("Second").WhenFound(f => f.First * f.Second)
                    .Or.By(mulDiv).As("First").Followed.By("/").Followed.By(factor).As("Second").WhenFound(f => f.First / f.Second)
                    .Or.By(factor);

                factor.IsMadeUp.By<int>()
                    .Or.By(variableIdentifier).As("Variable").WhenFound(f => variables[f.Variable])
                    .Or.By("(").Followed.By(expression).As("Expression").Followed.By(")").WhenFound(f => f.Expression);

                variableDeclaration.IsMadeUp.By("var")
                    .Followed.By(variableIdentifier).As("Name")
                    .Followed.By("=")
                    .Followed.By(expression).As("InitialValue").WhenFound(f =>
                    {
                        variables.Add(f.Name, f.InitialValue);
                        return null;
                    });

                statementList.IsMadeUp.ByListOf(statement);
                statement.IsMadeUp.By("pendown").WhenFound(f =>
                {
                    turtle.PenDown = true;
                    return null;
                })
                    .Or.By("penup").WhenFound(f =>
                    {
                        turtle.PenDown = false;
                        return null;
                    })
                    .Or.By("move").Followed.By(expression).As("Distance").WhenFound(f =>
                    {
                        turtle.Move(f.Distance);
                        return null;
                    })
                    .Or.By("rotate").Followed.By(expression).As("Angle").WhenFound(f =>
                    {
                        turtle.Rotate(f.Angle);
                        return null;
                    })
                    .Or.By(variableDeclaration);
                var parser = configurator.CreateParser();
                parser.Parse(code.Text);    
            }
            catch (ParseException ex)
            {
                MessageBox.Show(string.Format("{0}\nError at line {1} position {2}", ex.Message, ex.LexerState.CurrentLineNumber, ex.LexerState.CurrentLine.Length), "Turtle is confused!");
            }
            catch (LexerException ex)
            {
                MessageBox.Show(string.Format("{0}\nError at line {1} position {2}", ex.Message, ex.LineNumber, ex.LineContents.Length), "Turtle is confused!");
            }
        }
    }
}
