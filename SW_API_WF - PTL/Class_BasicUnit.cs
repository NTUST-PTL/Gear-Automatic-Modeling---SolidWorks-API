using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using System.Globalization;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace WindowsFormsApplication1
{
    #region 點、線、面 類別
    public class point
    {
        public double X, Y, Z;
        public double vectorX, vectorY, vectorZ;

        public point() { }
        public point(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public point MoveAndRotateZ(double dX, double dY, double dZ, double Phi)
        {
            point newDot = new point();
            newDot.X = (this.X + dX) * Math.Cos(Phi) + (this.Y + dY) * Math.Sin(Phi);
            newDot.Y = (this.X + dX) * (-Math.Sin(Phi)) + (this.Y + dY) * Math.Cos(Phi);
            newDot.vectorX = this.vectorX * Math.Cos(Phi) + this.vectorY * Math.Sin(Phi);
            newDot.vectorY = this.vectorX * (-Math.Sin(Phi)) + this.vectorY * Math.Cos(Phi);

            return newDot;
        }
        public point RotateZAndMove(double dX, double dY, double dZ, double Phi)
        {
            point newDot = new point();
            newDot.X = this.X * Math.Cos(Phi) + this.Y * Math.Sin(Phi) + dX;
            newDot.Y = this.X * (-Math.Sin(Phi)) + this.Y * Math.Cos(Phi) + dY;
            newDot.Z = this.Z + dZ;
            newDot.vectorX = this.vectorX * Math.Cos(Phi) + this.vectorY * Math.Sin(Phi);
            newDot.vectorY = this.vectorX * (-Math.Sin(Phi)) + this.vectorY * Math.Cos(Phi);

            return newDot;
        }
        public point Copy()
        {
            point newDot = new point(X, Y, Z);
            newDot.vectorX = vectorX;
            newDot.vectorY = vectorY;
            newDot.vectorZ = vectorZ;
            return newDot;
        }
    }

    public class Line
    {
        public point[] points;

        public Line() { }
        public Line(int dotNumberOfLine)
        {
            points = new point[dotNumberOfLine];
            for (int i = 0; i < dotNumberOfLine; i++)
            {
                points[i] = new point();
            }
        }

        public Line MoveAndRotateZ(double dX, double dY, double dZ, double Phi)
        {
            Line newLine = new Line(points.Length);
            for (int i = 0; i < points.Length; i++)
            {
                newLine.points[i] = points[i].MoveAndRotateZ(dX, dY, dZ, Phi);
            }
            return newLine;
        }
        public Line RotateZAndMove(double dX, double dY, double dZ, double Phi)
        {
            Line newLine = new Line(points.Length);
            for (int i = 0; i < points.Length; i++)
            {
                newLine.points[i] = points[i].RotateZAndMove(dX, dY, dZ, Phi);
            }
            return newLine;
        }
        public Line Copy()
        {
            Line newLine = new Line(points.Length);
            for (int i = 0; i < points.Length; i++)
                newLine.points[i] = points[i].Copy();
            return newLine;
        }
    }

    public class Face
    {
        public Line[] lines;

        public Face() { }
        public Face(int lineNumOfFace, int dotNumOfLine)
        {
            lines = new Line[lineNumOfFace];
            for (int i = 0; i < lineNumOfFace; i++)
            {
                lines[i] = new Line(dotNumOfLine);
            }
        }

        public Face MoveAndRotateZ(double dX, double dY, double dZ, double Phi)
        {
            Face newFace = new Face(lines.Length, lines[0].points.Length);
            for (int i = 0; i < lines.Length; i++)
            {
                newFace.lines[i] = lines[i].MoveAndRotateZ(dX, dY, dZ, Phi);
            }
            return newFace;
        }

        public Face RotateZAndMove(double dX, double dY, double dZ, double Phi)
        {
            Face newFace = new Face(lines.Length, lines[0].points.Length);
            for (int i = 0; i < lines.Length; i++)
            {
                newFace.lines[i] = lines[i].RotateZAndMove(dX, dY, dZ, Phi);
            }
            return newFace;
        }

        public void SovleNormalVector()
        {
            double a = 0, b = 0, c = 0, m = 0, n = 0, l = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].points.Length; j++)
                {
                    if (i == lines.Length - 1)
                    {
                        if (j == lines[i].points.Length - 1)
                        {
                            a = lines[i].points[j].X - lines[i - 1].points[j].X;
                            b = lines[i].points[j].Y - lines[i - 1].points[j].Y;
                            c = lines[i].points[j].Z - lines[i - 1].points[j].Z;
                            m = lines[i].points[j].X - lines[i].points[j - 1].X;
                            n = lines[i].points[j].Y - lines[i].points[j - 1].Y;
                            l = lines[i].points[j].Z - lines[i].points[j - 1].Z;
                        }
                        else
                        {
                            a = lines[i].points[j].X - lines[i - 1].points[j].X;
                            b = lines[i].points[j].Y - lines[i - 1].points[j].Y;
                            c = lines[i].points[j].Z - lines[i - 1].points[j].Z;
                            m = lines[i].points[j + 1].X - lines[i].points[j].X;
                            n = lines[i].points[j + 1].Y - lines[i].points[j].Y;
                            l = lines[i].points[j + 1].Z - lines[i].points[j].Z;
                        }
                    }
                    else
                    {
                        if (j == lines[i].points.Length - 1)
                        {
                            a = lines[i + 1].points[j].X - lines[i].points[j].X;
                            b = lines[i + 1].points[j].Y - lines[i].points[j].Y;
                            c = lines[i + 1].points[j].Z - lines[i].points[j].Z;
                            m = lines[i].points[j].X - lines[i].points[j - 1].X;
                            n = lines[i].points[j].Y - lines[i].points[j - 1].Y;
                            l = lines[i].points[j].Z - lines[i].points[j - 1].Z;
                        }
                        else
                        {
                            a = lines[i + 1].points[j].X - lines[i].points[j].X;
                            b = lines[i + 1].points[j].Y - lines[i].points[j].Y;
                            c = lines[i + 1].points[j].Z - lines[i].points[j].Z;
                            m = lines[i].points[j + 1].X - lines[i].points[j].X;
                            n = lines[i].points[j + 1].Y - lines[i].points[j].Y;
                            l = lines[i].points[j + 1].Z - lines[i].points[j].Z;
                        }
                    }
                    lines[i].points[j].vectorX = b * l - n * c;
                    lines[i].points[j].vectorX = b * l - n * c;
                    lines[i].points[j].vectorX = b * l - n * c;
                }
            }
        }

        public Face Copy()
        {
            Face newFace = new Face(lines.Length, lines[0].points.Length);
            for (int i = 0; i < lines.Length; i++)
                newFace.lines[i] = lines[i].Copy();
            return newFace;
        }
    }
    #endregion

    #region 齒條刀型、齒條刀軌跡 類別
    public class PassOfCutlery
    {
        public GearShape_2D[] PositionOfCutlery;

        public PassOfCutlery() { }
        public PassOfCutlery(int numOfPosition, int lineNumOfRack)
        {
            PositionOfCutlery = new GearShape_2D[numOfPosition];
            for (int i = 0; i < numOfPosition; i++)
            {
                PositionOfCutlery[i] = new GearShape_2D(lineNumOfRack);
            }
        }
    }
    #endregion

    #region 齒面 類別
    public class GearShape_1Face
    {
        public Face face;

        public GearShape_1Face() { }
        public GearShape_1Face(int lineNumOfFace, int dotNumOfLine)
        {
            face = new Face(lineNumOfFace, dotNumOfLine);
        }
    }

    public class GearShape_5Face
    {
        public Face[] faces;
        public Face face_外齒輪齒頂;
        public GearShape_5Face()
        {
            faces = new Face[5];
            for (int i = 0; i < 5; i++)
            {
                faces[i] = new Face();
            }
            face_外齒輪齒頂 = new Face();
        }
        public GearShape_5Face(int lineNumOfOneFace, int dotNumOfLine1, int dotNumOfLine2, int dotNumOfLine3, int dotNumOfLine4, int dotNumOfLine5)
        {
            faces = new Face[5];
            faces[0] = new Face(lineNumOfOneFace, dotNumOfLine1);
            faces[1] = new Face(lineNumOfOneFace, dotNumOfLine2);
            faces[2] = new Face(lineNumOfOneFace, dotNumOfLine3);
            faces[3] = new Face(lineNumOfOneFace, dotNumOfLine4);
            faces[4] = new Face(lineNumOfOneFace, dotNumOfLine5);
            face_外齒輪齒頂 = new Face(lineNumOfOneFace, 2);
        }
        public void TXT_Output()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = "齒輪端面齒型點資料";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = saveFileDialog1.FileName;
                FileInfo fileInfo = new FileInfo(path);
                StreamWriter sw = fileInfo.AppendText();

                for (int i = 0; i < faces.Length; i++)
                {
                    sw.WriteLine("Line, " + (i + 1) + ", " + faces[i].lines[0].points.Length);
                    for (int j = 0; j < faces[i].lines[0].points.Length; j++)
                    {
                        sw.Write(faces[i].lines[0].points[j].X.ToString("E6", CultureInfo.InvariantCulture));
                        sw.Write(", ");
                        sw.Write(faces[i].lines[0].points[j].Y.ToString("E6", CultureInfo.InvariantCulture));
                        sw.WriteLine();
                    }
                }

                sw.Flush();
                sw.Close();
            }
        }
    }

    public class GearShape_3D
    {
        public point[] BlankPoint;
        public Face[] faces;

        public GearShape_3D() { }
        public GearShape_3D(int BlankPointNum, int faceNum)
        {
            BlankPoint = new point[BlankPointNum];
            faces = new Face[faceNum];
        }
    }

    public class GearShape_2D
    {
        public Line[] lines;

        public GearShape_2D()
        {
        }
        public GearShape_2D(int lineNumOfGearShape)
        {
            lines = new Line[lineNumOfGearShape];
            for (int i = 0; i < lineNumOfGearShape; i++)
            {
                lines[i] = new Line();
            }
        }

        public GearShape_2D MoveAndRotateZ(double dX, double dY, double dZ, double Phi)
        {
            GearShape_2D newGS_2D = Copy();
            for (int i = 0; i < this.lines.Length; i++)
            {
                newGS_2D.lines[i] = this.lines[i].MoveAndRotateZ(dX, dY, dZ, Phi);
            }
            return newGS_2D;
        }
        public GearShape_2D RotateZAndMove(double dX, double dY, double dZ, double Phi)
        {
            GearShape_2D newGS_2D = Copy();
            for (int i = 0; i < this.lines.Length; i++)
            {
                newGS_2D.lines[i] = this.lines[i].RotateZAndMove(dX, dY, dZ, Phi);
            }
            return newGS_2D;
        }
        public GearShape_2D Copy()
        {
            GearShape_2D newGS_2D = new GearShape_2D();

            newGS_2D.lines = new Line[this.lines.Length];
            for (int i = 0; i < this.lines.Length; i++)
                newGS_2D.lines[i] = this.lines[i].Copy();
            return newGS_2D;
        }
    }
    #endregion

    #region SolidWorks零件檔 類別
    public class IModelDoc2_B9903003
    {
        public PTL.SolidWorks.SolidWorksAppAdapter swAppAdapter = new PTL.SolidWorks.SolidWorksAppAdapter();
        public ISldWorks swApp;
        public IModelDoc2 modDoc;
        private int curveNum = 0;
        public int CurveNum
        {
            get { return curveNum; }
        }

        public IModelDoc2_B9903003()
        {
            #region 建立新零件檔
            swApp = swAppAdapter.SWApp;
            modDoc = swAppAdapter.CreatePart();
            #endregion 建立新零件檔
        }

        public bool SelectByID2_ENG_ZHTW(string Name_ENG, string Name_ZHTW, string Type, double X, double Y, double Z, bool Append, int Mark, Callout Callout, int SelectOption)
        {
            bool selected = false;
            if (!modDoc.Extension.SelectByID2(Name_ENG, Type, X, Y, Z, Append, Mark, Callout, SelectOption))
            {
                if (!modDoc.Extension.SelectByID2(Name_ZHTW, Type, X, Y, Z, Append, Mark, Callout, SelectOption))
                {
                    selected = false;
                    //MessageBox.Show("not Selected：" + Name_ENG + "(" + Name_ZHTW + ")");
                }
            }
            else
                selected = true;
            return selected;
        }

        public bool SelectByID2_ENG_ZHTW(string Name_ENG, string Name_ZHTW, string Type, WindowsFormsApplication1.point P, bool Append, int Mark, Callout Callout, int SelectOption)
        {
            bool selected = false;
            if (!modDoc.Extension.SelectByID2(Name_ENG, Type, P.X, P.Y, P.Z, Append, Mark, Callout, SelectOption))
            {
                if (!modDoc.Extension.SelectByID2(Name_ZHTW, Type, P.X, P.Y, P.Z, Append, Mark, Callout, SelectOption))
                {
                    selected = false;
                    //MessageBox.Show("not Selected：" + Name_ENG + "(" + Name_ZHTW + ")");
                }
            }
            else
                selected = true;
            return selected;
        }

        public void BuildLine(point P1, point P2)
        {
            modDoc.CreateLine2(
                P1.X, P1.Y, P1.Z,
                P2.X, P2.Y, P2.Z
                );
        }

        public void BuildCurve(Line line)
        {
            modDoc.InsertCurveFileBegin();
            for (int j = 0; j < line.points.Length; j++)
            {
                modDoc.InsertCurveFilePoint(line.points[j].X,
                                            line.points[j].Y,
                                            line.points[j].Z);
            }
            modDoc.InsertCurveFileEnd();

            curveNum++;
        }

        public void BuildCurve(Line line1, Line line2)
        {
            modDoc.InsertCurveFileBegin();
            for (int j = 0; j < line1.points.Length; j++)
            {
                modDoc.InsertCurveFilePoint(line1.points[j].X,
                                            line1.points[j].Y,
                                            line1.points[j].Z);
            }
            for (int j = line1.points.Length - 1; j >= 0; j--)
            {
                modDoc.InsertCurveFilePoint(line2.points[j].X,
                                            line2.points[j].Y,
                                            line2.points[j].Z);
            }
            modDoc.InsertCurveFileEnd();

            this.curveNum++;
        }

        public void BuildFace(Face face)
        {
            for (int i = 0; i < face.lines.Length; i++)
            {
                BuildCurve(face.lines[i]);
            }
            modDoc.ClearSelection2(true);
            int j = 0;
            for (int i = (this.curveNum - face.lines.Length + 1); i <= this.curveNum; i++)
            {
                SelectByID2_ENG_ZHTW("Curve" + i, "曲線" + i, "REFERENCECURVES", face.lines[j].points[0], true, 0, null, 0);
                j++;
            }
            modDoc.InsertLoftRefSurface2(false, true, false, 1, 6, 6);
        }

        public void BuildFace(Face face1, Face face2)
        {
            for (int i = 0; i < face1.lines.Length; i++)
            {
                BuildCurve(face1.lines[i], face2.lines[i]);
            }
            modDoc.ClearSelection2(true);
            int j = 0;
            for (int i = (this.curveNum - face1.lines.Length + 1); i <= this.curveNum; i++)
            {
                SelectByID2_ENG_ZHTW("Curve" + i, "曲線" + i, "REFERENCECURVES", face1.lines[j].points[0], true, 0, null, 0);
                j++;
            }
            modDoc.InsertLoftRefSurface2(false, true, false, 1, 6, 6);
        }

        public void BuildFace_Transpose(Face face)
        {
            Face face_Transpose = new Face(face.lines[0].points.Length, face.lines.Length);

            for (int i = 0; i < face.lines.Length; i++)
            {
                for (int j = 0; j < face.lines[0].points.Length; j++)
                {
                    face_Transpose.lines[j].points[i] = face.lines[i].points[j].Copy();
                }
            }

            BuildFace(face_Transpose);
        }

        public void CreatePart(ReadFile GetPoint)
        {
            #region 限制條件關閉
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsPoints, true);//設定限制條件
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsPerpendicular, false);
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsParallel, false);
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsHVLines, false);
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsHVPoints, false);
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsLength, false);
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsAngle, false);
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsCenterPoints, false);
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsMidPoints, false);
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsQuadrantPoints, false);
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsIntersections, false);
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsNearest, true);//
            #endregion

            #region 建立基準軸
            SelectByID2_ENG_ZHTW("Top Plane", "上基準面", "PLANE", 0, 0, 0, true, 0, null, 0);
            SelectByID2_ENG_ZHTW("Front Plane", "前基準面", "PLANE", 0, 0, 0, true, 0, null, 0);
            modDoc.InsertAxis2(true);
            #endregion

            #region 建立齒胚

            modDoc.ClearSelection2(true);
            SelectByID2_ENG_ZHTW("Top Plane", "上基準面", "PLANE", 0, 0, 0, true, 0, null, 0);
            modDoc.InsertSketch2(true);//執行草圖繪製-開始
            modDoc.ClearSelection2(true);

            for (int i = 0; i < GetPoint.BlankPointNumber - 1; i++)
            {
                modDoc.SketchManager.CreateLine(GetPoint.BlankPoint[i].x,
                                                GetPoint.BlankPoint[i].y,
                                                GetPoint.BlankPoint[i].z,
                                                GetPoint.BlankPoint[i + 1].x,
                                                GetPoint.BlankPoint[i + 1].y,
                                                GetPoint.BlankPoint[i + 1].z);
            }
            modDoc.SketchManager.CreateLine(GetPoint.BlankPoint[0].x,
                                            GetPoint.BlankPoint[0].y,
                                            GetPoint.BlankPoint[0].z,
                                            GetPoint.BlankPoint[GetPoint.BlankPointNumber - 1].x,
                                            GetPoint.BlankPoint[GetPoint.BlankPointNumber - 1].y,
                                            GetPoint.BlankPoint[GetPoint.BlankPointNumber - 1].z);

            modDoc.SetPickMode();
            modDoc.ClearSelection2(true);
            modDoc.InsertSketch2(true);//執行草圖繪製-結束

            FeatureManager featMan = modDoc.FeatureManager;
            SelectByID2_ENG_ZHTW("Sketch1", "草圖1", "SKETCH", 0, 0, 0, false, 0, null, 0);
            SelectByID2_ENG_ZHTW("Axis1", "基準軸1", "AXIS", 0, 0, 0, true, 16, null, 0);
            featMan.FeatureRevolve2(true, true, false, false, false, false, 0, 0, 6.2831853071796, 0, false, false, 0.01, 0.01, 0, 0, 0, true, true, true);

            #endregion

            #region point to curve 繪製齒形曲面
            //疊層拉伸
            BuildFace(GetPoint.faces[0]);
            BuildFace(GetPoint.faces[1]);
            BuildFace(GetPoint.faces[2]);
            BuildFace(GetPoint.faces[3]);
            BuildFace(GetPoint.faces[4]);
            //縫織曲面
            modDoc.ClearSelection2(true);
            for (int i = 1; i <= 5; i++)
            {
                SelectByID2_ENG_ZHTW("Surface-Loft" + i, "曲面-疊層拉伸" + i, "SURFACEBODY", 0, 0, 0, true, 1, null, 0);
            }
            modDoc.FeatureManager.InsertSewRefSurface(true, false, false, 0.0000025, 0.0001);

            #endregion point to curve

            #region 基準軸、除料、環狀排列

            //除料
            modDoc.ClearSelection2(true);
            SelectByID2_ENG_ZHTW("Surface-Knit1", "曲面-縫織1", "SURFACEBODY", 0, 0, 0, true, 0, null, 0);
            modDoc.InsertCutSurface(false, 0);

            //環狀排列
            modDoc.ClearSelection2(true);
            SelectByID2_ENG_ZHTW("SurfaceCut1", "使用曲面除料1", "BODYFEATURE", 0, 0, 0, true, 4, null, 0);
            SelectByID2_ENG_ZHTW("Axis1", "基準軸1", "AXIS", 0, 0, 0, true, 1, null, 0);
            if (modDoc.FeatureManager.FeatureCircularPattern3(GetPoint.ToothNumber, 6.2831853071796, false, "NULL", false, true) == null)
            {
                modDoc.EditUndo2(1);
                //除料
                modDoc.ClearSelection2(true);
                SelectByID2_ENG_ZHTW("Surface-Knit1", "曲面-縫織1", "SURFACEBODY", 0, 0, 0, true, 0, null, 0);
                modDoc.InsertCutSurface(true, 0);

                //環狀排列
                modDoc.ClearSelection2(true);
                SelectByID2_ENG_ZHTW("SurfaceCut2", "使用曲面除料2", "BODYFEATURE", 0, 0, 0, true, 4, null, 0);
                SelectByID2_ENG_ZHTW("Axis1", "基準軸1", "AXIS", 0, 0, 0, true, 1, null, 0);
                modDoc.FeatureManager.FeatureCircularPattern3(GetPoint.ToothNumber, 6.2831853071796, false, "NULL", false, true);
            }

            #endregion

            #region 隱藏曲線、曲面
            SelectByID2_ENG_ZHTW("Surface-Knit1", "曲面-縫織1", "SURFACEBODY", 0, 0, 0, false, 0, null, 0);
            modDoc.FeatureManager.HideBodies();
            for (int i = 1; i <= GetPoint.CurveNumber[0] * 5; i++)
            {
                SelectByID2_ENG_ZHTW("Curve" + i.ToString(), "曲線" + i.ToString(), "REFERENCECURVES", 0, 0, 0, false, 0, null, 0);
                modDoc.BlankRefGeom();
            }
            #endregion

            #region 限制條件開啟
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsPoints, true);//設定限制條件
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsPerpendicular, true);
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsParallel, true);
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsHVLines, true);
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsHVPoints, true);
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsLength, true);
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsAngle, true);
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsCenterPoints, true);
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsMidPoints, true);
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsQuadrantPoints, true);
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsIntersections, true);
            swAppAdapter.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchSnapsNearest, true);//
            #endregion
        }
    }
    #endregion

    #region 齒輪屬性 類別
    public class GearParameterDouble
    {
        public string itemName;
        public string itemUnit;
        public double itemValue;

        public GearParameterDouble()
        {
            itemName = "";
            itemUnit = "";
            itemValue = 0.0;
        }

        public GearParameterDouble(string name, string unit)
        {
            itemName = name;
            itemUnit = unit;
            itemValue = 0.0;
        }

        public GearParameterDouble(string name, string unit, double Value)
        {
            itemName = name;
            itemUnit = unit;
            itemValue = Value;
        }

        public string ToString2(int dn)
        {
            string st = "           ";
            st += itemName + "  ";
            for (int i = 0; i < 20 - itemName.Length; i++)
            {
                st += "--";
            }
            st += "\t" + Math.Round(itemValue, dn) + "\t\t\t\t" + itemUnit;
            return st;
        }

        public string ToString_RadToDegree(int dn)
        {
            string st = "           ";
            st += itemName + "  ";
            for (int i = 0; i < 20 - itemName.Length; i++)
            {
                st += "--";
            }
            st += "\t" + Math.Round(itemValue / Math.PI * 180.0, dn) + "\t\t\t\t" + itemUnit;
            return st;
        }

        public string ValueToString(int dn)
        {
            string str;
            str = Math.Round(itemValue, dn).ToString();
            return str;
        }
    }

    public class GearParameterDouble2
    {
        public string itemName;
        public string itemUnit;
        public double[] itemValue;

        public GearParameterDouble2()
        {
            itemName = "";
            itemUnit = "";
            itemValue = new double[2] { 0.0, 0.0 };
        }

        public GearParameterDouble2(string name, string unit)
        {
            itemName = name;
            itemUnit = unit;
            itemValue = new double[2] { 0.0, 0.0 };
        }

        public string ToString2(int dn)
        {
            string st = "           ";
            st += itemName + "  ";
            for (int i = 0; i < 20 - itemName.Length; i++)
            {
                st += "--";
            }
            st += "\t" + Math.Round(itemValue[0], dn) + "\t\t" + Math.Round(itemValue[1], dn) + "\t\t" + itemUnit;
            return st;
        }

        public string ToString_RadToDegree(int dn)
        {
            string st = "           ";
            st += itemName + "  ";
            for (int i = 0; i < 20 - itemName.Length; i++)
            {
                st += "--";
            }
            st += "\t" + Math.Round(itemValue[0] / Math.PI * 180.0, dn) + "\t\t" + Math.Round(itemValue[0] / Math.PI * 180.0, dn) + "\t\t" + itemUnit;
            return st;
        }

        public string ValueToString(int i, int dn)
        {
            string str;
            str = Math.Round(itemValue[i], dn).ToString();
            return str;
        }
    }

    public class GearParameterInt
    {
        public string itemName;
        public string itemUnit;
        public int itemValue;

        public GearParameterInt()
        {
            itemName = "";
            itemUnit = "";
            itemValue = 0;
        }

        public GearParameterInt(string name, string unit)
        {
            itemName = name;
            itemUnit = unit;
            itemValue = 0;
        }

        public GearParameterInt(string name, string unit, int Value)
        {
            itemName = name;
            itemUnit = unit;
            itemValue = Value;
        }

        public string ToString2()
        {
            string st = "           ";
            st += itemName + "  ";
            for (int i = 0; i < 20 - itemName.Length; i++)
            {
                st += "--";
            }
            st += "\t" + itemValue + "\t\t\t\t" + itemUnit;
            return st;
        }

        public string ValueToString()
        {
            string str;
            str = itemValue.ToString();
            return str;
        }
    }

    public class GearParameterInt2
    {
        public string itemName;
        public string itemUnit;
        public int[] itemValue;

        public GearParameterInt2()
        {
            itemName = "";
            itemUnit = "";
            itemValue = new int[2] { 0, 0 };
        }

        public GearParameterInt2(string name, string unit)
        {
            itemName = name;
            itemUnit = unit;
            itemValue = new int[2] { 0, 0 };
        }

        public string ToString2()
        {
            string st = "           ";
            st += itemName + "  ";
            for (int i = 0; i < 20 - itemName.Length; i++)
            {
                st += "--";
            }
            st += "\t" + itemValue[0] + "\t\t" + itemValue[1] + "\t\t" + itemUnit;
            return st;
        }

        public string ValueToString(int i)
        {
            string str;
            str = itemValue[i].ToString();
            return str;
        }
    }

    public class GearParameterBool
    {
        public string itemName;
        public bool itemValue;

        public GearParameterBool()
        {
            itemName = "";
            itemValue = false;
        }

        public GearParameterBool(string name)
        {
            itemName = name;
            itemValue = false;
        }

        public GearParameterBool(string name, bool Value)
        {
            itemName = name;
            itemValue = Value;
        }

        public string ToString2()
        {
            string st = "           ";
            st += itemName + "  ";
            for (int i = 0; i < 20 - itemName.Length; i++)
            {
                st += "--";
            }
            st += "\t" + itemValue;
            return st;
        }

        public string ValueToString()
        {
            string str;
            str = itemValue.ToString();
            return str;
        }
    }

    public class GearParameterBool2
    {
        public string itemName;
        public bool[] itemValue;

        public GearParameterBool2()
        {
            itemName = "";
            itemValue = new bool[] { false, false };
        }

        public GearParameterBool2(string name)
        {
            itemName = name;
            itemValue = new bool[] { false, false };
        }

        public string ToString2()
        {
            string st = "           ";
            st += itemName + "  ";
            for (int i = 0; i < 20 - itemName.Length; i++)
            {
                st += "--";
            }
            st += "\t" + itemValue[0] + "\t\t" + itemValue[1];
            return st;
        }

        public string ValueToString(int i)
        {
            string str;
            str = itemValue[i].ToString();
            return str;
        }
    }
    #endregion

    #region 齒輪參數清單
    public class HelicalGearParameterList
    {
        //基本齒條刀法向齒形
        public GearParameterDouble 齒條刀法向節距 = new GearParameterDouble("齒條刀法向節距", "mm");
        public GearParameterDouble2 齒條刀齒冠高 = new GearParameterDouble2("齒條刀齒冠高", "mm");
        public GearParameterDouble2 齒條刀齒根高 = new GearParameterDouble2("齒條刀齒根高", "mm");
        public GearParameterDouble 齒條刀法向壓力角 = new GearParameterDouble("齒條刀法向壓力角", "mm");
        public GearParameterDouble2 齒條刀法向節齒厚 = new GearParameterDouble2("齒條刀法向節齒厚", "mm");
        public GearParameterDouble 齒條刀法向齒頂圓角半徑 = new GearParameterDouble("齒條刀法向齒頂圓角半徑", "mm");
        public GearParameterDouble2 齒條刀齒厚修正 = new GearParameterDouble2("齒條刀齒厚修正", "%");

        //漸 開 線 齒 直 角 螺 旋 齒 輪 數 據，齒輪Input參數
        public GearParameterDouble 齒直角模數 = new GearParameterDouble("齒直角模數", "mm /齒");
        public GearParameterInt2 齒數 = new GearParameterInt2("齒數", "齒");
        public GearParameterDouble 齒直角基準壓力角 = new GearParameterDouble("齒直角基準壓力角", "度");
        public GearParameterDouble 螺旋角 = new GearParameterDouble("螺旋角", "度");
        public GearParameterBool2 右旋 = new GearParameterBool2("右旋");
        public GearParameterDouble2 齒直角轉位係數 = new GearParameterDouble2("轉位係數", "");
        public GearParameterDouble2 齒面寬 = new GearParameterDouble2("齒面寬", "mm");
        public GearParameterBool2 內齒輪 = new GearParameterBool2("內齒輪");

        #region 外齒輪
        //漸 開 線 齒 直 角 螺 旋 齒 輪 數 據，計算
        public GearParameterDouble2 標準節圓直徑 = new GearParameterDouble2("標準節圓直徑", "mm");
        public GearParameterDouble2 基圓直徑 = new GearParameterDouble2("基圓直徑", "mm");
        public GearParameterDouble2 齒冠高 = new GearParameterDouble2("齒冠高", "mm");
        public GearParameterDouble2 全齒深 = new GearParameterDouble2("全齒深", "mm");
        public GearParameterDouble2 齒頂圓直徑 = new GearParameterDouble2("齒頂圓直徑", "mm");
        public GearParameterDouble2 齒底圓直徑 = new GearParameterDouble2("齒底圓直徑", "mm");
        public GearParameterDouble2 跨齒厚 = new GearParameterDouble2("跨齒厚", "mm");
        public GearParameterInt2 跨齒數 = new GearParameterInt2("跨齒數", "齒");
        public GearParameterDouble2 銷球直徑 = new GearParameterDouble2("理想銷球直徑", "mm");
        public GearParameterDouble2 跨銷球尺寸 = new GearParameterDouble2("跨銷球尺寸", "mm");

        //外 齒 輪 對 組 裝 數 據
        public GearParameterDouble 中心距離 = new GearParameterDouble("中心距離", "mm");
        public GearParameterDouble 中心距離修正係數 = new GearParameterDouble("中心距離修正係數", " ");
        public GearParameterDouble2 咬合節圓直徑 = new GearParameterDouble2("咬合節圓直徑", "mm");
        public GearParameterDouble 軸直角壓力角 = new GearParameterDouble("軸直角壓力角", "度");
        public GearParameterDouble 軸直角漸開線函數 = new GearParameterDouble("軸直角咬合壓力角的漸開線函數", "");
        public GearParameterDouble 軸直角咬合壓力角 = new GearParameterDouble("軸直角咬合壓力角", "度");
        #endregion 外齒輪

        #region 內齒輪
        ////漸 開 線 齒 直 角 螺 旋 齒 輪 數 據，計算


        ////內 外 齒 輪 對 組 裝 數 據
        //public GearParameterDouble 內外齒輪中心距離 = new GearParameterDouble("中心距離", "mm");
        //public GearParameterDouble 內外齒輪中心距離修正係數 = new GearParameterDouble("中心距離修正係數", " ");
        //public GearParameterDouble2 內外齒輪咬合節圓直徑 = new GearParameterDouble2("咬合節圓直徑", "mm");
        //public GearParameterDouble 內外齒輪軸直角咬合壓力角 = new GearParameterDouble("軸直角咬合壓力角", "度");

        #endregion 內齒輪

        public void ListOutputInTXT()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.FileName = "漸開線齒直角螺旋齒輪齒形數據表-" + 齒數.itemValue[0] + "-" + 齒數.itemValue[1] + "齒";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = saveFileDialog1.FileName;
                FileInfo fileInfo = new FileInfo(path);
                StreamWriter sw = fileInfo.AppendText();

                sw.WriteLine("                                               NTUST");
                sw.WriteLine();
                sw.WriteLine("                             漸 開 線 齒 直 角 螺 旋 齒 輪 齒 形 數 據 表");
                sw.WriteLine();
                sw.WriteLine("           日期 : " + System.DateTime.Now.ToString());
                sw.WriteLine();

                sw.WriteLine("                                                        第一齒輪        第二齒輪");
                sw.WriteLine();
                sw.WriteLine("           ************************** 基 本 齒 條 刀 法 向 齒 形 *************************");
                sw.WriteLine(齒條刀法向節距.ToString2(4));
                sw.WriteLine(齒條刀齒冠高.ToString2(4));
                sw.WriteLine(齒條刀齒根高.ToString2(4));
                sw.WriteLine(齒條刀法向壓力角.ToString2(4));
                sw.WriteLine(齒條刀法向節齒厚.ToString2(4));
                sw.WriteLine(齒條刀法向齒頂圓角半徑.ToString2(4));
                sw.WriteLine(齒條刀齒厚修正.ToString2(4));
                sw.WriteLine();

                sw.WriteLine("           ********************* 漸 開 線 齒 直 角 螺 旋 齒 輪 數 據 *********************");
                sw.WriteLine(齒直角模數.ToString2(4));
                sw.WriteLine(齒數.ToString2());
                sw.WriteLine(齒直角基準壓力角.ToString_RadToDegree(6));
                sw.WriteLine(螺旋角.ToString_RadToDegree(6));
                sw.WriteLine(右旋.ToString2());
                sw.WriteLine(齒直角轉位係數.ToString2(6));
                sw.WriteLine(齒面寬.ToString2(4));
                sw.WriteLine(標準節圓直徑.ToString2(4));
                sw.WriteLine(基圓直徑.ToString2(4));
                sw.WriteLine(齒冠高.ToString2(4));
                sw.WriteLine(全齒深.ToString2(4));
                sw.WriteLine(齒頂圓直徑.ToString2(4));
                sw.WriteLine(齒底圓直徑.ToString2(4));
                sw.WriteLine();
                sw.WriteLine(跨齒厚.ToString2(4));
                sw.WriteLine(跨齒數.ToString2());
                sw.WriteLine(銷球直徑.ToString2(4));
                sw.WriteLine(跨銷球尺寸.ToString2(4));
                sw.WriteLine();

                sw.WriteLine("           ***************************** 齒 輪 對 組 裝 數 據 ****************************");
                sw.WriteLine(中心距離修正係數.ToString2(4));
                sw.WriteLine(中心距離.ToString2(4));
                sw.WriteLine(咬合節圓直徑.ToString2(4));
                sw.WriteLine(軸直角壓力角.ToString_RadToDegree(4));
                sw.WriteLine(軸直角漸開線函數.ToString2(4));
                sw.WriteLine(軸直角咬合壓力角.ToString_RadToDegree(4));

                sw.Flush();
                sw.Close();
            }
        }
    }
    #endregion
}
