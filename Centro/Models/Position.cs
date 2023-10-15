using Binance.Net.Enums;
using Binance.Net.Objects.Models.Futures;

namespace Centro.Models
{

    public class Position
    {
    public string Symbol = "";
    decimal UnPnl = 0.0M;
    PositionSide Side;
    decimal EntryPrice = 0.0M;
    decimal PositionAmount = 0.0M;
    decimal IsolatedMargin = 0.0M;
    decimal IsolatedWallet = 0.0M;
    decimal RealizedPnl = 0.0M;
    decimal MaintMargin = 0.0M;
    FuturesMarginType MarginType;
    decimal MarkPrice = 0.0M;
    decimal LiquidationPrice = 0.0M;
    int Leverage = 0;

    bool Isolated = false;

    public Position(string symbol, decimal unpnl, PositionSide side, decimal amount, FuturesMarginType type, decimal isolatedMargin, decimal entryPrice, decimal markPrice)
    {
        this.Symbol = symbol;
        this.UnPnl = unpnl;
        this.Side = side;
        this.PositionAmount = amount;
        this.IsolatedMargin = isolatedMargin;
        this.MarginType = type;
        this.EntryPrice = entryPrice;
        this.MarkPrice = markPrice;
    }

    public Position(BinancePositionInfoUsdt position)
    {
        this.Symbol = position.Symbol;
        this.UnPnl = position.UnrealizedPnl;
        this.Side = position.PositionSide;
        this.PositionAmount = position.Quantity;
        this.Isolated = position.Isolated;
        this.EntryPrice = position.EntryPrice;
    }

    public void Update(BinancePositionDetailsUsdt position)
    {
        if (position.Symbol == this.Symbol)
        {
            this.MarginType = position.MarginType;
            this.IsolatedMargin = position.IsolatedMargin;
            this.LiquidationPrice = position.LiquidationPrice;
            this.MarkPrice = position.MarkPrice;
            this.Leverage = position.Leverage;
        }
    }

    public ListViewItem ToListItem()
    {
        return new ListViewItem(new[] {
                this.Symbol,
                this.UnPnl.ToString(),
                this.Side.ToString(),
                this.PositionAmount.ToString(),
                this.IsolatedMargin.ToString(),
                this.MarginType.ToString(),
                this.EntryPrice.ToString(),
                this.MarkPrice.ToString(),
                this.MarginType.ToString(),
                this.IsolatedMargin.ToString(),
                this.LiquidationPrice.ToString(), 
                this.Leverage.ToString()
            });
    }

    }
}
