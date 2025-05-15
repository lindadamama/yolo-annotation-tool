using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Shapes;
using System.Windows.Media;
using YoloAnnotationTool2.Enums;
using Point = System.Windows.Point;
using System.Windows;
using YoloAnnotationTool2.Drawing;
using YoloAnnotationTool2.Models;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using static YoloAnnotationTool.MainWindow;
namespace YoloAnnotationTool2.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _labelText;
        public string LabelText
        {
            get => _labelText;
            set
            {
                _labelText = value;
                OnPropertyChanged(nameof(LabelText));
            }
        }
        private string _currentProject;
        public string CurrentProject
        {
            get => _currentProject;
            set
            {
                _currentProject = value;
                OnPropertyChanged(nameof(CurrentProject));
            }
        }
        private bool _isOptionDetectChecked;
        private bool _isOptionOBBChecked;
        public bool IsOptionDetectChecked
        {
            get => _isOptionDetectChecked;
            set
            {
                _isOptionDetectChecked = value;
                OnPropertyChanged(nameof(IsOptionDetectChecked));
            }
        }
        public bool IsOptionOBBChecked
        {
            get => _isOptionOBBChecked;
            set
            {
                _isOptionOBBChecked = value;
                OnPropertyChanged(nameof(IsOptionOBBChecked));
            }
        }
        private bool _isOptionSegmentChecked;
        public bool IsOptionSegmentChecked
        {
            get => _isOptionSegmentChecked;
            set
            {
                _isOptionSegmentChecked = value;
                OnPropertyChanged(nameof(IsOptionSegmentChecked));
            }
        }
        private bool _isOptionClassifyChecked;
        public bool IsOptionClassifyChecked
        {
            get => _isOptionClassifyChecked;
            set
            {
                _isOptionClassifyChecked = value;
                OnPropertyChanged(nameof(IsOptionClassifyChecked));
            }
        }
        private bool _isOptionPoseChecked;
        public bool IsOptionPoseChecked
        {
            get => _isOptionPoseChecked;
            set
            {
                _isOptionPoseChecked = value;
                OnPropertyChanged(nameof(IsOptionPoseChecked));
            }
        }
        private string _titleOfProject;
        public string TitleOfProject
        {
            get => _titleOfProject;
            set
            {
                _titleOfProject = value;
                OnPropertyChanged(nameof(TitleOfProject));
            }
        }
        private string _projectWithSuffix;
        public string projectWithSuffix
        {
            get => _projectWithSuffix;
            set
            {
                _projectWithSuffix = value;
                OnPropertyChanged(nameof(projectWithSuffix));
            }
        }

        private ObservableCollection<string> _trainImages = new ObservableCollection<string>();
        public ObservableCollection<string> TrainImages
        {
            get => _trainImages;
            set
            {
                _trainImages = value;
                OnPropertyChanged(nameof(TrainImages));
            }
        }

        private ObservableCollection<string> _valImages = new ObservableCollection<string>();
        public ObservableCollection<string> ValImages
        {
            get => _valImages;
            set
            {
                _valImages = value;
                OnPropertyChanged(nameof(ValImages));
            }
        }

        private ObservableCollection<string> _testImages = new ObservableCollection<string>();
        public ObservableCollection<string> TestImages
        {
            get => _testImages;
            set
            {
                _testImages = value;
                OnPropertyChanged(nameof(TestImages));
            }
        }
        public ObservableCollection<UIElement> drawnPoses { get; set; } = new();



        private string _selectedImageName;
        public string SelectedImageName
        {
            get => _selectedImageName;
            set
            {
                if (_selectedImageName != value)
                {
                    _selectedImageName = value;
                    OnPropertyChanged(nameof(SelectedImageName));
                }
            }
        }
        private string _selectedTrainImage;
        public string SelectedTrainImage
        {
            get => _selectedTrainImage;
            set
            {
                _selectedTrainImage = value;
                OnPropertyChanged(nameof(SelectedTrainImage));
            }
        }

        private string _selectedValImage;
        public string SelectedValImage
        {
            get => _selectedValImage;
            set
            {
                _selectedValImage = value;
                OnPropertyChanged(nameof(SelectedValImage));
            }
        }

        private string _selectedTestImage;
        public string SelectedTestImage
        {
            get => _selectedTestImage;
            set
            {
                _selectedTestImage = value;
                OnPropertyChanged(nameof(SelectedTestImage));
            }
        }
        private string _selectedImagePath;
        public string SelectedImagePath
        {
            get => _selectedImagePath;
            set
            {
                if (_selectedImagePath != value)
                {
                    _selectedImagePath = value;
                    OnPropertyChanged(nameof(SelectedImagePath));
                }
            }
        }        
        private string _pointCounterText;
        public string pointCounterText
        {
            get => _pointCounterText;
            set
            {
                if (_pointCounterText != value)
                {
                    _pointCounterText = value;
                    OnPropertyChanged(nameof(pointCounterText));
                }
            }
        }
        public string[] PoseBodyParts { get; } =
        {
            "Nose", "Left Eye", "Right Eye", "Left Ear", "Right Ear",
            "Left Shoulder", "Right Shoulder", "Left Elbow", "Right Elbow",
            "Left Wrist", "Right Wrist", "Left Hip", "Right Hip",
            "Left Knee", "Right Knee", "Left Ankle", "Right Ankle"
        };
        public int PoseCurrentPointIndex { get; set; } = 0;
        public List<Point> PosePoints { get; set; } = new();
        public List<Ellipse> PoseKeypoints { get; set; } = new();
        public List<Line> PoseSkeletonLines { get; set; } = new();
        public List<List<Point>> AllPosePoints { get; set; } = new();
        public ObservableCollection<PoseAnnotation> Poses { get; set; } = new();

        public List<System.Windows.Shapes.Rectangle> drawnRectangles = new List<System.Windows.Shapes.Rectangle>();
        public List<System.Windows.Shapes.Polygon> drawnPolygons = new List<System.Windows.Shapes.Polygon>();
        public List<System.Windows.Shapes.Polyline> drawnPolylines = new List<System.Windows.Shapes.Polyline>();
        public List<Point> _points = new List<Point>();
        public Line _previewLine = new Line();
        public Polyline _currentPolyline = new Polyline();



        public ObservableCollection<string> ClassLabels { get; set; } = new ObservableCollection<string>();

        private string _selectedClassLabel;
        public string SelectedClassLabel
        {
            get => _selectedClassLabel;
            set
            {
                _selectedClassLabel = value;
                OnPropertyChanged(nameof(SelectedClassLabel));
            }
        }


        // Словник для зберігання кольорів для кожного класу
        public Dictionary<string, System.Windows.Media.Brush> ClassColors { get; set; } = new Dictionary<string, System.Windows.Media.Brush>();

        public System.Windows.Media.Brush GetRandomColor()
        {
            Random random = new Random();
            return new SolidColorBrush(System.Windows.Media.Color.FromRgb(
                (byte)random.Next(0, 256),
                (byte)random.Next(0, 256),
                (byte)random.Next(0, 256)
            ));
        }

        private System.Windows.Media.Brush _currentLabelColor = System.Windows.Media.Brushes.White;
        public System.Windows.Media.Brush CurrentLabelColor
        {
            get { return _currentLabelColor; }
            set
            {
                _currentLabelColor = value;
                OnPropertyChanged(nameof(CurrentLabelColor));
            }
        }

        private AnnotationType _selectedAnnotationType = AnnotationType.Detect;
        public AnnotationType SelectedAnnotationType
        {
            get => _selectedAnnotationType;
            set
            {
                _selectedAnnotationType = value;
                OnPropertyChanged(nameof(SelectedAnnotationType));
            }
        }
        private string _saveStatus;
        public string saveStatus
        {
            get => _saveStatus;
            set
            {
                if (_saveStatus != value)
                {
                    _saveStatus = value;
                    OnPropertyChanged(nameof(saveStatus));
                }
            }
        }
        // ⬇️ Подія оновлення властивостей
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
