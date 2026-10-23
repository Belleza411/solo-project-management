namespace SoloProjectManagement.UI.Common
{
	public sealed record PagedResult<T>(
		List<T> Items,
		int PageNumber,
		int PageSize,
		int TotalCount);
}
