using Binance.Net.Objects.Models.Futures;
using System.Collections.Generic;

namespace Centro.Tools
{
    static class SymbolMethods
    {

        static Dictionary<string, BinanceFuturesUsdtSymbol> SymbolsTether = new Dictionary<string, BinanceFuturesUsdtSymbol>();
        static IEnumerable<BinanceFuturesUsdtSymbol> TetherSymbolsInfo;

        static string[] SymbolsArray;
        
        public static async void InitializeSymbols() 
        {
            TetherSymbolsInfo = await BinanceFutures.GetSymbolsInformation();

            foreach(var symbol in TetherSymbolsInfo)
            {
                SymbolsTether.Add(symbol.Name, symbol);
            }

            SymbolsArray = new string[SymbolsTether.Keys.Count];
            SymbolsTether.Keys.CopyTo(SymbolsArray, 0);
        }

        public static BinanceFuturesUsdtSymbol GetSymbolInfo(string name="")
        {
            if(SymbolsTether.ContainsKey(name))
            {
                return SymbolsTether[name];
            }
            return null;
        }
    }
}
