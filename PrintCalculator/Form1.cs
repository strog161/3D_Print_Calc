using System;
using System.Drawing;
using System.Windows.Forms;

namespace PrintCalculator
{
    public partial class Form1 : Form
    {
        // Constants as specified in README
        private const decimal HUMAN_WORK_COST_PER_HOUR = 200.0m;
        private const decimal FDM_COST_PER_GRAM = 3.0m;
        private const decimal RESIN_COST_PER_GRAM = 5.0m;
        private const decimal FDM_PRINTER_COST_PER_HOUR = 20.0m;
        private const decimal RESIN_PRINTER_COST_PER_HOUR = 30.0m;
        private const decimal PRINT_TIME_OVERHEAD = 0.25m; // 25% additional time
        private const decimal FDM_METERS_TO_GRAMS = 3.0m; // 1 метр = 3 грамма

        private ComboBox cmbPrintMethod = null!;
        private NumericUpDown nudPrintTime = null!;
        private NumericUpDown nudMaterialWeight = null!;
        private NumericUpDown nudHumanWorkTime = null!;
        private NumericUpDown nudMarkup = null!;
        private Button btnCalculate = null!;
        private Label lblTotalCost = null!;
        private Label lblTotalPrintTime = null!;
        private Label lblMaterialCost = null!;
        private Label lblPrinterCost = null!;
        private Label lblWorkCost = null!;
        private Label lblMarkupAmount = null!;
        private Label lblMaterialWeight = null!;

        public Form1()
        {
            InitializeComponent();
            InitializeCustomComponents();
            this.Text = "3D Print Calculator";
        }

