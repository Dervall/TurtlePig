using System;
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

                var turtle = new Turtle(canvas1);

                var configurator = ParserFactory.Fluent();
                var statementList = configurator.Rule();
                var statement = configurator.Rule();

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
                    .Or.By("move").Followed.By<int>().As("Distance").WhenFound(f =>
                    {
                        turtle.Move(f.Distance);
                        return null;
                    })
                    .Or.By("rotate").Followed.By<int>().As("Angle").WhenFound(f =>
                    {
                        turtle.Rotate(f.Angle);
                        return null;
                    });
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
