/*  MainWindow.xaml.cs class standarts
 * 
 *  The code in MainWindow class is separated into regions in order to easily find needed method.
 *  Methods with Bt prefixes are linked to Manage Brand/Type tab and implement GUI/Logics only inside it.
 *  Methods with Mp prefixes are linked to Manage Product tab respectively.
 *  Rs are Refill Stock methods.
 *  
 *  Each method name is designed such a way that the purpose of method action 
 *  could be easily understanded mostly just by reading it's name.
 */

namespace Zvilgija
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;

    using ControllerLayer;

    using DelegateArgsLayer;

    using GUI;

    using ModuleLayer;

    using Type = ModuleLayer.Type;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Fields

        private readonly BrandTypeController brandTypeController = new BrandTypeController();

        private readonly EmployeeController employeeController = new EmployeeController();

        private readonly ProductController productController = new ProductController();

        private readonly ScheduleController scheduleController = new ScheduleController();

        private readonly StockController stockController = new StockController();

        private List<Product> StockSelectedProducts = new List<Product>();

        private LoginWindow login; //Variable for login window

        #endregion

        #region Constructors and Destructors

        public MainWindow()
        {
            try
            {
                this.LogoEntry();
                this.SetDefaultCulture();
                this.InitializeComponent();
                this.Visibility = Visibility.Hidden;
                this.LoginEntry();
            }
            catch
            {
                MessageBox.Show("We experience some problems while connecting to the database.\r\nPlease check if you are connected to the internet and/or you have an access to the database.\r\n\r\nThe program will be terminated now.", "Database connection error");
                Environment.Exit(0);
            }

            this.employeeController.PopulateEmployeeList();
            this.EmUpdateColumnProperties();
            this.EmUpdateItemsInColumns();

            this.brandTypeController.PopulateBrandList();
            this.brandTypeController.PopulateTypeList();

            const string defaultLable = "Brand";

            this.BtUpdateColumnProperties(defaultLable);
            this.BtUpdateItemsInColumns(defaultLable);

            this.productController.PopulateProductList();

            this.MpUpdateColumnProperties();
            this.MpUpdateItemsInColumns();

            this.stockController.PopulateStockList(this.employeeController.EmployeeList);

            this.RsUpdateColumnProperties();
            this.RsUpdateItemsInColumns();

            this.SlsUpdateColumnProperties();
            this.SlsUpdateItemsInColumns();

            this.BrandCb_Initialized();
            this.TypeCb_Initialized();
            this.ProductCb_Initialized();
            this.VolumeCb_Initialized();

            this.scheduleController.PopulateAppointmentList(this.employeeController.EmployeeList);
            this.scheduleController.PopulateWorkingDaysList(this.employeeController.EmployeeList);

            /*foreach (var curAppointment in this.scheduleController.Appointments)
                MessageBox.Show(curAppointment.Date.ToString("dd.MM.yy") + " " + curAppointment.Employee.Name + " " + curAppointment.Employee.SurName + " ||| FOR Client: " + curAppointment.Client.Name + " " + curAppointment.ServiceType);

            var test ="";
            foreach (var curShift in this.scheduleController.WorkingDays)
                foreach (var curEmployee in curShift.Employees)
                    if (curShift.Date.ToString("dd.MM.yy").Equals(DateTime.Now.ToString(("dd.MM.yy"))))
                        test += curEmployee.Name + " " + curEmployee.SurName + " ";

            MessageBox.Show(test);*/

            this.ScUpdateColumnProperties();
            string currentDate = DateTime.Today.ToString("dd.MM.yy");
            this.ScUpdateItemsInColumns(currentDate);
            this.ScDatePicker.Text = currentDate;

            ChooseSpecialization.AddVariables += this.InitializeSpecializationVariables;
            ManageSchedule.UpdateScheduleListEvent += this.InitializeUpdateScheduleListAction;
            RegisterAppointment.UpdateAppointmentListEvent += this.InitializeUpdateScheduleListAction;
        }

        #endregion

        #region Public Methods and Operators

        public void ComboBoxInitialize<T>(IReadOnlyList<T> curList, Selector curCombobox, bool volumeWorkaround)
        {
            if (curList == null)
                return;

            curCombobox.Items.Clear();

            foreach (T curItem in curList.Where(curItem => !curCombobox.Items.Contains(curItem.ToString())))
            {
                if (volumeWorkaround)
                {
                    var curProduct = curItem as Product;
                    if (curProduct != null)
                        curCombobox.Items.Add(curProduct.Volume);
                    continue;
                }
                curCombobox.Items.Add(curItem.ToString());
            }
        }

        public void ReturnResult(int returnResult, bool showMgs)
        {
            if (returnResult.Equals(int.MinValue))
                throw new SqlNotFilledException();

            if (returnResult.Equals(-2))
            {
                MessageBox.Show("There is already such element in the database. Please enter a unique name.");
                throw new NullReferenceException();
            }

            if (showMgs)
                MessageBox.Show(returnResult.Equals(-1) ? "Record might been added to the database, but the system cannot retrieve it's id.\nPlease, try reopening the program." : "Records have been added to the database.");
        }

        public void SetDefaultCulture()
        {
            var ci = new CultureInfo("da-DK");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            CultureInfo.DefaultThreadCurrentCulture = ci;
        }

        public void UpdateStockList(IReadOnlyList<Stock> stockList)
        {
            this.StockGrid.Items.Add(stockList[stockList.Count - 1]);
        }

        #endregion

        #region Methods

        private static bool ComboBoxCheck(ItemsControl sender)
        {
            return sender != null && !sender.Items.IsEmpty;
        }

        private static List<DataGridTextColumn> EmInitializeColumns()
        {
            return new List<DataGridTextColumn>
                       {
                           new DataGridTextColumn { Header = "ID", Width = 30, Binding = new Binding("Id") },
                           new DataGridTextColumn { Header = "Name", Width = new DataGridLength(0.375, DataGridLengthUnitType.Star), Binding = new Binding("Name") },
                           new DataGridTextColumn { Header = "Surname", Width = new DataGridLength(0.375, DataGridLengthUnitType.Star), Binding = new Binding("SurName") },
                           new DataGridTextColumn { Header = "Phone Number ", Width = new DataGridLength(0.25, DataGridLengthUnitType.Star), Binding = new Binding("TelNumber") }
                       };
        }

        private static List<DataGridTextColumn> MpInitializeColumns()
        {
            return new List<DataGridTextColumn>
                       {
                           new DataGridTextColumn { Header = "ID", Width = 30, Binding = new Binding("Id") },
                           new DataGridTextColumn { Header = "Brand", Width = new DataGridLength(0.3, DataGridLengthUnitType.Star), Binding = new Binding("Brand") },
                           new DataGridTextColumn { Header = "Type", Width = new DataGridLength(0.3, DataGridLengthUnitType.Star), Binding = new Binding("Type") },
                           new DataGridTextColumn { Header = "Name", Width = new DataGridLength(0.33, DataGridLengthUnitType.Star), Binding = new Binding("Name") },
                           new DataGridTextColumn { Header = "Volume", Width = new DataGridLength(0, DataGridLengthUnitType.Auto), Binding = new Binding("Volume") },
                           new DataGridTextColumn { Header = "Quantity", Width = new DataGridLength(0, DataGridLengthUnitType.Auto), Binding = new Binding("Quantity") }
                       };
        }

        private static IReadOnlyList<DataGridTextColumn> ScheduleJobsInitializeColumns()
        {
            return new List<DataGridTextColumn> { new DataGridTextColumn { Header = "Name", Width = new DataGridLength(0.5, DataGridLengthUnitType.Star), Binding = new Binding("Name") }, new DataGridTextColumn { Header = "Surname", Width = new DataGridLength(0.5, DataGridLengthUnitType.Star), Binding = new Binding("SurName") } };
        }

        private static IReadOnlyList<DataGridTextColumn> ScheduleListInitializeColumns()
        {
            return new List<DataGridTextColumn>
                       {
                           new DataGridTextColumn { Header = "From", Width = 40, Binding = new Binding("TimeFrom") },
                           new DataGridTextColumn { Header = "To", Width = 40, Binding = new Binding("TimeTo") },
                           new DataGridTextColumn { Header = "Client", Width = new DataGridLength(0.2, DataGridLengthUnitType.Star), Binding = new Binding("ClientName") },
                           new DataGridTextColumn { Header = "Phone number", Width = new DataGridLength(0.175, DataGridLengthUnitType.Star), Binding = new Binding("ClientPhone") },
                           new DataGridTextColumn { Header = "Employee", Width = new DataGridLength(0.2, DataGridLengthUnitType.Star), Binding = new Binding("Employee") },
                           new DataGridTextColumn { Header = "Job", Width = new DataGridLength(0.175, DataGridLengthUnitType.Star), Binding = new Binding("ServiceType") }
                       };
        }

        private static string SelectItemsInCombobox(Selector comboBox)
        {
            string returnValue = comboBox.SelectedItem != null ? comboBox.SelectedItem.ToString() : null;
            comboBox.Items.Clear();
            return returnValue;
        }

        private static List<DataGridTextColumn> StockInitializeColumns()
        {
            return new List<DataGridTextColumn>
                       {
                           new DataGridTextColumn { Header = "Date", Width = new DataGridLength(0, DataGridLengthUnitType.Auto), Binding = new Binding("Date") },
                           new DataGridTextColumn { Header = "Employee", Width = new DataGridLength(0.15, DataGridLengthUnitType.Star), Binding = new Binding("Employee") },
                           new DataGridTextColumn { Header = "Product Brand", Width = new DataGridLength(0.15, DataGridLengthUnitType.Star), Binding = new Binding("Brand") },
                           new DataGridTextColumn { Header = "Product Type", Width = new DataGridLength(0.15, DataGridLengthUnitType.Star), Binding = new Binding("Type") },
                           new DataGridTextColumn { Header = "Product Name", Width = new DataGridLength(0.2, DataGridLengthUnitType.Star), Binding = new Binding("Product") },
                           new DataGridTextColumn { Header = "Volume", Width = new DataGridLength(0, DataGridLengthUnitType.Auto), Binding = new Binding("Volume") },
                           new DataGridTextColumn { Header = "Amount", Width = new DataGridLength(0, DataGridLengthUnitType.Auto), Binding = new Binding("Quantity") }
                       };
        }

        private static string UnselectItemsInCombobox(Selector comboBox)
        {
            if (comboBox.Items.Count != 1)
                return "";

            comboBox.SelectedIndex = 0;
            return null;
        }

        private static void UpdateColumnProperties(IReadOnlyList<DataGridTextColumn> curColumns, IList<DataGridColumn> gridColumns)
        {
            for (int i = 0; i < curColumns.Count; i++)
                gridColumns[i] = curColumns[i];
        }

        private static void UpdateFilteredItems(ItemsControl comboBox, string curItem)
        {
            if (!comboBox.Items.Contains(curItem))
                comboBox.Items.Add(curItem);
        }

        private static void UpdateItemsInColumns<T>(IEnumerable<T> curList, ItemsControl curGrid)
        {
            if (curList == null)
                return;

            curGrid.Items.Clear();

            foreach (T curItem in curList)
                curGrid.Items.Add(curItem);
        }

        private static void UpdateLastElementInList<T>(IReadOnlyList<T> curList, ItemsControl curGrid)
        {
            curGrid.Items.Add(curList[curList.Count - 1]);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!this.StockSelectedProducts.Count.Equals(1) || this.StockSelectedProducts == null)
                {
                    MessageBox.Show("Please select a product to add it to the system.");
                    return;
                }

                var button = sender as Button;
                TextBox curTextbox = this.RsInputTextbox;
                char curChar = 'r';

                if (button != null && button.Name.Equals("SlAddButton"))
                {
                    curTextbox = this.SlInputTextbox;
                    curChar = 's';
                }

                int inputValue = 0;

                try
                {
                    inputValue = Convert.ToInt32(curTextbox.Text);
                }
                catch {}

                if (inputValue == 0)
                {
                    MessageBox.Show("You have to specify an amount that is greather than 0.\r\nPlease try again.");
                    return;
                }

                int selectedQuantity = Math.Abs(Convert.ToInt32(curTextbox.Text));

                if (button != null && (button.Name.Equals("RsSubstractButton") || button.Name.Equals("SlAddButton")))
                {
                    try
                    {
                        if (Convert.ToInt32(this.RsRemainingLabel.Content) < selectedQuantity)
                        {
                            MessageBox.Show("You cannot deduct more than it is currently in the stock");
                            this.RsInputTextbox.Text = "0";
                            this.SlInputTextbox.Text = "0";
                            return;
                        }
                    }
                    catch {}

                    try
                    {
                        if (Convert.ToInt32(this.SlRemainingLabel.Content) < selectedQuantity)
                        {
                            MessageBox.Show("You cannot sell more than it is currently in the stock");
                            this.RsInputTextbox.Text = "0";
                            this.SlInputTextbox.Text = "0";
                            return;
                        }
                    }
                    catch {}

                    if (this.SlRemainingLabel.Content.Equals("") && this.RsRemainingLabel.Content.Equals(""))
                    {
                        MessageBox.Show("Unfortunatelly, we couldn't get the amount of selected product from the list.\r\nPlease release your selection by pressing corresponding button and try again.");
                        return;
                    }

                    selectedQuantity = -selectedQuantity;
                }

                Product selectedProduct = this.StockSelectedProducts[0];
                DateTime currentDate = DateTime.Now;
                Employee currentEmployee = this.employeeController.EmployeeList.Find(a => a.Id == Login.Id);

                int returnResult = this.stockController.AddToStock(currentDate, currentEmployee, selectedProduct, selectedQuantity, curChar);

                if (returnResult == -1)
                    returnResult = 0;

                this.ReturnResult(returnResult, false);

                returnResult = this.productController.UpdateDepedencies(selectedQuantity, selectedProduct.Id);

                if (returnResult == -1)
                    returnResult = 0;

                this.ReturnResult(returnResult, true);

                List<Stock> stockList = this.stockController.Stock;
                DataGrid curGrid = curChar.Equals('r') ? this.StockGrid : this.SalesGrid;
                UpdateLastElementInList(stockList, curGrid);

                curGrid.SelectedItem = curGrid.Items[curGrid.Items.Count - 1];
                curGrid.ScrollIntoView(curGrid.Items[curGrid.Items.Count - 1]);
                this.RsInputTextbox.Text = "0";
                this.SlInputTextbox.Text = "0";

                this.MpUpdateItemsInColumns();
            }
            catch (SqlNotFilledException)
            {
                MessageBox.Show("Product has not been added to the Database.\r\nProbably there was a problem with the connection to DB.", "Database connection error");
            }
            catch (NullReferenceException) {}
            catch
            {
                MessageBox.Show("Check if every field has values in order to add the product.", "Fields not filled error");
            }
            finally
            {
                this.ReleaseSelected_Click(null, null);
            }
        }

        private void BrandCb_Initialized()
        {
            this.ComboBoxInitialize(this.brandTypeController.Brands, this.MpBrandCb, false);
            this.ComboBoxInitialize(this.brandTypeController.Brands, this.RsBrandCb, false);
            this.ComboBoxInitialize(this.brandTypeController.Brands, this.SlBrandCb, false);
        }

        private void BrandCb_Selector(object sender, RoutedEventArgs routedEventArgs)
        {
            Predicate<Product> predicate = item => item.Brand.BrandName == ((ComboBox) sender).SelectedValue.ToString();
            this.ComboBoxSelector(predicate, sender as ComboBox);
        }

        private void BrandTypeList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var brandSelection = this.BrandTypeGrid.SelectedItem as Brand;
                if (brandSelection != null)
                {
                    this.BtIdSelection.Text = brandSelection.Id.ToString(CultureInfo.InvariantCulture);
                    this.BtNameSelection.Text = brandSelection.BrandName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }

            try
            {
                var typeSeletion = this.BrandTypeGrid.SelectedItem as Type;

                if (typeSeletion == null)
                    return;

                this.BtIdSelection.Text = typeSeletion.Id.ToString(CultureInfo.InvariantCulture);
                this.BtNameSelection.Text = typeSeletion.TypeName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void BrandTypeList_ReleaseSelected()
        {
            this.BrandTypeGrid.SelectedItem = null;
            this.BtIdSelection.Text = "Not selected";
            this.BtNameSelection.Text = "Not selected";
        }

        private void BtAddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string content = this.BtTextBoxAdd.Text;
                if (content == "")
                    throw new Exception();
                if (this.BrandRButton.IsChecked != null && (bool) this.BrandRButton.IsChecked)
                {
                    int returnResult = this.brandTypeController.AddToBrand(content);

                    this.ReturnResult(returnResult, true);

                    List<Brand> brandList = this.brandTypeController.Brands;
                    this.BtUpdateColumnProperties("Brand");
                    UpdateLastElementInList(brandList, this.BrandTypeGrid);
                }
                else if (this.TypeRButton.IsChecked != null && (bool) this.TypeRButton.IsChecked)
                {
                    int returnResult = this.brandTypeController.AddToType(content);

                    this.ReturnResult(returnResult, true);

                    List<Type> typeList = this.brandTypeController.Types;
                    this.BtUpdateColumnProperties("Type");
                    UpdateLastElementInList(typeList, this.BrandTypeGrid);
                }

                this.BrandTypeGrid.SelectedItem = this.BrandTypeGrid.Items[this.BrandTypeGrid.Items.Count - 1];
                this.BrandTypeGrid.ScrollIntoView(this.BrandTypeGrid.Items[this.BrandTypeGrid.Items.Count - 1]);

                this.BrandCb_Initialized();
                this.TypeCb_Initialized();

                this.BtTextBoxAdd.Text = "";
            }
            catch (SqlNotFilledException)
            {
                MessageBox.Show("Product has not been added to the Database. Probably there was a problem with the connection to DB.", "Database connection error");
            }
            catch (NullReferenceException) {}
            catch
            {
                MessageBox.Show("Check if the brand/type name is entered in order to add the product.", "Fields not filled error");
            }
        }

        private void BtChangeLables(string label)
        {
            if (this.BtSelectedLabel != null)
                this.BtSelectedLabel.Content = "Selected " + label;
            if (this.BtLabelAdd != null)
                this.BtLabelAdd.Content = label + " name";
        }

        private Tuple<DataGridTextColumn, DataGridTextColumn> BtInitializeColumns(string label)
        {
            return Tuple.Create(new DataGridTextColumn { Header = "ID", Width = 30, Binding = new Binding("Id") }, new DataGridTextColumn { Header = label, Width = new DataGridLength(1, DataGridLengthUnitType.Star), Binding = new Binding(label + "Name") });
        }

        private void BtRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            var curButton = sender as RadioButton;
            if (this.BrandTypeGrid == null || curButton == null)
                return;

            string curString = curButton.Name.Equals("BrandRButton") ? "Brand" : "Type";

            this.BtChangeLables(curString);

            this.BrandTypeList_ReleaseSelected();
            this.BrandTypeGrid.Items.Clear();

            this.BtUpdateColumnProperties(curString);
            this.BtUpdateItemsInColumns(curString);
        }

        private void BtUpdateColumnProperties(string label)
        {
            Tuple<DataGridTextColumn, DataGridTextColumn> curTuple = this.BtInitializeColumns(label);

            this.BrandTypeGrid.Columns[0] = curTuple.Item1;
            this.BrandTypeGrid.Columns[1] = curTuple.Item2;
        }

        private void BtUpdateItemsInColumns(string label)
        {
            if (label.Equals("Brand"))
                UpdateItemsInColumns(this.brandTypeController.Brands, this.BrandTypeGrid);
            else
                UpdateItemsInColumns(this.brandTypeController.Types, this.BrandTypeGrid);
        }

        private void ComboBoxSelector(Predicate<Product> predicate, ItemsControl sender)
        {
            if (!ComboBoxCheck(sender))
                return;
            this.StockSelectedProducts = this.StockSelectedProducts.Count > 0 ? this.StockSelectedProducts.FindAll(predicate) : this.productController.Products.FindAll(predicate);

            if (sender.Name.Equals("RsBrandCb") || sender.Name.Equals("RsTypeCb") || sender.Name.Equals("RsProductCb") || sender.Name.Equals("RsVolumeCb"))
                this.UpdateCb(this.RsBrandCb, this.RsTypeCb, this.RsProductCb, this.RsVolumeCb, this.RsRemainingLabel);
            else
                this.UpdateCb(this.SlBrandCb, this.SlTypeCb, this.SlProductCb, this.SlVolumeCb, this.SlRemainingLabel);
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? selectedDate = this.ScDatePicker.SelectedDate;
            if (selectedDate != null)
                this.ScUpdateItemsInColumns(selectedDate.Value.ToString("dd.MM.yy"));
        }

        private void EmAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (this.EmNameBox.Text != "" && this.EmSurnBox.Text != "" && this.EmPhoneBox.Text != "" && this.EmAddressBox.Text != "" && this.EmZipCodeBox.Text != "" && this.EmEmailBox.Text != "" && this.EmUserBox.Text != "" && this.EmPassBox1.Password != "")
            {
                EmployeeVariables.Name = this.EmNameBox.Text;
                EmployeeVariables.Surname = this.EmSurnBox.Text;
                EmployeeVariables.Number = this.EmPhoneBox.Text;
                EmployeeVariables.Address = this.EmAddressBox.Text;
                EmployeeVariables.Zip = this.EmZipCodeBox.Text;
                EmployeeVariables.Mail = this.EmEmailBox.Text;
                EmployeeVariables.Username = this.EmUserBox.Text;
                EmployeeVariables.Password = new LoginController().GetMd5Hash(this.EmPassBox1.Password);

                var SpecializationWindow = new ChooseSpecialization();
                SpecializationWindow.Show();
            }
            else
                MessageBox.Show("Check if every field has values in order to add the product.", "Fields not filled error");
        }

        private void EmUpdateColumnProperties()
        {
            UpdateColumnProperties(EmInitializeColumns(), this.EmployeeGrid.Columns);
        }

        private void EmUpdateItemsInColumns()
        {
            UpdateItemsInColumns(this.employeeController.EmployeeList, this.EmployeeGrid);
        }

        private void InitializeSpecializationVariables(object sender, DelegateEventArgs e)
        {
            EmployeeVariables.SpecializationStringList = e.SpecializationStringList;
            this.RegisterEmployee();
        }

        private void InitializeUpdateScheduleListAction(object sender, DelegateEventArgs e)
        {
            if (!e.OnlySpecialists)
                this.scheduleController.PopulateAppointmentList(this.employeeController.EmployeeList);
            this.scheduleController.PopulateWorkingDaysList(this.employeeController.EmployeeList);
            this.ScUpdateItemsInColumns(e.CurrentDate.ToString("dd.MM.yy"));
        }

        private void LoginEntry()
        {
            this.login = new LoginWindow(this, this.usernameLabel, this.SecondTab);
            this.login.Show();
        }

        private void LogoEntry()
        {
            var logo = new LogoPresentation();
            logo.Show();
            new DBController().RunProcedureInDatabase("", "EJL15_DB", new List<SqlParameter>()); // Testing connection to DB
            Thread.Sleep(3000);
            logo.Close();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            this.usernameLabel.Content = string.Empty;

            this.login = new LoginWindow(this, this.usernameLabel, this.SecondTab);
            this.Visibility = Visibility.Hidden;
            this.login.Show();
        }

        private void MainTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.MainTab.SelectedIndex == 1)
                this.SecondTab.Visibility = Visibility.Hidden;
            if (this.MainTab.SelectedIndex == 2)
                this.SecondTab.Visibility = Visibility.Visible;
        }

        private void MpAddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Brand selectedBrand = this.brandTypeController.GetBrandByName(this.MpBrandCb.SelectedValue.ToString());
                Type selectedType = this.brandTypeController.GetTypeByName(this.MpTypeCb.SelectedValue.ToString());
                string selectedName = this.MpNameTb.Text;
                int selectedVolume = Convert.ToInt32(this.MpVolumeTb.Text);

                int returnResult = this.productController.AddProduct(selectedBrand, selectedType, selectedName, selectedVolume);

                this.ReturnResult(returnResult, true);

                List<Product> productList = this.productController.Products;
                this.MpUpdateColumnProperties();
                UpdateLastElementInList(productList, this.ProductGrid);

                this.ProductGrid.SelectedItem = this.ProductGrid.Items[this.ProductGrid.Items.Count - 1];
                this.ProductGrid.ScrollIntoView(this.ProductGrid.Items[this.ProductGrid.Items.Count - 1]);
            }
            catch (SqlNotFilledException)
            {
                MessageBox.Show("Product has not been added to the Database. Probably there was a problem with the connection to DB.");
            }
            catch (NullReferenceException) {}
            catch
            {
                MessageBox.Show("Check if every field has values in order to add the product.");
            }
        }

        private void MpUpdateColumnProperties()
        {
            UpdateColumnProperties(MpInitializeColumns(), this.ProductGrid.Columns);
        }

        private void MpUpdateItemsInColumns()
        {
            UpdateItemsInColumns(this.productController.Products, this.ProductGrid);
        }

        private void ProductCb_Initialized()
        {
            this.ComboBoxInitialize(this.productController.Products, this.RsProductCb, false);
            this.ComboBoxInitialize(this.productController.Products, this.SlProductCb, false);
        }

        private void ProductCb_Selector(object sender, RoutedEventArgs routedEventArgs)
        {
            Predicate<Product> predicate = item => item.Name == ((ComboBox) sender).SelectedValue.ToString();
            this.ComboBoxSelector(predicate, sender as ComboBox);
        }

        private void RegisterEmployee()
        {
            try
            {
                if (this.employeeController.EmployeeList.Any(e => e.Name == EmployeeVariables.Name && e.SurName == EmployeeVariables.Surname))
                    this.ReturnResult(-2, true);
                int newEmployeeId = this.employeeController.AddEmployee(EmployeeVariables.Name, EmployeeVariables.Surname, EmployeeVariables.Number, EmployeeVariables.Address, EmployeeVariables.Zip, EmployeeVariables.Mail, EmployeeVariables.Username, EmployeeVariables.Password, EmployeeVariables.SpecializationStringList);

                this.ReturnResult(newEmployeeId, true);

                this.EmployeeGrid.Items.Clear();
                this.EmUpdateItemsInColumns();
                this.EmNameBox.Clear();
                this.EmSurnBox.Clear();
                this.EmPhoneBox.Clear();
                this.EmAddressBox.Clear();
                this.EmZipCodeBox.Clear();
                this.EmEmailBox.Clear();
                this.EmUserBox.Clear();
                this.EmPassBox1.Clear();
            }
            catch (NullReferenceException) {}
            catch
            {
                MessageBox.Show("Something went wrong :(", "Oops!");
            }
        }

        private void ReleaseSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.StockSelectedProducts.Clear();
                this.RsRemainingLabel.Content = "";
                this.SlRemainingLabel.Content = "";
                this.BrandCb_Initialized();
                this.TypeCb_Initialized();
                this.ProductCb_Initialized();
                this.VolumeCb_Initialized();
            }
            catch {}
        }

        private void RsUpdateColumnProperties()
        {
            UpdateColumnProperties(StockInitializeColumns(), this.StockGrid.Columns);
        }

        private void RsUpdateItemsInColumns()
        {
            var curStockList = new List<Stock>(this.stockController.Stock.Where(curItem => curItem.ActionType == 'r'));
            UpdateItemsInColumns(curStockList, this.StockGrid);
        }

        private void ScManageEmployees_Click(object sender, RoutedEventArgs e)
        {
            var ManageEmployeesWindow = new ManageSchedule(this.scheduleController, this.employeeController.EmployeeList, this.ScEmployeeList.Items, this.ScDatePicker.Text);
            ManageEmployeesWindow.Show();
        }

        private void ScRegisterAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (this.ScEmployeeList.SelectedItem == null)
            {
                MessageBox.Show("Please select an employee from the upper grid in order to set an appointment.");
                return;
            }
            var currentEmployee = this.ScEmployeeList.SelectedItem as Employee;
            var RegisterAppointmentWindow = new RegisterAppointment(DateTime.Parse(this.ScDatePicker.Text), currentEmployee, this.scheduleController);
            RegisterAppointmentWindow.Show();
        }

        private void ScUpdateColumnProperties()
        {
            UpdateColumnProperties(ScheduleListInitializeColumns(), this.ScJobList.Columns);
            UpdateColumnProperties(ScheduleJobsInitializeColumns(), this.ScEmployeeList.Columns);
        }

        private void ScUpdateItemsInColumns(string selectedDate)
        {
            IEnumerable<Appointment> appointmentList = this.scheduleController.Appointments.Where(e => e.Date.ToString("dd.MM.yy") == selectedDate);
            var employeeList = new List<Employee>();

            foreach (Employee curEmployee in 
                from days in this.scheduleController.WorkingDays where days.Date.ToString("dd.MM.yy").Equals(selectedDate) from curEmployee in days.Employees where !employeeList.Contains(curEmployee) select curEmployee)
                employeeList.Add(curEmployee);

            IList<Appointment> curList = appointmentList as IList<Appointment> ?? appointmentList.ToList();
            foreach (Appointment curAppointment in curList.Where(curAppointment => !employeeList.Contains(curAppointment.Employee)))
                employeeList.Add(curAppointment.Employee);

            UpdateItemsInColumns(curList, this.ScJobList);
            UpdateItemsInColumns(employeeList, this.ScEmployeeList);
        }

        private void SlsUpdateColumnProperties()
        {
            UpdateColumnProperties(StockInitializeColumns(), this.SalesGrid.Columns);
        }

        private void SlsUpdateItemsInColumns()
        {
            var curStockList = new List<Stock>(this.stockController.Stock.Where(curItem => curItem.ActionType == 's'));
            UpdateItemsInColumns(curStockList, this.SalesGrid);
        }

        private void TypeCb_Initialized()
        {
            this.ComboBoxInitialize(this.brandTypeController.Types, this.MpTypeCb, false);
            this.ComboBoxInitialize(this.brandTypeController.Types, this.RsTypeCb, false);
            this.ComboBoxInitialize(this.brandTypeController.Types, this.SlTypeCb, false);
        }

        private void TypeCb_Selector(object sender, RoutedEventArgs routedEventArgs)
        {
            Predicate<Product> predicate = item => item.Type.TypeName == ((ComboBox) sender).SelectedValue.ToString();
            this.ComboBoxSelector(predicate, sender as ComboBox);
        }

        private void UpdateCb(Selector brandCb, Selector typeCb, Selector productCb, Selector volumeCb, ContentControl remainingLabel)
        {
            brandCb.SelectionChanged -= this.BrandCb_Selector;
            typeCb.SelectionChanged -= this.TypeCb_Selector;
            productCb.SelectionChanged -= this.ProductCb_Selector;
            volumeCb.SelectionChanged -= this.VolumeCb_Selector;

            string selectedBrandItem = SelectItemsInCombobox(brandCb);
            string selectedTypeItem = SelectItemsInCombobox(typeCb);
            string selectedProductItem = SelectItemsInCombobox(productCb);
            string selectedVolumeItem = SelectItemsInCombobox(volumeCb);

            foreach (Product curProduct in this.StockSelectedProducts)
            {
                UpdateFilteredItems(brandCb, curProduct.Brand.BrandName);
                UpdateFilteredItems(typeCb, curProduct.Type.TypeName);
                UpdateFilteredItems(productCb, curProduct.Name);
                UpdateFilteredItems(volumeCb, curProduct.Volume.ToString(CultureInfo.InvariantCulture));
            }

            if (brandCb.Items.Count == 0 && typeCb.Items.Count == 0 && productCb.Items.Count == 0 && volumeCb.Items.Count == 0)
            {
                const string NotFound = "Nothing found";

                brandCb.Items.Add(NotFound);
                typeCb.Items.Add(NotFound);
                productCb.Items.Add(NotFound);
                volumeCb.Items.Add("N/A");
                remainingLabel.Content = NotFound;
            }

            if (UnselectItemsInCombobox(brandCb) != null)
                brandCb.SelectedItem = selectedBrandItem;

            if (UnselectItemsInCombobox(typeCb) != null)
                typeCb.SelectedItem = selectedTypeItem;

            if (UnselectItemsInCombobox(productCb) != null)
                productCb.SelectedItem = selectedProductItem;

            if (UnselectItemsInCombobox(volumeCb) != null)
                volumeCb.SelectedItem = selectedVolumeItem;

            if (this.StockSelectedProducts.Count.Equals(1))
                remainingLabel.Content = this.StockSelectedProducts[0].Quantity;

            brandCb.SelectionChanged += this.BrandCb_Selector;
            typeCb.SelectionChanged += this.TypeCb_Selector;
            productCb.SelectionChanged += this.ProductCb_Selector;
            volumeCb.SelectionChanged += this.VolumeCb_Selector;
        }

        private void VolumeCb_Initialized()
        {
            this.ComboBoxInitialize(this.productController.Products, this.RsVolumeCb, true);
            this.ComboBoxInitialize(this.productController.Products, this.SlVolumeCb, true);
        }

        private void VolumeCb_Selector(object sender, RoutedEventArgs routedEventArgs)
        {
            Predicate<Product> predicate = item => item.Volume.ToString(CultureInfo.InvariantCulture) == ((ComboBox) sender).SelectedValue.ToString();
            this.ComboBoxSelector(predicate, sender as ComboBox);
        }

        #endregion
    }
}