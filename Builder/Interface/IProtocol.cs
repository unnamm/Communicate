﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Interface
{
    /// <summary>
    /// communication device
    /// </summary>
    public interface IProtocol
    {
        Task<string> QueryAsync(string command);
        Task WriteAsync(string command);
    }
}
