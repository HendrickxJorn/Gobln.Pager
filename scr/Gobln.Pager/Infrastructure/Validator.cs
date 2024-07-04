using System;

namespace Gobln.Pager
{
    internal static class Validator
    {
        private const string LESSTHENZEROEXCEPTIONSTRING = "Can not be less then zero or zero";

        internal static void Validate(int pageIndex, int pageSize)
        {
            if (pageIndex < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageIndex), LESSTHENZEROEXCEPTIONSTRING);
            }

            ValidatePageSize(pageSize);
        }

        internal static void Validate(IPageFilter pagerFilter)
        {
            if (pagerFilter.PageIndex < 1)
            {
                throw new ArgumentOutOfRangeException("IPagerFilter.PageIndex", LESSTHENZEROEXCEPTIONSTRING);
            }

            if (pagerFilter.PageSize < 1)
            {
                throw new ArgumentOutOfRangeException("IPagerFilter.PageSize", LESSTHENZEROEXCEPTIONSTRING);
            }
        }

        internal static void ValidatePageSize(int pageSize)
        {
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), LESSTHENZEROEXCEPTIONSTRING);
            }
        }
    }
}