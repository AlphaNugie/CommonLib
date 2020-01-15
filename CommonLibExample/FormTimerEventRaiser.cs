using CommonLib.Function;
using CommonLib.UIControlUtil;
using System.Windows.Forms;

namespace CommonLibExample
{
    public partial class FormTimerEventRaiser : Form
    {
        private readonly TimerEventRaiser raiser = new TimerEventRaiser(1000);

        public FormTimerEventRaiser()
        {
            InitializeComponent();
            this.raiser.Interval = 1000;
            this.raiser.RaiseThreshold = 1000;
            this.raiser.RaiseInterval = 3000;
            this.raiser.ThresholdReached += new TimerEventRaiser.ThresholdReachedEventHandler(Raiser_ThresholdReached);
            this.raiser.Clicked += new TimerEventRaiser.ClickedEventHandler(Raiser_Clicked);
        }

        private void Raiser_Clicked(object sender, ClickedEventArgs e)
        {
            this.label_Message.SafeInvoke(() => this.label_Message.Text = e.ClickMessage);
        }

        private void Raiser_ThresholdReached(object sender, ThresholdReachedEventArgs e)
        {
            this.label_Message.SafeInvoke(() =>
            {
                this.label_Count.Text = e.Counter.ToString();
                this.label_RaiseCount.Text = e.RaisedTimes.ToString();
            });
        }

        private void Button_Start_Click(object sender, System.EventArgs e)
        {
            this.raiser.Run();
        }

        private void Button_End_Click(object sender, System.EventArgs e)
        {
            this.raiser.Stop();
        }

        private void Button_Click_Click(object sender, System.EventArgs e)
        {
            this.raiser.Click(this.textBox_Message.Text);
            this.label_Count.Text = this.raiser.Counter.ToString();
            this.label_RaiseCount.Text = this.raiser.RaisedTimes.ToString();
        }

        private void Button_Reset_Click(object sender, System.EventArgs e)
        {
            this.raiser.Reset();
        }
    }
}
