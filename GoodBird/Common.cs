namespace GoodBird
{
    /// <summary>
    /// 全局静态变量
    /// </summary>
    public class Common
    {
        /// <summary>
        /// 在数组中添加新的咕咕机设备编号和你设备所绑定的咕咕号
        /// 添加格式: new MemoBirdInfo("456as4f5sa4f5a4s5f6", "1234567")
        /// 第一个字符串参数为咕咕机吐出的设备编号,第二个字符串参数为咕咕机绑定的咕咕号
        /// 同时发送多个咕咕机使用半角逗号分隔
        /// </summary>
        /// <value></value>
        public static MemoBirdInfo[] Birds = new MemoBirdInfo[]{
            new MemoBirdInfo("0fe007937340679f", "8464002"),
            new MemoBirdInfo("e37135d80b302f1c", "8464002")
        };

        /// <summary>
        /// 在数组中添加每日的发送任务
        /// 添加格式: new SendPeriod(7, "img/yourFloder/)
        /// 其中第一个参数为发送时间,取值0-23;第二个参数为存放需要发送的图片的保存文件夹
        /// 第一个参数设置的是小时,分钟会在该小时内随机取,如填写的是22,则会在22:00-22:59随机选一个时间发送
        /// 若第二个参数文件夹中若存在多张图片则会一次随机选一张发出去,所以大胆往里面放图片
        /// </summary>
        /// <value></value>
        public static SendPeriod[] SendPeriods = new SendPeriod[]{
            new SendPeriod(22, "img/night/", "Good Night!"),
            new SendPeriod(7, "img/morning/", "Good Morning!")
        };

        /// <summary>
        /// 咕咕机开放平台申请到的应用及参数:ACCESS_KEY, 咕咕机开放平台地址: https://open.memobird.cn/
        /// </summary>
        public static string ACCESS_KEY = "dafe4058bf284ec0a64858fbe853c7c4";

        /// <summary>
        /// 设置每次检查时刻匹配的间隔(ms)
        /// 不需要修改
        /// </summary>
        public static int ConfirmTime = 6000;
        /// <summary>
        /// 设置每天刷新发送时间的间隔(ms,一天)
        /// </summary>
        public static int UpdateDay = 86400000;

        /// <summary>
        /// 静态构造函数,初始化全局变量
        /// </summary>
        public Common() { }
    }
}