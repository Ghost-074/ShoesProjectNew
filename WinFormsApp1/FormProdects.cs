using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WinFormsApp1.Models;

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
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Image LoadProductsImage(string photoUrl)
        {
            if (!string.IsNullOrEmpty(photoUrl) && System.IO.File.Exists(photoUrl))
            {
                return Image.FromFile(photoUrl);
            }

            Bitmap bmp = new Bitmap(150, 100);
            using(Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.DrawRectangle(Pens.LightGreen, 0, 0, 100, 99);
            }

            return
        }

        private void FormProdects_Load(object sender, EventArgs e)
        {

        }

        private void dgvProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
