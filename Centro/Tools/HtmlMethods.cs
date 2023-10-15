namespace Centro
{
    class HtmlMethods
    {

        private static string currentTheme = "light";

        private static string currentSymbol = "BTCUSDT";

        private static int currentInterval = 1;

        public static string CreateAutoSizeDocument(string symbol = "", int interval = 0, string theme = "")
        {
            if (symbol.Trim().Length > 0)
            {
                currentSymbol = symbol;
            }
            if (interval > 0)
            {
                currentInterval = interval;
            }
            if (theme.Trim().Length > 0)
            {
                currentTheme = theme;
            }
            string document =

                    "<div class=\"tradingview-widget-container\">" +
                    "<div id =\"tradingview_1d317\" ></div>" +
                    "<script type =\"text/javascript\" src=\"https://s3.tradingview.com/tv.js\"></script>" +
                    "<script type=\"text/javascript\">" +
                    "new TradingView.widget({" +

                    "  \"autosize\":true," +
                    "  \"symbol\":\"BINANCE:" + currentSymbol + "\"," +
                    "\"interval\":\"" + currentInterval + "\", " +
                    "\"timezone\":\"Etc/UTC\"," +
                    "\"theme\":\"" + currentTheme + "\"," +
                    "\"style\":\"1\"," +
                    "\"locale\":\"en\"," +
                    "\"toolbar_bg\":\"#f1f3f6\"," +
                    "\"enable_publishing\":false," +
                    "\"allow_symbol_change\":true," +
                    "\"studies\":[" +
                    "       \"MASimple@tv-basicstudies\"" +
                    "     ]," +
                    "     \"container_id\":\"tradingview_1d317\"" +
                    "}" +
                    "  );" +
                    "  </script>" +
                    "</div>";

            return (document);
        }
    }
}
