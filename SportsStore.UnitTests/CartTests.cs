using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            //przygotowanie produktow testowych
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            //przygotowanie koszyka
            Cart target = new Cart();

            //dzialanie
            target.AddItem(p1, 1);
            target.AddItem(p2, 2);
            CartLine[] results = target.Lines.ToArray();

            //asercje
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            //przygotowanie produktow testowych
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            //przygotowanie koszyka
            Cart target = new Cart();

            //dzialanie
            target.AddItem(p1, 1);
            target.AddItem(p1, 1);
            target.AddItem(p2, 5);
            target.AddItem(p2, 7);
            CartLine[] result = target.Lines.ToArray();

            //asercje
            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(result[0].Quantity, 2);
            Assert.AreEqual(result[1].Quantity, 12);
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            //przygotowanie produktow testowych
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            //przygotowanie koszyka
            Cart target = new Cart();

            //dzialanie
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 4);
            target.RemoveLine(p2);
            CartLine[] result = target.Lines.ToArray();

            //asercje
            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(result[0].Product, p1);
            Assert.AreEqual(result[0].Quantity, 1);
            Assert.AreEqual(result[1].Product, p3);
            Assert.AreEqual(result[1].Quantity, 4);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            //przygotowanie produktow testowych
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            //przygotowanie koszyka
            Cart target = new Cart();

            //dzialanie
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);
            decimal result = target.ComputeTotalValue();

            //asercje
            Assert.AreEqual(result, 450M);
        }

        [TestMethod]
        public void Can_Clear_Contents()
        {
            //przygotowanie produktow testowych
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            //przygotowanie koszyka
            Cart target = new Cart();

            //przygotowanie produktow w koszyku
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            //dzialanie: czyszczenie koszyka
            target.Clear();

            //asercje
            Assert.AreEqual(target.Lines.Count(), 0);
        }
    }
}
