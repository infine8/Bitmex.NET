﻿using Bitmex.NET.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading;

namespace Bitmex.NET.IntegrationTests.Tests
{
	[TestClass]
	public class BitmexApiServiceOrderApiTests : IntegrationTestsClass<IBitmexApiService>
	{
		private decimal _xbtAvgPrice;

		[TestCleanup]
		public void TestCleanup()
		{
			DeleteAllOrders();
		}

		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();
			var paramCloseAfter = new OrderCancelAllAfterPOSTRequestParams
			{
				Timeout = int.MaxValue
			};

			Sut.Execute(BitmexApiUrls.Order.PostOrderCancelAllAfter, paramCloseAfter).Wait();
			_xbtAvgPrice = Sut.Execute(BitmexApiUrls.OrderBook.GetOrderBookL2, new OrderBookL2GETRequestParams() { Symbol = "XBTUSD", Depth = 1 }).Result.First()
				.Price;

		}

		[TestMethod]
		public void should_return_all_orders()
		{
			// arrange
			var @params = new OrderGETRequestParams();

			// act
			var result = Sut.Execute(BitmexApiUrls.Order.GetOrder, @params).Result;

			// assert

			result.Should().NotBeNull();
			result.Count.Should().BeGreaterThan(0);
		}

		private void DeleteAllOrders()
		{
			var @params = new OrderAllDELETERequestParams()
			{
				Symbol = "XBTUSD"
			};
			Sut.Execute(BitmexApiUrls.Order.DeleteOrderAll, @params).Wait();
		}

		[TestMethod]
		public void should_place_buy_market_order()
		{
			// arrange
			var @params = OrderPOSTRequestParams.CreateSimpleMarket("XBTUSD", 3, OrderSide.Buy);

			// act
			var result = Sut.Execute(BitmexApiUrls.Order.PostOrder, @params).Result;
			// assert

			result.Should().NotBeNull();
			result.OrdStatus.Should().Be("Filled");
			result.OrderId.Should().NotBeNull();
		}

		[TestMethod]
		public void should_place_sell_market_order()
		{
			// arrange
			var @params = OrderPOSTRequestParams.CreateSimpleMarket("XBTUSD", 3, OrderSide.Sell);

			// act
			var result = Sut.Execute(BitmexApiUrls.Order.PostOrder, @params).Result;

			// assert

			result.Should().NotBeNull();
			result.OrdStatus.Should().Be("Filled");
			result.OrderId.Should().NotBeNull();
		}

		[TestMethod]
		public void should_place_buy_limit_order()
		{
			// arrange
			var @params = OrderPOSTRequestParams.CreateSimpleLimit("XBTUSD", 3, _xbtAvgPrice - 500, OrderSide.Buy);

			// act
			var result = Sut.Execute(BitmexApiUrls.Order.PostOrder, @params).Result;
			// assert

			result.Should().NotBeNull();
			result.OrdType.Should().Be("Limit");
			result.OrdStatus.Should().Be("New");
			result.OrderId.Should().NotBeNull();
		}

		[TestMethod]
		public void should_place_sell_limit_order()
		{
			// arrange
			var @params = OrderPOSTRequestParams.CreateSimpleLimit("XBTUSD", 3, _xbtAvgPrice + 500, OrderSide.Sell);

			// act
			var result = Sut.Execute(BitmexApiUrls.Order.PostOrder, @params).Result;

			// assert

			result.Should().NotBeNull();
			result.OrdType.Should().Be("Limit");
			result.OrdStatus.Should().Be("New");
			result.OrderId.Should().NotBeNull();
		}

		[TestMethod]
		public void should_place_buy_market_stop_order()
		{
			// arrange
			var @params = OrderPOSTRequestParams.CreateMarketStopOrder("XBTUSD", 3, _xbtAvgPrice + 500, OrderSide.Buy);

			// act
			var result = Sut.Execute(BitmexApiUrls.Order.PostOrder, @params).Result;
			// assert

			result.Should().NotBeNull();
			result.OrdType.Should().Be("Stop");
			result.OrdStatus.Should().Be("New");
			result.OrderId.Should().NotBeNull();
		}

