using Binance.Net.Objects.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Centro
{
    class BookTraderWidget
    {
        public class MDWidget
        {

            Dictionary<decimal, int> PricesIndex = new Dictionary<decimal, int>();
            const string NoValue = "0.00";

            decimal CurrentPrice = 0.0M; // Mid price

            decimal[,] matrix;

            decimal TickSize;

            Font defaultFont = new Font(FontFamily.GenericSerif, 6.0F,
                            System.Drawing.FontStyle.Bold, GraphicsUnit.Point);

            decimal UnitSize = 0.001M;  // price difference
            uint Location = 0; // scroll
            uint MaxButtonsNumber = 50;
            Graphics g;
            Pen p;
            SolidBrush labelPen;
            SolidBrush sb;
            SolidBrush sbAsk;
            SolidBrush sbBid;
            SolidBrush sbPrice;
            SolidBrush sbVolAsk;
            SolidBrush sbVolBid;
            SolidBrush sbLotAsk;
            SolidBrush sbLotBid;
            SolidBrush sbMid;

            Panel panel;

            int PADDING_X = 1;
            int PADDING_Y = 1;
            int BUTTON_WIDTH = 50;
            int BUTTON_HEIGHT = 15;

            int LABEL_PADDING_X = 10;

            static int _last_agg_line = 0;

            public MDWidget(Panel panel, decimal tickSize)
            {
                // get minTick
                this.TickSize = tickSize;
                this.panel = panel;
                g = panel.CreateGraphics();
                p = new Pen(Color.Black);
                labelPen = new SolidBrush(Color.Black);

                sb = new SolidBrush(Color.Red);

                sbPrice = new SolidBrush(Color.LightGray);
                sbAsk = new SolidBrush(Color.Red);
                sbBid = new SolidBrush(Color.Blue);

                sbVolAsk = new SolidBrush(Color.White);
                sbVolBid = new SolidBrush(Color.White);

                sbLotAsk = new SolidBrush(Color.Gray);
                sbLotBid = new SolidBrush(Color.Gray);

                sbMid = new SolidBrush(Color.Yellow);

                matrix = createEmptyStr();

                CalcMaxButtons();
            }

            private decimal[,] createEmptyStr()
            {
                decimal[,] values = new decimal[7, MaxButtonsNumber];
                for (int i = 0; i < MaxButtonsNumber; i++)
                {
                    values[0, i] = new decimal(0.0);
                    values[1, i] = new decimal(0.0);// Vol
                    values[2, i] = new decimal(0.0);// Ask Qty
                    values[3, i] = new decimal(0.0);// Price
                    values[4, i] = new decimal(0.0);// Bid Qty
                    values[5, i] = new decimal(0.0);// Vol
                    values[6, i] = new decimal(0.0);
                }
                return values;
            }

            private void CalcMaxButtons()
            {
                MaxButtonsNumber = (uint)(panel.Height / (2 * PADDING_Y + BUTTON_HEIGHT)) + 1;
            }

            private void generateArray(decimal price,
                IEnumerable<BinanceOrderBookEntry> asks, IEnumerable<BinanceOrderBookEntry> bids)
            {
                int mid = (int)(MaxButtonsNumber / 2);
                matrix[3, mid] = price;
                for (int i = mid; i > 0; i--)
                {
                    matrix[3, mid + i] = price + (TickSize * i);
                    matrix[3, mid - i] = price - (TickSize * i);

                    PricesIndex[price + (TickSize * i)] = mid + i;
                    PricesIndex[price - (TickSize * i)] = mid - i;

                    foreach (var ask in asks)
                    {
                        if (ask.Price == price + (TickSize * i))
                        {
                            matrix[1, mid + i] = ask.Quantity;
                        }
                    }
                    foreach (var bid in bids)
                    {
                        if (bid.Price == price - (TickSize * i))
                        {
                            matrix[5, mid - i] = bid.Quantity;
                        }
                    }
                }
            }

            private void drawArray()
            {
                for (int i = 0; i < MaxButtonsNumber; i++)
                {
                    if (matrix[0, i] > 0)
                    {
                        g.DrawString(matrix[0, i].ToString(), defaultFont, labelPen,
                        new System.Drawing.Point((PADDING_X * 1) + LABEL_PADDING_X,
                        (PADDING_Y * (i + 1)) + (i) * BUTTON_HEIGHT));
                    }

                    if (matrix[1, i] > 0)
                    {
                        g.DrawString(matrix[1, i].ToString(), defaultFont, labelPen,
                        new System.Drawing.Point((PADDING_X * 2) + BUTTON_WIDTH + LABEL_PADDING_X,
                        (PADDING_Y * (i + 1)) + (i) * BUTTON_HEIGHT));
                    }

                    if (matrix[2, i] != 0)
                    {
                        g.DrawString(matrix[2, i].ToString(), defaultFont, labelPen,
                        new System.Drawing.Point((PADDING_X * 3) + (BUTTON_WIDTH * 2) + LABEL_PADDING_X,
                        (PADDING_Y * (i + 1)) + (i) * BUTTON_HEIGHT));
                    }


                    g.DrawString(matrix[3, i].ToString(), defaultFont, labelPen,
                        new System.Drawing.Point((PADDING_X * 4) + (BUTTON_WIDTH * 3) + LABEL_PADDING_X,
                        (PADDING_Y * (i + 1)) + (i) * BUTTON_HEIGHT));

                    if (matrix[4, i] != 0)
                    {
                        g.DrawString(matrix[4, i].ToString(), defaultFont, labelPen,
                        new System.Drawing.Point((PADDING_X * 5) + (BUTTON_WIDTH * 4) + LABEL_PADDING_X,
                        (PADDING_Y * (i + 1)) + (i) * BUTTON_HEIGHT));
                    }

                    if (matrix[5, i] > 0)
                    {
                        g.DrawString(matrix[5, i].ToString(), defaultFont, labelPen,
                        new System.Drawing.Point((PADDING_X * 6) + (BUTTON_WIDTH * 5) + LABEL_PADDING_X,
                        (PADDING_Y * (i + 1)) + (i) * BUTTON_HEIGHT));
                    }
                }
            }

            public decimal GetPrice(int x, int y)
            {

                decimal minX1 = (PADDING_X * 3) + (BUTTON_WIDTH * 2);
                decimal maxX1 = minX1 + BUTTON_WIDTH;

                decimal minX2 = (PADDING_X * 5) + (BUTTON_WIDTH * 4);
                decimal maxX2 = minX2 + BUTTON_WIDTH;
                if (x >= minX1 && x <= maxX1)
                {
                    for (int i = 0; i < MaxButtonsNumber; i++)
                    {
                        decimal minY = (PADDING_Y * (i + 1)) + (i * BUTTON_HEIGHT);
                        decimal maxY = minY + BUTTON_HEIGHT;
                        if (y >= minY && y <= maxY)
                        {
                            return matrix[3, i];
                        }
                    }
                }
                else if (x >= minX2 && x <= maxX2)
                {
                    for (int i = 0; i < MaxButtonsNumber; i++)
                    {
                        decimal minY = (PADDING_Y * (i + 1)) + (i * BUTTON_HEIGHT);
                        decimal maxY = minY + BUTTON_HEIGHT;
                        if (y >= minY && y <= maxY)
                        {
                            return -matrix[3, i];
                        }
                    }
                }


                return 0.0M;
            }

            public void SetAggTrades(decimal price, decimal lots)
            {
                if(PricesIndex.ContainsKey(price))
                {
                    matrix[0, PricesIndex[price]] = lots;
                }
            }

            public void SetSellAggTrades(decimal price, decimal lots)
            {
   
                if (PricesIndex.ContainsKey(price))
                {
                    matrix[6, PricesIndex[price]] = lots;
                }

            }

            public void SetAskValue(decimal price, decimal lots)
            {

                if (PricesIndex.ContainsKey(price))
                {
                    matrix[4, PricesIndex[price]] = lots;
                }

            }

            public void SetBidValue(decimal price, decimal lots)
            {

                if (PricesIndex.ContainsKey(price))
                {
                    matrix[2, PricesIndex[price]] = lots;
                }

            }

            private System.Drawing.Point getButtonPoint(int column, int row)
            {
                int mid = (int)MaxButtonsNumber / 2;

                return new System.Drawing.Point(0, 0);
            }

            private System.Drawing.Point getLabelPoint(int column, int row)
            {
                int mid = (int)MaxButtonsNumber / 2;
                decimal x = (PADDING_X * (column + 2)) + BUTTON_WIDTH + LABEL_PADDING_X;
                //decimal y = (PADDING_Y * (mid + i + 1)) + ((mid + i) * BUTTON_HEIGHT));
                return new System.Drawing.Point(0, 0);
            }

            public void DrawValues(IEnumerable<BinanceOrderBookEntry> asks, IEnumerable<BinanceOrderBookEntry> bids)
            {
   
                if(CurrentPrice == 0.0M)
                {
                    foreach (var bid in bids)
                    {
                        CurrentPrice = bid.Price;
                        //break;
                    }

                }
                
                generateArray(CurrentPrice, asks, bids);
                drawArray();
            }

            public void DrawValues1(decimal[,] asks, decimal[,] bids, int size)
            {
                int mid = (int)MaxButtonsNumber / 2;
                for (int i = 0; i < mid; i++)
                {
                    if (size - 1 - i >= 0)
                    {
                        g.DrawString(bids[1, size - 1 - i].ToString(), defaultFont, labelPen,
                        new System.Drawing.Point((PADDING_X * 2) + BUTTON_WIDTH + LABEL_PADDING_X,
                        (PADDING_Y * (mid + i + 1)) + ((mid + i) * BUTTON_HEIGHT)));

                        //Price
                        g.DrawString(asks[0, i].ToString(), defaultFont, labelPen,
                            new System.Drawing.Point((PADDING_X * 4) + (BUTTON_WIDTH * 3) + LABEL_PADDING_X,
                            (PADDING_Y * (mid - i + 1)) + ((mid - i - 1) * BUTTON_HEIGHT)));

                        g.DrawString(bids[0, size - 1 - i].ToString(), defaultFont, labelPen,
                            new System.Drawing.Point((PADDING_X * 4) + (BUTTON_WIDTH * 3) + LABEL_PADDING_X,
                            (PADDING_Y * (mid + i + 1)) + ((mid + i) * BUTTON_HEIGHT)));
                        //

                        g.DrawString(asks[1, i].ToString(), defaultFont, labelPen,
                            new System.Drawing.Point((PADDING_X * 6) + (BUTTON_WIDTH * 5) + LABEL_PADDING_X,
                            (PADDING_Y * (mid - i + 1)) + ((mid - i - 1) * BUTTON_HEIGHT)));
                    }
                }
            }


            public void BackGround()//int first, int last)
            {
                for (int i = 0; i < MaxButtonsNumber; i++)
                {
                    g.FillRectangle(sbLotBid,
                        PADDING_X,
                        (PADDING_Y * (i + 1)) + (i * BUTTON_HEIGHT),
                        BUTTON_WIDTH,
                        BUTTON_HEIGHT);

                    g.FillRectangle(sbVolBid,
                        (PADDING_X * 2) + BUTTON_WIDTH,
                        (PADDING_Y * (i + 1)) + (i * BUTTON_HEIGHT),
                        BUTTON_WIDTH,
                        BUTTON_HEIGHT);

                    g.FillRectangle(sbBid,
                        (PADDING_X * 3) + (BUTTON_WIDTH * 2),
                        (PADDING_Y * (i + 1)) + (i * BUTTON_HEIGHT),
                        BUTTON_WIDTH,
                        BUTTON_HEIGHT);

                    g.FillRectangle(sbPrice,
                        (PADDING_X * 4) + (BUTTON_WIDTH * 3),
                        (PADDING_Y * (i + 1)) + (i * BUTTON_HEIGHT),
                        BUTTON_WIDTH,
                        BUTTON_HEIGHT);

                    g.FillRectangle(sbAsk,
                        (PADDING_X * 5) + (BUTTON_WIDTH * 4),
                        (PADDING_Y * (i + 1)) + (i * BUTTON_HEIGHT),
                        BUTTON_WIDTH,
                        BUTTON_HEIGHT);

                    g.FillRectangle(sbVolAsk,
                        (PADDING_X * 6) + (BUTTON_WIDTH * 5),
                        (PADDING_Y * (i + 1)) + (i * BUTTON_HEIGHT),
                        BUTTON_WIDTH,
                        BUTTON_HEIGHT);

                    g.FillRectangle(sbLotAsk,
                        (PADDING_X * 7) + (BUTTON_WIDTH * 6),
                        (PADDING_Y * (i + 1)) + (i * BUTTON_HEIGHT),
                        BUTTON_WIDTH,
                        BUTTON_HEIGHT);

                }

                int j = (int)MaxButtonsNumber / 2;
                int midY = (PADDING_Y * (j + 1)) + (j * BUTTON_HEIGHT);
                g.FillRectangle(sbMid, PADDING_X, midY, 500, 1);
            }

            public void Resize(int width, int height)
            {
                g = panel.CreateGraphics();
            }
        }
    }
}
