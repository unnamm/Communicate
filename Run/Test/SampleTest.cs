using Com;
using Com.Common;
using Com.Interface;
using Com.Modbus;
using Run.Test.Packet;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run.Test
{
    /// <summary>
    /// how to use
    /// </summary>
    public class SampleTest
    {
        public static async void Run() //run test
        {
            Test t = new();

            await t.Tcp();
            await t.Serial();
            t.Serial2();
            await t.ModbusTcp();
            await t.ModbusRTU();
        }

        class Test
        {
            public async Task Tcp()
            {
                TcpCommunicate device = new("127.0.0.1", 6053);
                await ComRun(device);
            }

            public async Task Serial() //same packet other protocol
            {
                SerialCommunicate device = new("COM1", 9600, 8, Parity.None);
                await ComRun(device);
            }

            private async Task ComRun(Communicate device) //same packet other protocol
            {
                await device.ConnectAsync();

                await device.WriteAsync(new SetTargetVolt(), "15");

                var idnData = await device.QueryAsync(new ReadIDN());
                var sample = await device.QueryAsync(new QuerySamplePacket(), 1);

                device.Dispose();
            }

            public void Serial2()
            {
                SerialCommunicate device = new("COM2", receiveHandler: receiveEvent);
            }
            private void receiveEvent(object o, SerialDataReceivedEventArgs e)
            {
                var serialPort = (SerialPort)o;
                var data = serialPort.ReadExisting();
            }

            public async Task ModbusTcp()
            {
                TcpCommunicate tcp = new("127.0.0.1", 502);
                await tcp.ConnectAsync();
                var modbus = new ModbusTCP(tcp);

                await ModbusRun(modbus);
            }

            public async Task ModbusRTU()
            {
                SerialCommunicate serial = new("COM1");
                await serial.ConnectAsync();
                var modbus = new ModbusRTU(serial, false);

                await ModbusRun(modbus);
            }

            private async Task ModbusRun(IModbus modbus)
            {
                await modbus.WriteSingleRegister(0x0001, 0x000F);
                var result = await modbus.ReadHoldingRegisters(0x0000, 10, 1);
            }
        }
    }
}
