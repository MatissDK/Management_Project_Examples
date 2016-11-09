namespace UnitTest
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    using ControllerLayer;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using ModuleLayer;

    [TestClass]
    public class UnitTest1
    {
        #region Public Methods and Operators

        [TestMethod]
        public void BrandPopulateTest()
        {
            const string curString = "Dmitree";
            var btController = new BrandTypeController();
            var returnValue = btController.AddToBrand(curString);

            var dbController = new DBController();
            var curQuery = "UnitAddBrand";
            var curBrand = dbController.GetFromDatabase(curQuery, "Brand", "EJL15_DB", new List<SqlParameter>()).Cast<Brand>().ToList();

            Assert.AreEqual(curString, curBrand[0].BrandName);
            Assert.AreEqual(returnValue, curBrand[0].Id);

            btController.DeleteBrand(returnValue);

            curQuery = "UnitAddBrand";
            curBrand = dbController.GetFromDatabase(curQuery, "Brand", "EJL15_DB", new List<SqlParameter>()).Cast<Brand>().ToList();

            Assert.AreNotEqual(returnValue, curBrand[0].Id);
        }

        [TestMethod]
        public void LoginControllerMD5Test()
        {
            const string userPassword = "myCustomPASS123";
            const string MD5Hash = "352674370d4e4b11948aa786ad3c592b"; //This is generated MD5 hash for "userPassword" from online tool http://md5encryption.com/

            var loginController = new LoginController();
            var generatedMD5 = loginController.GetMd5Hash(userPassword);

            Assert.AreEqual(MD5Hash, generatedMD5);
        }

        [TestMethod]
        public void ProductControllerObjectExistanceTest()
        {
            var productList = new List<Product>();

            var brand1 = new Brand(0, "TestBrand1");
            var brand2 = new Brand(1, "TestBrand2");
            var brand3 = new Brand(2, "TestBrand3");

            var type1 = new Type(0, "TestType1");
            var type2 = new Type(1, "TestType2");
            var type3 = new Type(2, "TestType3");

            const string productName1 = "TestProduct1";
            const string productName2 = "TestProduct2";
            const string productName3 = "TestProduct3";

            const int productVolume1 = 1;
            const int productVolume2 = 10;
            const int productVolume3 = 100;

            productList.Add(new Product(0, new Brand(0, "TestBrand1"), new Type(0, "TestType1"), productName1, productVolume1, 1));
            productList.Add(new Product(1, new Brand(1, "TestBrand2"), new Type(1, "TestType2"), productName2, productVolume2, 10));
            productList.Add(new Product(2, new Brand(2, "TestBrand3"), new Type(2, "TestType3"), productName3, productVolume3, 100));

            productList.Add(new Product(3, new Brand(3, "TestBrand1"), new Type(3, "TestType2"), productName2, productVolume1, 1));
            productList.Add(new Product(4, new Brand(4, "TestBrand2"), new Type(4, "TestType3"), productName1, productVolume2, 10));
            productList.Add(new Product(5, new Brand(5, "TestBrand3"), new Type(5, "TestType1"), productName3, productVolume3, 100));

            productList.Add(new Product(6, new Brand(6, "TestBrand3"), new Type(6, "TestType1"), productName1, productVolume1, 1));
            productList.Add(new Product(7, new Brand(7, "TestBrand2"), new Type(7, "TestType3"), productName2, productVolume2, 10));
            productList.Add(new Product(8, new Brand(8, "TestBrand1"), new Type(8, "TestType2"), productName3, productVolume3, 100));

            productList.Add(new Product(9, new Brand(9, "TestBrand3"), new Type(9, "TestType2"), productName3, productVolume1, 1));
            productList.Add(new Product(10, new Brand(10, "TestBrand1"), new Type(10, "TestType1"), productName1, productVolume2, 10));
            productList.Add(new Product(11, new Brand(11, "TestBrand2"), new Type(11, "TestType3"), productName2, productVolume3, 100));

            productList.Add(new Product(12, new Brand(12, "TestBrand2"), new Type(12, "TestType3"), productName2, productVolume1, 1));
            productList.Add(new Product(13, new Brand(13, "TestBrand1"), new Type(13, "TestType2"), productName3, productVolume2, 10));
            productList.Add(new Product(14, new Brand(14, "TestBrand3"), new Type(14, "TestType1"), productName1, productVolume3, 100));

            var productController = new ProductController();
            productController.Products = productList;
            
            var returnResult = productController.AddProduct(brand2, type2, productName2, productVolume2);
            Assert.AreEqual(-2, returnResult);

            List<int> returnResultInts = new List<int>();

            returnResultInts.Add(productController.AddProduct(brand2, type1, productName2, productVolume1));
            returnResultInts.Add(productController.AddProduct(brand1, type3, productName1, productVolume2));
            returnResultInts.Add(productController.AddProduct(brand3, type1, productName2, productVolume3));
            returnResultInts.Add(productController.AddProduct(brand1, type3, productName3, productVolume2));
            returnResultInts.Add(productController.AddProduct(brand2, type1, productName3, productVolume1));
            returnResultInts.Add(productController.AddProduct(brand1, type3, productName2, productVolume1));
            returnResultInts.Add(productController.AddProduct(brand3, type1, productName1, productVolume2));
            returnResultInts.Add(productController.AddProduct(brand1, type3, productName1, productVolume2));
            returnResultInts.Add(productController.AddProduct(brand2, type2, productName3, productVolume1));
            returnResultInts.Add(productController.AddProduct(brand3, type2, productName2, productVolume3));

            foreach (var currentResult in returnResultInts)
            {
                Assert.AreNotEqual(-2, currentResult);
            }
        }

        #endregion
    }
}