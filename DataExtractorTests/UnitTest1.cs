using NUnit.Framework;
using DataExtractorClass;
using System.IO;

namespace DataExtractorTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void TestParseNoPriceMultiplier()
        {
            InputRecord i = new InputRecord();
            i.AlgoParams = "NoPriceMultiplier:24.0|;Bob:4";
            i.ISIN = "TEST";
            i.CFICode = "TEST";
            i.Venue = "TEST";
            try
            {
                OutputRecord o = new OutputRecord(i);
            }
            catch (System.MissingFieldException e)
            {
                if (e.Message == "Cannot find PriceMultiplier")
                    Assert.Pass();
            }
            Assert.Fail();
        }
        [Test]
        public void TestParseValiePriceMultiplier()
        {
            InputRecord i = new InputRecord();
            i.AlgoParams = "Dave:14.0|;PriceMultiplier:24.0|;Bob:4";
            i.ISIN = "TEST";
            i.CFICode = "TEST";
            i.Venue = "TEST";
            OutputRecord o = new OutputRecord(i);
            if (o.ContractSize == 24.0)
                Assert.Pass();
            else
                Assert.Fail();
        }
        [Test]

        public void TestValidFile()
        {
            Extractor e = new DataExtractorClass.Extractor();
            System.Collections.Generic.List<InputRecord> r = e.OpenCSV("C:\\TestData\\DataExtractor_Example_Input.csv");
            Assert.IsTrue(r.Count == 2);
            Assert.IsTrue(r[0].ISIN == "DE000ABCDEFG");
            System.Collections.Generic.List<OutputRecord> o = e.Parse(r);
            Assert.IsTrue(o.Count == 2);
            Assert.IsTrue(o[0].ContractSize == 20.0);
            Assert.IsTrue(o[1].ContractSize == 25.0);
            Assert.IsTrue(o[0].ISIN == "DE000ABCDEFG");
            Assert.IsTrue(o[1].ISIN == "PL0ABCDEFGHI");
            Assert.IsTrue(o[0].Venue == "XEUR");
            Assert.IsTrue(o[1].Venue == "WDER");
            Assert.IsTrue(o[0].CFICode == "FFICSX");
            Assert.IsTrue(o[1].CFICode == "FFICSX");
            if (System.IO.File.Exists("C:\\TestData\\Output.csv"))
                System.IO.File.Delete("C:\\TestData\\Output.csv");
            e.Write("C:\\TestData\\Output.csv", o);
            Assert.IsTrue(System.IO.File.Exists("C:\\TestData\\Output.csv"));
            Assert.Pass();

        }

        [Test]
        public void TestInvalidInputFile()
        {
            Extractor e = new DataExtractorClass.Extractor();
            try
            {
                System.Collections.Generic.List<InputRecord> r = e.OpenCSV("");
            }
            catch (System.IO.FileNotFoundException ex)
            {
                if (ex.Message == "Missing Filename")
                    Assert.Pass();
            }
            Assert.Fail();



        }

        [Test]
        public void TestMissingFile()
        {
            Extractor e = new DataExtractorClass.Extractor();
            try
            {
                System.Collections.Generic.List<InputRecord> r = e.OpenCSV("C:\\TestData\\NoFile.csv");
            }
            catch (System.IO.FileNotFoundException ex)
            {
                if (ex.Message == "Cannot find file")
                    Assert.Pass();
            }
            Assert.Fail();



        }

        [Test]
        public void TestValidFileInvalidOutput()
        {
            Extractor e = new DataExtractorClass.Extractor();
            System.Collections.Generic.List<InputRecord> r = e.OpenCSV("C:\\TestData\\DataExtractor_Example_Input.csv");
            System.Collections.Generic.List<OutputRecord> o = e.Parse(r);
            if (System.IO.File.Exists("C:\\TestData\\Output.csv"))
                System.IO.File.Delete("C:\\TestData\\Output.csv");
            try
            {
                e.Write("C:\\TestData\\DataExtractor_Example_Input.csv", o);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                if (ex.Message == "File already exists")
                    Assert.Pass();
            }
            Assert.Fail();
        }

    }
}