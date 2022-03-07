// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.GraphQL.TestApp.Tables;

public class WorkOrder
{
	public int WorkOrderID { get; set; }
	public int ProductID { get; set; }
	public int OrderQty { get; set; }
	public int StockedQty { get; set; }
	public int ScrappedQty { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime? EndDate { get; set; }
	public DateTime DueDate { get; set; }
	public int? ScrapReasonID { get; set; }
	public DateTime ModifiedDate { get; set; }
}
