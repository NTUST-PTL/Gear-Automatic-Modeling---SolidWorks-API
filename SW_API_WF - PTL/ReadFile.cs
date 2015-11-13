using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace WindowsFormsApplication1
{
    public class ReadFile
    {
        public int ToothNumber, BlankPointNumber, SurfaceNumber;//, flankpoint, arcpoint, kk = 0, curvenumber,  surfacenum, extendcurvenumber, extendflankpoint, extendarcpoint;
        public Face[] faces;
        public Coordinate[] BlankPoint;
        public int[] CurveNumber, PointNumber;
        public ReadFile()
        {
            #region 讀檔
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    System.IO.StreamReader myFile = new System.IO.StreamReader(openFileDialog1.FileName);

                    string myString1;
                    string[] myString2;
                    //int toothnumber, flankpoint, arcpoint, kk = 0, curvenumber;
                    //while ((myString1 = myFile.ReadLine()) != null) kk++;//算行數


                    myFile.BaseStream.Seek(0, 0);//回第一行
                    myString1 = myFile.ReadLine();
                    myString2 = myString1.Split(new Char[] { ',' });
                    ToothNumber = Convert.ToInt16(myString2[0]);
                    BlankPointNumber = Convert.ToInt16(myString2[1]);
                    SurfaceNumber = Convert.ToInt16(myString2[2]);

                    BlankPoint = new Coordinate[BlankPointNumber];
                    for (int ii = 0; ii < BlankPointNumber; ii++)//存齒胚點
                    {
                        myString1 = myFile.ReadLine();
                        myString2 = myString1.Split(new Char[] { ',' });
                        BlankPoint[ii] = new Coordinate();
                        BlankPoint[ii].x = Convert.ToDouble(myString2[1]) / 1000;
                        BlankPoint[ii].y = Convert.ToDouble(myString2[2]) / 1000;
                        BlankPoint[ii].z = Convert.ToDouble(myString2[3]) / 1000;
                    }



                    faces = new Face[SurfaceNumber];
                    CurveNumber = new int[SurfaceNumber];
                    PointNumber = new int[SurfaceNumber];
                    for (int ii = 0; ii < SurfaceNumber; ii++)//存拓普點
                    {
                        myString1 = myFile.ReadLine();
                        myString2 = myString1.Split(new Char[] { ',' });

                        CurveNumber[ii] = Convert.ToInt16(myString2[0]);
                        PointNumber[ii] = Convert.ToInt16(myString2[1]);

                        faces[ii] = new Face(CurveNumber[ii], PointNumber[ii]);

                        for (int jj = 0; jj < CurveNumber[ii]; jj++)
                        {
                            faces[ii].lines[jj] = new Line(PointNumber[ii]);

                            for (int kk = 0; kk < PointNumber[ii]; kk++)
                            {
                                myString1 = myFile.ReadLine();
                                myString2 = myString1.Split(new Char[] { ',' });
                                faces[ii].lines[jj].points[kk] = new point();
                                faces[ii].lines[jj].points[kk].X = Convert.ToDouble(myString2[2]) / 1000;
                                faces[ii].lines[jj].points[kk].Y = Convert.ToDouble(myString2[3]) / 1000;
                                faces[ii].lines[jj].points[kk].Z = Convert.ToDouble(myString2[4]) / 1000;
                            }
                        }
                    }
                    myFile.Close();
                }
                #endregion
        }
    }
}
