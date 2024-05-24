using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV;

namespace DialDemo
{
    public class SandPDail
    {
        public double GetValue(Image<Bgr, byte> iPImage)
        {
            // Load the image
            Image<Bgr, byte> image = iPImage;
            Image<Bgr, byte> resizeimage;
            if (image.Width > 1024 && image.Height > 1024)
            {
                resizeimage = image.Resize((image.Width / 4), (image.Height / 4), Inter.Linear);
                image = resizeimage;
            }
            else if (image.Width <= 512 && image.Height <= 512)
            {
                resizeimage = image.Resize((image.Width * 2), (image.Height * 2), Inter.Linear);
                image = resizeimage;
            }
            int shortbounded = (image.Width >= image.Height) ? image.Height / 2 : image.Width / 2; ;

            // 將圖像轉換為灰度圖
            Image<Gray, byte> grayImg = image.Convert<Gray, Byte>();
            CvInvoke.CLAHE(grayImg, 3.0, new Size(2, 2), grayImg);
            try
            {
                CircleF[] circleslinq = CvInvoke.HoughCircles(grayImg, HoughModes.Gradient, 1.0, 1, 90, 90, shortbounded / 32, shortbounded).
                OrderByDescending(c => (shortbounded > c.Radius && c.Center.X + c.Radius < shortbounded * 2 && c.Center.Y + c.Radius < shortbounded * 2
                && c.Center.X - c.Radius > 0 && c.Center.Y - c.Radius > 0, c.Radius)).ToArray();

                CircleF BestCircle = circleslinq[0];
                int CircleX = (int)Math.Round(BestCircle.Center.X);
                int CircleY = (int)Math.Round(BestCircle.Center.Y);
                int CircleRadius = (int)Math.Round(BestCircle.Radius) + 1;
                Rectangle boundingRect = new Rectangle(CircleX - CircleRadius, CircleY - CircleRadius, 2 * CircleRadius, 2 * CircleRadius);
                // 裁切圓形範圍影像
                Image<Bgr, byte> circleRoiImg = image.GetSubRect(boundingRect);
                image.ROI = boundingRect;
                image = GetCircleImg(circleRoiImg);
                grayImg = image.Convert<Gray, byte>();
                Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
                Image<Gray, byte> threshImage = grayImg.ThresholdAdaptive(new Gray(255), AdaptiveThresholdType.MeanC, ThresholdType.Binary, 25, new Gray(5)).Not();
                CvInvoke.MorphologyEx(threshImage, threshImage, MorphOp.Dilate, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar(0));
                CvInvoke.MorphologyEx(threshImage, threshImage, MorphOp.Erode, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar(0));

                threshImage = GetEliminate(threshImage, 2500, 0, true);
                LineSegment2D FinfPointerLine = CvInvoke.HoughLinesP(threshImage, 1, Math.PI / 180, 80, 50, 5).OrderByDescending(x => ((Math.Min(x.P1.X, x.P2.X) > image.Width * 0.1 &&
                Math.Min(x.P1.Y, x.P2.Y) > image.Height * 0.1 && (Math.Max(x.P1.X, x.P2.X) < image.Width * 0.9) && (Math.Max(x.P1.Y, x.P2.Y) < image.Height * 0.8)), x.Length)).First();
                LineSegment2D PointerLine = CheckLongLineDirection(FinfPointerLine, threshImage.Width);
                threshImage = GetEliminate(threshImage, 200, 5, true);                
                threshImage = GetLength(threshImage, 200, 5);
                
                // 找尋所有線段
                LineSegment2D[] lines = CvInvoke.HoughLinesP(threshImage, 1, Math.PI / 180, 5, 5, 8);

                // 建立一個 List 來儲存符合條件的短線條
                List<LineSegment2D> mergedLines = new List<LineSegment2D>();
                List<LineSegment2D> mergedLongLines = new List<LineSegment2D>();
                mergedLines.Add(lines[0]);
                for (int i = 1; i < lines.Length; i++)
                {
                    LineSegment2D prevLine = mergedLines.Last();
                    double prevAngle = Math.Atan2(prevLine.Direction.Y, prevLine.Direction.X);
                    double currAngle = Math.Atan2(lines[i].Direction.Y, lines[i].Direction.X);
                    double angleDiff = Math.Abs(currAngle - prevAngle) * 180 / Math.PI;
                    Point mergedLineCenter = new Point((prevLine.P1.X + prevLine.P2.X) / 2, (prevLine.P1.Y + prevLine.P2.Y) / 2);
                    Point currentLineCenter = new Point((lines[i].P1.X + lines[i].P2.X) / 2, (lines[i].P1.Y + lines[i].P2.Y) / 2);
                    double distance = Math.Sqrt(Math.Pow(currentLineCenter.X - mergedLineCenter.X, 2) + Math.Pow(currentLineCenter.Y - mergedLineCenter.Y, 2));

                    if (angleDiff < 30)// || distance < 15)
                    {
                        double length = (prevLine.Length + lines[i].Length) / 2;
                        double startX = (prevLine.P1.X + lines[i].P1.X) / 2;
                        double startY = (prevLine.P1.Y + lines[i].P1.Y) / 2;
                        double endX = startX + length * Math.Cos(prevAngle);
                        double endY = startY + length * Math.Sin(prevAngle);
                        mergedLines.RemoveAt(mergedLines.Count - 1);
                        mergedLines.Add(new LineSegment2D(new Point((int)startX, (int)startY), new Point((int)endX, (int)endY)));
                    }
                    else
                        mergedLines.Add(lines[i]);
                }
                //在图像上绘制合并后的线条
                for (int i = 0; i < mergedLines.Count; i++)
                {
                    LineSegment2D Drawline;
                    Drawline = ExtendLine(mergedLines[i], image.Size);
                    mergedLongLines.Add(Drawline);                    
                }
                LineSegment2D ExtendPointerLine = mergedLongLines.Last();
                PointF closestPointF = FindPointOnLine(mergedLongLines, ExtendPointerLine, PointerLine);
                Point mostCommonIntersection = new Point((int)closestPointF.X, (int)closestPointF.Y);
                
                // 定義一個距離閾值，這裡假設為100
                int distanceThreshold = 100;

                //過濾線段，將不相交或靠近圓心的線段刪除
                List<LineSegment2D> filteredLines = new List<LineSegment2D>();
                List<LineSegment2D> filteredShortLines = new List<LineSegment2D>();
                for (int i = 0; i < mergedLongLines.Count; i++)
                {
                    if (IsLineNearPoint(mergedLongLines[i], mostCommonIntersection, distanceThreshold) &&
                        PointOntheLine(mergedLines[i], mostCommonIntersection))
                    {
                        filteredLines.Add(mergedLongLines[i]);
                        filteredShortLines.Add(mergedLines[i]);
                    }
                }

                int leftBottomindex = filteredShortLines.Select((line, index) => new { Line = line, Index = index })
                                                        .OrderByDescending(y => Math.Max(y.Line.P1.Y, y.Line.P2.Y))
                                                        .ThenBy(x => Math.Min(x.Line.P1.X, x.Line.P2.X))
                                                        .Where(z => Math.Max(z.Line.P1.X, z.Line.P2.X) < threshImage.Width * 0.25 &&
                                                        Math.Max(z.Line.P1.X, z.Line.P2.X) > threshImage.Width * 0.05 &&
                                                        Math.Min(z.Line.P1.Y, z.Line.P2.Y) > threshImage.Height * 0.7)
                                                        .First().Index;
                
                LineSegment2D leftBottomLine = filteredShortLines[leftBottomindex];

                int rightBottomindex = filteredShortLines.Select((line, index) => new { Line = line, Index = index })
                                    .OrderByDescending(y => Math.Max(y.Line.P1.Y, y.Line.P2.Y))
                                    .ThenByDescending(x => Math.Max(x.Line.P1.X, x.Line.P2.X))
                                    .Where(z => Math.Min(z.Line.P1.X, z.Line.P2.X) > threshImage.Width * 0.8
                                            && Math.Min(z.Line.P1.X, z.Line.P2.X) < threshImage.Width * 0.9
                                            && Math.Min(z.Line.P1.Y, z.Line.P2.Y) > threshImage.Height * 0.65
                                            && Math.Max(z.Line.P1.Y, z.Line.P2.Y) < threshImage.Height * 0.8
                                            )
                                    .First().Index;
                LineSegment2D rightBottomLine = filteredShortLines[rightBottomindex];

                LineSegment2D StartShortLine = filteredShortLines[leftBottomindex];
                LineSegment2D EndShortLine = filteredShortLines[rightBottomindex];
                List<LineSegment2D> AngleLines = new List<LineSegment2D>();
                AngleLines.Add(leftBottomLine); AngleLines.Add(rightBottomLine); AngleLines.Add(ExtendPointerLine);

                Point FinalclosestPointF = mostCommonIntersection;//FindFinalUnionPoint(AngleLines);
                Point Left = new Point((StartShortLine.P1.X + StartShortLine.P2.X) / 2, (StartShortLine.P1.Y + StartShortLine.P2.Y) / 2);
                Point Rightt = new Point((EndShortLine.P1.X + EndShortLine.P2.X) / 2, (EndShortLine.P1.Y + EndShortLine.P2.Y) / 2);

                LineSegment2D line1 = new LineSegment2D(Left, FinalclosestPointF);
                LineSegment2D line2 = new LineSegment2D(Rightt, FinalclosestPointF);
                LineSegment2D StartLine = CheckLongLineDirection(line1, threshImage.Width);
                LineSegment2D EndLine = CheckLongLineDirection(line2, threshImage.Width);
                double LineAngle = 360 - EndLine.GetExteriorAngleDegree(StartLine);
                double PointLineAngle = Math.Abs(ExtendPointerLine.GetExteriorAngleDegree(StartLine));

                List<LineSegment2D> FinalLines = new List<LineSegment2D>();
                FinalLines.Add(StartLine); FinalLines.Add(EndLine); FinalLines.Add(ExtendPointerLine);

                //PointF FinalclosestPointF = FindPointOnLine(FinalLines, ExtendPointerLine);
                Point FinalclosestPoint = new Point((int)FinalclosestPointF.X, (int)FinalclosestPointF.Y);

                Point P1 = CalculateMidpoint(FinalLines[0]);//CalculateMidpoint(line1);
                Point P2 = CalculateMidpoint(FinalLines[1]);//CalculateMidpoint(line2);
                Point P3 = CalculateMidpoint(PointerLine);
                double maxAngle2 = 180 + Math.Abs(CalculateAngleBetweenPoints(P2, FinalclosestPoint, P1));
                double DialAngle = 180 + Math.Abs(CalculateAngleBetweenPoints(P3, FinalclosestPoint, P1));

                double value = Math.Round(600 * PointLineAngle / LineAngle,2);
                Console.WriteLine("maxAngle : " + LineAngle);
                Console.WriteLine("PointerLineAngle : " + PointLineAngle);
                Console.WriteLine("result：" + value);
                return value;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        
        private static LineSegment2D CheckLongLineDirection(LineSegment2D CheckLine, int Diameter)
        {
            LineSegment2D line = CheckLine;
            Point r = new Point(Diameter / 2, Diameter / 2);
            double P1 = Math.Sqrt(Math.Pow(CheckLine.P1.X - r.X, 2) + Math.Pow(CheckLine.P1.Y - r.Y, 2));
            double P2 = Math.Sqrt(Math.Pow(CheckLine.P2.X - r.X, 2) + Math.Pow(CheckLine.P2.Y - r.Y, 2));
            if (P1 > P2)
            {
                Point temp = line.P1;  // 將P1存到temp
                line.P1 = line.P2;     // 將P2的值指派給P1
                line.P2 = temp;
            }
            return line;
        }

        private static double CalculateAngleBetweenPoints(Point p1, Point p2, Point p3)
        {
            // Calculate vectors
            double v1x = p1.X - p2.X;
            double v1y = p1.Y - p2.Y;
            double v2x = p3.X - p2.X;
            double v2y = p3.Y - p2.Y;
            // Calculate dot product
            double dotProduct = v1x * v2x + v1y * v2y;
            // Calculate vector lengths
            double v1Length = Math.Sqrt(v1x * v1x + v1y * v1y);
            double v2Length = Math.Sqrt(v2x * v2x + v2y * v2y);
            // Calculate angle between vectors (in degrees)
            double angle = Math.Acos(dotProduct / (v1Length * v2Length)) * 180 / Math.PI;
            return angle;
        }

        private static Point CalculateMidpoint(LineSegment2D line)
        {
            int x = (line.P1.X + line.P2.X) / 2;
            int y = (line.P1.Y + line.P2.Y) / 2;
            return new Point(x, y);
        }

        private static bool PointOntheLine(LineSegment2D line, Point mostCommonIntersection)
        {
            double x1 = line.P1.X;
            double y1 = line.P1.Y;
            double x2 = line.P2.X;
            double y2 = line.P2.Y;
            double px = mostCommonIntersection.X;
            double py = mostCommonIntersection.Y;
            double dx = x2 - x1;
            double dy = y2 - y1;
            double u = ((px - x1) * dx + (py - y1) * dy) / (dx * dx + dy * dy);
            double closestX, closestY;
            if (u < 0)
            {
                closestX = x1;
                closestY = y1;
            }
            else if (u > 1)
            {
                closestX = x2;
                closestY = y2;
            }
            else
            {
                closestX = x1 + u * dx;
                closestY = y1 + u * dy;
            }

            double distance = Math.Sqrt(Math.Pow(px - closestX, 2) + Math.Pow(py - closestY, 2));
            bool result = (distance < 15) ? false : true;
            return result;
        }

        private static bool IsLineNearPoint(LineSegment2D line, Point point, int minLength)
        {
            double distance = DistanceFromLineToPoint(line, point);
            return distance <= minLength;
        }
        private static double DistanceFromLineToPoint(LineSegment2D line, Point point)
        {
            double distance = Math.Abs((point.Y - line.P1.Y) * (line.P2.X - line.P1.X) - (point.X - line.P1.X) * (line.P2.Y - line.P1.Y)) / Math.Sqrt(Math.Pow(line.P2.X - line.P1.X, 2) + Math.Pow(line.P2.Y - line.P1.Y, 2));
            return distance;
        }
        public static Point FindPointOnLine(List<LineSegment2D> mergedLongLines, LineSegment2D ExtendPointerLine, LineSegment2D PointerLine)
        {
            Dictionary<Point, int> pointToCount = new Dictionary<Point, int>();
            int ImageHeight = Math.Max(ExtendPointerLine.P1.Y, ExtendPointerLine.P2.Y);
            int ImageWidth = Math.Max(ExtendPointerLine.P1.X, ExtendPointerLine.P2.X);
            foreach (LineSegment2D line in mergedLongLines)
            {
                // 找到延長指針線段和線段集合中的每一條線段的交點
                Point intersection = GetIntersectionPoint(ExtendPointerLine, line);

                if (!pointToCount.ContainsKey(intersection))
                    pointToCount[intersection] = 0;
                pointToCount[intersection]++;
            }
            Point nearestPoint = new Point();
            int maxCount = 0;
            foreach (KeyValuePair<Point, int> entry in pointToCount)
            {
                if (entry.Value > maxCount && entry.Key.X > 0 && entry.Key.Y > 0 &&
                    entry.Key.X > ImageWidth * 0.45 && entry.Key.X < ImageWidth * 0.65 &&
                    entry.Key.Y > ImageHeight * 0.45 && entry.Key.Y < ImageHeight * 0.85) //&&
                {
                    maxCount = entry.Value;
                    nearestPoint = entry.Key;
                }
                else if (entry.Value == maxCount)
                {
                    // 如果有多個點距離相同，則選擇離延長指針線段中點最近的點
                    Point midpoint = new Point((ExtendPointerLine.P1.X + ExtendPointerLine.P2.X) / 2,
                        (ExtendPointerLine.P1.Y + ExtendPointerLine.P2.Y) / 2);
                    double distanceToMidpoint1 = FindDistance(midpoint, nearestPoint);
                    double distanceToMidpoint2 = FindDistance(midpoint, entry.Key);
                    if (distanceToMidpoint2 < distanceToMidpoint1)
                    {
                        nearestPoint = entry.Key;
                    }
                }
            }
            return nearestPoint;
        }

        public static Point GetIntersectionPoint(LineSegment2D line1, LineSegment2D line2)
        {
            double x1 = line1.P1.X;
            double y1 = line1.P1.Y;
            double x2 = line1.P2.X;
            double y2 = line1.P2.Y;
            double x3 = line2.P1.X;
            double y3 = line2.P1.Y;
            double x4 = line2.P2.X;
            double y4 = line2.P2.Y;
            double denominator = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
            double numerator1 = (x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3);
            double numerator2 = (x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3);
            double t1 = numerator1 / denominator;
            double t2 = numerator2 / denominator;
            double intersectionX = x1 + t1 * (x2 - x1);
            double intersectionY = y1 + t1 * (y2 - y1);
            return new Point((int)Math.Round(intersectionX), (int)Math.Round(intersectionY));
        }

        public static double FindDistance(Point p1, Point p2)
        {
            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        private static Image<Gray, byte> GetLength(Image<Gray, byte> threshImage, int Arclengthmax, int Arclengthmin)
        {
            Image<Gray, byte> Eliminateimage = threshImage.Clone();
            int shortlengh = (Eliminateimage.Width > Eliminateimage.Height) ? Eliminateimage.Height : Eliminateimage.Width;
            // 假設圓心座標為 (cx, cy)，半徑為 r
            int cx = (int)shortlengh / 2;
            int cy = (int)shortlengh / 2;
            int minr = (int)(shortlengh * 0.35);
            int maxr = (int)(shortlengh * 0.8);
            VectorOfVectorOfPoint sortedContoursVector = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(Eliminateimage, sortedContoursVector, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
            for (int i = 0; i < sortedContoursVector.Size; i++)
            {
                VectorOfPoint contour = sortedContoursVector[i];
                double ArcLength = CvInvoke.ArcLength(contour, true);
                double area = CvInvoke.ContourArea(contour);
                double areaPerimeterRatio = area / ArcLength;

                VectorOfPoint polygon = new VectorOfPoint();
                CvInvoke.ApproxPolyDP(contour, polygon, 3, true);
                // 輪廓頂點的座標
                Point[] contourPoints = polygon.ToArray();

                // 找出最上、最下、最左、最右的點
                Point topPoint = contourPoints.OrderBy(p => p.Y).FirstOrDefault();
                Point bottomPoint = contourPoints.OrderByDescending(p => p.Y).FirstOrDefault();
                Point leftPoint = contourPoints.OrderBy(p => p.X).FirstOrDefault();
                Point rightPoint = contourPoints.OrderByDescending(p => p.X).FirstOrDefault();
                // 計算頂點到圓心的距離
                double topdistance = Math.Sqrt(Math.Pow(topPoint.X - cx, 2) + Math.Pow(topPoint.Y - cy, 2));
                double bottomdistance = Math.Sqrt(Math.Pow(bottomPoint.X - cx, 2) + Math.Pow(bottomPoint.Y - cy, 2));
                double leftdistance = Math.Sqrt(Math.Pow(leftPoint.X - cx, 2) + Math.Pow(leftPoint.Y - cy, 2));
                double rightdistance = Math.Sqrt(Math.Pow(rightPoint.X - cx, 2) + Math.Pow(rightPoint.Y - cy, 2));

                if (topdistance > maxr && bottomdistance > maxr && leftdistance > maxr && rightdistance > maxr)
                {
                    CvInvoke.DrawContours(Eliminateimage, sortedContoursVector, i, new MCvScalar(0), -1);
                }
                else if (topdistance < minr && bottomdistance < minr && leftdistance < minr && rightdistance < minr)
                {
                    CvInvoke.DrawContours(Eliminateimage, sortedContoursVector, i, new MCvScalar(0), -1);
                }
                else if (topPoint.Y < Eliminateimage.Height * 0.05 || topPoint.Y > Eliminateimage.Height * 0.8)
                {
                    CvInvoke.DrawContours(Eliminateimage, sortedContoursVector, i, new MCvScalar(0), -1);
                }
                else if (topPoint.X < Eliminateimage.Width * 0.1 || topPoint.X > Eliminateimage.Width * 0.95)
                {
                    CvInvoke.DrawContours(Eliminateimage, sortedContoursVector, i, new MCvScalar(0), -1);
                }
            }
            return Eliminateimage;
        }

        private static Image<Gray, byte> GetEliminate(Image<Gray, byte> threshImage, int thresholdmax, int thresholdmin, bool reverse)
        {
            Image<Gray, byte> Eliminateimage = threshImage.Clone();
            VectorOfVectorOfPoint sortedContoursVector = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(Eliminateimage, sortedContoursVector, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxSimple);
            for (int i = 0; i < sortedContoursVector.Size; i++)
            {
                double length = CvInvoke.ArcLength(sortedContoursVector[i], true);
                double area = CvInvoke.ContourArea(sortedContoursVector[i]);
                RotatedRect rotatedRect = CvInvoke.MinAreaRect(sortedContoursVector[i]);
                VectorOfPoint approxCurve = new VectorOfPoint();
                CvInvoke.ApproxPolyDP(sortedContoursVector[i], approxCurve, 10, true);
                if (area < thresholdmax && area > thresholdmin)
                    CvInvoke.DrawContours(Eliminateimage, sortedContoursVector, i, new MCvScalar(0), -1);
            }
            if (reverse)
            {
                Image<Gray, byte> diff = new Image<Gray, byte>(Eliminateimage.Width, Eliminateimage.Height);
                CvInvoke.AbsDiff(Eliminateimage, threshImage, diff);
                return diff;
            }
            else
                return Eliminateimage;
        }

        private static Image<Bgr, byte> GetCircleImg(Image<Bgr, byte> circleRoiImg)
        {
            Image<Bgr, byte> img = circleRoiImg.Clone();
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    // offset to center
                    int virtX = x - img.Width / 2;
                    int virtY = y - img.Height / 2;
                    if (Math.Sqrt(virtX * virtX + virtY * virtY) > img.Width / 2)
                    {
                        img.Data[y, x, 0] = 255;
                        img.Data[y, x, 1] = 255;
                        img.Data[y, x, 2] = 255;
                    }
                }
            }
            return img; // your circle cropped image
        }

        // 將線段延伸到圖像邊界
        static LineSegment2D ExtendLine(LineSegment2D line, Size size)
        {
            Point p1 = line.P1;
            Point p2 = line.P2;
            if (p1.X == p2.X) // 垂直線段
            {
                if (p1.Y < p2.Y)
                {
                    p2.Y = size.Height;
                    p1.Y = 0;
                }
                else
                {
                    p2.Y = 0;
                    p1.Y = size.Height;
                }
            }
            else if (p1.Y == p2.Y) // 水平線段
            {
                if (p1.X < p2.X)
                {
                    p2.X = size.Width;
                    p1.X = 0;
                }
                else
                {
                    p2.X = 0;
                    p1.X = size.Width;
                }
            }
            else // 斜線段
            {
                double slope = (double)(p2.Y - p1.Y) / (double)(p2.X - p1.X);
                double b = p1.Y - slope * p1.X;
                if (p1.X < p2.X)
                {
                    p2.X = size.Width;
                    p2.Y = (int)Math.Round(slope * p2.X + b);
                    if (Math.Abs(p2.Y) > size.Height)
                    {
                        p2.Y = size.Height;
                        p2.X = (int)((p2.Y - b) / slope);
                    }
                    else if (p2.Y < 0)
                    {
                        p2.Y = 0;
                        p2.X = (int)((p2.Y - b) / slope);
                    }
                    p1.X = 0;
                    p1.Y = (int)Math.Round(slope * p1.X + b);
                    if (Math.Abs(p1.Y) > size.Height)
                    {
                        p1.Y = size.Height;
                        p1.X = (int)((p1.Y - b) / slope);
                    }
                    else if (p1.Y < 0)
                    {
                        p1.Y = 0;
                        p1.X = (int)((p1.Y - b) / slope);
                    }
                }
                else
                {
                    p2.X = 0;
                    p2.Y = (int)Math.Round(slope * p2.X + b);
                    if (Math.Abs(p2.Y) > size.Height)
                    {
                        p2.Y = size.Height;
                        p2.X = (int)((p2.Y - b) / slope);
                    }
                    else if (p2.Y < 0)
                    {
                        p2.Y = 0;
                        p2.X = (int)((p2.Y - b) / slope);
                    }
                    p1.X = size.Width;
                    p1.Y = (int)Math.Round(slope * p1.X + b);
                    if (Math.Abs(p1.Y) > size.Height)
                    {
                        p1.Y = size.Height;
                        p1.X = (int)((p1.Y - b) / slope);
                    }
                    else if (p1.Y < 0)
                    {
                        p1.Y = 0;
                        p1.X = (int)((p1.Y - b) / slope);
                    }
                }
            }
            return new LineSegment2D(p1, p2);
        }
    }
}
