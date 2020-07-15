using System.Timers;

namespace GoodBird
{
    /// <summary>
    /// 计时器类,用于控制定时任务
    /// </summary>
    class TimeTick
    {
        public Timer aTimer = new Timer();

        public TimeTick(int interval)
        {
            aTimer.Interval = interval;
            aTimer.AutoReset = true;
        }
        public void Start(Handle handle)
        {
            aTimer.Enabled = true;
            aTimer.Elapsed += new ElapsedEventHandler(handle);
        }
        public void Stop(){
            aTimer.Enabled = false;
        }

    }
    /// <summary>
    /// 计时器委托事件
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    public delegate void Handle(object source, System.Timers.ElapsedEventArgs e);
}