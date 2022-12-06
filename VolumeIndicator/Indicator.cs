using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using HtmlAgilityPack;
using VolumeIndicator;

namespace VolumeIndicator
{
    internal class Indicator
    {
        const string FILE_NAME = "Ticket.csv";
        const string BASE_URL = "https://www.moneycontrol.com/india/stockpricequote/pharmaceuticals/alembicpharmaceuticals/";

        List<TicketInfo> Tickets = new List<TicketInfo>();

        static async Task<int> GetVolumeFromUrlAsync(string url)
        {

            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(url);
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);
                var node = htmlDoc.DocumentNode.SelectSingleNode("//div[@class=\"mob-hide\"]/div[1]/table/tbody/tr[3]/td[2]");
                var volume = (int)decimal.Parse(node.InnerText.Replace(",", ""));
                return volume;

            }
            catch (NullReferenceException)
            {
                return -1;
            }
            
        }

        void SaveWatchList()
        {
            using (StreamWriter writetext = new StreamWriter(FILE_NAME))
            {
                foreach (var item in Tickets)
                {
                    writetext.WriteLine($"{item.TicketId},{item.Limit}");
                }
            }
        }

        void ShowMenu()
        {
            Console.WriteLine("");
            Console.WriteLine("1: View Ticket");
            Console.WriteLine("2: Add Ticket to watchlist");
            Console.WriteLine("3: Remove Ticket to watchlist");
            Console.WriteLine("4: Run Search");

        }

        int GetInputInRange(int min, int max)
        {
            int value;
            var input = Console.ReadLine();
            var isInt = int.TryParse(input, out value);
            while (isInt && (value < min || value > max))
            {
                Console.WriteLine("Invalid Input. Please try again");
                input = Console.ReadLine();
                isInt = int.TryParse(input, out value);
            }
            return value;
        }

        async Task AddTicketAsync()
        {
            Console.WriteLine("Please enter the ticket Id");
            var ticketId = Console.ReadLine().ToUpper();

            if (await GetVolumeFromUrlAsync(BASE_URL + ticketId) == -1)
            {
                Console.WriteLine("Incorrect ID");
                return;
            }

            Console.WriteLine("Please enter the ticket limit");
            var limit = GetInputInRange(0, int.MaxValue);

            Tickets.Add(new TicketInfo(ticketId, limit));

        }


        void RemoveTicket()
        {
            Console.WriteLine("Please enter the ticket Id");
            var ticketId = Console.ReadLine();
            Tickets.RemoveAll(ticket => ticket.TicketId == ticketId.ToUpper());

        }


        void ReadFile()
        { 
            if (!File.Exists(FILE_NAME))
            {
                return;
            }
            string[] lines = File.ReadAllLines(FILE_NAME);

            foreach (string line in lines)
            {
                var values = line.Split(',');
                Tickets.Add(new TicketInfo(values[0], int.Parse(values[1])));
            }
        }

        void ShowTickets()
        {
            foreach (var item in Tickets)
            {
                Console.WriteLine($"Ticket ID: {item.TicketId}\t\t\tLimit: {item.Limit}");
            }
        }

        async Task CheckLimitReachedAsync()
        {
            bool limitReached = false;
            foreach (var item in Tickets)
            {
                var currentVolume = await GetVolumeFromUrlAsync(BASE_URL + item.TicketId);
                if (currentVolume >= item.Limit)
                {
                    limitReached = true;
                    Console.WriteLine($"Ticket Id: {item.TicketId}\t\t Set Limit: {item.Limit}\t\t Current Value: {currentVolume}\t\t Difference: {currentVolume - item.Limit}");
                }
            }
            if (!limitReached)
            {
                Console.WriteLine("No ticket has reached the limit.");
            }
                
        }

        public async Task ExecuteAsync()
        {
            ReadFile();
            await CheckLimitReachedAsync();
            while (true)
            {

                ShowMenu();
                var selectOption = GetInputInRange(1, 4);
                switch (selectOption)
                {
                    case 1:
                        ShowTickets();
                        break;
                    case 2:
                        await AddTicketAsync();
                        SaveWatchList();
                        break;
                    case 4:
                        await CheckLimitReachedAsync();
                        break;
                    default:
                        break;
                }
            }

        }


    }
}