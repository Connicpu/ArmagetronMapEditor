using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using ArmagetronMapEditor.Models;
using ArmagetronMapEditor.ViewModel;
using Point = System.Windows.Point;

namespace ArmagetronMapEditor.Views {
    /// <summary>
    /// Interaction logic for FieldRender.xaml
    /// </summary>
    public partial class FieldRender {
        readonly Dictionary<int, RenderGroup> renderedItems = new Dictionary<int, RenderGroup>();
        private Vector renderOffset = new Vector(-250, -250);
        private double renderScale = 0.9;

        public FieldRender() {
            InitializeComponent();
            var mapBinding = new Binding {
                RelativeSource = new RelativeSource(RelativeSourceMode.Self),
                Path = new PropertyPath("DataContext")
            };
            SetBinding(MapProperty, mapBinding);
        }

        public void ResetMap() {
            var removalQueue = new Queue<int>();
            foreach (var group in renderedItems) {
                group.Value.Derender(renderCanvas);
                removalQueue.Enqueue(group.Key);
            }
            while (removalQueue.Count > 0) {
                renderedItems.Remove(removalQueue.Dequeue());
            }
            foreach (var item in Map.World.Field.Items) {
                DrawItem(item);
            }
        }

        private void RenderLoaded(object sender, RoutedEventArgs e) {
            if (Map == null) return;
            Map.World.Field.Items.CollectionChanged += OnFieldChanged;
        }

        private void RenderUnloaded(object sender, RoutedEventArgs e) {
            if (Map == null) return;
            Map.World.Field.Items.CollectionChanged -= OnFieldChanged;
        }

        private void OnFieldChanged(object sender, NotifyCollectionChangedEventArgs args) {
            var items = (ObservableCollection<object>)sender;
            switch (args.Action) {
                case NotifyCollectionChangedAction.Reset:
                    var removalQueue = new Queue<int>();
                    foreach (var group in renderedItems) {
                        group.Value.Derender(renderCanvas);
                        removalQueue.Enqueue(group.Key);
                    }
                    while (removalQueue.Count > 0) {
                        renderedItems.Remove(removalQueue.Dequeue());
                    }
                    foreach (var item in items) {
                        DrawItem(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in args.NewItems) {
                        DrawItem(item);
                    }
                    break;
            }
        }

        private void DrawItem(object item) {
            if (renderCanvas.ActualWidth < 1) {
                Dispatcher.BeginInvoke(new Action(() => {
                    DrawItem(item);
                }), DispatcherPriority.Background);
                return;
            }

            RenderGroup group = null;
            if (item is Wall) {
                group = new WallRenderGroup();
                group.Update(renderCanvas, item);
            } else if (item is Spawn) {

            } else if (item is Zone) {

            }
            if (group == null) return;

            group.UpdateOffset(renderCanvas, renderOffset, renderScale);
            renderedItems[item.GetHashCode()] = group;
        }

        public static readonly DependencyProperty MapProperty =
            DependencyProperty.Register("Map", typeof(Map), typeof(FieldRender), new PropertyMetadata(default(Map), MapChanged));

        private static void MapChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args) {
            var render = (FieldRender)dependencyObject;
            if (args.OldValue != null) {
                ((Map)args.OldValue).World.Field.Items.CollectionChanged -= render.OnFieldChanged;
            }
            if (args.NewValue == null) return;

            ((Map)args.NewValue).World.Field.Items.CollectionChanged += render.OnFieldChanged;

            foreach (var item in ((Map)args.NewValue).World.Field.Items) {
                render.DrawItem(item);
            }
        }

        private void UpdateOffset() {
            foreach (var item in renderedItems.Values) {
                item.UpdateOffset(renderCanvas, renderOffset, renderScale);
            }
        }

        public Map Map {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }

        private bool mouseCaptured;
        private Point lastMousePos;
        private void RenderCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Mouse.Capture(renderCanvas);
            mouseCaptured = true;
            lastMousePos = Mouse.GetPosition(renderCanvas);
        }

        private void RenderCanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            mouseCaptured = false;
            Mouse.Capture(null);
        }

        private void RenderCanvasMouseMove(object sender, MouseEventArgs e) {
            if (mouseCaptured) {
                renderOffset += (e.GetPosition(renderCanvas) - lastMousePos) / renderScale;
                lastMousePos = e.GetPosition(renderCanvas);
                UpdateOffset();
            }
        }

        private void renderCanvas_MouseWheel(object sender, MouseWheelEventArgs e) {
            var change = ((double)e.Delta) / 1000;
            change *= Math.Log(renderScale + 1);
            renderScale += change;
            if (renderScale < 0.05) {
                renderScale = 0.05;
            } else if (renderScale > 3) {
                renderScale = 3;
            }
            UpdateOffset();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e) {
            UpdateOffset();
        }
    }

    public abstract class RenderGroup {
        public abstract void Derender(Canvas canvas);
        public abstract void Update(Canvas canvas, object fieldItem);
        public abstract void UpdateOffset(Canvas canvas, Vector offset, double scale);
    }

    static class CanvasHelpers {
        public static double CenterX(this Canvas canvas) {
            return canvas.ActualWidth / 2;
        }
        public static double CenterY(this Canvas canvas) {
            return canvas.ActualHeight / 2;
        }
    }
}
