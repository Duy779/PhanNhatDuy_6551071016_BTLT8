namespace QuanCaPhe;

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
        const string fmtNgay = "dd/MM/yyyy";

        // ---------- Tìm kiếm ----------
        grpTim = new GroupBox { Text = "Tìm kiếm hóa đơn", Location = new Point(10, 6), Size = new Size(980, 72), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        lblTu = new Label { Text = "Từ ngày", Location = new Point(15, 17), AutoSize = true };
        dtpTu = new DateTimePicker { Location = new Point(15, 36), Size = new Size(110, 23), Format = DateTimePickerFormat.Custom, CustomFormat = fmtNgay };
        lblDen = new Label { Text = "Đến ngày", Location = new Point(140, 17), AutoSize = true };
        dtpDen = new DateTimePicker { Location = new Point(140, 36), Size = new Size(110, 23), Format = DateTimePickerFormat.Custom, CustomFormat = fmtNgay };
        lblLocBan = new Label { Text = "Số bàn", Location = new Point(265, 17), AutoSize = true };
        cboLocBan = new ComboBox { Location = new Point(265, 36), Size = new Size(100, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cboLocBan.Items.Add("Tất cả");
        for (int i = 1; i <= 10; i++) cboLocBan.Items.Add(i.ToString());
        lblLocHT = new Label { Text = "Hình thức", Location = new Point(385, 17), AutoSize = true };
        cboLocHT = new ComboBox { Location = new Point(385, 36), Size = new Size(120, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cboLocHT.Items.AddRange(new object[] { "Tất cả", "Tiền mặt", "Chuyển khoản" });
        lblLocMon = new Label { Text = "Tên món", Location = new Point(525, 17), AutoSize = true };
        txtLocMon = new TextBox { Location = new Point(525, 36), Size = new Size(250, 23) };
        btnTim = new Button { Text = "Tìm", Location = new Point(800, 34), Size = new Size(100, 27) };
        grpTim.Controls.AddRange(new Control[] { lblTu, dtpTu, lblDen, dtpDen, lblLocBan, cboLocBan, lblLocHT, cboLocHT, lblLocMon, txtLocMon, btnTim });

        // ---------- Danh sách hóa đơn (trái) ----------
        grpDS = new GroupBox { Text = "Danh sách hóa đơn", Location = new Point(10, 84), Size = new Size(430, 458), Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left };
        dgvHD = new DataGridView
        {
            Location = new Point(10, 22), Size = new Size(410, 372),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            AutoGenerateColumns = false, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            ReadOnly = true, MultiSelect = false, RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect, BackgroundColor = SystemColors.AppWorkspace
        };
        var right = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight };
        var rightN0 = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N0" };
        var ngayGio = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" };
        dgvHD.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn { Name = "colMaHD",  HeaderText = "Mã HD",        DataPropertyName = "MaHDText",   Width = 75 },
            new DataGridViewTextBoxColumn { Name = "colNgay",  HeaderText = "Ngày lập",     DataPropertyName = "NgayLap",    Width = 115, DefaultCellStyle = ngayGio },
            new DataGridViewTextBoxColumn { Name = "colBan",   HeaderText = "Số bàn",       DataPropertyName = "SoBan",      Width = 50 },
            new DataGridViewTextBoxColumn { Name = "colHT",    HeaderText = "Hình thức TT", DataPropertyName = "HinhThucTT", Width = 90 },
            new DataGridViewTextBoxColumn { Name = "colTong",  HeaderText = "Tổng tiền",    DataPropertyName = "TongTien",   Width = 78, DefaultCellStyle = rightN0 },
        });
        lblTongDT = new Label
        {
            Text = "Tổng doanh thu đang hiển thị: 0 đ", Location = new Point(10, 400), Size = new Size(410, 50),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft
        };
        grpDS.Controls.AddRange(new Control[] { dgvHD, lblTongDT });

        // ---------- Chi tiết hóa đơn (phải) ----------
        grpCT = new GroupBox { Text = "Chi tiết hóa đơn", Location = new Point(450, 84), Size = new Size(540, 458), Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
        dtpNgay = new DateTimePicker { Location = new Point(15, 25), Size = new Size(150, 23), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm" };
        lblBan = new Label { Text = "Số bàn:", Location = new Point(180, 29), AutoSize = true };
        cboBan = new ComboBox { Location = new Point(232, 25), Size = new Size(55, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        for (int i = 1; i <= 10; i++) cboBan.Items.Add(i.ToString());
        radTienMat = new RadioButton { Text = "Tiền mặt", Location = new Point(305, 27), AutoSize = true, Checked = true };
        radChuyenKhoan = new RadioButton { Text = "Chuyển khoản", Location = new Point(395, 27), AutoSize = true };
        lblGhiChu = new Label { Text = "Ghi chú:", Location = new Point(15, 62), AutoSize = true };
        txtGhiChu = new TextBox { Location = new Point(80, 58), Size = new Size(445, 52), Multiline = true, ScrollBars = ScrollBars.Vertical, MaxLength = 200 };
        lblMon = new Label { Text = "Chọn món:", Location = new Point(15, 124), AutoSize = true };
        cboMon = new ComboBox { Location = new Point(80, 120), Size = new Size(250, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        lblSL = new Label { Text = "SL:", Location = new Point(342, 124), AutoSize = true };
        nudSL = new NumericUpDown { Location = new Point(368, 120), Size = new Size(60, 23), Minimum = 1, Maximum = 50, Value = 1 };
        btnThemMon = new Button { Text = "Thêm món", Location = new Point(440, 118), Size = new Size(85, 27) };

        dgvCT = new DataGridView
        {
            Location = new Point(15, 155), Size = new Size(510, 238),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            AutoGenerateColumns = false, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            ReadOnly = true, MultiSelect = false, RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect, BackgroundColor = SystemColors.AppWorkspace
        };
        dgvCT.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn { Name = "colSTT",   HeaderText = "STT",        DataPropertyName = "STT",       Width = 40 },
            new DataGridViewTextBoxColumn { Name = "colTen",   HeaderText = "Tên món",    DataPropertyName = "TenMon",    Width = 200 },
            new DataGridViewTextBoxColumn { Name = "colSL",    HeaderText = "SL",         DataPropertyName = "SoLuong",   Width = 45 },
            new DataGridViewTextBoxColumn { Name = "colGia",   HeaderText = "Đơn giá",    DataPropertyName = "DonGia",    Width = 100, DefaultCellStyle = rightN0 },
            new DataGridViewTextBoxColumn { Name = "colTT",    HeaderText = "Thành tiền", DataPropertyName = "ThanhTien", Width = 110, DefaultCellStyle = rightN0 },
        });
        lblTongCong = new Label
        {
            Text = "Tổng cộng: 0 đ", Location = new Point(15, 402), Size = new Size(510, 46),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            Font = new Font("Segoe UI", 16F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleRight
        };
        grpCT.Controls.AddRange(new Control[] { dtpNgay, lblBan, cboBan, radTienMat, radChuyenKhoan, lblGhiChu, txtGhiChu, lblMon, cboMon, lblSL, nudSL, btnThemMon, dgvCT, lblTongCong });

        // ---------- Nút ----------
        const AnchorStyles bl = AnchorStyles.Bottom | AnchorStyles.Left;
        btnHDMoi = new Button { Text = "Hóa đơn mới", Location = new Point(10, 552), Size = new Size(100, 30), Anchor = bl };
        btnSuaHD = new Button { Text = "Sửa hóa đơn", Location = new Point(116, 552), Size = new Size(100, 30), Anchor = bl };
        btnXoaHD = new Button { Text = "Xóa hóa đơn", Location = new Point(222, 552), Size = new Size(100, 30), Anchor = bl };
        btnSuaSL = new Button { Text = "Sửa SL", Location = new Point(450, 552), Size = new Size(80, 30), Anchor = bl };
        btnXoaMon = new Button { Text = "Xóa món", Location = new Point(536, 552), Size = new Size(80, 30), Anchor = bl };
        btnXemHD = new Button { Text = "Xem hóa đơn", Location = new Point(622, 552), Size = new Size(100, 30), Anchor = bl };
        btnXuatPdf = new Button { Text = "Xuất PDF", Location = new Point(728, 552), Size = new Size(90, 30), Anchor = bl };
        btnIn = new Button { Text = "In", Location = new Point(824, 552), Size = new Size(80, 30), Anchor = bl };

        // ---------- Form ----------
        AutoScaleMode = AutoScaleMode.Font;
        Font = new Font("Segoe UI", 9F);
        ClientSize = new Size(1000, 595);
        MinimumSize = new Size(1020, 640);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Quán cà phê Góc Phố - Quản lý hóa đơn";
        Controls.AddRange(new Control[] { grpTim, grpDS, grpCT, btnHDMoi, btnSuaHD, btnXoaHD, btnSuaSL, btnXoaMon, btnXemHD, btnXuatPdf, btnIn });

        // ---------- Sự kiện ----------
        Load += MainForm_Load;
        btnTim.Click += btnTim_Click;
        dgvHD.SelectionChanged += dgvHD_SelectionChanged;
        dgvCT.SelectionChanged += dgvCT_SelectionChanged;
        btnHDMoi.Click += btnHDMoi_Click;
        btnSuaHD.Click += btnSuaHD_Click;
        btnXoaHD.Click += btnXoaHD_Click;
        btnThemMon.Click += btnThemMon_Click;
        btnSuaSL.Click += btnSuaSL_Click;
        btnXoaMon.Click += btnXoaMon_Click;
        btnXemHD.Click += btnXemHD_Click;
        btnXuatPdf.Click += btnXuatPdf_Click;
        btnIn.Click += btnIn_Click;
    }

    private GroupBox grpTim, grpDS, grpCT;
    private Label lblTu, lblDen, lblLocBan, lblLocHT, lblLocMon, lblBan, lblGhiChu, lblMon, lblSL, lblTongDT, lblTongCong;
    private DateTimePicker dtpTu, dtpDen, dtpNgay;
    private ComboBox cboLocBan, cboLocHT, cboBan, cboMon;
    private TextBox txtLocMon, txtGhiChu;
    private RadioButton radTienMat, radChuyenKhoan;
    private NumericUpDown nudSL;
    private DataGridView dgvHD, dgvCT;
    private Button btnTim, btnThemMon, btnHDMoi, btnSuaHD, btnXoaHD, btnSuaSL, btnXoaMon, btnXemHD, btnXuatPdf, btnIn;
}
