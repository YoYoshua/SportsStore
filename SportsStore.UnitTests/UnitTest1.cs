using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            //przygotowanie
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"}
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //dzialanie
            ProductListViewModel result =
                (ProductListViewModel)controller.List(null, 2).Model;

            //asercje
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            //przygotowanie - definiowanie metody pomocniczej HTML
            HtmlHelper myHelper = null;

            //przygotowanie - tworzenie danych PageInfo
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            //przygotowanie - konfigurowanie delegatu z uzyciem wyrazenia lambda
            Func<int, string> pageUrlDelegate = i => "Strona" + i;

            //dzialanie
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            // asercje
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Strona1"">1</a>"
            + @"<a class=""btn btn-default btn-primary selected"" href=""Strona2"">2</a>"
            + @"<a class=""btn btn-default"" href=""Strona3"">3</a>", result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            //przygotowanie
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"}
            });

            //przygotowanie
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //dzialanie
            ProductListViewModel result = (ProductListViewModel)controller.List(null, 2).Model;

            //asercje
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            //przygotowanie imitacji repozytorium
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
            });

            //przygotowanie kontrolera i 3-elementowej strony
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //dzialanie
            Product[] result = ((ProductListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();

            //asercje
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            //przygotowanie imitacji repozytorium
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Jabłka"},
                new Product {ProductID = 2, Name = "P2", Category = "Jabłka"},
                new Product {ProductID = 3, Name = "P3", Category = "Śliwki"},
                new Product {ProductID = 4, Name = "P4", Category = "Pomarańcze"},
            });

            //przygotowanie kontrolera
            NavController target = new NavController(mock.Object);

            //dzialanie: pobranie zbioru kategorii
            string[] results = ((IEnumerable<string>)target.Menu().Model).ToArray();

            //asercje
            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Jabłka");
            Assert.AreEqual(results[1], "Pomarańcze");
            Assert.AreEqual(results[2], "Śliwki");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            //przygotowanie imitacji repozytorium
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Jabłka"},
                new Product {ProductID = 4, Name = "P2", Category = "Pomarańcze"},
            });

            //przygotowanie kontrolera
            NavController target = new NavController(mock.Object);

            //przygotowanie kategorii do wybrania
            string categoryToSelect = "Jabłka";

            //działanie
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            //asercje
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            //przygotowanie imitacji repozytorium
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1" },
                new Product {ProductID = 2, Name = "P2", Category = "Cat2" },
                new Product {ProductID = 3, Name = "P3", Category = "Cat1" },
                new Product {ProductID = 4, Name = "P4", Category = "Cat1" },
                new Product {ProductID = 5, Name = "P5", Category = "Cat3" }
            });

            //przygotowanie kontrolera
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //działanie
            int res1 = ((ProductListViewModel)controller.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductListViewModel)controller.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductListViewModel)controller.List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((ProductListViewModel)controller.List(null).Model).PagingInfo.TotalItems;

            //asercje
            Assert.AreEqual(res1, 3);
            Assert.AreEqual(res2, 1);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }
    }
}
