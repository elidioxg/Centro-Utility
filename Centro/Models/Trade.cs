using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Centro.Models
{
    public class Trade
    {
        public long Id = -1;
        public long OrderId = -1;
        public string Symbol = "";
        public decimal Quantity = 0.0M;
        public decimal Price = 0.0M;
        public decimal Commission = 0.0M;
        public DateTime TradeTime;
        public string TradeTimeString = "";

        public Trade()
        {

        }

        public Trade(string symbol, decimal quantity, DateTime tradeTime, decimal price = 0.0M, decimal commission = 0.0M,
            long id = -1, long orderId = -1)
        {
            this.Symbol = symbol;
            this.Quantity = quantity;
            this.Price = price;
            this.Commission = commission;
            this.TradeTime = tradeTime;
            this.Id = id;
            this.OrderId = orderId;
            this.TradeTimeString = tradeTime.ToString();
        }

        public ListViewItem ToListItem()
        {
            ListViewItem listItem = new ListViewItem(new[] {
                            Symbol,
                            Id.ToString(),
                            OrderId.ToString(),
                            Quantity.ToString(),
                            Price.ToString(),
                            Commission.ToString(),
                            TradeTime.ToString(),
                    });
            return listItem;
        }
    }
}
