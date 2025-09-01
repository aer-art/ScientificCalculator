using System;
using System.Data;
using System.Windows.Forms;

namespace StandardCalculator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            txtAnswer.ReadOnly = true;
            txtAnswer.TabStop = false;
        }

        private string expression = "";   // stores the real text
        private int cursorPos = 0;        // cursor position

        private void RefreshDisplay()
        {
            string display = expression.Insert(cursorPos, "|");
            txtAnswer.Text = display;
        }
        private void InsertText(string value)
        {
            expression = expression.Insert(cursorPos, value);
            cursorPos += value.Length;
            RefreshDisplay();
        }
        private void btn0_Click(object sender, EventArgs e) {InsertText("0");}
        private void btn1_Click(object sender, EventArgs e) {InsertText("1");}
        private void btn2_Click(object sender, EventArgs e) {InsertText("2");}
        private void btn3_Click(object sender, EventArgs e) {InsertText("3");}
        private void btn4_Click(object sender, EventArgs e) {InsertText("4");}
        private void btn5_Click(object sender, EventArgs e) {InsertText("5");}
        private void btn6_Click(object sender, EventArgs e) {InsertText("6");}
        private void btn7_Click(object sender, EventArgs e) {InsertText("7");}
        private void btn8_Click(object sender, EventArgs e) {InsertText("8");}
        private void btn9_Click(object sender, EventArgs e) {InsertText("9");}

        private void btnAdd_Click(object sender, EventArgs e)
        {
            InsertText("+");
        }
        private void btnSubtract_Click(object sender, EventArgs e)
        {
            InsertText("-");
        }
        private void btnDivide_Click(object sender, EventArgs e)
        {
            InsertText("/");
        }
        private void btnMultiply_Click(object sender, EventArgs e)
        {
            InsertText("*"); 
        }

        private void leftParenthesis_Click(object sender, EventArgs e)
        {
            InsertText("(");
        }

        private void rightParenthesis_Click(object sender, EventArgs e)
        {
            InsertText(")");
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (cursorPos > 0)
            {
                expression = expression.Remove(cursorPos - 1, 1);
                cursorPos--;
                RefreshDisplay();
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            expression = "";
            cursorPos = 0;
            RefreshDisplay();
        }
        private void btnEqual_Click(object sender, EventArgs e)
        {
            try
            {
                double result = EvaluateExpression(expression);
                expression = result.ToString();
                cursorPos = expression.Length;
                RefreshDisplay();
            }
            catch
            {
                MessageBox.Show("Invalid Expression!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void leftArrow_Click(object sender, EventArgs e)
        {
            if (cursorPos > 0)
            {
                cursorPos--;
                RefreshDisplay();
            }
        }

        private void rightArrow_Click(object sender, EventArgs e)
        {
            if (cursorPos < expression.Length)
            {
                cursorPos++;
                RefreshDisplay();
            }
        }

        private void btnSquareroot_Click(object sender, EventArgs e)
        {
            InsertText("√");
        }
        private void btnPie_Click(object sender, EventArgs e)
        {
            InsertText("π");
        }
        private void btnPercent_Click(object sender, EventArgs e)
        {
            InsertText("%");
        }

        private void btnFactorial_Click(object sender, EventArgs e)
        {
            InsertText("!");
        }

        private void btnDot_Click(object sender, EventArgs e)
        {
            InsertText(".");
        }

        private void btnExponent_Click(object sender, EventArgs e)
        {
            InsertText("^");
        }

        private void b_Click(object sender, EventArgs e)
        {
            InsertText("^2");
        }

        private void btnCube_Click(object sender, EventArgs e)
        {
            InsertText("^3");
        }

        private double EvaluateExpression(string expr)
        {
            // Replace π with Math.PI
            expr = expr.Replace("π", Math.PI.ToString());

            // Handle all √ operations
            while (expr.Contains("√"))
            {
                int sqrtIndex = expr.IndexOf("√");
                int i = sqrtIndex + 1;
                string numberStr = "";
                while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.' || expr[i] == 'E' || (i == sqrtIndex + 1 && expr[i] == '-')))
                {
                    numberStr += expr[i];
                    i++;
                }

                if (numberStr == "")
                    throw new Exception("Invalid √ usage!");

                double number = Convert.ToDouble(numberStr);
                double sqrtVal = Math.Sqrt(number);
                expr = expr.Substring(0, sqrtIndex) + sqrtVal.ToString() + expr.Substring(i);
            }

            // Handle all % operations
            while (expr.Contains("%"))
            {
                int percentIndex = expr.IndexOf("%");
                int i = percentIndex - 1;
                string numberStr = "";
                while (i >= 0 && (char.IsDigit(expr[i]) || expr[i] == '.' || expr[i] == 'E'))
                {
                    numberStr = expr[i] + numberStr;
                    i--;
                }

                if (numberStr == "")
                    throw new Exception("Invalid % usage!");

                double number = Convert.ToDouble(numberStr);
                double percentVal = number / 100.0;
                expr = expr.Substring(0, i + 1) + percentVal.ToString() + expr.Substring(percentIndex + 1);
            }

            // Handle all ! operations
            while (expr.Contains("!"))
            {
                int factorialIndex = expr.IndexOf("!");
                int i = factorialIndex - 1;
                string numberStr = "";
                while (i >= 0 && (char.IsDigit(expr[i]) || expr[i] == '.' || expr[i] == 'E'))
                {
                    numberStr = expr[i] + numberStr;
                    i--;
                }

                if (numberStr == "" || numberStr.Contains("."))
                    throw new Exception("Invalid ! usage! Factorial only works with non-decimal numbers.");

                double number = Convert.ToDouble(numberStr);
                double factorialVal = Factorial(number);
                expr = expr.Substring(0, i + 1) + factorialVal.ToString() + expr.Substring(factorialIndex + 1);
            }

            // --- ADDED FOR EXPONENTS ---
            // Handle all ^ operations
            while (expr.Contains("^"))
            {
                int exponentIndex = expr.IndexOf("^");

                // Find the base number or expression
                int baseStart = exponentIndex - 1;
                int openParenCount = 0;
                while (baseStart >= 0)
                {
                    if (expr[baseStart] == ')')
                        openParenCount++;
                    else if (expr[baseStart] == '(')
                        openParenCount--;

                    if (openParenCount == 0 && (expr[baseStart] == '+' || expr[baseStart] == '-' || expr[baseStart] == '*' || expr[baseStart] == '/' || expr[baseStart] == '^'))
                        break;

                    baseStart--;
                }
                baseStart++;

                string baseStr = expr.Substring(baseStart, exponentIndex - baseStart);

                // Find the exponent number or expression
                int exponentEnd = exponentIndex + 1;
                int closeParenCount = 0;
                while (exponentEnd < expr.Length)
                {
                    if (expr[exponentEnd] == '(')
                        closeParenCount++;
                    else if (expr[exponentEnd] == ')')
                        closeParenCount--;

                    if (closeParenCount == 0 && (expr[exponentEnd] == '+' || expr[exponentEnd] == '-' || expr[exponentEnd] == '*' || expr[exponentEnd] == '/' || expr[exponentEnd] == '^'))
                        break;

                    exponentEnd++;
                }
                string exponentStr = expr.Substring(exponentIndex + 1, exponentEnd - (exponentIndex + 1));

                if (baseStr == "" || exponentStr == "")
                    throw new Exception("Invalid ^ usage!");

                double baseVal = Convert.ToDouble(new System.Data.DataTable().Compute(baseStr, null));
                double exponentVal = Convert.ToDouble(new System.Data.DataTable().Compute(exponentStr, null));
                double resultVal = Math.Pow(baseVal, exponentVal);

                expr = expr.Substring(0, baseStart) + resultVal.ToString() + expr.Substring(exponentEnd);
            }
            // --- END ADDED FOR EXPONENTS ---

            // Final evaluation (MDAS + parentheses)
            return Convert.ToDouble(new System.Data.DataTable().Compute(expr, null));

        }
        private double Factorial(double n)
        {
            if (n < 0)
                throw new Exception("Factorial is not defined for negative numbers!");

            // Convert to long for integer-only operation
            long num = (long)n;

            if (num == 0)
                return 1;

            double result = 1;
            for (int i = 2; i <= num; i++)
            {
                result *= i;
            }
            return result;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}