namespace TiemTapHoa;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        // ---------- GroupBox thông tin ----------
        grpThongTin = new GroupBox { Text = "Thông tin hàng hóa", Location = new Point(12, 8), Size = new Size(520, 155) };
        lblTen = new Label { Text = "Tên hàng:", Location = new Point(15, 30), AutoSize = true };
        txtTenHang = new TextBox { Location = new Point(90, 27), Size = new Size(410, 23), MaxLength = 100 };
        lblLoai = new Label { Text = "Loại hàng:", Location = new Point(15, 62), AutoSize = true };
        cboLoai = new ComboBox { Location = new Point(90, 59), Size = new Size(150, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cboLoai.Items.AddRange(new object[] { "Đồ uống", "Thực phẩm khô", "Gia vị", "Đồ gia dụng" });
        lblDonVi = new Label { Text = "Đơn vị:", Location = new Point(15, 94), AutoSize = true };
        cboDonVi = new ComboBox { Location = new Point(90, 91), Size = new Size(150, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cboDonVi.Items.AddRange(new object[] { "Chai", "Gói", "Hộp", "Túi" });
        lblGia = new Label { Text = "Giá bán:", Location = new Point(15, 126), AutoSize = true };
        nudGia = new NumericUpDown
        {
            Location = new Point(90, 123), Size = new Size(150, 23),
            Minimum = 0, Maximum = 1000000000, Increment = 500, DecimalPlaces = 0, ThousandsSeparator = true
        };
        lblTon = new Label { Text = "Số lượng tồn:", Location = new Point(260, 62), AutoSize = true };
        nudTon = new NumericUpDown { Location = new Point(350, 59), Size = new Size(150, 23), Minimum = 0, Maximum = 1000000, ThousandsSeparator = true };
        lblNgay = new Label { Text = "Ngày nhập:", Location = new Point(260, 94), AutoSize = true };
        dtpNgay = new DateTimePicker { Location = new Point(350, 91), Size = new Size(150, 23), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
        chkConKD = new CheckBox { Text = "Còn kinh doanh", Location = new Point(350, 126), AutoSize = true, Checked = true };
        grpThongTin.Controls.AddRange(new Control[] { lblTen, txtTenHang, lblLoai, cboLoai, lblDonVi, cboDonVi, lblGia, nudGia, lblTon, nudTon, lblNgay, dtpNgay, chkConKD });

        // ---------- GroupBox tìm kiếm ----------
        grpTim = new GroupBox { Text = "Tìm kiếm", Location = new Point(545, 8), Size = new Size(303, 155), Anchor = AnchorStyles.Top | AnchorStyles.Right };
        txtTim = new TextBox { Location = new Point(15, 27), Size = new Size(272, 23), PlaceholderText = "Nhập tên hàng cần tìm..." };
        cboLocLoai = new ComboBox { Location = new Point(15, 59), Size = new Size(272, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cboLocLoai.Items.AddRange(new object[] { "Tất cả loại", "Đồ uống", "Thực phẩm khô", "Gia vị", "Đồ gia dụng" });
        chkChiCon = new CheckBox { Text = "Chỉ hiện hàng còn kinh doanh", Location = new Point(15, 93), AutoSize = true };
        grpTim.Controls.AddRange(new Control[] { txtTim, cboLocLoai, chkChiCon });

        tmrTim = new System.Windows.Forms.Timer(components) { Interval = 350 };

        // ---------- DataGridView ----------
        dgv = new DataGridView
        {
            Location = new Point(12, 172), Size = new Size(836, 290),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            AutoGenerateColumns = false, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            ReadOnly = true, MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            BackgroundColor = SystemColors.AppWorkspace
        };
        var right = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight };
        var rightN0 = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" };
        var dateStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "dd/MM/yyyy" };
        dgv.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn { Name = "colMa",    HeaderText = "Mã",        DataPropertyName = "MaHang",     Width = 50 },
            new DataGridViewTextBoxColumn { Name = "colTen",   HeaderText = "Tên hàng",  DataPropertyName = "TenHang",    Width = 230 },
            new DataGridViewTextBoxColumn { Name = "colLoai",  HeaderText = "Loại hàng", DataPropertyName = "LoaiHang",   Width = 110 },
            new DataGridViewTextBoxColumn { Name = "colDonVi", HeaderText = "Đơn vị",    DataPropertyName = "DonVi",      Width = 80 },
            new DataGridViewTextBoxColumn { Name = "colGia",   HeaderText = "Giá bán",   DataPropertyName = "GiaBan",     Width = 90,  DefaultCellStyle = rightN0 },
            new DataGridViewTextBoxColumn { Name = "colTon",   HeaderText = "Tồn",       DataPropertyName = "SoLuongTon", Width = 60,  DefaultCellStyle = right },
            new DataGridViewTextBoxColumn { Name = "colNgay",  HeaderText = "Ngày nhập", DataPropertyName = "NgayNhap",   Width = 100, DefaultCellStyle = dateStyle },
            new DataGridViewCheckBoxColumn{ Name = "colKD",    HeaderText = "Kinh doanh",DataPropertyName = "ConKinhDoanh", Width = 85 },
        });

        // ---------- Nút ----------
        const AnchorStyles bl = AnchorStyles.Bottom | AnchorStyles.Left;
        btnThem = new Button { Text = "Thêm", Location = new Point(12, 472), Size = new Size(80, 30), Anchor = bl };
        btnSua = new Button { Text = "Sửa", Location = new Point(98, 472), Size = new Size(80, 30), Anchor = bl };
        btnXoa = new Button { Text = "Xóa", Location = new Point(184, 472), Size = new Size(80, 30), Anchor = bl };
        btnLamMoi = new Button { Text = "Làm mới", Location = new Point(270, 472), Size = new Size(80, 30), Anchor = bl };
        btnXemBangGia = new Button { Text = "Xem bảng giá", Location = new Point(400, 472), Size = new Size(100, 30), Anchor = bl };
        btnXuatPdf = new Button { Text = "Xuất PDF", Location = new Point(506, 472), Size = new Size(80, 30), Anchor = bl };
        btnXuatExcelLuoi = new Button { Text = "Xuất Excel (lưới)", Location = new Point(592, 472), Size = new Size(110, 30), Anchor = bl };
        btnXuatExcelTonKho = new Button { Text = "Xuất Excel tồn kho", Location = new Point(708, 472), Size = new Size(140, 30), Anchor = bl };

        // ---------- Status ----------
        statusStrip1 = new StatusStrip();
        lblTrangThai = new ToolStripStatusLabel { Text = "Sẵn sàng" };
        statusStrip1.Items.Add(lblTrangThai);

        // ---------- Form ----------
        AutoScaleMode = AutoScaleMode.Font;
        Font = new Font("Segoe UI", 9F);
        ClientSize = new Size(860, 535);
        MinimumSize = new Size(880, 520);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Quản lý hàng hóa - Tạp hóa UTC2";
        Controls.AddRange(new Control[]
        {
            grpThongTin, grpTim, dgv, btnThem, btnSua, btnXoa, btnLamMoi,
            btnXemBangGia, btnXuatPdf, btnXuatExcelLuoi, btnXuatExcelTonKho, statusStrip1
        });

        // ---------- Sự kiện ----------
        Load += MainForm_Load;
        dgv.SelectionChanged += dgv_SelectionChanged;
        txtTim.TextChanged += txtTim_TextChanged;
        tmrTim.Tick += tmrTim_Tick;
        cboLocLoai.SelectedIndexChanged += LocThayDoi;
        chkChiCon.CheckedChanged += LocThayDoi;
        btnThem.Click += btnThem_Click;
        btnSua.Click += btnSua_Click;
        btnXoa.Click += btnXoa_Click;
        btnLamMoi.Click += btnLamMoi_Click;
        btnXemBangGia.Click += btnXemBangGia_Click;
        btnXuatPdf.Click += btnXuatPdf_Click;
        btnXuatExcelLuoi.Click += btnXuatExcelLuoi_Click;
        btnXuatExcelTonKho.Click += btnXuatExcelTonKho_Click;
    }

    private GroupBox grpThongTin, grpTim;
    private Label lblTen, lblLoai, lblDonVi, lblGia, lblTon, lblNgay;
    private TextBox txtTenHang, txtTim;
    private ComboBox cboLoai, cboDonVi, cboLocLoai;
    private NumericUpDown nudGia, nudTon;
    private DateTimePicker dtpNgay;
    private CheckBox chkConKD, chkChiCon;
    private System.Windows.Forms.Timer tmrTim;
    private DataGridView dgv;
    private Button btnThem, btnSua, btnXoa, btnLamMoi, btnXemBangGia, btnXuatPdf, btnXuatExcelLuoi, btnXuatExcelTonKho;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel lblTrangThai;
}
