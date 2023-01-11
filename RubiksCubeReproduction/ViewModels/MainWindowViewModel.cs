using RubiksCubeReproduction.Models;
using RubiksCubeReproduction.ViewModels;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using RubiksCubeReproduction.Converters;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Windows.Documents;
using System.Collections.Generic;

namespace RubiksCubeReproduction.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        #region Properties

        public RubiksCubeImageReproduction RubiksCubeImageReproduction
        {
            get { return _rubiksCubeImageReproduction; }
            set
            {
                if (value == _rubiksCubeImageReproduction) return;
                _rubiksCubeImageReproduction = value;
                //OnPropertyChanged();
            }
        }
        private RubiksCubeImageReproduction _rubiksCubeImageReproduction;

        public bool isAssemblerLibraryActive
        {
            get { return _isAssemblerLibraryActive; }
            set
            {
                if (value == _isAssemblerLibraryActive) return;
                _isAssemblerLibraryActive = value;
                //OnPropertyChanged();
            }
        }
        private bool _isAssemblerLibraryActive;

        public bool isCSharpLibraryActive
        {
            get { return _isCSharpLibraryActive; }
            set
            {
                if (value == _isCSharpLibraryActive) return;
                _isCSharpLibraryActive = value;
                //OnPropertyChanged();
            }
        }
        private bool _isCSharpLibraryActive;

        public string InformationText
        {
            get { return _informationText; }
            set
            {
                if (value == _informationText) return;
                _informationText = value;
                OnPropertyChanged("InformationText");
            }
        }
        private string _informationText;


        public byte[] MainPanelImage
        {
            get { return _mainPanelImage; }
            private set
            {
                if (Equals(value, _mainPanelImage)) return;
                _mainPanelImage = value;
                OnPropertyChanged("MainPanelImage");
            }
        }
        private byte[] _mainPanelImage;

        public SolidColorBrush ImageDropContainerBackground
        {
            get { return _imageDropContainerBackground; }
            private set
            {
                if (!Equals(value, _imageDropContainerBackground))
                {
                    _imageDropContainerBackground = value;
                    //OnPropertyChanged();
                }
            }
        }
        private SolidColorBrush _imageDropContainerBackground;

        public string MainPanelDescription
        {
            get { return _mainPanelDescription; }
            set
            {
                if (value != _mainPanelDescription)
                {
                    _mainPanelDescription = value;
                    //OnPropertyChanged();
                }
            }
        }
        private string _mainPanelDescription;

        public int NumberOfThreads
        {
            get { return _numberOfThreads; }
            set
            {
                if (value.Equals(_numberOfThreads)) return;
                _numberOfThreads = value;
                //OnPropertyChanged();
            }
        }
        private int _numberOfThreads;

        public TimeSpan ComputationTime
        {
            get { return _computationTime; }
            set
            {
                if (value.Equals(_computationTime)) return;
                _computationTime = value;
                OnPropertyChanged("ComputationTime");
            }
        }
        private TimeSpan _computationTime;

        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            InitializeProperties();
            InitializeCommands();
        }

        #endregion

        private void InitializeProperties()
        {
            NumberOfThreads = 2;
            //ComputationTime = new TimeSpan(0, 10, 10 );
            isAssemblerLibraryActive = true;

            InformationText = "Waiting for image...";
        }


        #region Commands
        public RelayCommand SelectImageCommand { get; private set; }
        public RelayCommand SaveImageCommand { get; private set; }
        public RelayCommand GenerateReproductionCommand { get; private set; }
        public RelayCommand TestCommand { get; private set; }
        public void InitializeCommands()
        {
            InitializeSelectImageCommand();
            InitializeSaveImageCommand();
            InitializeGenerateReproductionCommand();
            InitializeTestCommand();
        }

        private int SavedImagesCounter = 0;
        private string SavedImageFileName = "converted";
        private string SavedImageFileNameExtension = ".bmp";
        private void InitializeSaveImageCommand()
        {
            SaveImageCommand = new RelayCommand(() =>
            {
                string newFileName = SavedImagesCounter == 0 ?
                                        SavedImageFileName + SavedImageFileNameExtension :
                                        SavedImageFileName + SavedImagesCounter + SavedImageFileNameExtension;
                SavedImagesCounter++;
                SaveShowingImage(newFileName);
            });
        }

        private void InitializeSelectImageCommand()
        {
            SelectImageCommand = new RelayCommand(() =>
            {
                MainPanelImage = null;
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.InitialDirectory = "c:\\";
                dlg.Filter = "Image files (*.bmp)|*.bmp|All Files (*.*)|*.*";
                dlg.RestoreDirectory = true;
                bool? wasOKClicked = dlg.ShowDialog();

                if (wasOKClicked == null ? false : (bool)wasOKClicked)
                {
                    string selectedFileName = dlg.FileName;
                    //byte[] bitmap = File.ReadAllBytes(selectedFileName);
                    RubiksCubeImageReproduction = new RubiksCubeImageReproduction(selectedFileName);
                    MainPanelImage = RubiksCubeImageReproduction.OriginalImage;
                    InformationText = "Press button below!";
                }
            });
        }

        private void SaveShowingImage(string path)
        {
            try
            {
                File.WriteAllBytes(path, MainPanelImage);
                MessageBox.Show("Saved file in the current location.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InitializeGenerateReproductionCommand()
        {
            GenerateReproductionCommand = new RelayCommand(() =>
            {
                if (MainPanelImage == null) 
                {
                    MessageBox.Show("Select an image first!");
                    return;
                }
                int miliseconds = RubiksCubeImageReproduction.GenerateImageReproduction(isAssemblerLibraryActive, NumberOfThreads);
                ComputationTime = TimeSpan.FromMilliseconds(miliseconds);
                MainPanelImage = RubiksCubeImageReproduction.ImageReproduction;
                InformationText = "Your image is ready!";
            });
        }

        private void InitializeTestCommand()
        {
            TestCommand = new RelayCommand(() =>
            {
                if (MainPanelImage == null)
                {
                    MessageBox.Show("Select an image first!");
                    return;
                }
                List<int> threadCounts = new List<int>() { 1, 2, 4, 8, 16, 32};

                //ONLY USE IN DEBUG:
                List<bool> useAssemblyLib = new List<bool>() { true, false };
                //ONLY USE IN RELEASE:
                //List<bool> useAssemblyLib = new List<bool>() { false };

                List<TestResult> testResults = new List<TestResult>();
                foreach (int threadCountUsed in threadCounts)
                {
                    foreach(bool isAsseblyLibUsed in useAssemblyLib)
                    {
                        int milisecondsEstimated = RubiksCubeImageReproduction.GenerateImageReproduction(isAsseblyLibUsed, threadCountUsed);
                        testResults.Add(new TestResult()
                        {
                            isAssemblerLibraryActive = isAsseblyLibUsed,
                            miliseconds = milisecondsEstimated,
                            threadCount = threadCountUsed
                        });
                    }                    
                }
                string result = "";
                foreach (TestResult testResult in testResults)
                {
                    string library = testResult.isAssemblerLibraryActive
                        ? "assembly"
                        : "C#";
                    result += $"{testResult.threadCount} threads, generated in {testResult.miliseconds} ms using the {library} library. \r\n";
                }
                MessageBox.Show(result);
            });
        }
        #endregion
    }
}