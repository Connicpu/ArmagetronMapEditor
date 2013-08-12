using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ArmagetronMapEditor.Models;

namespace ArmagetronMapEditor.Views {
    public partial class MainWindow {
        public Resource CurrentMap { get; private set; }

        public MainWindow() {
            CurrentMap = NewMap();

            InitializeComponent();
        }

        private Resource NewMap() {
            return new Resource {
                author = Environment.UserName,
                category = "untitled_category",
                version = "1.0.0",
                comissioner = "",
                type = ResourceType.aamap,
                Map = new Map {
                    Settings = new ObservableCollection<Setting>(),
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
