using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using PTL.Geometry;
using PTL.SolidWorks;
using PTL.SolidWorks.GearConstruction;
using PTL.SolidWorks.Edit;
using SolidWorks.Interop.sldworks;

namespace SW_API2012
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GearData gearData = APIFileReader.OpenRead();
            if (gearData != null)
                GearCreator.PublishToSolidWorks(gearData);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<PolyLine> curves = SLDCRVReader.OpenRead();
            if (curves != null)
            {
                SolidWorksAppAdapter swApp = new SolidWorksAppAdapter();
                IModelDoc2 modDoc;

                if (swApp.ActiveDoc != null)
                    modDoc = swApp.ActiveDoc;
                else
                    modDoc = swApp.CreatePart();

                int crvNum = 0;
                foreach (var cv in curves)
                    PartEditMethods.AddPolyLineToSolidWorksPart(modDoc, cv, ref crvNum);
            }
        }
    }
}
