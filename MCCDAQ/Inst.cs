using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MccDaq;

namespace Inst_TESEQ_PS5740
{
    public class Inst
    {
        private MccDaq.MccBoard DaqBoard;
        MccDaq.ErrorInfo ULStat;
        private IntPtr MemHandle = IntPtr.Zero;
        string _resourceName;
        public string resourceName {
            get => _resourceName;
            set => _resourceName = value;
        }

        public void open(string parameters = "0")
        {
            // Initiate error handling
            //  activating error handling will trap errors like
            //  bad channel numbers and non-configured conditions.
            //  Parameters:
            //    MccDaq.ErrorReporting.PrintAll :all warnings and errors encountered will be printed
            //    MccDaq.ErrorHandling.StopAll   :if any error is encountered, the program will stop

            clsErrorDefs.ReportError = MccDaq.ErrorReporting.PrintAll;
            clsErrorDefs.HandleError = MccDaq.ErrorHandling.StopAll;
            ULStat = MccDaq.MccService.ErrHandling
                (clsErrorDefs.ReportError, clsErrorDefs.HandleError);
            DaqBoard = new MccDaq.MccBoard(int.Parse(parameters));

            Trace.TraceInformation("MccDaq.MccBoard instrument open");
        }
        public void selfTest()
        {
            Trace.TraceInformation("MccDaq.MccBoard instrument self test not available");
        }

        public void close()
        {
            ULStat = MccDaq.MccService.WinBufFreeEx(MemHandle); // Free up memory for use by
            Trace.TraceInformation("MccDaq.MccBoard instrument close");
        }
        public void reset()
        {
            ULStat = MccDaq.MccService.WinBufFreeEx(MemHandle); // Free up memory for use by
        }

        public void setVoltage(int channel, float setVoltage)
        {
            ushort DataValue = 0;
            MccDaq.Range Range = MccDaq.Range.Bip10Volts;

            ULStat = DaqBoard.FromEngUnits(Range, setVoltage, out DataValue);

            ULStat = DaqBoard.AOut(channel, Range, DataValue);
        }
        public void setVoltageScan(int channel, double[] dataVolts, int numElements)
        {
            MemHandle = MccDaq.MccService.ScaledWinBufAllocEx(numElements);

            ULStat = MccDaq.MccService.ScaledWinArrayToBuf(dataVolts, MemHandle, 0, numElements);

            MccDaq.ScanOptions Options;

            //  Parameters:
            //    LowChan    :the lower channel of the scan
            //    HighChan   :the upper channel of the scan
            //    Count      :the number of D/A values to send
            //    Rate       :per channel sampling rate ((samples per second) per channel)
            //    DAData     :array of values to send to the scanned channels
            //    Options    :data send options
            int Count = numElements;
            int LowChan = 1;    // First analog output channel
            int HighChan = 1;   // Last analog output channel
            int Rate = 1000;    // Rate of data update (ignored if board does not support timed analog output)
            MccDaq.Range Gain = MccDaq.Range.Bip10Volts; // Ignored if gain is not programmable
            Options = MccDaq.ScanOptions.Background | MccDaq.ScanOptions.ScaleData;


            ULStat = DaqBoard.AOutScan(LowChan, HighChan, Count, ref Rate, Gain, MemHandle, Options);

        }
        public double getVoltage(int channel)
        {
            short dataValue;
            float DataValueF;
            MccDaq.Range Range = MccDaq.Range.Bip10Volts;

            ULStat = DaqBoard.AIn(channel, Range, out dataValue);
            ULStat = DaqBoard.ToEngUnits(Range, dataValue, out DataValueF);

            return DataValueF;
        }

        public float[] getVoltageScan(int channel, int numElements)
        {

            MccDaq.Range Range = MccDaq.Range.Bip10Volts;
            int i;
            MccDaq.ErrorInfo ULStat;
            MccDaq.ScanOptions Options;
            int Rate;

            //  per channel sampling rate ((samples per second) per channel)
            Rate = 1000;

            //  return data as 12-bit values (ignored for 16-bit boards)
            Options = MccDaq.ScanOptions.ConvertData;

            MemHandle = MccDaq.MccService.WinBufAllocEx(numElements);

            ULStat = DaqBoard.AInScan(channel, channel, numElements, ref Rate, Range, MemHandle, Options);

            ushort[] ADData = new ushort[numElements];
            ULStat = MccDaq.MccService.WinBufToArray(MemHandle, ADData, 0, numElements);

            float[] ADDataF = new float[numElements];
            for (i = 0; i < numElements; i++)
            {
                ULStat = DaqBoard.ToEngUnits(Range, (short)ADData[i], out ADDataF[i]);
            }

            return ADDataF;

        }
        public void abortVoltageScan()
        {
            ULStat = DaqBoard.StopBackground(MccDaq.FunctionType.AoFunction);
            Trace.TraceWarning("MccDaq.MccBoard Out Scan");
        }
        public bool isRunningVoltageScan()
        {
            int CurIndex;
            int CurCount;
            short Status;

            ULStat = DaqBoard.GetStatus(out Status, out CurCount, out CurIndex, MccDaq.FunctionType.AoFunction);

            return Status == MccDaq.MccBoard.Running;
        }
    }

    public class clsErrorDefs
    {
        public static MccDaq.ErrorReporting ReportError;
        public static MccDaq.ErrorHandling HandleError;
        public static bool GeneralError;

        public static void DisplayError(MccDaq.ErrorInfo ErrCode)
        {
            System.Windows.Forms.MessageBox.Show
                ("Cannot run sample program. Error reported: " +
                ErrCode.Message, "Unexpected Universal Library Error",
                System.Windows.Forms.MessageBoxButtons.OK);
            GeneralError = true;
        }

    }
}
