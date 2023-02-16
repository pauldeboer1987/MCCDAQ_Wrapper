using System.Diagnostics.Metrics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace TestLoader
{
    public partial class Form1 : Form
    {
        public dynamic instrument;
        Assembly dll;
        public Form1()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);
            InitializeComponent();

        }

        static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
            string folderPath = Path.GetDirectoryName(args.RequestingAssembly.Location);
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath)) return null;
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            dll = Assembly.LoadFile(Environment.CurrentDirectory + "\\MCCDAQ_wrapper.dll");
            Type instType = null;
            foreach (Type t in dll.GetExportedTypes())
            {
                if (t.Name == "Inst")
                {
                    instType = t;
                    break;
                }
            }
            if (instType == null)
                throw new Exception($"Instrument {dll.GetName().Name} must contain type \"Inst\"");

            instrument = Activator.CreateInstance(instType);
            instrument.open("0");
            textBox1.Text = instrument.getVoltage(0).ToString();
        }
    }
}