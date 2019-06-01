using KWI.Format.Structure;
using KWI.Format.Typing.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace KWI.Viewer.MapRender
{
    public class Renderer : INotifyPropertyChanged
    {
        private Scale _currentScale;
        public Scale CurrentScale
        {
            get { return _currentScale; }
            set
            {
                _currentScale = value;
                OnScaleUpdate();
                ComputeViewPort();
                OnPropertyChanged(nameof(CurrentScale));
            }
        }

        private void OnScaleUpdate()
        {
            _currentLevel = _levels[CurrentScale.LevelCode];
            if (!_parcels.ContainsKey(CurrentScale))
                _parcels.Add(CurrentScale, new Dictionary<Tuple<int, int, int>, ParcelShape>());

            _bsSizeLong = _coverageWidth.Divide(_currentLevel.BlockSetsCountLong);
            _bsSizeLat = _coverageHeight.Divide(_currentLevel.BlockSetsCountLat);
            _bSizeLong = _coverageWidth.Divide(_currentLevel.BlockSetsCountLong * _currentLevel.BlocksCountLong);
            _bSizeLat = _coverageHeight.Divide(_currentLevel.BlockSetsCountLat * _currentLevel.BlocksCountLat);
            _parcelSizeLong = _coverageWidth.Divide(_currentLevel.BlockSetsCountLong * _currentLevel.BlocksCountLong * _currentLevel.ParcelsCountLong);
            _parcelSizeLat = _coverageHeight.Divide(_currentLevel.BlockSetsCountLat * _currentLevel.BlocksCountLat * _currentLevel.ParcelsCountLat);

            foreach (var shape in _shapesOnCanvas)
            {
                shape.ClearFromCanvas();
            }
            _shapesOnCanvas.Clear();

            ComputeViewPort();
            Render();
        }

        private LevelRecord _currentLevel;
        private GeoCord _viewportHeight;
        private GeoCord _viewportWidth;

        private GeoCord _bsSizeLong;
        private GeoCord _bsSizeLat;
        private GeoCord _bSizeLong;
        private GeoCord _bSizeLat;
        private GeoCord _parcelSizeLong;
        private GeoCord _parcelSizeLat;

        private void ComputeViewPort()
        {
            var pixelToEightsRatioX = _parcelSizeLong.GetAsEightsS() / CurrentScale.ParcelSize;
            var pixelToEightsRatioY = _parcelSizeLat.GetAsEightsS() / CurrentScale.ParcelSize;

            _viewportHeight = GeoCord.FromEights(pixelToEightsRatioY * _canvas.ActualHeight);
            _viewportWidth = GeoCord.FromEights(pixelToEightsRatioX * _canvas.ActualWidth);
        }

        private GeoCord _currentLat;
        public GeoCord CurrentLat
        {
            get => _currentLat;
            set
            {
                _currentLat = value;
                OnPropertyChanged(nameof(CurrentLatStr));
            }
        }
        private GeoCord _currentLong;
        public GeoCord CurrentLong
        {
            get => _currentLong;
            set
            {
                _currentLong = value;
                OnPropertyChanged(nameof(CurrentLongStr));
            }
        }

        public string CurrentLongStr => CurrentLong?.ToString();
        public string CurrentLatStr => CurrentLat?.ToString();

        public ObservableCollection<Scale> Scales { get; } = new ObservableCollection<Scale>();

        private readonly GeoCord _coverageHeight = new GeoCord(85, 20, 0);
        private readonly GeoCord _coverageWidth = new GeoCord(256, 0, 0);
        private readonly GeoCord _coverageStartLong = new GeoCord(31, 37, 30, -1);
        private readonly GeoCord _coverageStartLat = new GeoCord(0, 0, 0);

        private readonly Dictionary<int, LevelRecord> _levels = new Dictionary<int, LevelRecord>();
        private readonly Dictionary<Scale, Dictionary<Tuple<int, int, int>, ParcelShape>> _parcels = new Dictionary<Scale, Dictionary<Tuple<int, int, int>, ParcelShape>>();

        private readonly Canvas _canvas;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public Renderer(Canvas canvas, ParcelRelatedDataFrame mapData)
        {
            _canvas = canvas;

            LoadScales(mapData);
            CurrentScale = Scales.First();
            CurrentLat = new GeoCord(30, 0, 0);
            CurrentLong = new GeoCord(64, 0, 0);

            _canvas.SizeChanged += _canvas_SizeChanged;

            _canvas.MouseMove += _canvas_MouseMove;
            _canvas.MouseWheel += _canvas_MouseWheel;
        }

        private void _canvas_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var sign = Math.Sign(e.Delta);
            var index = Scales.IndexOf(CurrentScale);
            if (index > 0 && sign < 0)
                CurrentScale = Scales[index - 1];
            if (index < Scales.Count && sign > 0)
                CurrentScale = Scales[index + 1];
        }

        private Point? _prevMousePoint = null;
        private void _canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(_canvas);
                if (_prevMousePoint != null)
                {
                    var offsetX = _prevMousePoint.Value.X - pos.X;
                    var offsetY = pos.Y - _prevMousePoint.Value.Y;

                    var pixelToEightsRatioX = _parcelSizeLong.GetAsEightsS() / CurrentScale.ParcelSize;
                    var pixelToEightsRatioY = _parcelSizeLat.GetAsEightsS() / CurrentScale.ParcelSize;

                    CurrentLong = CurrentLong + GeoCord.FromEights(offsetX * pixelToEightsRatioX);
                    CurrentLat = CurrentLat + GeoCord.FromEights(offsetY * pixelToEightsRatioY);

                    Render();
                }
                _prevMousePoint = pos;
            }
            else
                _prevMousePoint = null;
        }

        private void _canvas_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            ComputeViewPort();
            Render();
        }

        private void LoadScales(ParcelRelatedDataFrame mapData)
        {
            bool firstLevel = true;
            foreach (LevelRecord level in mapData.Childs.OfType<LevelRecord>())
            {
                _levels.Add(level.Header.LevelCode, level);
                if (firstLevel)
                {
                    Scales.Add(new Scale("1:2048", 512, level.Header.LevelCode, level.Name));
                    Scales.Add(new Scale("1:1024", 1024, level.Header.LevelCode, level.Name));
                    Scales.Add(new Scale("1:512", 2048, level.Header.LevelCode, level.Name));
                    firstLevel = false;
                    continue;
                }
                var startSize = 1024;
                foreach (var scale in level.Scales)
                {
                    Scales.Add(new Scale(scale, startSize, level.Header.LevelCode, level.Name));
                    startSize *= 2;
                }
            }
        }

        private bool SquareInView(GeoCord top, GeoCord left, GeoCord bottom, GeoCord right,
                                  GeoCord viewTop, GeoCord viewLeft, GeoCord viewBottom, GeoCord viewRight)
        {
            return
                (
                    (right > viewLeft && left < viewLeft)
                || (left < viewRight && right > viewRight)
                )
                && (
                    (bottom < viewTop && top > viewTop)
                || (top > viewBottom && bottom < viewBottom)
                )
                || (
                (
                    (right > viewLeft && left < viewLeft)
                || (left < viewRight && right > viewRight)
                )
                &&
                (
                    bottom > viewBottom && top < viewTop
                )
                )
                || (
                (
                    (bottom < viewTop && top > viewTop)
                || (top > viewBottom && bottom < viewBottom)
                )
                &&
                (
                    left > viewLeft && right < viewRight
                )
                )
                || (
                    left > viewLeft
                && right < viewRight
                && top < viewTop
                && bottom > viewBottom
                );
        }

        public void Render()
        {
            if (CurrentLong == null || CurrentLat == null || _canvas.ActualWidth == 0 || _canvas.ActualHeight == 0)
                return;

            var leftEdge = CurrentLong - _viewportWidth.Divide(2);
            var bottomEdge = CurrentLat - _viewportHeight.Divide(2);
            var topEdge = bottomEdge + _viewportHeight;
            var rightEdge = leftEdge + _viewportWidth;

            var longPointer = _coverageStartLong;
            var latPointer = _coverageStartLat;

            var parcelPointers = new List<Tuple<int, int, int>>();

            GeoCord bsLongPointer = longPointer;
            GeoCord bsLatPointer = latPointer;

            for (int bsY = 0; bsY < _currentLevel.BlockSetsCountLat; bsY++)
            {
                bsLongPointer = longPointer;
                for (int bsX = 0; bsX < _currentLevel.BlockSetsCountLong; bsX++)
                {
                    GeoCord bLongPointer = bsLongPointer;
                    GeoCord bLatPonter = bsLatPointer;
                    for (int bY = 0; bY < _currentLevel.BlocksCountLat; bY++)
                    {
                        bLongPointer = bsLongPointer;
                        for (int bX = 0; bX < _currentLevel.BlocksCountLong; bX++)
                        {
                            GeoCord pLongPointer = bLongPointer;
                            GeoCord pLatPointer = bLatPonter;
                            for (int pY = 0; pY < _currentLevel.ParcelsCountLat; pY++)
                            {
                                pLongPointer = bLongPointer;
                                for (int pX = 0; pX < _currentLevel.ParcelsCountLong; pX++)
                                {
                                    var parcelLeft = pLongPointer;
                                    var parcelRight = pLongPointer + _parcelSizeLong;
                                    var parcelBottom = pLatPointer;
                                    var parcelTop = pLatPointer + _parcelSizeLat;

                                    if (
                                        (
                                           (parcelRight > leftEdge && parcelLeft < leftEdge)
                                        || (parcelLeft < rightEdge && parcelRight > rightEdge)
                                        )
                                     && (
                                           (parcelBottom < topEdge && parcelTop > topEdge)
                                        || (parcelTop > bottomEdge && parcelBottom < bottomEdge)
                                        )
                                     || (
                                        (
                                           (parcelRight > leftEdge && parcelLeft < leftEdge)
                                        || (parcelLeft < rightEdge && parcelRight > rightEdge)
                                        )
                                        &&
                                        (
                                            parcelBottom > bottomEdge && parcelTop < topEdge
                                        )
                                     )
                                     || (
                                        (
                                           (parcelBottom < topEdge && parcelTop > topEdge)
                                        || (parcelTop > bottomEdge && parcelBottom < bottomEdge)
                                        )
                                        &&
                                        (
                                            parcelLeft > leftEdge && parcelRight < rightEdge
                                        )
                                     )
                                     || (
                                           parcelLeft > leftEdge
                                        && parcelRight < rightEdge
                                        && parcelTop < topEdge
                                        && parcelBottom > bottomEdge
                                        )
                                       )
                                        parcelPointers.Add(new Tuple<int, int, int>(
                                            bsY * _currentLevel.BlockSetsCountLong + bsX,
                                            bY * _currentLevel.BlocksCountLong + bX,
                                            pY * _currentLevel.ParcelsCountLong + pX));

                                    pLongPointer += _parcelSizeLong;
                                }
                                pLatPointer += _parcelSizeLat;
                            }

                            bLongPointer += _bSizeLong;
                        }
                        bLatPonter += _bSizeLat;
                    }
                    bsLongPointer += _bsSizeLong;
                }
                bsLatPointer += _bsSizeLat;
            }

            EnsureParcels(parcelPointers);
            SetupTransform();
        }

        private void EnsureParcels(List<Tuple<int, int, int>> parcelPointers)
        {
            var parcels = _parcels[CurrentScale];
            var shapes = new List<ParcelShape>();
            foreach (var pointer in parcelPointers)
            {
                ParcelShape shape = null;
                if (!parcels.ContainsKey(pointer))
                {
                    var blockSetIndex = pointer.Item1;
                    var blockIndex = pointer.Item2;
                    var parcelIndex = pointer.Item3;
                    var blockSet = _currentLevel.Childs.OfType<BlockSetRecord>().FirstOrDefault(b => b.Index == blockSetIndex) ?? throw new ApplicationException($"Blockset {blockSetIndex} not found");
                    var block = blockSet.Childs.OfType<BlockRecord>().FirstOrDefault(b => b.Index == blockIndex) ?? throw new ApplicationException($"Block {blockIndex} not found");
                    var parcelManagement = block.Childs.OfType<ParcelInformationRecord>().FirstOrDefault() ?? throw new ApplicationException($"No parcel info in block {blockIndex}");
                    parcelManagement.LoadChilds();
                    var parcel = parcelManagement.Childs.OfType<ParcelRecord>().FirstOrDefault(p => p.Index == parcelIndex) ?? throw new ApplicationException($"Parcel {parcelIndex} not found");
                    shape = new ParcelShape(parcel, CurrentScale.ParcelSize, _parcelSizeLong, _parcelSizeLat, parcelIndex);
                    parcels.Add(pointer, shape);
                }
                else
                    shape = parcels[pointer];
                shapes.Add(shape);
            }

            DrawShapes(shapes);
        }

        private readonly List<ParcelShape> _shapesOnCanvas = new List<ParcelShape>();
        private void DrawShapes(List<ParcelShape> shapes)
        {
            foreach (var shape in shapes)
            {
                if (!_shapesOnCanvas.Contains(shape))
                {
                    shape.Draw(_canvas);
                    _shapesOnCanvas.Add(shape);
                }

            }
        }

        private void SetupTransform()
        {
            foreach (var shape in _shapesOnCanvas)
            {
                shape.SetupTransform(CurrentLong, CurrentLat, _viewportWidth, _viewportHeight);
            }
        }
    }
}
