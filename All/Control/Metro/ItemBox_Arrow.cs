using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All.Control.Metro
{
    public partial class ItemBox_Arrow : Component
    {
        public ItemBox_Arrow()
        {
            InitializeComponent();
        }

        public ItemBox_Arrow(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
