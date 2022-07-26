using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using TextParsingLibrary;

namespace TextParserApp.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private BackgroundWorker workerParse = new BackgroundWorker();
        // Encoding: "For ANSI encodings, the best fit behavior is the default." [1]
        // [1] https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding?view=netframework-4.7.2
        private TextParsingLibrary.Parser parser = new Parser(Encoding.Default);
        public string _statusText;
        private ICommand _cmdSelect;
        private ICommand _cmdParse;
        private ICommand _cmdCancel;
        private Dictionary<string, int> _wordFrequency;
        public string _filePath;
        int _progress;

        public MainViewModel()
        {
            // Create handlers for BackgroundWorker and initialize properties
            workerParse.DoWork += new DoWorkEventHandler(parser.Worker_DoParse);
            workerParse.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerParse_RunWorkerCompleted);
            workerParse.ProgressChanged += new ProgressChangedEventHandler(WorkerParse_ProgressChanged);
            workerParse.WorkerSupportsCancellation = true;
            workerParse.WorkerReportsProgress = true;
        }

        /// <summary>
        /// Command to be executed by clicking the "Select" button.
        /// </summary>
        public ICommand CmdSelect
        {
            get
            {
                if (_cmdSelect == null)
                {
                    // no canExecute Function -> button select always clickable 
                    _cmdSelect = new RelayCommand(param => this.Select(), null);
                }
                return _cmdSelect;
            }
        }

        /// <summary>
        /// Command to be executed by clicking the "Parse" button.
        /// </summary>
        public ICommand CmdParse
        {
            get
            {
                if (_cmdParse == null)
                {
                    _cmdParse = new RelayCommand(param => this.StartParsing(),
                        param => this.CanParse);
                }
                return _cmdParse;
            }
        }

        /// <summary>
        /// Command to be executed by clicking the "Cancel" button.
        /// </summary>
        public ICommand CmdCancel
        {
            get
            {
                if (_cmdCancel == null)
                {
                    _cmdCancel = new RelayCommand(param => this.CancelParsing(),
                        param => this.CanCancel);
                }
                return _cmdCancel;
            }
        }

        /// <summary>
        /// Getter and setter for the WordFrequency property. 
        /// If this property is set, it triggers an update for every bound GUI object.
        /// This property is publicly read-only.
        /// </summary>
        public Dictionary<string, int> WordFrequency
        {
            get { return _wordFrequency; }
            private set
            {
                _wordFrequency = value;
                base.TriggerPropertyChanged("WordFrequency");
            }
        }

        /// <summary>
        /// Getter and setter for the StatusText property.
        /// If this property is set, it triggers an update for every bound GUI object.
        /// This property is publicly read-only.
        /// </summary>
        public string StatusText
        {
            get { return _statusText; }
            private set
            {
                _statusText = value;
                base.TriggerPropertyChanged("StatusText");
            }
        }

        /// <summary>
        /// Getter and setter for the FilePath property.
        /// If this property is set, it triggers an update for every bound GUI object.
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                base.TriggerPropertyChanged("FilePath");
            }
        }

        /// <summary>
        /// Getter and setter for the Progress property. 
        /// If this property is set, it triggers an update for every bound GUI object.
        /// This property is publicly read-only.
        /// </summary>
        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                TriggerPropertyChanged("Progress");
            }
        }

        /// <summary>
        /// This method is triggered after the associated BackgroundWorker calls the ReportProgress() function.
        /// With setting the Progress property it also triggers an update of every bound GUI object.
        /// </summary>
        /// <param name="sender">this paramter holds the object to the triggering background worker</param>
        /// <param name="e">this parameter holds the event object</param>
        private void WorkerParse_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
            StatusText = "Progress " + Progress + "%";
        }

        /// <summary>
        /// This method is triggered after the associated BackgroundWorker is finished with its work.
        /// Depending on the result of the worker it either indicates an error or otherwise it stores the 
        /// text statistics in the WordFreuqency property. With setting the WordFreuqency property it 
        /// also triggers an update of every bound GUI object.
        /// </summary>
        /// <param name="sender">this paramter holds the object to the triggering background worker</param>
        /// <param name="e">this parameter holds the event object</param>
        private void WorkerParse_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                StatusText = "Parsing Error: " + e.Error.Message;
                return;
            }

            if (e.Cancelled == true)
            {
                StatusText = "Parsing canceled!";
                return;
            }

            StatusText = "Parsing finished successfully!";
            TextStatistics stats = e.Result as TextStatistics;
            stats.SortWordFrequency();
            WordFrequency = ((TextStatistics)(e.Result)).WordFrequency;
        }

        /// <summary>
        /// This method is the action which should be executed after command CmdSelect is received.
        /// </summary>
        public void Select()
        {
            Console.WriteLine("Select pressed!");
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FilePath = openFileDialog.FileName;
                    StatusText = "Selected File: " + FilePath;
                }
            }
        }

        /// <summary>
        /// This property is used by the command CmdParse object to evaluate if the bound GUI object should be enabled.
        /// </summary>
        public bool CanParse
        {
            get
            {
                if (FilePath == null)
                {
                    return false;
                }

                try
                {
                    parser.SetTextFile(FilePath);
                    return true;
                }
                catch (FileNotFoundException)
                {
                    StatusText = "File not found!";
                    return false;
                }
            }
        }

        /// <summary>
        /// This method is the action which should be executed after the command CmdParse is received.
        /// </summary>
        public void StartParsing()
        {
            try
            {
                parser.SetTextFile(FilePath);
            }
            catch (FileNotFoundException)
            {
                StatusText = "File not found!";
                return;
            }

            if (workerParse.IsBusy == true)
            {
                StatusText = "Parsing already running! Cancel it first to run again...";
            }

            Progress = 0;
            WordFrequency = null;
            workerParse.RunWorkerAsync();
            StatusText = "Parsing started...";
        }

        /// <summary>
        /// This property is used by the command CmdCancel object to evaluate if the bound GUI object should be enabled.
        /// </summary>
        public bool CanCancel
        {
            get
            {
                if (workerParse.IsBusy && workerParse.WorkerSupportsCancellation)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// This method is the action which should be executed after the command CmdCancel is received.
        /// </summary>
        public void CancelParsing()
        {
            if (workerParse.WorkerSupportsCancellation == false)
            {
                StatusText = "Cancelling not supported!";
                return;
            }

            workerParse.CancelAsync();
            Progress = 0;
        }
    }
}