		[TestMethod]
		public void should_place_sell_market_stop_order()
		{
			// arrange
			var @params = OrderPOSTRequestParams.CreateMarketStopOrder("XBTUSD", 3, _xbtAvgPrice - 500, OrderSide.Sell);

			// act
			var result = Sut.Execute(BitmexApiUrls.Order.PostOrder, @params).Result;

			// assert

			result.Should().NotBeNull();
			result.OrdType.Should().Be("Stop");
			result.OrdStatus.Should().Be("New");
			result.OrderId.Should().NotBeNull();
		}

		[TestMethod]
		public void should_change_buy_limit_order()
		{
			// arrange
			var @params = OrderPOSTRequestParams.CreateMarketStopOrder("XBTUSD", 3, _xbtAvgPrice + 500, OrderSide.Buy);
			var result = Sut.Execute(BitmexApiUrls.Order.PostOrder, @params).Result;
			result.Should().NotBeNull();
			result.OrdType.Should().Be("Stop");
			result.OrdStatus.Should().Be("New");
			result.OrderId.Should().NotBeNull();

			var @paramsPut = new OrderPUTRequestParams()
			{
				OrderID = result.OrderId,
				OrderQty = 2
			};

			// act
			var resultPut = Sut.Execute(BitmexApiUrls.Order.PutOrder, paramsPut).Result;

			// assert
			resultPut.Should().NotBeNull();
			resultPut.OrdType.Should().Be("Stop");
			resultPut.OrdStatus.Should().Be("New");
			resultPut.OrderQty.Should().Be(2);
			resultPut.OrderId.Should().NotBeNull();
		}

		[TestMethod]
		public void should_change_sell_limit_order()
		{
			// arrange
			var @params = OrderPOSTRequestParams.CreateMarketStopOrder("XBTUSD", 3, _xbtAvgPrice - 500, OrderSide.Sell);
			var result = Sut.Execute(BitmexApiUrls.Order.PostOrder, @params).Result;
			result.Should().NotBeNull();
			result.OrdType.Should().Be("Stop");
			result.OrdStatus.Should().Be("New");
			result.OrderId.Should().NotBeNull();
			var @paramsPut = new OrderPUTRequestParams()
			{
				OrderID = result.OrderId,
				OrderQty = 2
			};

			// act
			var resultPut = Sut.Execute(BitmexApiUrls.Order.PutOrder, paramsPut).Result;

			// assert
			resultPut.Should().NotBeNull();
			resultPut.OrdType.Should().Be("Stop");
			resultPut.OrdStatus.Should().Be("New");
			resultPut.OrderQty.Should().Be(2);
			resultPut.OrderId.Should().NotBeNull();
		}

		[TestMethod]
		public void should_delete_buy_limit_order()
		{
			// arrange
			var @params = OrderPOSTRequestParams.CreateMarketStopOrder("XBTUSD", 3, _xbtAvgPrice + 500, OrderSide.Buy);
			var result = Sut.Execute(BitmexApiUrls.Order.PostOrder, @params).Result;
			result.Should().NotBeNull();
			result.OrdType.Should().Be("Stop");
			result.OrdStatus.Should().Be("New");
			result.OrderId.Should().NotBeNull();

			var @paramsPut = new OrderDELETERequestParams()
			{
				OrderID = result.OrderId,
				Text = "testing api"
			};

			// act
			var resultDelete = Sut.Execute(BitmexApiUrls.Order.DeleteOrder, paramsPut).Result.FirstOrDefault(a => a.OrderId == result.OrderId);

			// assert
			resultDelete.Should().NotBeNull();
			resultDelete.OrdType.Should().Be("Stop");
			resultDelete.OrdStatus.Should().Be("Canceled");
			resultDelete.OrderId.Should().NotBeNull();
		}

