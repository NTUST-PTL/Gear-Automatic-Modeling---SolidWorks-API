using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace WindowsFormsApplication1
{
    public class ReadFile_Curve
    {
        public Coordinate[][] Point;
        public int[] CurveNumber, PointNumber;
        public ReadFile_Curve()
        {

            #region 讀檔
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Multiselect = true;


            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Point = new Coordinate[openFileDialog1.FileNames.GetLength(0)][];
                for (int zz = 0; zz < openFileDialog1.FileNames.GetLength(0); zz++)
                {
                    System.IO.StreamReader myFile = new System.IO.StreamReader(openFileDialog1.FileNames[zz]);

                    string myString1;
                    string[] myString2;
                    int kk = 0;
                    while ((myString1 = myFile.ReadLine()) != null) kk++;//算行數
                    Point[zz] = new Coordinate[kk];

                    myFile.BaseStream.Seek(0, 0);//回第一行

                    for (int ii = 0; ii < kk; ii++)//存點
                    {
                        myString1 = myFile.ReadLine();
                        myString2 = myString1.Split(new Char[] { ',' });
                        Point[zz][ii] = new Coordinate();
                        Point[zz][ii].x = Convert.ToDouble(myString2[0]) / 1000;
                        Point[zz][ii].y = Convert.ToDouble(myString2[1]) / 1000;
                        Point[zz][ii].z = Convert.ToDouble(myString2[2]) / 1000;
                    }

                    myFile.Close();
                }
            }

            #endregion

        }
 

    }
}
