using System.Diagnostics.Metrics;
using System.Reflection;
using System.Runtime.Loader;

namespace TestLoader
{
    public partial class Form1 : Form
    {
        public dynamic instrument;
        Assembly dll;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Type instType = null;
            dll = AssemblyLoadContext.Default.LoadFromAssemblyPath("C:\\repos\\GITHUB\\MCCDAQ_Wrapper\\MCCDAQ\\bin\\Debug\\net6.0-windows\\MCCDAQ_wrapper.dll");
            AssemblyName[] ReferencedAssemblies = dll.GetReferencedAssemblies();
            Type[] types = dll.GetExportedTypes();
            //foreach (Type t in dll.GetExportedTypes())
            //{
            //    if (t.Name == "Inst")
            //    {
            //        instType = t;
            //        break;
            //    }
            //}
            //if (instType == null)
            //    throw new Exception($"Instrument {dll.GetName().Name} must contain type \"Inst\"");

            //instrument = Activator.CreateInstance(instType);
            //instrument.open("0");
            //textBox1.Text = instrument.getVoltage(0).ToString();
            textBox1.Text = "Loaded";
        }
    }
}