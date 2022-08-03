using CsvHelper;
using System.IO;



/// <summary>
/// Data Extractor class - contacts functions to extract, parse and write CSV files
/// </summary>
namespace DataExtractorClass
{

    /// <summary>
    /// InputRecord format (used by CSV Helper Class)
    /// </summary>
    public class InputRecord
    {
        public string IsMultiFill { get; set; }
        public string ISIN { get; set; }
        public string Currency { get; set; }
        public string Venue { get; set; }
        public string OrderRef { get; set; }
        public string PMID { get; set; }
        public string CFICode { get; set; }
        public string ParticipantCode { get; set; }
        public string TraderID { get; set; }
        public string CounterPartyCode { get; set; }
        public string DecisionTime { get; set; }
        public string ArrivalTime_QuoteTime { get; set; }
        public DateTime FirstFillTime_TradeTime { get; set; }
        public string LastFillTime { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Side { get; set; }
        public string TradeFlag { get; set; }
        public string SettlementDate { get; set; }
        public string PublicTradeID { get; set; }
        public string UserDefinedFilter { get; set; }
        public string TradingNetworkID { get; set; }
        public string SettlementPeriod { get; set; }
        public string MarketOrderId { get; set; }
        public string ParticipationRate { get; set; }
        public string BenchmarkVenues { get; set; }
        public string BenchmarkType { get; set; }
        public string FlowType { get; set; }
        public string BasketID { get; set; }
        public string MessageType { get; set; }
        public string ParentOrderRef { get; set; }
        public string ExecutionType { get; set; }
        public string LimitPrice { get; set; }
        public string Urgency { get; set; }
        public string AlgoName { get; set; }
        public string AlgoParams { get; set; }
        public string Index { get; set; }
        public string Sector { get; set; }

    }

    /// <summary>
    /// OutputRecord format (used by CSV Helper Class)
    /// </summary>
    public class OutputRecord
    {
        public string ISIN { get; set; }
        public string CFICode { get; set; }
        public string Venue { get; set; }
        public double? ContractSize { get; set; }

        /// <summary>
        /// Output record constructor for use with CSVHelper
        /// </summary>
        /// <param name="i">the Input record to create an output record</param>
        /// <exception cref="System.MissingFieldException">AlgoParams is malformed/missing the PriceMultiplier</exception>
        public OutputRecord(InputRecord i)
        {
            ISIN = i.ISIN;
            CFICode = i.CFICode;
            Venue = i.Venue;
            List<string> values = new List<string>(i.AlgoParams.Split("|;").ToList()); // Turn AlgoParams into a list of strings for each element
            ContractSize = null;
            foreach (string v in values) // Could convert to a Dictionary here but seems a waste given we want one parameter
                if (v.StartsWith("PriceMultiplier"))
                    ContractSize = Convert.ToDouble(v.Split(":")[1]);
            if (ContractSize == null)
                throw new System.MissingFieldException("Cannot find PriceMultiplier");
        }


    }

    /// <summary>
    /// Extractor class
    /// </summary>
    public class Extractor
    {
        private List<InputRecord>? records;



        /// <summary>
        /// Opens a CSV File and parses it into an array of InputRecords
        /// </summary>
        /// <param name="f"></param>
        /// <exception cref="System.IO.FileNotFoundException">Cannot find the input file</exception>
        public List<InputRecord> OpenCSV(string filename)
        {
            TextReader inputReader;
            CsvReader input;

            
            if (filename == "")
                throw new System.IO.FileNotFoundException("Missing Filename");
            if (File.Exists(filename))
            {
                try
                {
                    inputReader = new StreamReader(filename);
                    input = new CsvReader(inputReader, System.Globalization.CultureInfo.InvariantCulture);
                    input.Read();
                    records = new List<InputRecord>(input.GetRecords<InputRecord>());
                    inputReader.Close();
                    return records;
                }
                catch
                {
                    throw;
                }
            }
            else
                throw new System.IO.FileNotFoundException("Cannot find file");
        }

        public List<OutputRecord> Parse(List<InputRecord> outputRecords)
        {
            return new List<OutputRecord>(outputRecords.Select(x => new OutputRecord(x)).ToList());
        }

        /// <summary>
        /// Writes a List of OutputRecords to a CSV File
        /// </summary>
        /// <param name="outputFilename">Destination Filename</param>
        /// <param name="outputRecords">List of Output Records</param>
        /// <returns></returns>
        public bool Write(string outputFilename, List<OutputRecord> outputRecords)
        {
            if (records == null) // Check we have data to write
                return false;
            if (System.IO.File.Exists(outputFilename)) // Check the output file doesn't exist
                throw new System.IO.FileNotFoundException("File already exists");
            TextWriter outputWriter = new StreamWriter(outputFilename);
            CsvWriter output = new CsvWriter(outputWriter, System.Globalization.CultureInfo.InvariantCulture);
            output.WriteRecords(outputRecords);
            outputWriter.Flush();
            outputWriter.Close();
            return true;
        }

    }
}