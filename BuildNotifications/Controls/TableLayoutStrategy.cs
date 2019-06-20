using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BuildNotifications.Controls
{
    public class TableLayoutStrategy : ILayoutStrategy
    {
        private int _columnCount;
        private double[] _colWidths;
        private readonly List<double> _rowHeights = new List<double>();
        private int _elementCount;

        public Size ResultSize
        {
            get { return _colWidths!=null && _rowHeights.Any() ? new Size(_colWidths.Sum(), _rowHeights.Sum()) : new Size(0, 0); }
        }

        public void Calculate(Size availableSize, Size[] measures)
        {
            BaseCalculation(availableSize, measures);
            AdjustEmptySpace(availableSize);
        }

        private void BaseCalculation(Size availableSize, Size[] measures)
        {
            _elementCount = measures.Length;
            _columnCount = GetColumnCount(availableSize, measures);
            if (_colWidths==null || _colWidths.Length < _columnCount)
                _colWidths = new double[_columnCount];
            var calculating = true;
            while (calculating)
            {
                calculating = false;
                ResetSizes();
                int row;
                for (row = 0; row*_columnCount < measures.Length; row++)
                {
                    var rowHeight = 0.0;
                    int col;
                    for (col = 0; col < _columnCount; col++)
                    {
                        int i = row*_columnCount + col;
                        if (i >= measures.Length) break;
                        _colWidths[col] = Math.Max(_colWidths[col], measures[i].Width);
                        rowHeight = Math.Max(rowHeight, measures[i].Height);
                    }

                    if (_columnCount > 1 && _colWidths.Sum() > availableSize.Width)
                    {
                        _columnCount--;
                        calculating = true;
                        break;
                    }
                    _rowHeights.Add(rowHeight);
                }
            }
        }

        public Rect GetPosition(int index)
        {
            var columnIndex = index%_columnCount;
            var rowIndex = index/_columnCount;
            var x = 0d;
            for (int i = 0; i < columnIndex; i++)
            {
                x += _colWidths[i];
            }
            var y = 0d;
            for (int i = 0; i < rowIndex; i++)
            {
                y += _rowHeights[i];
            }
            return new Rect(new Point(x, y), new Size(_colWidths[columnIndex], _rowHeights[rowIndex]));
        }

        public int GetIndex(Point position)
        {
            var col = 0;
            var x = 0d;
            while (x < position.X && _columnCount > col)
            {
                x += _colWidths[col];
                col++;
            }
            col--;
            var row = 0;
            var y = 0d;
            while (y < position.Y && _rowHeights.Count > row)
            {
                y += _rowHeights[row];
                row++;
            }
            row--;
            if (row < 0) row = 0;
            if (col < 0) col = 0;
            if (col >= _columnCount) col = _columnCount - 1;
            var result = row*_columnCount + col;
            if (result > _elementCount) result = _elementCount - 1;
            return result;
        }

        private void AdjustEmptySpace(Size availableSize)
        {
            var width = _colWidths.Sum();
            if (!double.IsNaN(availableSize.Width) && availableSize.Width > width)
            {
                var dif = (availableSize.Width - width)/_columnCount;

                for (var i = 0; i < _columnCount; i++)
                {
                    _colWidths[i] += dif;
                }
            }
        }

        private void ResetSizes()
        {
            _rowHeights.Clear();
            for (var j = 0; j < _colWidths.Length; j++)
            {
                _colWidths[j] = 0;
            }
        }

        private static int GetColumnCount(Size availableSize, Size[] measures)
        {
            double width = 0;
            for (int colCnt = 0; colCnt < measures.Length; colCnt++)
            {
                var nwidth = width + measures[colCnt].Width;
                if (nwidth > availableSize.Width)
                    return Math.Max(1, colCnt);
                width = nwidth;
            }
            return measures.Length;
        }
    }
}