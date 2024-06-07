using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private Random random = new Random();
        private Stopwatch stopwatch = new Stopwatch();
        private List<ReactionRecord> records = new List<ReactionRecord>();
        private const string RecordsFilePath = "reaction_records.txt";

        public MainWindow()
        {
            InitializeComponent();
            ResetRecords();
            LoadRecords();
            DisplayRecords();
        }

        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
            TestButton.IsEnabled = false;
            ResultTextBlock.Text = "잠시 기다려주세요...";

            int delay = random.Next(1000, 5000);
            await Task.Delay(delay);

            TestButton.IsEnabled = true;
            ResultTextBlock.Text = "클릭 버튼을 눌러주세요!";
            stopwatch.Restart();

            TestButton.Click -= TestButton_Click;
            TestButton.Click += ButtonClicked;
        }

        private void ButtonClicked(object sender, RoutedEventArgs e)
        {
            stopwatch.Stop();
            long reactionTime = stopwatch.ElapsedMilliseconds;

            ResultTextBlock.Text = $"당신의 반응속도는 {reactionTime} 입니다.";

            SaveRecord(reactionTime);

            TestButton.Click -= ButtonClicked;
            TestButton.Click += TestButton_Click;
            TestButton.IsEnabled = false;
        }

        private void SaveRecord(long reactionTime)
        {
            var record = new ReactionRecord
            {
                Time = DateTime.Now,
                ReactionTime = reactionTime
            };
            records.Add(record);
            File.AppendAllText(RecordsFilePath, $"{record.Time},{record.ReactionTime}\n");
            DisplayRecords();
        }

        private void LoadRecords()
        {
            if (File.Exists(RecordsFilePath))
            {
                var lines = File.ReadAllLines(RecordsFilePath);
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 2 && DateTime.TryParse(parts[0], out var time) && long.TryParse(parts[1], out var reactionTime))
                    {
                        records.Add(new ReactionRecord { Time = time, ReactionTime = reactionTime });
                    }
                }
            }
        }

        private void DisplayRecords()
        {
            RecordsListBox.Items.Clear();
            foreach (var record in records)
            {
                RecordsListBox.Items.Add($"{record.Time}: {record.ReactionTime} ms");
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            TestButton.IsEnabled = true;
            ResultTextBlock.Text = "";
        }
        private void ResetRecords()
        {
            records.Clear();
            if (File.Exists(RecordsFilePath))
            {
                File.Delete(RecordsFilePath);
            }
        }
    }

    public class ReactionRecord
    {
        public DateTime Time { get; set; }
        public long ReactionTime { get; set; }
    }
}
