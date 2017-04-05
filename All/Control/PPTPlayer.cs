using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;

namespace All.Control
{
    public partial class PPTPlayer : UserControl
    {
        int flushSpeed = 5;
        /// <summary>
        /// 切换速度
        /// </summary>
        public int FlushSpeed
        {
            get { return flushSpeed; }
            set { flushSpeed = value; }
        }
        Microsoft.Office.Interop.PowerPoint.Application pptApp = null;
        Microsoft.Office.Interop.PowerPoint.SlideShowTransition pptSST;
        Microsoft.Office.Interop.PowerPoint.SlideRange pptSldRng;
        Presentation pptDoc;
        public PPTPlayer()
        {
            InitializeComponent();
        }
        public void Open(string PPT)
        {
            if (!System.IO.File.Exists(PPT))
            {
                return;
            }
            try
            {
                axFramerControl1.Open(PPT, true, "PowerPoint.Show", "", "");
                pptDoc = (Presentation)axFramerControl1.ActiveDocument;
                pptApp = pptDoc.Application;
                pptDoc.SlideShowSettings.RangeType = PpSlideShowRangeType.ppShowNamedSlideShow;
                pptDoc.SlideShowSettings.ShowType = PpSlideShowType.ppShowTypeWindow;
                pptDoc.SlideShowSettings.LoopUntilStopped = MsoTriState.msoCTrue;
                int Slides = pptDoc.Slides.Count;
                int[] SlideIdx = new int[Slides];
                for (int i = 0; i < SlideIdx.Length; i++)
                {
                    SlideIdx[i] = i + 1;
                }
                pptSldRng = pptDoc.Slides.Range(SlideIdx);
                pptSST = pptSldRng.SlideShowTransition;

                pptSST.AdvanceOnTime = MsoTriState.msoCTrue;
                pptSST.AdvanceTime = flushSpeed;
                pptSST.EntryEffect = PpEntryEffect.ppEffectFade;

                pptDoc.Save();


                SendKeys.Send("{F5}");
            }
            catch (Exception e)
            {
                All.Class.Error.Add(e);
            }
        }
        public void Close()
        {
            pptSST = null;
            pptSldRng = null;
            if (pptApp != null)
            {
                pptApp.Quit();
            }
            pptApp = null;
            if (pptDoc != null)
            {
                pptDoc.Close();
            }
            pptDoc = null;
            GC.Collect();
        }
        ~PPTPlayer()
        {
            Close();
        }
    }
}
