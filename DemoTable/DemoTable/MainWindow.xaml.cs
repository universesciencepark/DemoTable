﻿using Phidget22;
using Phidget22.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DemoTable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //Needed for binding
        public event PropertyChangedEventHandler PropertyChanged;

        //How long the countdown before the game should last in seconds
        public static int countdownTime = 10;
        //How long the game should last in seconds
        public static int gameTime = 50;
        //Time from hit until fire in milliseconds
        public static int outputDelay1 = 1000;
        //Time from fire until reset in milliseconds
        public static int outputDelay2 = 500;

        //Timer used for countdown before and during game
        private DispatcherTimer countDown = new DispatcherTimer();

        //The list of players
        private List<Player> players = new List<Player>();

        //If the countdown before the game is ticking
        public bool isCountdownStarted = false;

        //If the game has started
        public bool isGameStarted = false;

        //The time to display
        private int timer = countdownTime;
        public int Timer
        {
            get => timer;
            set
            {
                timer = value;
                OnPropertyChanged("Timer");
            }
        }

        public MainWindow()
        {
            //Show stuff on the window
            InitializeComponent();

            //Adds players to GUI, and to players
            players.Add(new Player(this, 1, 0, 0));
            players.Add(new Player(this, 5, 4, 4));
            players.Add(new Player(this, 9, 8, 8));
            players.Add(new Player(this, 13, 12, 12));
            
            for(int i = 0; i < 4; i++)
            {
                //players.Add(new Player(this, (i*2), (i*2)+1, i));
                (MainGrid.Children[i] as Frame).Content = players[i];
            }
            

            //Set up timer
            countDown.Interval = new TimeSpan(0, 0, 1);
            countDown.Tick += CountDown_Tick;
        }

        /// <summary>
        /// Used to start the countdown, which starts the game
        /// </summary>
        public void StartGame()
        {
            //Start the timer
            countDown.Start();
            //Make sure timer knows we're in the countdown, not the game
            isCountdownStarted = true;
			//Reset status
            players.ForEach(p => p.Status = "Tryk for at starte!");
            //Reset Player scores
            players.ForEach(p => p.ResetScore());
        }

        private void CountDown_Tick(object sender, EventArgs e)
        {
            //Tick the timer down
            Timer -= 1;
            //When timer is over
            if (Timer <= 0)
            {
                //if the game is over
                if (isGameStarted)
                {
                    //game is no longer running
                    isGameStarted = false;
                    //Stop the timer
                    countDown.Stop();
                    //Marks players as not in-game
                    players.ForEach(p => p.IsPlayerJoined = false);
                    //set timer to countdown
                    Timer = countdownTime;
                }

                //if the countdown is over
                if (isCountdownStarted)
                {
                    //countdown is no longer running
                    isCountdownStarted = false;
                    //game starts
                    isGameStarted = true;
                    players.Where(p => p.IsPlayerJoined).ToList().ForEach(p => p.Status = "0");
                    //setup game time
                    Timer = gameTime;
                }
            }
        }

        private void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
}