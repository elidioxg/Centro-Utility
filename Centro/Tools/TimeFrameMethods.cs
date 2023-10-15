using Binance.Net.Enums;

namespace Centro.Tools
{
    public class TimeFrameMethods
    {
        public static KlineInterval GetKlineInterval(string timeframe)
        {
            switch (timeframe.Trim())
            {
                case "M1":
                    return KlineInterval.OneMinute;

                case "M3":
                    return KlineInterval.ThreeMinutes;

                case "M5":
                    return KlineInterval.FiveMinutes;

                case "M15":
                    return KlineInterval.FifteenMinutes;

                case "M30":
                    return KlineInterval.ThirtyMinutes;

                case "H1":
                    return KlineInterval.OneHour;

                case "H2":
                    return KlineInterval.TwoHour;

                case "H4":
                    return KlineInterval.FourHour;

                case "H6":
                    return KlineInterval.SixHour;

                case "H8":
                    return KlineInterval.EightHour;

                case "H12":
                    return KlineInterval.TwelveHour;

                case "D1":
                    return KlineInterval.OneDay;

                case "W1":
                    return KlineInterval.OneWeek;

            }
            return KlineInterval.OneMonth;
        }


        public static int GetKlinePeriod(string timeframe)
        {
            if (timeframe.Trim() == "M1")
            {
                return 1;
            }

            if (timeframe.Trim() == "M3")
            {
                return 3;
            }

            if (timeframe.Trim() == "M5")
            {
                return 5;
            }

            if (timeframe.Trim() == "M15")
            {
                return 15;
            }

            if (timeframe.Trim() == "M30")
            {
                return 30;
            }

            if (timeframe.Trim() == "H1")
            {
                return 60;
            }

            if (timeframe.Trim() == "H2")
            {
                return 60 * 2;
            }

            if (timeframe.Trim() == "H4")
            {
                return 60 * 4;
            }

            if (timeframe.Trim() == "H6")
            {
                return 60 * 6;
            }

            if (timeframe.Trim() == "H8")
            {
                return 60 * 8;
            }

            if (timeframe.Trim() == "H12")
            {
                return 60 * 12;
            }

            if (timeframe.Trim() == "D1")
            {
                return 60 * 24;
            }

            if (timeframe.Trim() == "W1")
            {
                return 60 * 24 * 7;
            }

            return 0;
        }
    }
}
