using KWI.Format.Structure;
using KWI.Format.Structure.BackgroundFrame;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace KWI.Viewer.MapRender
{
    public class ParcelShape
    {
        private readonly ParcelRecord _parcel;
        private System.Windows.Controls.Image _image;

        private readonly int _parcelScreenSize;
        private readonly GeoCord _parcelWidth;
        private readonly GeoCord _parcelHeight;
        private readonly GeoCord _parcelX;
        private readonly GeoCord _parcelY;

        private readonly int _integratedOffsetX;
        private readonly int _integratedOffsetY;

        private bool _onCanvas;

        private readonly List<ParcelRecord> _dividedParcels = new List<ParcelRecord>();

        public ParcelShape(ParcelRecord parcel, int parcelSize, GeoCord parcelXSize, GeoCord parcelYSize, int parcelIndex)
        {
            _parcel = parcel;
            _parcelScreenSize = parcelSize;
            _parcelWidth = parcelXSize;
            _parcelHeight = parcelYSize;

            _parcel.LoadChilds();

            if (_parcel.DividedWrapper)
            {
                var parcelInfo = _parcel.Childs.OfType<ParcelInformationRecord>().FirstOrDefault();
                parcelInfo.LoadChilds();
                foreach (var dividedParcel in parcelInfo.Childs.OfType<ParcelRecord>())
                {
                    dividedParcel.LoadChilds();
                    _parcelX = dividedParcel.ParcelLong;
                    _parcelY = dividedParcel.ParcelLat;

                    _dividedParcels.Add(dividedParcel);
                }
            }
            else if (_parcel.IsPartOfIntegrated)
            {
                var blockWidth = _parcel.FindParentOfType<LevelRecord>().ParcelsCountLong;
                var blockHeight = _parcel.FindParentOfType<LevelRecord>().ParcelsCountLat;
                var posBlockRelativeY = parcelIndex / blockWidth;
                var posBlockRelativeX = parcelIndex % blockWidth;
                _integratedOffsetX = posBlockRelativeX - _parcel.X;
                _integratedOffsetY = posBlockRelativeY - _parcel.Y;
                _parcelX = _parcel.ParcelLong + parcelXSize.Multiply(_integratedOffsetX);
                _parcelY = _parcel.ParcelLat + parcelYSize.Multiply(_integratedOffsetY);
            }
            else
            {
                _parcelX = parcel.ParcelLong;
                _parcelY = parcel.ParcelLat;
            }

        }

        internal void ClearFromCanvas()
        {
            if (_onCanvas && _image != null)
            {
                var parent = _image.Parent as Canvas;
                parent.Children.Remove(_image);
            }
            _onCanvas = false;
        }

        public void SetupTransform(GeoCord viewportCenterLong, GeoCord viewportCenterLat, GeoCord viewportWidth, GeoCord viewportHeight)
        {
            var pixelToEightsRatioX = _parcelWidth.GetAsEightSeconds() / _parcelScreenSize;
            var pixelToEightsRatioY = _parcelHeight.GetAsEightSeconds() / _parcelScreenSize;

            var offsetFromCenterX = viewportWidth.Divide(2) + _parcelX - viewportCenterLong;
            var offsetFromCenterY = viewportHeight.Divide(2) - _parcelY + viewportCenterLat;

            var offsetX = offsetFromCenterX.GetAsEightSeconds() / pixelToEightsRatioX;
            var offsetY = offsetFromCenterY.GetAsEightSeconds() / pixelToEightsRatioY;

            var transformGroup = new System.Windows.Media.TransformGroup();
            transformGroup.Children.Add(new System.Windows.Media.ScaleTransform(1, -1));
            transformGroup.Children.Add(new System.Windows.Media.TranslateTransform(offsetX, offsetY));
            _image.RenderTransform = transformGroup;
        }

        internal void Draw(Canvas canvas)
        {
            if (_image == null)
            {
                _image = new System.Windows.Controls.Image();
                _image.Source = RenderToImage();
                _image.Width = _parcelScreenSize;
                _image.Height = _parcelScreenSize;
                _image.RenderTransformOrigin = new System.Windows.Point(0, 0);
            }
            if (!_onCanvas)
            {
                canvas.Children.Add(_image);
                _onCanvas = true;
            }
        }

        private (Brush brush, Pen pen) GetBgColor(int type)
        {
            switch (type)
            {
                case 0://green land
                    return (Brushes.DarkOrange, Pens.DarkOrange);
                case 3://linear water system
                case 1: //water system
                    return (Brushes.LightBlue, Pens.LightBlue);
                case 2: //island
                    return (Brushes.LightCoral, Pens.LightCoral);
                case 4: //countour
                    return (Brushes.LightGray, Pens.LightGray);
                case 5: //administrative field
                case 6: //site field
                    return (Brushes.DarkGray, Pens.DarkGray);
                case 7:
                case 8:
                case 9:
                    return (Brushes.Black, Pens.Black);
                case 16:
                    return (new SolidBrush(Color.FromArgb(100, 0, 0, 0)), Pens.LightBlue);
                default:
                    return (Brushes.Black, Pens.LightBlue);
            }
        }


        private void ExtractShapes(ParcelRecord parcel, out List<Tuple<Brush, Point[]>> backgrounds, out List<Tuple<Pen, Point[]>> backgroundLines)
        {
            backgrounds = new List<Tuple<Brush, Point[]>>();
            backgroundLines = new List<Tuple<Pen, Point[]>>();
            var backgroundFrame = parcel.Childs.OfType<BackgroundFrameRecord>().FirstOrDefault();
            if (backgroundFrame != null)
            {
                foreach (var bgElementInfo in backgroundFrame.Childs.OfType<BgElementInformation>())
                {
                    bgElementInfo.LoadChilds();
                    foreach (var bgUnit in bgElementInfo.Childs.OfType<BackgroundUnit>())
                    {
                        bgUnit.LoadChilds();

                        foreach (var area in bgUnit.Childs.OfType<MinimunGraphicsDataRecord>())
                        {
                            int nominator = 1;
                            if (area.MultConst > 1)
                                nominator = (int)Math.Pow(2, area.MultConst);
                            var x = area.StartX;
                            var y = area.StartY;
                            if (parcel.IsPartOfIntegrated)
                            {
                                x += (area.IntegratedX - _integratedOffsetX) * 4096;
                                y += (area.IntegratedY - _integratedOffsetY) * 4096;
                            }
                            var points = new List<Point> { new Point(x, y) };
                            bool penUp = false;
                            foreach (var offset in area.Offsets)
                            {
                                if (offset.X == 0 && offset.Y == 0)
                                    break;
                                x = x + offset.X / nominator;
                                y = y + offset.Y / nominator;
                                points.Add(new Point(x, y));
                            }
                            if (bgUnit.Type == Format.Typing.Graphics.ShapeType.Line)
                            {
                                if (points.Any(p => (p.X >= 0 && p.X <= 4096) || (p.Y >= 0 && p.Y <= 4096)))
                                    backgroundLines.Add(new Tuple<Pen, Point[]>(GetBgColor(bgElementInfo.DisplayType).pen, points.ToArray()));
                            }
                            if (bgUnit.Type == Format.Typing.Graphics.ShapeType.Area)
                            {
                                if (points.Any(p => (p.X >= 0 && p.X <= 4096) || (p.Y >= 0 && p.Y <= 4096)))
                                    backgrounds.Add(new Tuple<Brush, Point[]>(GetBgColor(bgElementInfo.DisplayType).brush, points.ToArray()));
                            }
                        }
                    }
                }
            }
        }
        private BitmapImage RenderToImage()
        {
            List<Tuple<Brush, Point[]>> backgroundAreas = null;
            List<Tuple<Pen, Point[]>> backgroundLines = null;
            if (!_parcel.DividedWrapper)
            {
                ExtractShapes(_parcel, out backgroundAreas, out backgroundLines);
            }
            else
            {
                backgroundAreas = new List<Tuple<Brush, Point[]>>();
                backgroundLines = new List<Tuple<Pen, Point[]>>();
                var parcelInformation = _parcel.Childs.OfType<ParcelInformationRecord>().FirstOrDefault();
                foreach (var dividedParcel in parcelInformation.Childs.OfType<ParcelRecord>())
                {
                    ExtractShapes(dividedParcel, out List<Tuple<Brush, Point[]>> backgrounds, out List<Tuple<Pen, Point[]>> lines);
                    backgroundAreas.AddRange(backgrounds);
                    backgroundLines.AddRange(lines);
                }
            }


            using (var image = new System.Drawing.Bitmap(_parcelScreenSize, _parcelScreenSize))
            {
                var ratio = _parcelScreenSize / 4096.0f;
                using (var g = Graphics.FromImage(image))
                {
                    foreach (var bgArea in backgroundAreas)
                    {
                        var brush = bgArea.Item1;
                        var point = bgArea.Item2;
                        g.FillPolygon(brush, point.Select(p => new PointF((p.X) * ratio, (p.Y) * ratio)).ToArray(), System.Drawing.Drawing2D.FillMode.Winding);
                    }
                    foreach (var bgLine in backgroundLines)
                    {
                        var pen = bgLine.Item1;
                        var point = bgLine.Item2;
                        g.DrawLines(pen, point.Select(p => new PointF((p.X) * ratio, (p.Y) * ratio)).ToArray());
                    }
                    if (!_parcel.IsPartOfIntegrated)
                    {
                        g.DrawRectangle(Pens.Green, 0, 0, _parcelScreenSize - 1, _parcelScreenSize - 1);
                        if (_parcel.DividedWrapper)
                        {

                        }
                    }
                    else
                    {
                        if (_integratedOffsetX == 0 && _integratedOffsetY == 0)
                        {
                            g.DrawRectangle(Pens.Yellow, 0, 0, _parcelScreenSize + 1, _parcelScreenSize + 1);
                            g.DrawRectangle(Pens.Green, 1, 1, _parcelScreenSize - 2, _parcelScreenSize - 2);
                        }
                        else if (_integratedOffsetX == 0 && _integratedOffsetY == _parcel.PartY)
                        {
                            g.DrawRectangle(Pens.Yellow, 0, -1, _parcelScreenSize - 1, _parcelScreenSize);
                            g.DrawRectangle(Pens.Green, 1, 0, _parcelScreenSize - 2, _parcelScreenSize - 1);
                        }
                        else if (_integratedOffsetY == 0 && _integratedOffsetX == _parcel.PartX)
                        {
                            g.DrawRectangle(Pens.Yellow, -1, 0, _parcelScreenSize, _parcelScreenSize - 1);
                            g.DrawRectangle(Pens.Green, 0, 1, _parcelScreenSize - 1, _parcelScreenSize - 2);
                        }
                        else if (_integratedOffsetX == _parcel.PartX && _integratedOffsetY == _parcel.PartY)
                        {
                            g.DrawRectangle(Pens.Yellow, -1, -1, _parcelScreenSize, _parcelScreenSize);
                            g.DrawRectangle(Pens.Green, 0, 0, _parcelScreenSize - 2, _parcelScreenSize - 2);
                        }
                        else
                        {
                            g.DrawRectangle(Pens.Green, 0, 0, _parcelScreenSize - 1, _parcelScreenSize - 1);
                        }
                    }
                    g.FillRectangle(Brushes.Green, new Rectangle(1, 1, 100, 15));
                    g.ScaleTransform(1, -1);
                    g.DrawString($"{_parcel.Name}", SystemFonts.DefaultFont, Brushes.White, new PointF(0, -15));
                    g.Save();
                }

                using (var ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;

                    var bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.StreamSource = ms;
                    bi.EndInit();
                    return bi;
                }
            }
        }
    }
}
