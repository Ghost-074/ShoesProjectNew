using Microsoft.EntityFrameworkCore;
using WinFormsApp1.Models;
using User = WinFormsApp1.Models.User; // Направляем компилятор на вашу модель из БД


namespace WinFormsApp1
{
    public partial class FormProdects : Form
    {

        public User CurrentUser { get; private set; }
        public bool IsGuest { get; private set; }

        public FormProdects(User user, bool guest)
        {
            InitializeComponent();

            var colPhoto = new DataGridViewImageColumn
            {
                Name = "colPhoto",
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                Width = 200,
                FillWeight = 30
            };

            var colInfo = new DataGridViewTextBoxColumn
            {
                Name = "colInfo",
                FillWeight = 60
            };
            colInfo.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            var colDiscount = new DataGridViewTextBoxColumn
            {
                Name = "colDiscount",
                FillWeight = 10
            };
            colDiscount.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvProduct.Columns.AddRange(
            [
                colPhoto, colInfo, colDiscount
            ]);

            CurrentUser = user;
            IsGuest = guest;

            lblUserName.Text = IsGuest ? "Гость" : CurrentUser.FullName;

            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                using (var db = new DbShopContext())
                {
                    var products = db.Products
                        .Include(i => i.Category)
                        .Include(i => i.Manufacturer)
                        .Include(i => i.Supplier)
                        .Include(i => i.Measure)
                        .ToList();

                    dgvProduct.SuspendLayout();
                    dgvProduct.Rows.Clear();

                    foreach (var product in products)
                    {
                        int rowIndex = dgvProduct.Rows.Add();
                        var row = dgvProduct.Rows[rowIndex];

                        row.Cells["colPhoto"].Value = LoadProductsImage(product.PhotoUrl);

                        row.Cells["colInfo"].Value = FormatProductInfo(product);

                        row.Cells["colDiscount"].Value = $"{product.Discount}%";
                        row.Cells["colDiscount"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                        ApplyRowStyles(row, product);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyRowStyles(DataGridViewRow row, Product product)
        {
            if (product.Discount > 15)
            {
                row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#2E8B57");
                row.DefaultCellStyle.ForeColor = Color.White;
            }

            if (product.CointInStock <= 0)
            {
                row.DefaultCellStyle.ForeColor = Color.LightBlue;
                if (product.Discount <= 15)
                {
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }

            if (product.Discount > 0)
            {
                row.Cells["colDiscount"].Style.ForeColor = Color.Red;
                row.Cells["colDiscount"].Style.Font = new Font(
                    "Times New Roman",
                    12,
                    FontStyle.Bold);
            }
        }

        private string FormatProductInfo(Product product)
        {
            string priceText;

            if (product.Discount > 0)
            {
                decimal finalProce = product.Price * (100 - product.Discount) / 100;
                priceText = $"Цена: {product.Price:C} -> {finalProce:C}";

            }
            else
            {
                priceText = $"Цена: {product.Price:C}";
            }

            return $"{product.Category.CategoryName} | {product.ProductType.ProdType}" + Environment.NewLine +
                $"Описание товара: {product.Discount}" + Environment.NewLine +
                $"Производитель: {product.Manufacturer.ManufacturerName}" + Environment.NewLine +
                $"Поставщик: {product.Supplier.SupplierName}" + Environment.NewLine +
                $"Цена: {priceText}" + Environment.NewLine +
                $"Единица измерения: {product.Measure.MeasureName}" + Environment.NewLine +
                $"Количество на складе: {product.CointInStock}";

        }

        private Image LoadProductsImage(string photoUrl)
        {
            if (!string.IsNullOrEmpty(photoUrl) && System.IO.File.Exists(photoUrl))
            {
                return Image.FromFile(photoUrl);
            }

            return Resource1.picture;

        }

        private void FormProdects_Load(object sender, EventArgs e)
        {

        }

        private void dgvProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void BtnLogOut_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }
    }
}
