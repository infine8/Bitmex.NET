using System;

namespace Bitmex.NET.Models
{
    public partial class OrderPOSTRequestParams
    {
        public static OrderPOSTRequestParams ClosePositionByMarket(string symbol, string comment = null)
        {
            return new OrderPOSTRequestParams
            {
                Symbol = symbol,
                ExecInst = "Close",
                Text = comment
            };
        }
        public static OrderPOSTRequestParams ClosePositionByLimit(string symbol, decimal price, string comment = null)
        {
            return new OrderPOSTRequestParams
            {
                Symbol = symbol,
                ExecInst = "Close",
                Price = price,
                Text = comment
            };
        }

        public static OrderPOSTRequestParams CreateSimpleMarket(string symbol, decimal quantity, OrderSide side, string comment = null)
        {
            return new OrderPOSTRequestParams
            {
                Symbol = symbol,
                Side = Enum.GetName(typeof(OrderSide), side),
                OrderQty = quantity,
                //DisplayQty = quantity,
                OrdType = Enum.GetName(typeof(OrderType), OrderType.Market),
                Text = comment
            };
        }

        public static OrderPOSTRequestParams CreateSimpleLimit(string symbol, decimal quantity, decimal price, OrderSide side, string comment = null, bool cancelIfExecOnPlace = true)
        {
            return new OrderPOSTRequestParams
            {
                Symbol = symbol,
                Side = Enum.GetName(typeof(OrderSide), side),
                OrderQty = quantity,
                OrdType = Enum.GetName(typeof(OrderType), OrderType.Limit),
                //DisplayQty = quantity,
                Price = price,
                ExecInst = cancelIfExecOnPlace ? "ParticipateDoNotInitiate" : string.Empty,
                Text = comment
            };
        }

        /// <summary>
        /// Be aware that bitmex takes fee for hiden limit orders
        /// </summary>
        public static OrderPOSTRequestParams CreateSimpleHidenLimit(string symbol, decimal quantity, decimal price, OrderSide side, string comment = null, bool cancelIfExecOnPlace = true)
        {
            return new OrderPOSTRequestParams
            {
                Symbol = symbol,
                Side = Enum.GetName(typeof(OrderSide), side),
                OrderQty = quantity,
                OrdType = Enum.GetName(typeof(OrderType), OrderType.Limit),
                DisplayQty = 0,
                Price = price,
                ExecInst = cancelIfExecOnPlace ? "ParticipateDoNotInitiate" : string.Empty,
                Text = comment
            };
        }

        public static OrderPOSTRequestParams CreateMarketStopOrder(string symbol, decimal quantity, decimal stopPrice, OrderSide side, string comment = null, bool reduceOnly = false)
        {
            return new OrderPOSTRequestParams
            {
                Symbol = symbol,
                Side = Enum.GetName(typeof(OrderSide), side),
                OrderQty = quantity,
                //DisplayQty = quantity,
                OrdType = Enum.GetName(typeof(OrderType), OrderType.Stop),
                StopPx = stopPrice,
                ExecInst = (reduceOnly ?  "ReduceOnly," : "") + "LastPrice",
                Text = comment
            };
        }

        public static OrderPOSTRequestParams CreateLimitStopOrder(string symbol, decimal quantity, decimal stopPrice, decimal price, OrderSide side, string comment = null, bool reduceOnly = false)
        {
            return new OrderPOSTRequestParams
            {
                Symbol = symbol,
                Side = Enum.GetName(typeof(OrderSide), side),
                OrderQty = quantity,
                //DisplayQty = quantity,
                OrdType = Enum.GetName(typeof(OrderType), OrderType.Stop),
                StopPx = stopPrice,
                Price = price,
                ExecInst = (reduceOnly ? "ReduceOnly," : "") + "LastPrice",
                Text = comment
            };
        }
    }
}
