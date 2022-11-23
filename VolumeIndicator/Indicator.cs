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


        List<TicketInfo> Tickets = new List<TicketInfo>();

        ~Indicator()
        {
            using (StreamWriter writetext = new StreamWriter("Ticket.csv"))
            {
                foreach (var item in Tickets)
                {
                    writetext.WriteLine(@"{item}");

                }
            }
        }

        static async Task<int> GetVolumeFromUrlAsync(string url)
        {


            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(url);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);
            var node = htmlDoc.DocumentNode.SelectSingleNode("//div[@class=\"mob-hide\"]/div[1]/table/tbody/tr[3]/td[2]");
            var volume = (int)decimal.Parse(node.InnerText.Replace(",", ""));
            return volume;
        }

        void ShowMenu()
        {
            Console.WriteLine("1: View Ticket");
            Console.WriteLine("2: Add Ticket to watchlist");
            Console.WriteLine("3: Remove Ticket to watchlist");

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

        void AddTickets()
        {
            Console.WriteLine("Please enter the ticket Id");
            var ticketId = Console.ReadLine();

            Console.WriteLine("Please enter the ticket limit");
            var limit = GetInputInRange(0, int.MaxValue);

            Tickets.Add(new TicketInfo(ticketId, limit));


        }

        public void Execute()
        {
            ShowMenu();
            var selectOption = GetInputInRange(1, 3);
            switch (selectOption)
            {
                case 1:
                    break;
                case 2:
                    AddTickets();
                    break;
                case 3:
                    break;
                default:
                    break;
            }
        }


    }
}