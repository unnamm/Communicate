﻿using Com.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run.Test.Packet
{
    internal class ReadIDN : QueryPacket<string>
    {
        public override string GetCommand() => "*IDN?\n";

        protected override string Convert(string receiveData) => receiveData;
    }
}
