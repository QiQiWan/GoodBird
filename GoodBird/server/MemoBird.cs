using System;

namespace GoodBird
{
    /// <summary>
    /// 咕咕机打印所需参数类型
    /// </summary>
    public class MemoBirdInfo
    {
        // 咕咕机设备编号
        public readonly string MemobirdID;
        // 咕咕机所绑定的咕咕号
        public readonly string UserID;
        public MemoBirdInfo(string memobirdID, string userID)
        {
            MemobirdID = memobirdID;
            UserID = userID;
        }
    }
    /// <summary>
    /// 设置每日发送时间和图片路径
    /// </summary>
    public class SendPeriod
    {
        /// <summary>
        /// 每天发送的周期时间类
        /// </summary>
        public CycleTime Time;
        /// <summary>
        /// 每天发送的时间(小时)
        /// </summary>
        public readonly int Hour;
        /// <summary>
        /// 发送图片保存的路径
        /// </summary>
        public readonly string ImgDic;
        /// <summary>
        /// 需要一起发送的文字
        /// </summary>
        public readonly string Message;
        public SendPeriod(int hour, string imgDic)
        {
            this.Hour = hour;
            this.ImgDic = imgDic;
            this.Message = null;
            Time = new CycleTime(hour);
        }
        public SendPeriod(int hour, string imgDic, string message){
            this.Hour = hour;
            this.ImgDic = imgDic;
            this.Message = message;
            Time = new CycleTime(hour);
        }
        public string ToTimeString(){
            return Time.ToString();
        }
        public int RefreshTime() => Time.RefreshTime();
        public bool ConfirmTime() => Time.ComfirmTime();
    }
    /// <summary>
    /// 记录每天重复时间类
    /// </summary>
    public class CycleTime
    {
        public readonly int Hour;
        public int Minute;
        public CycleTime(int hour)
        {
            this.Hour = hour;
            RefreshTime();
        }
        public CycleTime(int hour, int mimute)
        {
            this.Hour = hour;
            this.Minute = mimute;
        }
        /// <summary>
        /// 刷新预定计划的发送时间
        /// </summary>
        /// <returns></returns>
        public int RefreshTime(){
            this.Minute = new Random().Next(0, 59);
            Loger.Log($"时间已刷新至{ToString()}");
            return Minute;
        }
        public override string ToString() {
            return Hour + ":" + Minute;
        }
        /// <summary>
        /// 确认当前时间是否到指定时间
        /// </summary>
        /// <returns></returns>
        public bool ComfirmTime()
        {
            DateTime now = DateTime.Now;
            if (now.Hour == this.Hour && now.Minute == this.Minute) return true;
            else return false;
        }
    }
}