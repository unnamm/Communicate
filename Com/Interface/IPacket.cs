﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Interface
{
    /// <summary>
    /// packet parent
    /// </summary>
    public interface IPacket
    {
        object[]? Params { get; set; }

        string GetCommand();
    }
}