		[TestMethod]
		public void should_delete_sell_limit_order()
		{
			// arrange
			var @params = OrderPOSTRequestParams.CreateMarketStopOrder("XBTUSD", 3, _xbtAvgPrice - 500, OrderSide.Sell);
			var result = Sut.Execute(BitmexApiUrls.Order.PostOrder, @params).Result;
			result.Should().NotBeNull();
			result.OrdType.Should().Be("Stop");
			result.OrdStatus.Should().Be("New");
			result.OrderId.Should().NotBeNull();
			var @paramsPut = new OrderDELETERequestParams()
			{
				OrderID = result.OrderId,
				Text = "testing api"
			};

			// act
			var resultDelete = Sut.Execute(BitmexApiUrls.Order.DeleteOrder, paramsPut).Result.FirstOrDefault(a => a.OrderId == result.OrderId);

			// assert
			resultDelete.Should().NotBeNull();
			resultDelete.OrdType.Should().Be("Stop");
			resultDelete.OrdStatus.Should().Be("Canceled");
			resultDelete.OrderId.Should().NotBeNull();
		}

		[TestMethod]
		public void should_place_buy_market_orders_bulk()
		{
			// arrange
			var @params = new OrderBulkPOSTRequestParams
			{
				Orders = new[]
				{
					OrderPOSTRequestParams.CreateSimpleMarket("XBTUSD", 3, OrderSide.Buy),
					OrderPOSTRequestParams.CreateSimpleMarket("XBTUSD", 3, OrderSide.Buy)
				}
			};

			// act
			var result = Sut.Execute(BitmexApiUrls.Order.PostOrderBulk, @params).Result;

			// assert
			result.Should().NotBeNull();
			result.All(a => a.OrdStatus == "Filled").Should().BeTrue();
			result.All(a => !string.IsNullOrWhiteSpace(a.OrderId)).Should().BeTrue();
		}

		[TestMethod]
		public void should_place_sell_market_orders_bulk()
		{
			// arrange
			var @params = new OrderBulkPOSTRequestParams
			{
				Orders = new[]
				{
					OrderPOSTRequestParams.CreateSimpleMarket("XBTUSD", 3, OrderSide.Sell),
					OrderPOSTRequestParams.CreateSimpleMarket("XBTUSD", 3, OrderSide.Sell)
				}
			};

			// act
			var result = Sut.Execute(BitmexApiUrls.Order.PostOrderBulk, @params).Result;

			// assert
			result.Should().NotBeNull();
			result.All(a => a.OrdStatus == "Filled").Should().BeTrue();
			result.All(a => !string.IsNullOrWhiteSpace(a.OrderId)).Should().BeTrue();
		}

		[TestMethod]
		public void should_amend_buy_limit_orders_bulk()
		{
			// arrange
			var @params = new OrderBulkPOSTRequestParams
			{
				Orders = new[]
				{
					OrderPOSTRequestParams.CreateSimpleLimit("XBTUSD", 3, _xbtAvgPrice - 500, OrderSide.Buy),
					OrderPOSTRequestParams.CreateSimpleLimit("XBTUSD", 3, _xbtAvgPrice - 500, OrderSide.Buy)
				}
			};
			var result = Sut.Execute(BitmexApiUrls.Order.PostOrderBulk, @params).Result;
			result.Should().NotBeNull();
			result.All(a => a.OrdStatus == "New").Should().BeTrue();
			result.All(a => !string.IsNullOrWhiteSpace(a.OrderId)).Should().BeTrue();

			var @paramsPut = new OrderBulkPUTRequestParams()
			{
				Orders = result.Select(a => new OrderPUTRequestParams()
				{
					OrderID = a.OrderId,
					OrderQty = 2,
				}).ToArray()
			};

			// act
			var resultPut = Sut.Execute(BitmexApiUrls.Order.PutOrderBulk, @paramsPut).Result;

			// assert
			resultPut.Should().NotBeNull();
			resultPut.All(a => a.OrdStatus == "New").Should().BeTrue();
			resultPut.All(a => a.OrderQty == 2).Should().BeTrue();
			resultPut.All(a => !string.IsNullOrWhiteSpace(a.OrderId)).Should().BeTrue();
		}

