using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;
using ArmagetronMapEditor.Models;
using Microsoft.Win32;

namespace ArmagetronMapEditor.Views {
    public partial class MainWindow : INotifyPropertyChanged {
        private Resource _currentMap;
        public Resource CurrentMap {
            get { return _currentMap; }
            private set {
                var hadOld = _currentMap != null;
                if (_currentMap != null) {
                    _currentMap.PropertyChanged -= MapPropertyChanged;
                }
                _currentMap = value;
                value.PropertyChanged += MapPropertyChanged;
                MapPropertyChanged(value, new PropertyChangedEventArgs(""));
                OnPropertyChanged();
                if (hadOld) {
                    fieldRender.DataContext = value.Map;
                    fieldRender.ResetMap();
                }
            }
        }

        private void MapPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs) {
            Title = string.Format("/{0}/{1}/{2}-{3}.{4}.xml",
                _currentMap.author, _currentMap.category, _currentMap.name,
                _currentMap.version, _currentMap.type);
        }

        public MainWindow() {
            CurrentMap = NewMap();
            InitializeComponent();
        }

        private static Resource NewMap() {
            return new Resource {
                author = Environment.UserName,
                category = "untitled-category",
                version = "1.0.0",
                name = "my-new-map",
                comissioner = "",
                type = ResourceType.aamap,
                Map = new Map {
                    version = "1.0.0",
                    World = new World {
                        Field = new Field {
                            Axes = new Axes {
                                number = "4"
                            },
                            Items = {
                                new Wall {
                                    height = 10,
                                    Point = {
                                        new Point { x = 0, y = 0 },
                                        new Point { x = 0, y = 500 },
                                        new Point { x = 500, y = 500 },
                                        new Point { x = 500, y = 0 },
                                        new Point { x = 0, y = 0 },
                                    }
                                },
                            }
                        }
                    }
                }
            };
        }

        private readonly OpenFileDialog openFile = new OpenFileDialog { Filter = "Armagetron Map|*.aamap.xml" };
        private void OpenClicked(object sender, System.Windows.RoutedEventArgs e) {
            if (openFile.ShowDialog(this) != true) {
                return;
            }

            var serializer = new XmlSerializer(typeof(Resource));
            using (var fs = File.OpenRead(openFile.FileName)) {
                var newMap = (Resource)serializer.Deserialize(fs);
                CurrentMap = newMap;
            }
        }

        private void SaveClicked(object sender, System.Windows.RoutedEventArgs e) {
            var serializer = new XmlSerializer(typeof(Resource));
            var buffer = new MemoryStream();
            var writer = new StreamWriter(buffer);
            var reader = new StreamReader(buffer);
            serializer.Serialize(writer, CurrentMap);
            buffer.Seek(0, SeekOrigin.Begin);

            var codez = reader.ReadToEnd();
            return;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
