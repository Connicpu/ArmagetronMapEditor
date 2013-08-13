using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ArmagetronMapEditor.Models;

namespace ArmagetronMapEditor.Views {
    public partial class MainWindow {
        private Resource _currentMap;
        public Resource CurrentMap {
            get { return _currentMap; }
            private set {
                if (_currentMap != null) {
                    _currentMap.PropertyChanged -= MapPropertyChanged;
                }
                _currentMap = value;
                value.PropertyChanged += MapPropertyChanged;
                MapPropertyChanged(value, new PropertyChangedEventArgs(""));
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
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
