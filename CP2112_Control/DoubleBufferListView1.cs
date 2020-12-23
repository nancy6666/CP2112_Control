using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CP2112_Control
{
    class DoubleBufferListView1:ListView
    {
        public DoubleBufferListView1()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
            UpdateStyles();
        }
    }
}