		[TestMethod]
		public void should_amend_sell_limit_orders_bulk()
		{
			// arrange
			var @params = new OrderBulkPOSTRequestParams
			{
				Orders = new[]
				{
					OrderPOSTRequestParams.CreateSimpleLimit("XBTUSD", 3, _xbtAvgPrice + 500, OrderSide.Sell),
					OrderPOSTRequestParams.CreateSimpleLimit("XBTUSD", 3, _xbtAvgPrice + 500, OrderSide.Sell)
				}
			};
			var result = Sut.Execute(BitmexApiUrls.Order.PostOrderBulk, @params).Result;
			result.Should().NotBeNull();
			result.All(a => a.OrdStatus == "New").Should().BeTrue();
			result.All(a => !string.IsNullOrWhiteSpace(a.OrderId)).Should().BeTrue();

			var @paramsPut = new OrderBulkPUTRequestParams()
			{
				Orders = result.Select(a => new OrderPUTRequestParams()
				{
					OrderID = a.OrderId,
					OrderQty = 2,
				}).ToArray()
			};

			// act
			var resultPut = Sut.Execute(BitmexApiUrls.Order.PutOrderBulk, @paramsPut).Result;

			// assert
			resultPut.Should().NotBeNull();
			resultPut.All(a => a.OrdStatus == "New").Should().BeTrue();
			resultPut.All(a => a.OrderQty == 2).Should().BeTrue();
			resultPut.All(a => !string.IsNullOrWhiteSpace(a.OrderId)).Should().BeTrue();
		}

		[TestMethod]
		public void should_cancel_all_buy_limit_orders_after()
		{
			// arrange
			var @params = OrderPOSTRequestParams.CreateSimpleLimit("XBTUSD", 3, _xbtAvgPrice - 500, OrderSide.Buy);
			var result = Sut.Execute(BitmexApiUrls.Order.PostOrder, @params).Result;
			result.Should().NotBeNull();
			result.OrdType.Should().Be("Limit");
			result.OrdStatus.Should().Be("New");
			result.OrderId.Should().NotBeNull();

			var paramCloseAfter = new OrderCancelAllAfterPOSTRequestParams
			{
				Timeout = 1000
			};

			// act
			Sut.Execute(BitmexApiUrls.Order.PostOrderCancelAllAfter, paramCloseAfter).Wait();
			Thread.Sleep(1500);
			var resultCloseAfter = Sut.Execute(BitmexApiUrls.Order.GetOrder, new OrderGETRequestParams { Symbol = "XBTUSD" }).Result;


			// assert
			resultCloseAfter.Should().NotBeNull();
			resultCloseAfter.All(a => !string.IsNullOrWhiteSpace(a.OrderId)).Should().BeTrue();
		}

		[TestMethod]
		public void should_cancel_all_sell_limit_orders_after()
		{
			// arrange
			var @params = OrderPOSTRequestParams.CreateSimpleLimit("XBTUSD", 3, _xbtAvgPrice + 500, OrderSide.Sell);
			var result = Sut.Execute(BitmexApiUrls.Order.PostOrder, @params).Result;
			result.Should().NotBeNull();
			result.OrdType.Should().Be("Limit");
			result.OrdStatus.Should().Be("New");
			result.OrderId.Should().NotBeNull();

			var paramCloseAfter = new OrderCancelAllAfterPOSTRequestParams
			{
				Timeout = 1000
			};

			// act
			Sut.Execute(BitmexApiUrls.Order.PostOrderCancelAllAfter, paramCloseAfter).Wait();
			Thread.Sleep(1500);
			var resultCloseAfter = Sut.Execute(BitmexApiUrls.Order.GetOrder, new OrderGETRequestParams { Symbol = "XBTUSD" }).Result;

			// assert
			resultCloseAfter.Should().NotBeNull();
			resultCloseAfter.All(a => !string.IsNullOrWhiteSpace(a.OrderId)).Should().BeTrue();
		}