        private void InitializeCustomComponents()
        {
            this.BackColor = ColorTranslator.FromHtml("#F0F0F0");
            this.ForeColor = ColorTranslator.FromHtml("#333333");
            
            // Print Method Selection
            var lblPrintMethod = new Label
            {
                Text = "Метод печати:",
                Location = new Point(20, 20),
                AutoSize = true
            };
            
            cmbPrintMethod = new ComboBox
            {
                Location = new Point(20, 40),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPrintMethod.Items.AddRange(new string[] { "FDM", "Resin" });
            cmbPrintMethod.SelectedIndex = 0;
            cmbPrintMethod.SelectedIndexChanged += CmbPrintMethod_SelectedIndexChanged;

            // Print Time
            var lblPrintTime = new Label
            {
                Text = "Время печати (часы):",
                Location = new Point(20, 80),
                AutoSize = true
            };

            nudPrintTime = new NumericUpDown
            {
                Location = new Point(20, 100),
                Width = 200,
                DecimalPlaces = 2,
                Minimum = 0,
                Maximum = 1000
            };

            // Material Weight
            lblMaterialWeight = new Label
            {
                Text = "Длина нити (метры):",
                Location = new Point(20, 140),
                AutoSize = true
            };

            nudMaterialWeight = new NumericUpDown
            {
                Location = new Point(20, 160),
                Width = 200,
                DecimalPlaces = 2,
                Minimum = 0,
                Maximum = 10000
            };

            // Human Work Time
            var lblHumanWorkTime = new Label
            {
                Text = "Затраченное время (часы):",
                Location = new Point(20, 200),
                AutoSize = true
            };

            nudHumanWorkTime = new NumericUpDown
            {
                Location = new Point(20, 220),
                Width = 200,
                DecimalPlaces = 2,
                Minimum = 0,
                Maximum = 1000
            };

            // Markup Percentage
            var lblMarkup = new Label
            {
                Text = "Процент наценки (%):",
                Location = new Point(20, 260),
                AutoSize = true
            };

            nudMarkup = new NumericUpDown
            {
                Location = new Point(20, 280),
                Width = 200,
                DecimalPlaces = 2,
                Minimum = 0,
                Maximum = 1000
            };

            // Calculate Button
            btnCalculate = new Button
            {
                Text = "Рассчитать",
                Location = new Point(20, 320),
                Width = 200,
                Height = 40,
                BackColor = ColorTranslator.FromHtml("#008080"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCalculate.Click += BtnCalculate_Click;

            // Results Labels
            lblMaterialCost = new Label
            {
                Text = "Стоимость материала: 0 ₽",
                Location = new Point(250, 40),
                AutoSize = true
            };

            lblPrinterCost = new Label
            {
                Text = "Стоимость использования принтера: 0 ₽",
                Location = new Point(250, 80),
                AutoSize = true
            };

            lblWorkCost = new Label
            {
                Text = "Стоимость работы: 0 ₽",
                Location = new Point(250, 120),
                AutoSize = true
            };

            lblMarkupAmount = new Label
            {
                Text = "Сумма наценки: 0 ₽",
                Location = new Point(250, 160),
                AutoSize = true
            };

            lblTotalCost = new Label
            {
                Text = "ИТОГО: 0 ₽",
                Location = new Point(250, 200),
                AutoSize = true,
                Font = new Font(this.Font.FontFamily, 12, FontStyle.Bold)
            };

            lblTotalPrintTime = new Label
            {
                Text = "Итоговое время печати: 0 ч",
                Location = new Point(450, 200),
                AutoSize = true,
                Font = new Font(this.Font.FontFamily, 12, FontStyle.Bold)
            };

            // Add controls to form
            this.Controls.AddRange(new Control[] {
                lblPrintMethod, cmbPrintMethod,
                lblPrintTime, nudPrintTime,
                lblMaterialWeight, nudMaterialWeight,
                lblHumanWorkTime, nudHumanWorkTime,
                lblMarkup, nudMarkup,
                btnCalculate,
                lblMaterialCost, lblPrinterCost,
                lblWorkCost, lblMarkupAmount, lblTotalCost, lblTotalPrintTime
            });
        }

        private void CmbPrintMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isFDM = cmbPrintMethod.SelectedIndex == 0;
            lblMaterialWeight.Text = isFDM ? "Длина нити (метры):" : "Масса материала (граммы):";
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            // Get input values
            bool isFDM = cmbPrintMethod.SelectedIndex == 0;
            decimal printTime = nudPrintTime.Value;
            decimal materialInput = nudMaterialWeight.Value;
            decimal humanWorkTime = nudHumanWorkTime.Value;
            decimal markupPercentage = nudMarkup.Value;

            // Calculate material weight based on print method
            decimal materialWeight = isFDM ? materialInput * FDM_METERS_TO_GRAMS : materialInput;

            // Calculate costs
            decimal materialCostPerGram = isFDM ? FDM_COST_PER_GRAM : RESIN_COST_PER_GRAM;
            decimal printerCostPerHour = isFDM ? FDM_PRINTER_COST_PER_HOUR : RESIN_PRINTER_COST_PER_HOUR;

            decimal totalPrintTime = printTime * (1 + PRINT_TIME_OVERHEAD);
            
            decimal materialCost = materialWeight * materialCostPerGram;
            decimal printerCost = printerCostPerHour * totalPrintTime;
            decimal workCost = HUMAN_WORK_COST_PER_HOUR * humanWorkTime;
            
            decimal subtotal = materialCost + printerCost + workCost;
            decimal markup = subtotal * (markupPercentage / 100);
            decimal total = subtotal + markup;

            // Update labels
            lblMaterialCost.Text = $"Стоимость материала: {materialCost:N2} ₽";
            lblPrinterCost.Text = $"Стоимость использования принтера: {printerCost:N2} ₽";
            lblWorkCost.Text = $"Стоимость работы: {workCost:N2} ₽";
            lblMarkupAmount.Text = $"Сумма наценки: {markup:N2} ₽";
            lblTotalCost.Text = $"ИТОГО: {total:N2} ₽";
            lblTotalPrintTime.Text = $"Итоговое время печати: {totalPrintTime:N2} ч";
        }
    }
}
