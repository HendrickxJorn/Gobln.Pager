using System;

namespace Gobln.Pager;

internal static class Validator
{
    private const string LESSTHENZEROORZEROEXCEPTIONSTRING = "Can not be less then zero or zero";
    private const string LESSTHENZEROEXCEPTIONSTRING = "Can not be less then zero";

    internal static void Validate(int pageIndex, int pageSize, int? totalItemCount = null)
    {
        if (pageIndex < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageIndex), LESSTHENZEROORZEROEXCEPTIONSTRING);
        }

        ValidatePageSize(pageSize);

        if (totalItemCount is not null)
        {
            ValidateTotalItemCount(totalItemCount.Value);
        }
    }

    internal static void Validate(IPageFilter pagerFilter, int? totalItemCount = null)
    {
        if (pagerFilter.PageIndex < 1)
        {
            throw new ArgumentOutOfRangeException("IPagerFilter.PageIndex", LESSTHENZEROORZEROEXCEPTIONSTRING);
        }

        if (pagerFilter.PageSize < 1)
        {
            throw new ArgumentOutOfRangeException("IPagerFilter.PageSize", LESSTHENZEROORZEROEXCEPTIONSTRING);
        }

        if (totalItemCount is not null)
        {
            ValidateTotalItemCount(totalItemCount.Value);
        }
    }

    internal static void ValidatePageSize(int pageSize)
    {
        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), LESSTHENZEROORZEROEXCEPTIONSTRING);
        }
    }

    internal static void ValidateTotalItemCount(int totalItemCount)
    {
        if (totalItemCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalItemCount), LESSTHENZEROEXCEPTIONSTRING);
        }
    }
}