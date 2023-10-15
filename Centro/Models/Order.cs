using Binance.Net.Enums;
using Binance.Net.Objects.Models.Futures;
using Binance.Net.Objects.Models.Futures.Socket;
using Centro.Models;
using System;

namespace Centro.Models
{
    public static class Order
    {
 
        public class OrderEvent
        {

            public string Event = "";
            public DateTime EventTime;
            public string Symbol = "";
            public PositionSide Side;
            public decimal Quantity = 0.0M;
            public decimal Price = 0.0M;
            public FuturesOrderType OrderType;
            public ExecutionType ExecutionType;
            public decimal AveragePrice = 0.0M;
            public decimal ActivationPrice = 0.0M;
            public decimal StopPrice = 0.0M;
            public decimal QuantityFilledTrades = 0.0M;
            public decimal Commission = 0.0M;
            public string CommissionAsset = "";
            public string ClientOrderId = "";        
            public long TradeId = 0;
            public bool IsReduce = false;

            public OrderEvent(BinanceFuturesStreamOrderUpdate obj)
            {
                this.Event = obj.Event;
                this.EventTime = obj.EventTime;
                this.QuantityFilledTrades = obj.UpdateData.AccumulatedQuantityOfFilledTrades;
                this.ActivationPrice = obj.UpdateData.ActivationPrice;
                this.AveragePrice = obj.UpdateData.AveragePrice;
                this.ClientOrderId = obj.UpdateData.ClientOrderId;
                this.Commission = obj.UpdateData.Fee;
                this.CommissionAsset = obj.UpdateData.FeeAsset;
                this.ExecutionType = obj.UpdateData.ExecutionType;
                this.OrderType = obj.UpdateData.Type;
                this.TradeId = obj.UpdateData.TradeId;
                this.Symbol = obj.UpdateData.Symbol;
                //this.Status = obj.UpdateData.Status;
                this.Side = obj.UpdateData.PositionSide;
                this.Price = obj.UpdateData.Price;
                this.Quantity = obj.UpdateData.Quantity;
                this.StopPrice = obj.UpdateData.StopPrice;
                this.IsReduce = obj.UpdateData.IsReduce;
            }

            public ListViewItem ToListItem()
            {
                return new ListViewItem(new[] {
                        this.Event,
                        this.EventTime.ToString(),
                        this.Symbol,
                        this.Side.ToString(),
                        this.Quantity.ToString(),
                        this.Price.ToString(),
                        this.OrderType.ToString(),
                        this.ExecutionType.ToString(),
                        this.AveragePrice.ToString(),
                        this.ActivationPrice.ToString(),
                        this.StopPrice.ToString(),
                        this.QuantityFilledTrades.ToString(),
                        this.Commission.ToString(),
                        this.CommissionAsset,
                        this.ClientOrderId.ToString(),
                        this.TradeId.ToString(),
                        this.IsReduce.ToString(),
                });
            }
        }
    }
}
