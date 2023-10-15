using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Models.Futures;
using Binance.Net.Objects.Models.Futures.Socket;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Binance.Net.Objects.Models.Spot;
using Binance.Net.Objects.Models;
using Binance.Net.Clients;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using System.Windows.Forms;

namespace Centro
{
    class BinanceFutures
    {

        static BinanceClient client;
        static BinanceClient publicClient;
        static BinanceSocketClient socketClient;
        static BinanceSocketClient publicSocketClient;

        static string listenkeyTether;

        static bool logged = false;

        public static void CreateClients()
        {
            publicClient = new BinanceClient();
            publicSocketClient = new BinanceSocketClient();
        }

        public static BinanceClient GetPublicClient()
        {
            return publicClient;
        }

        public static BinanceClient GetClient()
        {
            return client;
        }

        public static BinanceSocketClient GetPublicSocketClient()
        {
            return publicSocketClient;
        }

        public static BinanceSocketClient GetSocketClient()
        {
            return socketClient;
        }

        public static string GetTetherListenKey()
        {
            return listenkeyTether;
        }

        public static bool IsLogged() 
        {
            return logged;
        }

        public static async Task<bool> Login(string key, string secret)
        {
            try
            {
                client = new BinanceClient(new BinanceClientOptions
                {
                    ApiCredentials = new ApiCredentials(key, secret)
                });

                socketClient = new BinanceSocketClient(new BinanceSocketClientOptions
                {
                    ApiCredentials = new ApiCredentials(key, secret)
                });

                //var result = await client.UsdFuturesApi.Account.StartUserStreamAsync();
                //listenkeyTether = result.Data;

                logged = true;

            }
            catch (Exception ex)
            {
                
                Logger.Debug(ex.Message, ex.HResult);
                logged = false;
                return (false);
            }

            bool conected = await BinanceFutures.CheckApiConnection();
            if (!conected)
            {
                MessageBox.Show(Strings.FailedCreateClient, Strings.ApiError,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                logged = false;
                return (false);
            }

            logged = true;
            return (logged);
        }

        public static async void Logout()
        {
            
            //await publicSocketClient.UnsubscribeAllAsync();
            await socketClient.UnsubscribeAllAsync();
            socketClient = null;
            client = null;
        }


        public static async Task<BinanceFuturesPlacedOrder> BuyLimitTether(string symbol, decimal quantity, decimal price)
        {

            try
            {
                var response = await client.UsdFuturesApi.Trading.PlaceOrderAsync(
                    symbol, OrderSide.Buy, FuturesOrderType.Limit, quantity, null, null, null, false
                    //PositionSide.Long
                    );
                return (response.Data);

            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }
            return null;
        }

        public static async Task<bool> BuyLimitTether(string symbol, decimal quantity, decimal price,
            decimal StopLoss = 0.0M, decimal TakeProfit = 0.0M)
        {
            int numOrders = 1;
            if (StopLoss > 0.0M)
            {
                numOrders++;
            }
            if (TakeProfit > 0.0M)
            {
                numOrders++;
            }
            int current = 0;
            BinanceFuturesBatchOrder[] orders = new BinanceFuturesBatchOrder[numOrders];
            orders[current] = new BinanceFuturesBatchOrder();
            orders[current].Symbol = symbol;
            orders[current].Price = price;
            orders[current].Type = FuturesOrderType.Limit;
            orders[current].Quantity = quantity; 
            current++;
            if (StopLoss > 0.0M)
            {
                orders[current] = new BinanceFuturesBatchOrder();
                orders[current].Symbol = symbol;
                orders[current].Price = StopLoss;
                orders[current].Type = FuturesOrderType.StopMarket;
                orders[current].Quantity = quantity;
                orders[current].ReduceOnly = true;
                current++;
            }
            if (TakeProfit > 0.0M)
            {
                orders[current] = new BinanceFuturesBatchOrder();
                orders[current].Symbol = symbol;
                orders[current].Price = TakeProfit;
                orders[current].Type = FuturesOrderType.TakeProfit;
                orders[current].Quantity = quantity;
                orders[current].ReduceOnly = true;
            }

            
            try
            {
                var response = await client.UsdFuturesApi.Trading.PlaceMultipleOrdersAsync(orders);
                return response.Success;
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }
            return false;

        }

        public static async Task<bool> BuyMarketTether(string symbol, decimal quantity,
            decimal StopLoss = 0.0M, decimal TakeProfit = 0.0M)
        {
            int numOrders = 1;
            if (StopLoss > 0.0M)
            {
                numOrders++;
            }
            if (TakeProfit > 0.0M)
            {
                numOrders++;
            }
            int current = 0;
            BinanceFuturesBatchOrder[] orders = new BinanceFuturesBatchOrder[numOrders];
            orders[current] = new BinanceFuturesBatchOrder();
            orders[current].Symbol = symbol;
            orders[current].Type = FuturesOrderType.Market;
            orders[current].Quantity = quantity; //TODO prestar atenção no Quantity, é negativo para vendar, fazer checagem geral
            //orders[current].PositionSide = PositionSide.Both;
            current++;
            if (StopLoss > 0.0M)
            {
                orders[current] = new BinanceFuturesBatchOrder();
                orders[current].Symbol = symbol;
                orders[current].Price = StopLoss;
                orders[current].Type = FuturesOrderType.StopMarket;
                orders[current].Quantity = quantity;
                orders[current].ReduceOnly = true;
                current++;
            }
            if (TakeProfit > 0.0M)
            {
                orders[current] = new BinanceFuturesBatchOrder();
                orders[current].Symbol = symbol;
                orders[current].Price = TakeProfit;
                orders[current].Type = FuturesOrderType.TakeProfit;
                orders[current].Quantity = quantity;
                orders[current].ReduceOnly = true;
            }

            
            try
            {
                var response = await client.UsdFuturesApi.Trading.PlaceMultipleOrdersAsync(orders);
                return response.Success;
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }
            return false;

        }

        public static async Task<BinanceFuturesPlacedOrder> BuyStopTether(
            string symbol, decimal quantity, decimal price, decimal? stopPrice = null)
        {
            
        
            try
            {
                var response = await client.UsdFuturesApi.Trading.PlaceOrderAsync(
                    symbol, OrderSide.Buy, FuturesOrderType.Stop, quantity, null, null, null, false, null, 
                    stopPrice
                    //PositionSide.Long
                    );
                return (response.Data);

            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }
            return null;
        }

        public static async Task<bool> BuyStopTether(string symbol, decimal quantity, decimal price, decimal? stopPrice = null,
            decimal StopLoss = 0.0M, decimal TakeProfit = 0.0M)
        {
            int numOrders = 1;
            if (StopLoss > 0.0M)
            {
                numOrders++;
            }
            if (TakeProfit > 0.0M)
            {
                numOrders++;
            }
            int current = 0;
            BinanceFuturesBatchOrder[] orders = new BinanceFuturesBatchOrder[numOrders];
            orders[current] = new BinanceFuturesBatchOrder();
            orders[current].Symbol = symbol;
            orders[current].StopPrice = stopPrice;
            orders[current].ActivationPrice = price;
            orders[current].Type = FuturesOrderType.Stop;
            orders[current].Quantity = quantity;
            current++;
            if (StopLoss > 0.0M)
            {
                orders[current] = new BinanceFuturesBatchOrder();
                orders[current].Symbol = symbol;
                orders[current].Price = StopLoss;
                orders[current].Type = FuturesOrderType.StopMarket;
                orders[current].Quantity = quantity;
                orders[current].ReduceOnly = true;
                current++;
            }
            if (TakeProfit > 0.0M)
            {
                orders[current] = new BinanceFuturesBatchOrder();
                orders[current].Symbol = symbol;
                orders[current].Price = TakeProfit;
                orders[current].Type = FuturesOrderType.TakeProfit;
                orders[current].Quantity = quantity;
                orders[current].ReduceOnly = true;
            }

            
            try
            {
                var response = await client.UsdFuturesApi.Trading.PlaceMultipleOrdersAsync(orders);
                return response.Success;
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }
            return false;

        }

        public static async Task<bool> CheckApiConnection()
        {
            

            try
            {
                
                //WebCallResult<BinanceAccountInfo> info = await client.General.GetAccountInfoAsync();
               
                //return (info.Success);
                
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }
            return (false);
        }

        public static async Task<bool> ChangeLeverage(string symbol, int leverage)
        {
            

            try
            {
                var response = await client.UsdFuturesApi.Account.ChangeInitialLeverageAsync(symbol, leverage);

                return response.Success;
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }
            return (false);
        }

        public static async Task<bool> CheckConnection()
        {
            

            try
            {
                var response = await client.UsdFuturesApi.ExchangeData.PingAsync();
                return (response.Success);

            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }
            return (false);

        }

        public static async Task<bool> CloseOrders(string symbol)
        {
            
            var response = await client.UsdFuturesApi.Trading.CancelAllOrdersAsync(symbol);
            return (response.Success);
        }

        public static async Task<bool> ClosePositions(string symbol)
        {
            
            var response = await client.UsdFuturesApi.Account.GetPositionInformationAsync(symbol);

            var data = response.Data;

            foreach(var item in data)
            {
                decimal lot = item.Quantity;
                PositionSide side = item.PositionSide;
                if(side == PositionSide.Long)
                {
                    var result = await client.UsdFuturesApi.Trading.PlaceOrderAsync(
                        symbol, OrderSide.Sell, FuturesOrderType.Market, lot, null, null, null, true);
                    //return (result.Success);

                }
                else if(side == PositionSide.Short)
                {
                    var result = await client.UsdFuturesApi.Trading.PlaceOrderAsync(
                        symbol, OrderSide.Buy, FuturesOrderType.Market, lot, null, null, null, true);
                    //return (result.Success);
                }
                else//TODO positionSide.Both
                {
                    return false;
                }
                
            }
            return (false);
        }

        public static async Task<int> GetLeverageTether(string symbol)
        {
            

            try
            {
                var response = await client.UsdFuturesApi.Account.GetPositionInformationAsync(symbol);

                foreach(var position in response.Data)
                {
                    if(position.Symbol == symbol)
                    {
                        return position.Leverage;
                    }
                    
                }
                return 0;
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }
            return 0;
        }


        public static async Task<BinanceFuturesAccountInfo> GetAccountInfo()
        {
            

            try
            {
                var info = await client.UsdFuturesApi.Account.GetAccountInfoAsync();
                return info.Data;
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }

            return null;
        }

        

        public static async Task<BinanceFuturesUsdtExchangeInfo> GetTetherFuturesInformation()
        {
            

            try
            {
                WebCallResult<BinanceFuturesUsdtExchangeInfo> info = await client.UsdFuturesApi.ExchangeData.GetExchangeInfoAsync();
                return (info.Data);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }

            return new BinanceFuturesUsdtExchangeInfo();
        }

        public static async Task<BinanceFuturesUsdtExchangeInfo> GetTetherFuturesInformationAsync()
        {
            

            try
            {
                WebCallResult<BinanceFuturesUsdtExchangeInfo> info = await client.UsdFuturesApi.ExchangeData.GetExchangeInfoAsync();
                return (info.Data);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }

            return new BinanceFuturesUsdtExchangeInfo();
        }

        public static async Task<IEnumerable<BinancePositionInfoUsdt>> GetTetherPositions()
        {
            

            try
            {
                var info = await client.UsdFuturesApi.Account.GetAccountInfoAsync();

                return info.Data.Positions;
                /*foreach(var position in info.Data.Positions)
                {
                    position.EntryPrice;
                    position.InitialMargin;
                    position.Isolated;
                    position.Leverage;
                    position.MaintMargin;
                    position.MaxNotional;
                    position.OpenOrderInitialMargin;
                    position.PositionAmount;
                    position.PositionSide;
                    position.Symbol;
                    position.UnrealizedProfit;
                }*/
                
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }

            return null;
        }

        public static async Task<IEnumerable<BinanceFuturesUsdtSymbol>> GetTetherFuturesSymbols()
        {
            

            try
            {
                var info = await client.UsdFuturesApi.ExchangeData.GetExchangeInfoAsync();
                /*foreach (var s in info.Data.Symbols)
                {
                    
                    s.MinNotionalFilter.MinNotional;
                    s.PriceFilter.MinPrice;
                    s.PriceFilter.MaxPrice;
                    s.LotSizeFilter.MaxQuantity;
                    s.LotSizeFilter.MinQuantity;
                    s.LotSizeFilter.StepSize;

                }*/
                return (info.Data.Symbols);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }

            return null;
        }

        public async static Task<BinanceFuturesUsdtSymbol> GetTetherFuturesSymbols(string symbol)
        {
            

            try
            {
                var info = await publicClient.UsdFuturesApi.ExchangeData.GetExchangeInfoAsync();
                
                foreach (var s in info.Data.Symbols)
                {
                    if(s.Name == symbol)
                    {
                        return s;
                    }
                    
                }
               
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }

            return null;
        }

        public static async Task<IEnumerable<BinanceFuturesUsdtSymbol>> GetSymbolsInformation()
        {
            

            try
            {
               var info = await publicClient.UsdFuturesApi.ExchangeData.GetExchangeInfoAsync();
                return (info.Data.Symbols);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }

            return null;
        }

        public static async Task<BinanceFuturesUsdtSymbol> GetSymbolInformation(string symbolName)
        {

            try
            {
                
                var info = await client.UsdFuturesApi.ExchangeData.GetExchangeInfoAsync();
                foreach(var symbol in info.Data.Symbols)
                {
                    if(symbol.Name == symbolName)
                    {
                        return symbol;
                    }
                }
                return (null);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }

            return null;
        }

        public static async Task<IEnumerable<BinanceBookPrice>> GetOrderBook(string symbol)
        {
            

            try
            {
                WebCallResult<IEnumerable<BinanceBookPrice>> bookPrices = await client.
                    UsdFuturesApi.ExchangeData.GetBookPricesAsync();
                return bookPrices.Data;
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }
            return null;

        }

        
        

        public static async Task<IEnumerable<BinanceFuturesAccountBalance>> GetBalancesTether()
        {
            
            try
            {
                var balances = await client.UsdFuturesApi.Account.GetBalancesAsync();
                return balances.Data;
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }
            return null;
        }

        /*public static async Task<IEnumerable<IBinance24HPrice>> get24MiniTicker()
        {
            WebCallResult<IEnumerable<IBinance24HPrice>> result = await Program.getPublicClient()
                .UsdFuturesApi.ExchangeData.GetPricesAsync();
            return result.Data;
        }*/


        public async Task<UpdateSubscription> InitOrderBook(string symbol, int updateInterval = 500, Action<IBinanceOrderBook> OnMessage=null)
        {
            
            var result = await publicSocketClient.UsdFuturesStreams.SubscribeToOrderBookUpdatesAsync(symbol, updateInterval, (Action<DataEvent<IBinanceFuturesEventOrderBook>>)OnMessage);
            if (result.Success)
            {
                return result.Data;
            }

            return null;
        }

        public async Task<UpdateSubscription> InitPartialBook(string symbol, int levels, int updateInterval, 
            Action<IBinanceFuturesEventOrderBook> OnMessage=null)
        {

            var result = await publicSocketClient.UsdFuturesStreams.SubscribeToPartialOrderBookUpdatesAsync(
                symbol, levels, updateInterval, (Action<DataEvent<IBinanceFuturesEventOrderBook>>)OnMessage);
            if (result.Success)
            {
                return result.Data;
            }

            return null;
        }

        public async Task<UpdateSubscription> InitUserStreams(string symbol, int updateInterval, 
            Action<BinanceFuturesStreamConfigUpdate> OnLeverage=null,
            Action<BinanceFuturesStreamMarginUpdate> OnMargin=null,
            Action<BinanceFuturesStreamAccountUpdate> OnAccount=null,
            Action<BinanceFuturesStreamOrderUpdate> OnOrder=null,
            Action<BinanceStreamEvent> OnExpired=null
            )
        {
           

            if(socketClient is null)
            {
                return null;
            }

            var request = await client.UsdFuturesApi.Account.StartUserStreamAsync();

            var result = await socketClient.UsdFuturesStreams.SubscribeToUserDataUpdatesAsync(
                symbol, (Action<DataEvent<BinanceFuturesStreamConfigUpdate>>)OnLeverage, 
                (Action<DataEvent<BinanceFuturesStreamMarginUpdate>>)OnMargin,
                (Action<DataEvent<BinanceFuturesStreamAccountUpdate>>)OnAccount,
                (Action<DataEvent<BinanceFuturesStreamOrderUpdate>>)OnOrder,
                (Action<DataEvent<BinanceStreamEvent>>)OnExpired);
            if (result.Success)
            {
                return result.Data;
            }

            return null;
        }

        public static async Task<BinanceFuturesPlacedOrder> SellLimitTether(string symbol, decimal quantity, decimal price)
        {
            

            try
            {
                var response = await client.UsdFuturesApi.Trading.PlaceOrderAsync(
                    symbol, OrderSide.Sell, FuturesOrderType.Stop, quantity, null, null, null, false
                    //PositionSide.Long
                   );
                return (response.Data);

            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }
            return null;
        }

        public static async Task<bool> SellLimitTether(string symbol, decimal quantity, decimal price, decimal? stopPrice = null,
            decimal StopLoss = 0.0M, decimal TakeProfit = 0.0M)
        {
            int numOrders = 1;
            if (StopLoss > 0.0M)
            {
                numOrders++;
            }
            if (TakeProfit > 0.0M)
            {
                numOrders++;
            }
            int current = 0;
            BinanceFuturesBatchOrder[] orders = new BinanceFuturesBatchOrder[numOrders];
            orders[current] = new BinanceFuturesBatchOrder();
            orders[current].Symbol = symbol;
            orders[current].Price = price;
            orders[current].Type = FuturesOrderType.Limit;
            orders[current].Quantity = -quantity;
            current++;
            if (StopLoss > 0.0M)
            {
                orders[current] = new BinanceFuturesBatchOrder();
                orders[current].Symbol = symbol;
                orders[current].Price = StopLoss;
                orders[current].Type = FuturesOrderType.StopMarket;
                orders[current].Quantity = -quantity;
                orders[current].ReduceOnly = true;
                current++;
            }
            if (TakeProfit > 0.0M)
            {
                orders[current] = new BinanceFuturesBatchOrder();
                orders[current].Symbol = symbol;
                orders[current].Price = TakeProfit;
                orders[current].Type = FuturesOrderType.TakeProfit;
                orders[current].Quantity = -quantity;
                orders[current].ReduceOnly = true;
            }

            
            try
            {
                var response = await client.UsdFuturesApi.Trading.PlaceMultipleOrdersAsync(orders);
                return response.Success;
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }
            return false;

        }

        public static async Task<bool> SellMarketTether(string symbol, decimal quantity,
            decimal StopLoss = 0.0M, decimal TakeProfit = 0.0M)
        {
            int numOrders = 1;
            if (StopLoss > 0.0M)
            {
                numOrders++;
            }
            if (TakeProfit > 0.0M)
            {
                numOrders++;
            }
            int current = 0;
            BinanceFuturesBatchOrder[] orders = new BinanceFuturesBatchOrder[numOrders];
            orders[current] = new BinanceFuturesBatchOrder();
            orders[current].Symbol = symbol;
            orders[current].Type = FuturesOrderType.Market;
            orders[current].Quantity = -quantity;
            if (StopLoss > 0.0M)
            {
                orders[current] = new BinanceFuturesBatchOrder();
                orders[current].Symbol = symbol;
                orders[current].Price = StopLoss;
                orders[current].Type = FuturesOrderType.StopMarket;
                orders[current].Quantity = -quantity;
                orders[current].ReduceOnly = true;
                current++;
            }
            if (TakeProfit > 0.0M)
            {
                orders[current] = new BinanceFuturesBatchOrder();
                orders[current].Symbol = symbol;
                orders[current].Price = TakeProfit;
                orders[current].Type = FuturesOrderType.TakeProfit;
                orders[current].Quantity = -quantity;
                orders[current].ReduceOnly = true;
            }

            
            try
            {
                var response = await client.UsdFuturesApi.Trading.PlaceMultipleOrdersAsync(orders);
                return response.Success;
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }
            return false;

        }

        public static async Task<BinanceFuturesPlacedOrder> SellStopTether(string symbol, decimal quantity, decimal price, decimal? stopPrice=null)
        {
            

            try
            {
                var response = await client.UsdFuturesApi.Trading.PlaceOrderAsync(
                    symbol, OrderSide.Sell, FuturesOrderType.Limit, quantity,
                   null, null, null, null, null, stopPrice, null, null, null);

                return (response.Data);

            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }
            return null;
        }

        public static async Task<bool> SellStopTether(string symbol, decimal quantity, decimal price, decimal? stopPrice = null,
            decimal StopLoss= 0.0M, decimal TakeProfit=0.0M)
        {
            int numOrders = 1;
            if (StopLoss > 0.0M)
            {
                numOrders++;
            }
            if (TakeProfit > 0.0M)
            {
                numOrders++;
            }
            int current = 0;
            BinanceFuturesBatchOrder[] orders = new BinanceFuturesBatchOrder[numOrders];
            orders[current] = new BinanceFuturesBatchOrder();
            orders[current].Symbol = symbol;
            orders[current].StopPrice = stopPrice;
            orders[current].ActivationPrice = price;
            orders[current].Type = FuturesOrderType.Stop;
            orders[current].Quantity = -quantity;
            current++;
            if (StopLoss > 0.0M)
            {
                orders[current] = new BinanceFuturesBatchOrder();
                orders[current].Symbol = symbol;
                orders[current].Price = StopLoss;
                orders[current].Type = FuturesOrderType.StopMarket;
                orders[current].Quantity = -quantity;
                orders[current].ReduceOnly = true;
                current++;
            }
            if (TakeProfit > 0.0M)
            {
                orders[current] = new BinanceFuturesBatchOrder();
                orders[current].Symbol = symbol;
                orders[current].Price = TakeProfit;
                orders[current].Type = FuturesOrderType.TakeProfit;
                orders[current].Quantity = -quantity;
                orders[current].ReduceOnly = true;
            }

            
            try
            {
                var response = await client.UsdFuturesApi.Trading.PlaceMultipleOrdersAsync(orders);
                return  response.Success;
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message, ex.HResult);
            }
            return false;

        }


    }
}
