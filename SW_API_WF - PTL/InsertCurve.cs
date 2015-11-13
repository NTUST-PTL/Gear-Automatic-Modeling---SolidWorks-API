using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using PTL.SolidWorks;
using SolidWorks.Interop.sldworks;

namespace WindowsFormsApplication1
{
    public class InsertCurve
    {

        public ReadFile_Curve GetPoint;
        public SolidWorksAppAdapter swApp = new SolidWorksAppAdapter();
        public IModelDoc2 modDoc;

        public InsertCurve()
        {
            GetPoint = new ReadFile_Curve();
            #region 建立新零件檔
            //開啟新零件圖
            if (swApp.ActiveDoc != null)
                modDoc = swApp.ActiveDoc;
            else
                modDoc = swApp.CreatePart();
            
            #endregion 建立新零件檔
        }



        public  void InsertMulCurve()//齒輪
        {
            double x1,y1,z1;

            #region 曲線
            for (int ii = 0; ii < GetPoint.Point.GetLength(0); ii++)
            {
                modDoc.ClearSelection2(true);
                modDoc.InsertSketch2(true);//曲線 側邊
                modDoc.InsertCurveFileBegin();
                for (int jj = 0; jj < GetPoint.Point[ii].GetLength(0); jj++)
                {
                        x1 = GetPoint.Point[ii][jj].x;
                        y1 = GetPoint.Point[ii][jj].y;
                        z1 = GetPoint.Point[ii][jj].z;
                        modDoc.InsertCurveFilePoint(x1, y1, z1);

                }
                modDoc.InsertCurveFileEnd();
            }
            #endregion

        }
    }
}
