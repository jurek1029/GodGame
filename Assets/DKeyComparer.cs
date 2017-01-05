﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public class DKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable
    {
        public int Compare(TKey x, TKey y)
        {
            int result = x.CompareTo(y);

            if (result == 0)
                return 1;
            else return result;
        }
    }
}
