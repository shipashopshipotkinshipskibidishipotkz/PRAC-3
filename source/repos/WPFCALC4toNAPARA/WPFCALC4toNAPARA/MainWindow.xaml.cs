using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace WPFCALC4toNAPARA
{
    public partial class MainWindow : Window
    {
        private StringBuilder expression = new StringBuilder();
        private bool resultDisplayed = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string content = btn.Content.ToString();

            if (resultDisplayed && "0123456789πe,".Contains(content))
            {
                Display.Clear();
                expression.Clear();
                resultDisplayed = false;
            }

            switch (content)
            {
                case "π":
                    expression.Append(Math.PI.ToString().Replace(",", "."));
                    break;
                case "e":
                    expression.Append(Math.E.ToString().Replace(",", "."));
                    break;
                case ",":
                    expression.Append(".");
                    break;
                case "DEL":
                    if (expression.Length > 0) expression.Remove(expression.Length - 1, 1);
                    break;
                case "CE":
                    expression.Clear();
                    break;
                case "x^y":
                    expression.Append(" ^ ");
                    break;
                default:
                    expression.Append(content);
                    break;
            }

            Display.Text = expression.ToString();
        }

        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            Calculate_Click(sender, e);
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string expr = expression.ToString().Replace(",", ".");
                expr = ReplacePowers(expr);

                var result = new DataTable().Compute(expr, null);
                Display.Text = result.ToString();
                expression.Clear();
                expression.Append(result.ToString());
                resultDisplayed = true;
            }
            catch
            {
                Display.Text = "Ошибка";
                expression.Clear();
                resultDisplayed = true;
            }
        }

        private string ReplacePowers(string input)
        {
            var regex = new Regex(@"(\d+(\.\d+)?)\s*\^\s*(\d+(\.\d+)?)");
            while (regex.IsMatch(input))
            {
                input = regex.Replace(input, m =>
                {
                    double baseValue = Convert.ToDouble(m.Groups[1].Value);
                    double exponentValue = Convert.ToDouble(m.Groups[3].Value);
                    double powResult = Math.Pow(baseValue, exponentValue);
                    return powResult.ToString().Replace(",", ".");
                });
            }

            return input;
        }

        private void Function_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(Display.Text.Replace(".", ","), out double value))
            {
                Button btn = sender as Button;
                string content = btn.Content.ToString().ToLower();
                double result = 0;

                try
                {
                    switch (content)
                    {
                        case "sin":
                            result = Math.Sin(value * Math.PI / 180);
                            break;
                        case "cos":
                            result = Math.Cos(value * Math.PI / 180);
                            break;
                        case "tg":
                            result = Math.Tan(value * Math.PI / 180);
                            break;
                        case "ln":
                            result = Math.Log(value);
                            break;
                        case "log":
                            result = Math.Log10(value);
                            break;
                        case "x^2":
                            result = Math.Pow(value, 2);
                            break;
                        case "1/x":
                            result = 1 / value;
                            break;
                        case "|x|":
                            result = Math.Abs(value);
                            break;
                        case "2√x":
                            result = Math.Sqrt(value);
                            break;
                        case "10^x":
                            result = Math.Pow(10, value);
                            break;
                        case "n!":
                            result = Factorial((int)value);
                            break;
                        default:
                            break;
                    }

                    Display.Text = result.ToString("G");
                    expression.Clear();
                    expression.Append(result.ToString().Replace(",", "."));
                    resultDisplayed = true;
                }
                catch
                {
                    Display.Text = "Ошибка";
                    expression.Clear();
                    resultDisplayed = true;
                }
            }
        }

        private void Negate_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(Display.Text, out double value))
            {
                value = -value;
                Display.Text = value.ToString("G");
                expression.Clear();
                expression.Append(value.ToString().Replace(",", "."));
            }
        }

        private double Factorial(int n)
        {
            if (n < 0) throw new ArgumentException("Отрицательное значение!");
            if (n == 0 || n == 1) return 1;
            double f = 1;
            for (int i = 2; i <= n; i++) f *= i;
            return f;
        }
    }
}
