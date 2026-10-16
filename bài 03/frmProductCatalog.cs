using System.Data;
using System.Windows.Forms;

namespace SuperMarketMDI;

/// <summary>
/// Form con 1: Quản lý sản phẩm (CRUD) - kế thừa giao diện Bài 1 & 2.
/// Lưới dùng các cột MaSP, TenSP, DonViTinh, DonGia, SoLuong.
/// </summary>
public class frmProductCatalog : Form
{
    private TextBox txtMa, txtTen, txtDvt, txtGia, txtSl;
    private Button btnAdd, btnEdit, btnDelete, btnClear;
    private DataGridView dgv;
    private DataTable _table;

    public frmProductCatalog()
    {
        Text = "Quản lý sản phẩm";
        Width = 1000;
        Height = 650;
        BuildUi();
        InitData();
    }

    // ===================== UI =====================
    private void BuildUi()
    {
        var grp = new GroupBox { Text = "Thông tin sản phẩm", Dock = DockStyle.Top, Height = 150, Padding = new Padding(10) };

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 5, RowCount = 3 };
        for (int i = 0; i < 5; i++) layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        string[] caps = { "Mã sản phẩm", "Tên sản phẩm", "Đơn vị tính", "Đơn giá", "Số lượng" };
        for (int i = 0; i < caps.Length; i++)
            layout.Controls.Add(new Label { Text = caps[i], AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Bottom }, i, 0);

        txtMa = NewBox(); txtTen = NewBox(); txtDvt = NewBox(); txtGia = NewBox(); txtSl = NewBox();
        layout.Controls.Add(txtMa, 0, 1);
        layout.Controls.Add(txtTen, 1, 1);
        layout.Controls.Add(txtDvt, 2, 1);
        layout.Controls.Add(txtGia, 3, 1);
        layout.Controls.Add(txtSl, 4, 1);

        btnAdd = NewButton("Thêm sản phẩm", BtnAdd_Click);
        btnEdit = NewButton("Sửa sản phẩm", BtnEdit_Click);
        btnDelete = NewButton("Xóa sản phẩm", BtnDelete_Click);
        btnClear = NewButton("Làm mới", (s, e) => ClearInput());
        layout.Controls.Add(btnAdd, 0, 2);
        layout.Controls.Add(btnEdit, 1, 2);
        layout.Controls.Add(btnDelete, 2, 2);
        layout.Controls.Add(btnClear, 3, 2);

        grp.Controls.Add(layout);

        dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        dgv.SelectionChanged += Dgv_SelectionChanged;

        Controls.Add(dgv);   // Fill thêm trước, Top thêm sau để không bị che
        Controls.Add(grp);
    }

    private static TextBox NewBox() => new TextBox { Dock = DockStyle.Fill, Margin = new Padding(3, 3, 8, 3) };

    private static Button NewButton(string text, EventHandler onClick)
    {
        var b = new Button { Text = text, Dock = DockStyle.Fill, Margin = new Padding(3, 3, 8, 3) };
        b.Click += onClick;
        return b;
    }

    // ===================== Dữ liệu =====================
    private void InitData()
    {
        _table = new DataTable();
        _table.Columns.Add("MaSP", typeof(string));
        _table.Columns.Add("TenSP", typeof(string));
        _table.Columns.Add("DonViTinh", typeof(string));
        _table.Columns.Add("DonGia", typeof(decimal));
        _table.Columns.Add("SoLuong", typeof(int));
        _table.PrimaryKey = new[] { _table.Columns["MaSP"] };

        _table.Rows.Add("SP001", "Sữa tươi", "Hộp", 32000m, 50);
        _table.Rows.Add("SP002", "Mì ăn liền", "Gói", 4500m, 200);

        dgv.DataSource = _table;
    }

    // Chọn dòng trên lưới -> đổ dữ liệu lên các ô nhập
    private void Dgv_SelectionChanged(object sender, EventArgs e)
    {
        if (dgv.CurrentRow?.DataBoundItem is not DataRowView v) return;

        txtMa.Text = v["MaSP"].ToString();
        txtTen.Text = v["TenSP"].ToString();
        txtDvt.Text = v["DonViTinh"].ToString();
        txtGia.Text = v["DonGia"].ToString();
        txtSl.Text = v["SoLuong"].ToString();
        txtMa.ReadOnly = true;   // không cho đổi khóa chính khi đang sửa
    }

    private bool ReadInput(out decimal gia, out int sl)
    {
        gia = 0; sl = 0;

        if (string.IsNullOrWhiteSpace(txtMa.Text) || string.IsNullOrWhiteSpace(txtTen.Text) ||
            string.IsNullOrWhiteSpace(txtDvt.Text))
        {
            MessageBox.Show("Vui lòng nhập đầy đủ Mã, Tên và Đơn vị tính.", "Thiếu dữ liệu");
            return false;
        }
        if (!decimal.TryParse(txtGia.Text, out gia) || gia < 0)
        {
            MessageBox.Show("Đơn giá phải là số không âm.", "Dữ liệu không hợp lệ");
            txtGia.Focus();
            return false;
        }
        if (!int.TryParse(txtSl.Text, out sl) || sl < 0)
        {
            MessageBox.Show("Số lượng phải là số nguyên không âm.", "Dữ liệu không hợp lệ");
            txtSl.Focus();
            return false;
        }
        return true;
    }

    // ===================== Thêm / Sửa / Xóa / Làm mới =====================
    private void BtnAdd_Click(object sender, EventArgs e)
    {
        if (!ReadInput(out var gia, out var sl)) return;

        string ma = txtMa.Text.Trim();
        if (_table.Rows.Contains(ma))
        {
            MessageBox.Show("Mã sản phẩm đã tồn tại.", "Trùng mã");
            return;
        }
        _table.Rows.Add(ma, txtTen.Text.Trim(), txtDvt.Text.Trim(), gia, sl);
        ClearInput();
    }

    private void BtnEdit_Click(object sender, EventArgs e)
    {
        var row = _table.Rows.Find(txtMa.Text.Trim());
        if (row == null)
        {
            MessageBox.Show("Hãy chọn sản phẩm cần sửa trên lưới.", "Chưa chọn");
            return;
        }
        if (!ReadInput(out var gia, out var sl)) return;

        row["TenSP"] = txtTen.Text.Trim();
        row["DonViTinh"] = txtDvt.Text.Trim();
        row["DonGia"] = gia;
        row["SoLuong"] = sl;
    }

    private void BtnDelete_Click(object sender, EventArgs e)
    {
        var row = _table.Rows.Find(txtMa.Text.Trim());
        if (row == null)
        {
            MessageBox.Show("Hãy chọn sản phẩm cần xóa trên lưới.", "Chưa chọn");
            return;
        }
        if (MessageBox.Show($"Xóa sản phẩm {txtMa.Text}?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        row.Delete();
        _table.AcceptChanges();
        ClearInput();
    }

    private void ClearInput()
    {
        txtMa.ReadOnly = false;
        txtMa.Clear(); txtTen.Clear(); txtDvt.Clear(); txtGia.Clear(); txtSl.Clear();
        dgv.ClearSelection();
        txtMa.Focus();
    }
}