		[TestMethod]
		public void should_close_long_market_position_by_market()
		{
			// arrange
			DeleteAllOrders();
			var @params = OrderPOSTRequestParams.CreateSimpleMarket("XBTUSD", 3, OrderSide.Buy);
			var result = Sut.Execute(BitmexApiUrls.Order.PostOrder, @params).Result;
			result.Should().NotBeNull();
			result.OrdStatus.Should().Be("Filled");
			result.OrderId.Should().NotBeNull();

			// act
			var @paramsClosePosition = new OrderClosePositionPOSTRequestParams
			{
				Symbol = "XBTUSD"
			};

			var resultClosePostion = Sut.Execute(BitmexApiUrls.Order.PostOrderClosePosition, @paramsClosePosition).Result;
			// assert
			resultClosePostion.Should().NotBeNull();
			resultClosePostion.OrderId.Should().NotBeNull();
		}

		[TestMethod]
		public void should_close_short_martket_position_by_market()
		{
			// arrange
			DeleteAllOrders();
			var @params = OrderPOSTRequestParams.CreateSimpleMarket("XBTUSD", 3, OrderSide.Sell);
			var result = Sut.Execute(BitmexApiUrls.Order.PostOrder, @params).Result;
			result.Should().NotBeNull();
			result.OrdStatus.Should().Be("Filled");
			result.OrderId.Should().NotBeNull();

			// act
			var @paramsClosePosition = new OrderClosePositionPOSTRequestParams
			{
				Symbol = "XBTUSD"
			};

			var resultClosePostion = Sut.Execute(BitmexApiUrls.Order.PostOrderClosePosition, @paramsClosePosition).Result;

			// assert
			resultClosePostion.Should().NotBeNull();
			resultClosePostion.OrderId.Should().NotBeNull();
		}

		[TestMethod]
		public void should_close_long_market_position_by_limit()
		{
			// arrange
			DeleteAllOrders();
			var @params = OrderPOSTRequestParams.CreateSimpleMarket("XBTUSD", 3, OrderSide.Buy);
			var result = Sut.Execute(BitmexApiUrls.Order.PostOrder, @params).Result;
			result.Should().NotBeNull();
			result.OrdStatus.Should().Be("Filled");
			result.OrderId.Should().NotBeNull();

			// act
			var @paramsClosePosition = new OrderClosePositionPOSTRequestParams
			{
				Symbol = "XBTUSD",
				Price = _xbtAvgPrice + 500
			};

			var resultClosePostion = Sut.Execute(BitmexApiUrls.Order.PostOrderClosePosition, @paramsClosePosition).Result;
			// assert
			resultClosePostion.Should().NotBeNull();
			resultClosePostion.OrderId.Should().NotBeNull();
		}

		[TestMethod]
		public void should_close_short_martket_position_by_limit()
		{
			// arrange
			DeleteAllOrders();
			var @params = OrderPOSTRequestParams.CreateSimpleMarket("XBTUSD", 3, OrderSide.Sell);
			var result = Sut.Execute(BitmexApiUrls.Order.PostOrder, @params).Result;
			result.Should().NotBeNull();
			result.OrdStatus.Should().Be("Filled");
			result.OrderId.Should().NotBeNull();

			// act
			var @paramsClosePosition = new OrderClosePositionPOSTRequestParams
			{
				Symbol = "XBTUSD",
				Price = _xbtAvgPrice - 500
			};

			var resultClosePostion = Sut.Execute(BitmexApiUrls.Order.PostOrderClosePosition, @paramsClosePosition).Result;
			Thread.Sleep(1000);

			// assert
			resultClosePostion.Should().NotBeNull();
			resultClosePostion.OrderId.Should().NotBeNull();
		}
	}
}
