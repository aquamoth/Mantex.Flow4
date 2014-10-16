using Flow4.Entities.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Entities
{
    public class ScanlinePool : Pool<ScanlineBuilder>, IScanlinePool
    {
        readonly int _numberOfPixels;

        public ScanlinePool(int numberOfPixels)
        {
            this._numberOfPixels = numberOfPixels;
        }

        protected override ScanlineBuilder OnCreateNew()
        {
            var builder = new ScanlineBuilder(_numberOfPixels);
            //Debug.WriteLine("Created new object: " + builder.GetHashCode());
            return builder;
        }
    }
}
