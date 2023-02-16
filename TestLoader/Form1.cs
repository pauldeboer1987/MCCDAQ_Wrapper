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
            InitializeComponent();

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);
        }

        static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
            string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath)) return null;
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Type instType = null;
            dll = AssemblyLoadContext.Default.LoadFromAssemblyPath(Environment.CurrentDirectory + "\\MCCDAQ_wrapper.dll");
            AssemblyName[] ReferencedAssemblies = dll.GetReferencedAssemblies();
            Type[] types = dll.GetExportedTypes();
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