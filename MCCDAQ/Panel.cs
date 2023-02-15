using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inst_TESEQ_PS5740;

namespace Inst_TESEQ_PS5740
{
    public partial class Panel: UserControl
    {
        public Inst inst;
        public Panel()
        {
            InitializeComponent();
            this.inst = new Inst();
            this.inst.open();
        }
        public Panel(Inst inst)
        {
            InitializeComponent();
            this.inst = inst;
        }

    }
}
