using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ArmagetronMapEditor.Models;
using ArmagetronMapEditor.Views;

namespace ArmagetronMapEditor.ViewModel {
    class WallRenderGroup : RenderGroup {
        private readonly List<Ellipse> points = new List<Ellipse>();
        private readonly List<Line> walls = new List<Line>();
        private Vector offset;
        private double scale;
        private Wall dataItem;
        private double centerX;
        private double centerY;

        private void Render(Canvas canvas) {
            foreach (var wall in walls) {
                canvas.Children.Add(wall);
            }
            foreach (var point in points) {
                canvas.Children.Add(point);
            }
        }

        private void Rerender(Canvas canvas) {
            Derender(canvas);
            centerX = canvas.CenterX();
            centerY = canvas.CenterY();
            points.Clear();
            walls.Clear();

            if (dataItem.Point.Count < 1) {
                return;
            }

            var pointQueue = new Queue<Models.Point>(dataItem.Point);

            var lastPoint = pointQueue.Dequeue();
            points.Add(CreatePoint(lastPoint));

            while (pointQueue.Count > 0) {
                var newPoint = pointQueue.Dequeue();
                walls.Add(CreateWall(lastPoint, newPoint));
                points.Add(CreatePoint(newPoint));
                lastPoint = newPoint;
            }

            points.First().Fill = Brushes.DarkOrange;
            points.Last().Fill = Brushes.DarkRed;

            Render(canvas);
        }

        public override void Derender(Canvas canvas) {
            foreach (var ellipse in points) {
                canvas.Children.Remove(ellipse);
            }
            foreach (var line in walls) {
                canvas.Children.Remove(line);
            }
        }

        public override void Update(Canvas canvas, object fieldItem) {
            if (!(fieldItem is Wall)) {
                throw new ArgumentException("The fieldItem wasn't a wall :I");
            }

            dataItem = (Wall)fieldItem;

            Rerender(canvas);
        }

        public override void UpdateOffset(Canvas canvas, Vector p_offset, double p_scale) {
            scale = p_scale;
            offset = p_offset * scale;

            Rerender(canvas);
        }

        private Ellipse CreatePoint(Models.Point point) {
            var x = point.x * scale + offset.X;
            var y = point.y * scale + offset.Y;

            var ellipse = new Ellipse {
                Width = 5 * scale,
                Height = 5 * scale,
                Fill = Brushes.Lime,
            };

            Canvas.SetLeft(ellipse, x - (ellipse.Width / 2) + centerX);
            Canvas.SetTop(ellipse, y - (ellipse.Height / 2) + centerY);

            return ellipse;
        }

        private Line CreateWall(Models.Point p1, Models.Point p2) {
            var x1 = p1.x * scale + offset.X;
            var y1 = p1.y * scale + offset.Y;

            var x2 = p2.x * scale + offset.X;
            var y2 = p2.y * scale + offset.Y;

            var line = new Line {
                Stroke = Brushes.Lime,
                StrokeThickness = 3.5 * scale,
                X1 = x1 + centerX, X2 = x2 + centerX,
                Y1 = y1 + centerY, Y2 = y2 + centerY
            };

            return line;
        }
    }
}