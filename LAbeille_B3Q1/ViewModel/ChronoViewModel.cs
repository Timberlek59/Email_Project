using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using LAbeille_B3Q1.Model;

namespace LAbeille_B3Q1.ViewModel
{
    public class ChronoViewModel : INotifyPropertyChanged
    {
        private readonly ChronometerModel _model;
        private readonly DispatcherTimer _timer;
        private bool _isRunning;

        public event PropertyChangedEventHandler PropertyChanged;

        public ChronoViewModel()
        {
            _model = new ChronometerModel();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;

            StartCommand = new RelayCommand(Start, () => !_isRunning);
            StopCommand = new RelayCommand(Stop, () => _isRunning);
            ResetCommand = new RelayCommand(Reset, () => !_isRunning && _model.ElapsedTime > TimeSpan.Zero);
        }

        public double SecondAngle => _model.ElapsedTime.Seconds * 6; 
        public double MinuteAngle => (_model.ElapsedTime.Minutes + _model.ElapsedTime.Seconds / 60.0) * 6; 
        public string FormattedTime => _model.ElapsedTime.ToString(@"mm\:ss");

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand ResetCommand { get; }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _model.AddSecond();
            NotifyTimeProperties();
            ((RelayCommand)ResetCommand).RaiseCanExecuteChanged();
        }

        private void Start()
        {
            _isRunning = true;
            _timer.Start();
            UpdateCommandStates();
        }

        private void Stop()
        {
            _isRunning = false;
            _timer.Stop();
            UpdateCommandStates();
        }

        private void Reset()
        {
            _model.Reset();
            NotifyTimeProperties();
            UpdateCommandStates();
        }

        private void UpdateCommandStates()
        {
            ((RelayCommand)StartCommand).RaiseCanExecuteChanged();
            ((RelayCommand)StopCommand).RaiseCanExecuteChanged();
            ((RelayCommand)ResetCommand).RaiseCanExecuteChanged();
        }

        private void NotifyTimeProperties()
        {
            OnPropertyChanged(nameof(SecondAngle));
            OnPropertyChanged(nameof(MinuteAngle));
            OnPropertyChanged(nameof(FormattedTime));
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}